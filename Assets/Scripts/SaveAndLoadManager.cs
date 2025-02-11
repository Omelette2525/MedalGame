using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* long型も扱えるようにplayerprefs + 追加の関数 を持ったクラスを作成 */
[Serializable]
public class SaveAndLoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    public static void SetInt(string key, int setValue)
    {
        PlayerPrefs.SetInt(key, setValue);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public static void SetFloat(string key, float setValue)
    {
        PlayerPrefs.SetFloat(key, setValue);
    }

    public static string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    public static void SetString(string key, string setValue)
    {
        PlayerPrefs.SetString(key, setValue);
    }

    public static long GetLong(string key, long defaultValue)
    {
        string value = PlayerPrefs.GetString(key, "null");
        /* 初期文字列が入っていたら、データが入っていなかったのでdefaultValueを返す */
        if(value == "null")
        {
            return defaultValue;
        }
        else
        {
            return long.Parse(value); // long型に変換して返す
        }
    }
    public static void SetLong(string key, long setValue)
    {
        PlayerPrefs.SetString(key, setValue.ToString()); // stringにして保存
    }
}
