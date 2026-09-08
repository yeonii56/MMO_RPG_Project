using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

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
            GameObject[] maps = Resources.LoadAll<GameObject>("03_Prefabs/Map");                     
            
            foreach (GameObject map in maps)
            {
                Tilemap tm = Util.FindChild<Tilemap>(map, "Tilemap_Env", true);
                if (tm == null)
                {
                    Debug.LogError("tm null");
                    return;
                }

                // 파일 생성
                using (var writer = File.CreateText($"Assets/06_Data/Map/{tm.name}.txt"))
                {
                    tm.CompressBounds();
                    writer.WriteLine(tm.cellBounds.xMin);
                    writer.WriteLine(tm.cellBounds.xMax);
                    writer.WriteLine(tm.cellBounds.yMin);
                    writer.WriteLine(tm.cellBounds.yMax);

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
}

#endif