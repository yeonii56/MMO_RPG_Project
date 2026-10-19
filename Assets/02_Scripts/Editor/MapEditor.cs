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
            List<GameObject> maps  = GetAssetByLabel<GameObject>("Map");

            foreach (GameObject map in maps)
            {
                Tilemap bas = Util.FindChild<Tilemap>(map, "Tilemap_Base", true);
                Tilemap env = Util.FindChild<Tilemap>(map, "Tilemap_Env", true);
                // Tilemap interact = Util.FindChild<Tilemap>(map, "interact", true);

                if (env == null)
                {
                    Debug.LogError("tm null");
                    return;
                }

                // 파일 생성
                using (var writer = File.CreateText($"Assets/06_Data/Map/MapCollision_{map.name.Split("_")[1]}.txt"))
                {
                    bas.CompressBounds();
                    writer.WriteLine(bas.cellBounds.xMin);
                    writer.WriteLine(bas.cellBounds.xMax);
                    writer.WriteLine(bas.cellBounds.yMin);
                    writer.WriteLine(bas.cellBounds.yMax);
                     
                    // min은 포함. max는 비포함
                    for (int y = bas.cellBounds.yMax - 1; y >= bas.cellBounds.yMin; y--)
                    {
                        for (int x = bas.cellBounds.xMin; x < bas.cellBounds.xMax; x++)
                        {
                            TileBase envTile = env.GetTile(new Vector3Int(x, y, 0));
                            // TileBase interactTile = potal.GetTile(new Vector3Int(x, y, 0));

/*                            if (interactTile != null)
                                writer.Write(2);
                            else*/ if (envTile != null)
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