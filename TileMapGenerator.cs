using NaughtyAttributes;
using System.Text;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    #region Variables

    public DungeonGenerator dungeonGenerator;
    
    public GameObject[] assets = new GameObject[16];
    private int[,] tileMap;
    #endregion

    private void Start()
    {
        dungeonGenerator = FindAnyObjectByType<DungeonGenerator>();
    }

    [Button]
    public void GenerateTileMap()
    {
        if (dungeonGenerator == null)
        {
            Debug.LogError("DungeonGenerator reference is null!", this);
            return;
        }

        tileMap = new int[dungeonGenerator.masterRoom.height, dungeonGenerator.masterRoom.width];

        foreach (var room in dungeonGenerator.rooms)
        {
            AlgorithmsUtils.FillRectangleOutline(tileMap, room, 1);
        }

        foreach (RectInt door in dungeonGenerator.doors)
        {
            AlgorithmsUtils.FillRectangleOutline(tileMap, door, 0);
        }
    }

    public string ToString(bool flip)
    {
        if (tileMap == null) return "Tile map not generated yet.";

        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);

        var sb = new StringBuilder();

        int start = flip ? rows - 1 : 0;
        int end = flip ? -1 : rows;
        int step = flip ? -1 : 1;

        for (int i = start; i != end; i += step)
        {
            for (int j = 0; j < colss; j++)
            {
                sb.Append((tileMap[i, j] == 0 ? '0' : '#'));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public int[,] GetTileMap()
    {
        return tileMap.Clone() as int[,];
    }

    [Button]
    public void PrintTileMap()
    {
        Debug.Log(ToString(true));
    }

    static int Compute2x2Bitmask(int[,] map, int row, int col)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        int bottomLeft = map[row, col] != 0 ? 8 : 0;  
        int bottomRight = map[row, col + 1] != 0 ? 1 : 0;  
        int topRight = map[row + 1, col + 1] != 0 ? 2 : 0; 
        int topLeft = map[row + 1, col] != 0 ? 4 : 0;

        return bottomLeft | bottomRight | topRight | topLeft;
    }

    [Button]
    public void PlaceAssets()
    {
        GameObject parentRoom = new GameObject("RoomAssets");

        if (tileMap == null)
        {
            Debug.LogError("TileMap not generated yet. Call GenerateTileMap() first.");
            return;
        }

        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);

        for (int row = 0; row < rows - 1; row++)
        {
            for (int col = 0; col < cols - 1; col++)
            {
                int code = Compute2x2Bitmask(tileMap, row, col);

                GameObject prefab = (code >= 0 && code < assets.Length) ? assets[code] : null;

                if (prefab != null)
                {
                    Vector3 worldPos = new Vector3(col + 1, 0f, row + 1);
                    Instantiate(prefab, worldPos, Quaternion.identity, parentRoom.transform);
                }
            }
        }

        Debug.Log("Assets are placed");
    }
}
