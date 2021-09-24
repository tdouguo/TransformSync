using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TransformSync
{
    public enum SyncTag : ushort
    {
        TI_POS_X = 1, // 0000 0000 0000 0001
        TI_POS_Y = 2, // 0000 0000 0000 0010
        TI_POS_Z = 4, // 0000 0000 0000 0100
        TI_ROTATION_X = 8, // 0000 0000 0000 1000
        TI_ROTATION_Y = 16, // 0000 0000 0001 0000
        TI_ROTATION_Z = 32, // 0000 0000 0010 0000
        TI_ROTATION_W = 64, // 0000 0000 0100 0000
        TI_SCALE_X = 128, // 0000 0000 1000 0000
        TI_SCALE_Y = 256, // 0000 0001 0000 0000
        TI_SCALE_Z = 512, // 0000 0010 0000 0000
    }

    public static class TransformSyncUtils
    {
        public static SyncTag[] TI_Arrays = new[]
        {
            SyncTag.TI_POS_X,
            SyncTag.TI_POS_Y,
            SyncTag.TI_POS_Z,

            SyncTag.TI_ROTATION_X,
            SyncTag.TI_ROTATION_Y,
            SyncTag.TI_ROTATION_Z,
            SyncTag.TI_ROTATION_W,

            SyncTag.TI_SCALE_X,
            SyncTag.TI_SCALE_Y,
            SyncTag.TI_SCALE_Z
        };


        /// <summary>
        /// 创建Data
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TransformInfo Create(Transform target)
        {
            TransformInfo info = new TransformInfo();
            Create(target, ref info);
            return info;
        }

        /// <summary>
        /// 创建Data
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TransformInfo Create(Transform target, ref TransformInfo info)
        {
            // pos
            var position = target.position;
            info.pX = position.x;
            info.pY = position.y;
            info.pZ = position.z;

            // rotation
            var rotation = target.rotation;
            info.rX = rotation.x;
            info.rY = rotation.y;
            info.rZ = rotation.z;
            info.rW = rotation.w;

            // scale
            var localScale = target.localScale;
            info.sX = localScale.x;
            info.sY = localScale.y;
            info.sZ = localScale.z;


            for (var i = 0; i < TI_Arrays.Length; i++)
            {
                if (!info.IsNan(TI_Arrays[i]))
                    info.Choose(TI_Arrays[i]);
            }

            return info;
        }

        /// <summary>
        /// 增量 创建Data
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TransformInfo CreateAttit(Transform target, ref TransformInfo lastInfo, ref TransformInfo info)
        {
            float distance = 0.02f; // 误差

            // pos
            if (Mathf.Abs(lastInfo.pX - target.position.x) > distance)
            {
                info.pX = target.position.x;
                info.Choose(SyncTag.TI_POS_X);
            }

            if (Mathf.Abs(lastInfo.pY - target.position.y) > distance)
            {
                info.pY = target.position.y;
                info.Choose(SyncTag.TI_POS_Y);
            }

            if (Mathf.Abs(lastInfo.pZ - target.position.z) > distance)
            {
                info.pZ = target.position.z;
                info.Choose(SyncTag.TI_POS_Z);
            }

            // rotation
            if (Mathf.Abs(lastInfo.rX - target.rotation.x) > distance)
            {
                info.rX = target.rotation.x;
                info.Choose(SyncTag.TI_ROTATION_X);
            }

            if (Mathf.Abs(lastInfo.rY - target.rotation.y) > distance)
            {
                info.rY = target.rotation.y;
                info.Choose(SyncTag.TI_ROTATION_Y);
            }

            if (Mathf.Abs(lastInfo.rZ - target.rotation.z) > distance)
            {
                info.rZ = target.rotation.z;
                info.Choose(SyncTag.TI_ROTATION_Z);
            }

            if (Mathf.Abs(lastInfo.rW - target.rotation.w) > distance)
            {
                info.rW = target.rotation.w;
                info.Choose(SyncTag.TI_ROTATION_W);
            }

            // scale
            if (Mathf.Abs(lastInfo.sX - target.localScale.x) > distance)
            {
                info.sX = target.localScale.x;
                info.Choose(SyncTag.TI_SCALE_X);
            }

            if (Mathf.Abs(lastInfo.sY - target.localScale.y) > distance)
            {
                info.sY = target.localScale.y;
                info.Choose(SyncTag.TI_SCALE_Y);
            }

            if (Mathf.Abs(lastInfo.sZ - target.localScale.z) > distance)
            {
                info.sZ = target.localScale.z;
                info.Choose(SyncTag.TI_SCALE_Z);
            }

            return info;
        }

        /// <summary>
        /// 设置Info数据到Transform
        /// </summary>
        /// <param name="target"></param>
        /// <param name="info"></param>
        public static void Set(Transform target, ref TransformInfo info)
        {
            Vector3 pos = target.position;
            if (info.IsChoose(SyncTag.TI_POS_X))
            {
                pos.x = info.pX;
            }

            if (info.IsChoose(SyncTag.TI_POS_Y))
            {
                pos.y = info.pY;
            }

            if (info.IsChoose(SyncTag.TI_POS_Z))
            {
                pos.z = info.pZ;
            }

            target.position = pos;

            Quaternion rotation = target.rotation;
            if (info.IsChoose(SyncTag.TI_ROTATION_X))
            {
                rotation.x = info.rX;
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_Y))
            {
                rotation.y = info.rY;
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_Z))
            {
                rotation.z = info.rZ;
            }

            if (info.IsChoose(SyncTag.TI_ROTATION_W))
            {
                rotation.w = info.rW;
            }

            target.rotation = rotation;

            Vector3 scale = target.localScale;
            if (info.IsChoose(SyncTag.TI_SCALE_X))
            {
                scale.x = info.sX;
            }

            if (info.IsChoose(SyncTag.TI_SCALE_Y))
            {
                scale.y = info.sY;
            }

            if (info.IsChoose(SyncTag.TI_SCALE_Z))
            {
                scale.z = info.sZ;
            }

            target.localScale = scale;
        }

        #region TransformInfo 逻辑方法

        public static long Choose(int index, long tag)
        {
            // Debug.LogFormat("Choose Begin: tag:{0} index:{1}", tag, index);
            tag |= (1 << index);
            // Debug.LogFormat("Choose End: tag:{0} index:{1}", tag, index);
            return tag;
        }

        public static long UnChoose(int index, long tag)
        {
            // Debug.LogFormat("UnChoose Begin: tag:{0} index:{1}", tag, index);
            tag &= ~(1 << index);
            // Debug.LogFormat("UnChoose End: tag:{0} index:{1}", tag, index);
            return tag;
        }

        public static bool IsChoose(int index, long tag)
        {
            long isHave = tag & (1 << index);
            return isHave != 0;
        }


        public static bool IsChoose(SyncTag index, long tag)
        {
            return IsChoose(0, 0);
        }

        public static long Choose(SyncTag index, long tag)
        {
            return Choose((int) index, tag);
        }

        public static long UnChoose(SyncTag index, long tag)
        {
            return UnChoose((int) index, tag);
        }

        #endregion

        #region 流操作

        public static void WriteStream(Stream stream, long tag
            , float pX, float pY, float pZ
            , float rX, float rY, float rZ, float rW
            , float sX, float sY, float sZ)
        {
            byte[] bytes = GetBytes(tag);
            stream.Write(bytes, 0, bytes.Length);

            if (IsChoose(SyncTag.TI_POS_X, tag))
            {
                bytes = GetBytes(pX);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_POS_Y, tag))
            {
                bytes = GetBytes(pY);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_POS_Z, tag))
            {
                bytes = GetBytes(pZ);
                stream.Write(bytes, 0, bytes.Length);
            }


            if (IsChoose(SyncTag.TI_ROTATION_X, tag))
            {
                bytes = GetBytes(rX);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_ROTATION_Y, tag))
            {
                bytes = GetBytes(rY);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_ROTATION_Z, tag))
            {
                bytes = GetBytes(rZ);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_ROTATION_W, tag))
            {
                bytes = GetBytes(rW);
                stream.Write(bytes, 0, bytes.Length);
            }


            if (IsChoose(SyncTag.TI_SCALE_X, tag))
            {
                bytes = GetBytes(sX);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_SCALE_Y, tag))
            {
                bytes = GetBytes(sY);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (IsChoose(SyncTag.TI_SCALE_Z, tag))
            {
                bytes = GetBytes(sZ);
                stream.Write(bytes, 0, bytes.Length);
            }

            stream.Flush();
        }

        public static void ReadStream(Stream stream, ref long tag
            , ref float pX, ref float pY, ref float pZ
            , ref float rX, ref float rY, ref float rZ, ref float rW
            , ref float sX, ref float sY, ref float sZ)
        {
            int offset = 0;

            tag = ReadStreamInt64(stream, offset, out offset);

            if (IsChoose(SyncTag.TI_POS_X, tag))
            {
                pX = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_POS_Y, tag))
            {
                pY = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_POS_Z, tag))
            {
                pZ = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_ROTATION_X, tag))
            {
                rX = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_ROTATION_Y, tag))
            {
                rY = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_ROTATION_Z, tag))
            {
                rZ = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_ROTATION_W, tag))
            {
                rW = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_SCALE_X, tag))
            {
                sX = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_SCALE_Y, tag))
            {
                sY = ReadStreamFloat(stream, offset, out offset);
            }

            if (IsChoose(SyncTag.TI_SCALE_Z, tag))
            {
                sZ = ReadStreamFloat(stream, offset, out offset);
            }
        }


        private static byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        private static byte[] GetBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }


        public static void WriteStream(Stream stream, ref TransformInfo info)
        {
            byte[] bytes = GetBytes(info.tag);
            stream.Write(bytes, 0, bytes.Length);

            for (var i = 0; i < TI_Arrays.Length; i++)
            {
                if (info.IsChoose(TI_Arrays[i]))
                {
                    float value = info.GetValue(TI_Arrays[i]);
                    bytes = GetBytes(value);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            stream.Flush();
        }

        public static void ReadStream(Stream stream, ref TransformInfo info)
        {
            int offset = 0;
            long tag = ReadStreamInt64(stream, offset, out offset);

            for (var i = 0; i < TI_Arrays.Length; i++)
            {
                if (IsChoose(TI_Arrays[i], tag))
                {
                    float value = ReadStreamFloat(stream, offset, out offset);
                    info.SetValue(TI_Arrays[i], value);
                    info.Choose(TI_Arrays[i]);
                }
            }
        }


        #region 定点数操作

        private static byte[] GetBytesFixedNumber(float value)
        {
            long longValue = UFixedNumber.CreateFromFloat(value);
            return BitConverter.GetBytes(longValue);
        }

        public static void WriteStreamFixedNumber(Stream stream, ref TransformInfo info)
        {
            long tag = info.tag;
            if (tag == 0)
                return;

            byte[] bytes = GetBytes(tag);
            stream.Write(bytes, 0, bytes.Length);
            for (var i = 0; i < TI_Arrays.Length; i++)
            {
                if (info.IsChoose(TI_Arrays[i]))
                {
                    float value = info.GetValue(TI_Arrays[i]);
                    bytes = GetBytesFixedNumber(value);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            stream.Flush();
        }

        public static void ReadStreamFixedNumber(Stream stream, ref TransformInfo info)
        {
            int offset = 0;

            long tag = ReadStreamInt64(stream, offset, out offset);
            if (tag == 0)
            {
                return;
            }

            for (var i = 0; i < TI_Arrays.Length; i++)
            {
                if (IsChoose(TI_Arrays[i], tag))
                {
                    float value = ReadStreamFloatFixedNumber(stream, offset, out offset);
                    info.SetValue(TI_Arrays[i], value);
                    info.Choose(TI_Arrays[i]);
                }
            }
        }

        private static float ReadStreamFloatFixedNumber(Stream stream, int offset, out int outOffset)
        {
            ReadBytes(stream, ref TempBytes8, offset, out outOffset);
            long value = BitConverter.ToInt64(TempBytes8, 0);
            return value.ToFloat();
        }

        #endregion

        #endregion

        #region Tool

        public static Type TAG_TYPE = typeof(long);
        public const int TAG_SIZE = sizeof(long);

        public static Type POS_TYPE = typeof(float);
        public const int POS_SIZE = sizeof(float);

        public static Type ROTATION_TYPE = typeof(float);
        public const int ROTATION_SIZE = sizeof(float);

        public static Type SCALE_TYPE = typeof(float);
        public const int SCALE_SIZE = sizeof(float);

        public const int MAX_SIZE = TAG_SIZE + (POS_SIZE * 3) + (ROTATION_SIZE * 4) + (SCALE_SIZE * 3);


        private static byte[] TempBytes2 = new byte[2];
        private static byte[] TempBytes4 = new byte[4];
        private static byte[] TempBytes8 = new byte[8];

        private static byte[] CleanBytes(ref byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0;
            }

            return bytes;
        }

        private static byte[] ReadBytes(Stream stream, ref byte[] bytes, int offset, out int outOffset)
        {
            bytes = CleanBytes(ref bytes);
            stream.Read(bytes, offset, bytes.Length);
            outOffset = offset + bytes.Length;
            return bytes;
        }


        private static ushort ReadStreamUInt16(Stream stream, int offset, out int outOffset)
        {
            ReadBytes(stream, ref TempBytes2, offset, out outOffset);
            ushort value = BitConverter.ToUInt16(TempBytes2, 0);
            return value;
        }

        private static long ReadStreamInt64(Stream stream, int offset, out int outOffset)
        {
            ReadBytes(stream, ref TempBytes8, offset, out outOffset);
            long value = BitConverter.ToInt64(TempBytes8, 0);
            return value;
        }

        private static float ReadStreamFloat(Stream stream, int offset, out int outOffset)
        {
            ReadBytes(stream, ref TempBytes4, offset, out outOffset);
            float value = BitConverter.ToSingle(TempBytes4, 0);
            return value;
        }

        #endregion
    }
}