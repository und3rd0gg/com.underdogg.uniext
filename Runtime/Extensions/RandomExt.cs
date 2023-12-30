using System;

namespace UniExt.Extensions
{
    public static class RandomExt
    {
        public static bool NextBoolean(this Random random) =>
            random.Next() > int.MaxValue / 2;
        
        public static bool RandomBoolOptions(this int chance, int options = 101) {
            var result = UnityEngine.Random.Range(1, options);
            //Debug.Log("шанс: " + chance + " из " + (options-1) + ". Выпало: " + result);
            return result < chance;
        }
        
    }
}