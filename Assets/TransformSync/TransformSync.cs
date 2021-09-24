using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TransformSync
{
    public class TransformSync : MonoBehaviour
    {
        public TransformInfo lastInfo;
        public TransformInfo tempInfo;
        public bool isSend;

        public int id;
        public int bindId;

        private void Awake()
        {
            if (bindId != 0)
            {
                TransformSyncEvent.AddListener(bindId, Sync);
            }
        }

        private void Reset()
        {
            id = GetInstanceID();
        }

        private void Start()
        {
            lastInfo = TransformSyncUtils.Create(transform);
            Send(lastInfo.ToBytes());
        }

        [SerializeField] private TransformInfo lastSendInfo;

        private float fps = 30;

        private void FixedUpdate()
        {
            if (isSend)
            {
                TransformSyncUtils.CreateAttit(transform, ref lastInfo, ref tempInfo);
                if (tempInfo.tag != 0)
                {
                    lastSendInfo = tempInfo;
                    lastInfo.Merge(ref tempInfo);
                    Send(tempInfo.ToBytes(false));
                }
            }
        }


        public bool isPos, isRot, isScale;

        private void Update()
        {
            if (isSend)
            {
                if (isPos)
                    transform.Translate(Vector3.one * (Random.value >= 0.5f ? Random.value : -Random.value));
                if (isRot)
                    transform.Rotate(Vector3.one * (Random.value >= 0.5f ? Random.value : -Random.value));
                if (isScale)
                    transform.localScale += Vector3.one * (Random.value >= 0.5f ? Random.value : -Random.value);
            }
        }

        #region 网络 发生/接受 数据

        [SerializeField] long sendByteCount;
        [SerializeField] long syncByteCount;

        public void Sync(byte[] bytes)
        {
            syncByteCount += bytes.Length;

            // lastInfo.Fill(bytes);
            // TransformSyncUtils.Set(transform, ref lastInfo);
        }

        public void Send(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            sendByteCount += bytes.Length;
            TransformSyncEvent.Send(id, bytes);
            Debug.LogFormat("Sned {0} byteLength:{1}", id, bytes.Length);
        }

        #endregion
    }
}