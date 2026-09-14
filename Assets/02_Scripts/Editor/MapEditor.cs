using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;

public class MapEditor
{
    [MenuItem("Tools/GanerateMap")]
    private static void GanerateMap()
    {
        if (EditorUtility.DisplayDialog("GanerateMap", "Create?", "Ok", "Cancle"))
        {
            // 어드레서블로 바꾸기
            //GameObject[] maps = Resources.LoadAll<GameObject>("03_Prefabs/Map");                     
            List<GameObject> maps = GetAssetByLabel<GameObject>("Map");

            foreach (GameObject map in maps)
            {
                Tilemap tm = Util.FindChild<Tilemap>(map, "Tilemap_Env", true);
                if (tm == null)
                {
                    Debug.LogError("tm null");
                    return;
                }

                // 파일 생성
                using (var writer = File.CreateText($"Assets/06_Data/Map/{map.name.Split("_")[1]}.txt"))
                {
                    tm.CompressBounds();
                    writer.WriteLine(tm.cellBounds.xMin);
                    writer.WriteLine(tm.cellBounds.xMax);
                    writer.WriteLine(tm.cellBounds.yMin);
                    writer.WriteLine(tm.cellBounds.yMax);
                     
                    // min은 포함. max는 비포함
                    for (int y = tm.cellBounds.yMax - 1; y >= tm.cellBounds.yMin; y--)
                    {
                        for (int x = tm.cellBounds.xMin; x < tm.cellBounds.xMax; x++)
                        {
                            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                            if (tile != null)
                                writer.Write(1);
                            else
                                writer.Write(0);
                        }
                        writer.WriteLine();
                    }
                }
            }           
        }
    }

    private static List<T> GetAssetByLabel<T>(string label) where T : UnityEngine.Object
    {
        List<T> list = new List<T>();

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings를 찾을 수 없습니다.");
            return null;
        }

        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (entry.labels.Contains(label))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                    if (asset != null)
                        list.Add(asset);
                }
            }
        }
        return list;
    }
}

#endif