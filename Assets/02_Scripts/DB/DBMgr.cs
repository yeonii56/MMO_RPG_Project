using System;
using UnityEngine;
using Newtonsoft.Json;
using Manager;

public static class DBMgr
{
    public static Map_TB Map_TB { get; set; }

    public static void Init()
    {
        Map_TB = DeserializeDB<Map_TB>(Manager.AddressableMgr.Instance.GetAsset<TextAsset>("Map_TB").text);

        AddressableMgr.Instance.ReleaseAsset("Map_TB");
    }

    public static T DeserializeDB<T>(string text)
    {
        try
        {
            T t  = JsonConvert.DeserializeObject<T>(text);
            return t;
        }
        catch (SystemException e)
        {
            Debug.LogError(e.Message);
        }

        return default;
    }
}
