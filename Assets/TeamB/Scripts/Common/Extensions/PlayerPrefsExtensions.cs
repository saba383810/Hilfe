using UnityEngine;

namespace Fresh23_N.Scripts.Extensions
{
    public static class PlayerPrefsExtensions
    {
        public static void SetBool(string key, bool value)
        {
            var intValue = value ? 1 : 0;
            PlayerPrefs.SetInt(key, intValue);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            var intValue = PlayerPrefs.GetInt(key, -1);

            if (intValue == -1)
            {
                return defaultValue;
            }

            return intValue == 1;
        }
    }
}