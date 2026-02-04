using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
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
            for (int j = 0; j < cols; j++)
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

    static int Compute2x2Bitmask(int[,] map, int row, int col) //assigns a value to 4 marching squares
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        int bottomRight = map[row, col + 1] != 0 ? 1 : 0;  //if it is not 0, set the bit to 1
        int topRight = map[row + 1, col + 1] != 0 ? 2 : 0; //if it is not 0, set the bit to 2
        int topLeft = map[row + 1, col] != 0 ? 4 : 0; //if it is not 0, set the bit to 4
        int bottomLeft = map[row, col] != 0 ? 8 : 0; //if it is not 0, set the bit to 8

        return bottomLeft | bottomRight | topRight | topLeft; //it calculates the whole value
    }

    public void StartFloodFillFloor()
    {
        StartCoroutine(FloodFillFloor());
    }

    IEnumerator FloodFillFloor() //DFS for placing the floor asset
    {
        FloodFillDone = false;

        if (tileMap == null)
        {
            Debug.LogError("TileMap not generated yet.");
            yield break;
        }

        GameObject parent = new GameObject("FloodFilledFloor");

        RectInt startRoom = dungeonGenerator.rooms[Random.Range(0, dungeonGenerator.rooms.Count)]; //selects a random room from the "rooms" list
        Vector2Int start = new Vector2Int(startRoom.x + startRoom.width / 2, startRoom.y + startRoom.height / 2); //calculates the center of that room

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            Vector3 worldPos = new Vector3(current.x + 1, 0f, current.y + 1); //calculate the correct world position
            Instantiate(assets[0], worldPos, Quaternion.identity, parent.transform); //place the asset

            yield return new WaitForSeconds(0.01f);

            foreach (var dir in Directions)
            {
                Vector2Int neighbor = current + dir;

                if (!IsInsideTileMap(neighbor)) continue; //Skip if outside the dungeon
                if (visited.Contains(neighbor)) continue; //Skip if visited

                if (tileMap[neighbor.y, neighbor.x] != 0) continue;

                visited.Add(neighbor);
                stack.Push(neighbor);
            }
        }

        Debug.Log("Flood fill floor completed.");

        FloodFillDone = true;
    }

    static readonly Vector2Int[] Directions = new Vector2Int[]
    {
    new Vector2Int(1, 0),  // right
    new Vector2Int(-1, 0), // left
    new Vector2Int(0, 1),  // up
    new Vector2Int(0, -1), // down
    };

    bool IsInsideTileMap(Vector2Int pos)
    {
        return pos.y >= 0 && pos.y < tileMap.GetLength(0) &&
               pos.x >= 0 && pos.x < tileMap.GetLength(1);
    }

    public bool FloodFillDone { get; private set; }

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
                int code = Compute2x2Bitmask(tileMap, row, col); //calculate the value of 4 squares

                if (code == 0) continue;

                GameObject prefab = (code >= 0 && code < assets.Length) ? assets[code] : null; 

                if (prefab != null)
                {
                    Vector3 worldPos = new Vector3(col + 1, 0f, row + 1);
                    Instantiate(prefab, worldPos, Quaternion.identity, parentRoom.transform); //place the coresponding assets
                }
            }
        }

        Debug.Log("Assets are placed");
    }
}
