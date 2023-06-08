using System;

namespace UniExt
{
    public static class RandomExt
    {
        public static bool NextBoolean(this Random random) =>
            random.Next() > int.MaxValue / 2;
    }
}