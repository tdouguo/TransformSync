using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TransformSync
{
    [Serializable]
    public struct TransformInfo : IDisposable
    {
        // 0000 0011 1111 1111   
        public long tag; // 按位运算 那些位置变化了 
        public float pX, pY, pZ;
        public float rX, rY, rZ, rW;
        public float sX, sY, sZ;

        public long GetTagNoNan()
        {
            long _tag = tag;
            for (var i = 0; i < TransformSyncUtils.TI_Arrays.Length; i++)
            {
                SyncTag syncTag = TransformSyncUtils.TI_Arrays[i];
                if (IsChoose(syncTag) && IsNan(syncTag))
                {
                    TransformSyncUtils.UnChoose(syncTag, _tag);
                }
            }

            return _tag;
        }

        #region 流操作

        private MemoryStream m_Stream;
        private bool m_Disposed;

        public MemoryStream Stream
        {
            get
            {
                if (m_Stream == null)
                    m_Stream = new MemoryStream(TransformSyncUtils.MAX_SIZE);
                return m_Stream;
            }
        }

        public void ResetStream()
        {
            Stream.Position = 0L;
            Stream.SetLength(0L);
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_Stream != null)
                {
                    m_Stream.Dispose();
                    m_Stream = null;
                }
            }

            m_Disposed = true;
        }

        #endregion

        public void CleanValue()
        {
            tag = 0;
            pX = pY = pZ =
                rX = rY = rZ = rW = 0f;
            sX = sY = sZ = 1f;
        }

        public void Dispose()
        {
            CleanValue();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Merge(ref TransformInfo info)
        {
            if (info.IsChoose(SyncTag.TI_POS_X))
            {
                pX = info.pX;
                Choose(SyncTag.TI_POS_X);
            }

            if (info.IsChoose(SyncTag.TI_POS_Y))
            {
                pY = info.pY;
                Choose(SyncTag.TI_POS_Y);
            }

            if (info.IsChoose(SyncTag.TI_POS_Z))
            {
                pZ = info.pZ;
                Choose(SyncTag.TI_POS_Z);
            }


            if (info.IsChoose(SyncTag.TI_ROTATION_X))
            {
                rX = info.rX;
                Choose(SyncTag.TI_ROTATION_X);
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_Y))
            {
                rY = info.rY;
                Choose(SyncTag.TI_ROTATION_Y);
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_Z))
            {
                rZ = info.rZ;
                Choose(SyncTag.TI_ROTATION_Z);
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_W))
            {
                rW = info.rW;
                Choose(SyncTag.TI_ROTATION_W);
            }


            if (info.IsChoose(SyncTag.TI_SCALE_X))
            {
                sX = info.sX;
                Choose(SyncTag.TI_SCALE_X);
            }

            if (info.IsChoose(SyncTag.TI_SCALE_Y))
            {
                sY = info.sY;
                Choose(SyncTag.TI_SCALE_Y);
            }

            if (info.IsChoose(SyncTag.TI_SCALE_Z))
            {
                sZ = info.sZ;
                Choose(SyncTag.TI_SCALE_Z);
            }
        }

        public void Merge(long _tag, float _pX, float _pY, float _pZ
            , float _rX, float _rY, float _rZ, float _rW
            , float _sX, float _sY, float _sZ)
        {
            if (TransformSyncUtils.IsChoose(SyncTag.TI_POS_X, _tag))
            {
                pX = _pX;
                Choose(SyncTag.TI_POS_X);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_POS_Y, _tag))
            {
                pY = _pY;
                Choose(SyncTag.TI_POS_Y);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_POS_Z, _tag))
            {
                pZ = _pZ;
                Choose(SyncTag.TI_POS_Z);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_ROTATION_X, _tag))
            {
                rX = _rX;
                Choose(SyncTag.TI_ROTATION_X);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_ROTATION_Y, _tag))
            {
                rY = _rY;
                Choose(SyncTag.TI_ROTATION_Y);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_ROTATION_Z, _tag))
            {
                rZ = _rZ;
                Choose(SyncTag.TI_ROTATION_Z);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_ROTATION_W, _tag))
            {
                rW = _rW;
                Choose(SyncTag.TI_ROTATION_W);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_SCALE_X, _tag))
            {
                sX = _sX;
                Choose(SyncTag.TI_SCALE_X);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_SCALE_Y, _tag))
            {
                sY = _sY;
                Choose(SyncTag.TI_SCALE_Y);
            }

            if (TransformSyncUtils.IsChoose(SyncTag.TI_SCALE_Z, _tag))
            {
                sZ = _sZ;
                Choose(SyncTag.TI_SCALE_Z);
            }
        }

        public byte[] ToBytes(bool isFixedNumber = true)
        {
            try
            {
                ResetStream();
                if (isFixedNumber)
                {
                    TransformSyncUtils.WriteStreamFixedNumber(Stream, ref this);
                }
                else
                {
                    TransformSyncUtils.WriteStream(Stream, ref this);
                }

                byte[] bytes = Stream.ToArray();
                return bytes;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        public void Fill(byte[] bytes, bool isFixedNumber = true)
        {
            try
            {
                ResetStream();
                Stream.Read(bytes, 0, bytes.Length);
                if (isFixedNumber)
                {
                    TransformSyncUtils.ReadStreamFixedNumber(Stream, ref this);
                }
                else
                {
                    TransformSyncUtils.ReadStream(Stream, ref this);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static TransformInfo Create(byte[] bytes)
        {
            TransformInfo info = new TransformInfo();
            info.Fill(bytes);
            return info;
        }

        #region Tag

        public void Choose(SyncTag index)
        {
            Choose((int) index);
        }

        public void UnChoose(SyncTag index)
        {
            UnChoose((int) index);
        }

        public bool IsChoose(SyncTag index)
        {
            return IsChoose((int) index);
        }


        private void Choose(int index)
        {
            if (IsChoose(index))
                return;
            tag = TransformSyncUtils.Choose(index, tag);
            if (chooseList == null)
                chooseList = new List<SyncTag>();
            chooseList.Add((SyncTag) index);
        }

        private void UnChoose(int index)
        {
            if (!IsChoose(index))
                return;
            tag = TransformSyncUtils.UnChoose(index, tag);

            if (chooseList == null)
                chooseList = new List<SyncTag>();
            chooseList.Remove((SyncTag) index);
        }

        private bool IsChoose(int index)
        {
            return TransformSyncUtils.IsChoose(index, tag);
        }

        public List<SyncTag> chooseList;

        #endregion

        public bool IsNan(SyncTag tag)
        {
            switch (tag)
            {
                case SyncTag.TI_POS_X:
                    return float.IsNaN(pX);
                case SyncTag.TI_POS_Y:
                    return float.IsNaN(pY);
                case SyncTag.TI_POS_Z:
                    return float.IsNaN(pZ);
                case SyncTag.TI_ROTATION_X:
                    return float.IsNaN(rX);
                case SyncTag.TI_ROTATION_Y:
                    return float.IsNaN(rY);
                case SyncTag.TI_ROTATION_Z:
                    return float.IsNaN(rZ);
                case SyncTag.TI_ROTATION_W:
                    return float.IsNaN(rW);
                case SyncTag.TI_SCALE_X:
                    return float.IsNaN(sX);
                case SyncTag.TI_SCALE_Y:
                    return float.IsNaN(sY);
                case SyncTag.TI_SCALE_Z:
                    return float.IsNaN(sZ);
            }

            return false;
        }

        public float GetValue(SyncTag tag)
        {
            switch (tag)
            {
                case SyncTag.TI_POS_X:
                    return pX;
                case SyncTag.TI_POS_Y:
                    return pY;
                case SyncTag.TI_POS_Z:
                    return pZ;
                case SyncTag.TI_ROTATION_X:
                    return rX;
                case SyncTag.TI_ROTATION_Y:
                    return rY;
                case SyncTag.TI_ROTATION_Z:
                    return rZ;
                case SyncTag.TI_ROTATION_W:
                    return rW;
                case SyncTag.TI_SCALE_X:
                    return sX;
                case SyncTag.TI_SCALE_Y:
                    return sY;
                case SyncTag.TI_SCALE_Z:
                    return sZ;
            }

            return float.NaN;
        }

        public void SetValue(SyncTag tag, float value)
        {
            switch (tag)
            {
                case SyncTag.TI_POS_X:
                    pX = value;
                    break;
                case SyncTag.TI_POS_Y:
                    pY = value;
                    break;
                case SyncTag.TI_POS_Z:
                    pZ = value;
                    break;
                case SyncTag.TI_ROTATION_X:
                    rX = value;
                    break;
                case SyncTag.TI_ROTATION_Y:
                    rY = value;
                    break;
                case SyncTag.TI_ROTATION_Z:
                    rZ = value;
                    break;
                case SyncTag.TI_ROTATION_W:
                    rW = value;
                    break;
                case SyncTag.TI_SCALE_X:
                    sX = value;
                    break;
                case SyncTag.TI_SCALE_Y:
                    sY = value;
                    break;
                case SyncTag.TI_SCALE_Z:
                    sZ = value;
                    break;
            }
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
            // return base.ToString();
        }
    }
}