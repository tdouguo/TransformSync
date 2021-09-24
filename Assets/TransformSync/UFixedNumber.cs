namespace TransformSync
{
    /// <summary>
    /// 定点数
    /// https://blog.csdn.net/weixin_35343226/article/details/112643511
    /// </summary>
    public static class UFixedNumber
    {
        /// <summary>
        /// 位运算移动标准长度
        /// </summary>
        public const int kUNIT_NUM = 16;

        /// <summary>
        /// 定点数单位1值
        /// </summary>
        public const long kONE = 1 << 16;

        /// <summary>
        /// 从int和long值构造
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Create(long value)
        {
            return value << kUNIT_NUM;
        }

        /// <summary>
        /// 从float值构造
        /// </summary>
        /// <param name="floatValue"></param>
        /// <returns></returns>
        public static long CreateFromFloat(float floatValue)
        {
            return (long) ((double) floatValue * kONE);
        }

        /// <summary>
        /// 从double值构造
        /// </summary>
        /// <param name="doubleValue"></param>
        /// <returns></returns>
        public static long CreateFromDouble(float doubleValue)
        {
            return (long) (doubleValue * kONE);
        }


        /// <summary>
        /// 从定点数转换回本来的int值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this long value)
        {
            return (int) (value >> kUNIT_NUM);
        }

        /// <summary>
        /// 从定点数转换回原来的Double值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this long value)
        {
            return (double) value / kONE;
        }

        /// <summary>
        /// 从定点数转换回原来的Double值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat(this long value)
        {
            return (float) ToDouble(value);
        }
    }
}