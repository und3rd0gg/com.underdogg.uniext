using System;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class RandomExt
    {
        public static bool NextBoolean(this Random random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            return random.Next() > int.MaxValue / 2;
        }

        public static bool RandomBoolOptions(this int chance, int options = 101)
        {
            if (options <= 1)
                throw new ArgumentOutOfRangeException(nameof(options), "Options must be greater than 1.");

            if (chance <= 0)
                return false;

            if (chance >= options)
                return true;

            var result = UnityEngine.Random.Range(1, options);
            return result < chance;
        }
    }
}
