using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformSync
{
    public class TransformSyncEvent : MonoBehaviour
    {
        private static Dictionary<int, Action<byte[]>> eventDict = new Dictionary<int, Action<byte[]>>();

        public static TransformSyncEvent inst;

        private void Awake()
        {
            inst = this;
        }

        private long sendLenght;
        private long receiveLenght;
        private int time;

        public long lastSendLenght;
        public long lastReceiveLenght;

        private void CheckReset()
        {
            if (time != (int) Time.realtimeSinceStartup)
            {
                time = (int) Time.realtimeSinceStartup;
                lastReceiveLenght = receiveLenght;
                lastSendLenght = sendLenght;
                receiveLenght = sendLenght = 0;
            }
        }

        public static void Send(int id, byte[] param)
        {
            inst.sendLenght += param.Length;
            inst.CheckReset();
            // 检查重置

            // todo: 模拟网络收发消息
            Receive(id, param);
        }

        public static void Receive(int id, byte[] param)
        {
            inst.receiveLenght += param.Length;
            inst.CheckReset();
            // 检查重置

            if (!eventDict.ContainsKey(id))
                return;
            var callback = eventDict[id];
            callback(param);
        }

        public static void AddListener(int id, Action<byte[]> listener)
        {
            if (eventDict.ContainsKey(id))
            {
                eventDict[id] += listener;
            }
            else
            {
                eventDict[id] = listener;
            }
        }

        public static void RemoveListener(int id, Action<byte[]> listener)
        {
            if (!eventDict.ContainsKey(id))
                return;
            eventDict[id] -= listener;

            if (eventDict[id] == null)
                eventDict.Remove(id);
        }
    }
}