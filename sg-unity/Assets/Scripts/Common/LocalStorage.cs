using UnityEngine;

namespace Common
{
    public static partial class LocalStorage
    {
        public static string GetString(string key, string value = default)
        {
            return PlayerPrefs.GetString(key, value);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static int GetInt(string key, int value = default)
        {
            return PlayerPrefs.GetInt(key, value);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static float GetFloat(string key, float value = default)
        {
            return PlayerPrefs.GetFloat(key, value);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
    }
}