using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    #region Variables

    #region RoomRelated
    public int overlap;
    public int maxRooms;
    public int minWidht;
    public int minHeight;
    public float timePerRoom;
    #endregion

    #region DungeonRelated
    public bool automaticGeneration = false;
    public float removePercentage;
    private int[] percentageSplit = { 5, 4, 3, 2, 1 };

    public int seed;
    public bool setSeed;

    public enum DungeonSize {Mini, Small, Medium, Large, Custom};

    [Tooltip("Mini - 50x50, Small - 100x100, Medim - 150x150, Large - 200x200, Custom - write your own positives values")]
    public DungeonSize dungeonSize;

    public RectInt masterRoom;
    public List<RectInt> rooms = new List<RectInt>();
    public List<RectInt> doors = new List<RectInt>();

    private Coroutine coroutine;
    #endregion

    #region GraphRelated

    bool generatingDoors = false;
    bool generatingGraph = false;
    bool graphDone = false;

    public DungeonGraph<RectInt> dungeonGraph = new DungeonGraph<RectInt>();

    private List<Vector3> nodePos = new List<Vector3>();
    private List<(Vector3, Vector3)> edges = new List<(Vector3, Vector3)>();
    #endregion

    #region PostDungeon

    NavMeshSurface navMeshSurface;
    TileMapGenerator tilemap;
    
    [SerializeField]GameObject player;

    #endregion

    #endregion

    void Start()
    {
        DungeonSizeGenerator(dungeonSize); //select dungeon size

        rooms.Add(masterRoom);

        if(automaticGeneration)
        {
            coroutine = StartCoroutine(GeneratingRooms());
        }

        if (masterRoom.width <= minWidht || masterRoom.height <= minHeight)
        {
            StopCoroutine(coroutine);
            Debug.LogError("Input positive values for wigth and height");
        }

        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        tilemap = FindAnyObjectByType<TileMapGenerator>();
    }

    void Update()
    {
        if (setSeed)
        {
            seed = seed.GetHashCode();
            Random.InitState(seed);
        }

        bool allRoomsAreTooSmall = true;

        foreach (var room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.green);

            if (room.width > minWidht * 2 || room.height > minHeight * 2)
            {
                allRoomsAreTooSmall = false;
            }

            if (room.width < minWidht || room.height < minHeight)
            {
                Debug.Log("Min reached");
            }
        }

        foreach (var door in doors)
        {
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }

        if (allRoomsAreTooSmall && coroutine != null && !generatingDoors) //cannot divide any room 
        {
            StopCoroutine(coroutine);
            coroutine = null;
            Debug.Log("All rooms are generated");

            generatingDoors = true;
            coroutine = StartCoroutine(GeneratingDoors());
        }

        if (generatingDoors && generatingGraph) //all doors are generated
        {
            StopCoroutine(coroutine);
            Debug.Log("All doors are generated");
            coroutine = null;

            generatingGraph = true;
            coroutine = StartCoroutine(BuildGraph());
        }

        if (graphDone) //the graph is complete
        {
            StartCoroutine(FinalizeGraphAndFlood());
            graphDone = false;
            Debug.Log("The graph is done");
            Debug.Log("All coroutines are finished");
        }
    }

    void DungeonSizeGenerator(DungeonSize dungeonSize)
    {
        switch(dungeonSize)
        {
            case DungeonSize.Mini:
                 masterRoom = new RectInt(0, 0, 50,50);
                break;
            case DungeonSize.Small:
                masterRoom = new RectInt(0, 0, 100, 100);
                break;
            case DungeonSize.Medium:
                masterRoom = new RectInt(0, 0, 150, 150);
                break;
            case DungeonSize.Large:
                masterRoom = new RectInt(0, 0, 200, 200);
                break;
            case DungeonSize.Custom:
                masterRoom = masterRoom;
                break;
        }
    }

    #region Coroutines
    IEnumerator GeneratingRooms()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(timePerRoom);

            int random = Random.Range(0, 2);

            if (rooms.Count < maxRooms)
            {
                if (random == 0)
                {
                    SplitingHorizontaly();
                }

                else
                {
                    SplitingVeriticaly();
                }
            }

            else
            {
                Debug.Log("Max amount of rooms is reached");
                break;
            }
        }
    }

    IEnumerator GeneratingDoors()
    {
        RectInt breakingRoom;
        if (!RemoveRooms(out breakingRoom))
        {
            Debug.LogWarning("Room removal stopped to keep graph connected. Problem room: " + breakingRoom);
            yield break;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                RectInt roomA = rooms[i];
                RectInt roomB = rooms[j];

                if (roomB.Overlaps(roomA))
                {
                    int xMin = Mathf.Max(roomA.xMin, roomB.xMin);
                    int xMax = Mathf.Min(roomA.xMax, roomB.xMax);
                    int yMin = Mathf.Max(roomA.yMin, roomB.yMin);
                    int yMax = Mathf.Min(roomA.yMax, roomB.yMax);

                    if (xMax - xMin == overlap)
                    {
                        int doorY = (yMin + yMax) / 2;

                        if (doorY - overlap * 2 > yMin && doorY + overlap * 2 < yMax)
                        {
                            RectInt door = new RectInt(xMin, doorY - overlap / 2, overlap, overlap * 2);
                            if (!doors.Contains(door))
                            {
                                doors.Add(door);
                            }
                        }
                    }

                    else if (yMax - yMin == overlap)
                    {
                        int doorX = (xMin + xMax) / 2;

                        if (doorX - overlap * 2 > xMin && doorX + overlap * 2 < xMax)
                        {
                            RectInt door = new RectInt(doorX - overlap / 2, yMin, overlap * 2, overlap);
                            if (!doors.Contains(door))
                            {
                                doors.Add(door);
                            }
                        }
                    }

                    yield return new WaitForSeconds(timePerRoom);
                }
            }
        }

        foreach (var door in doors)
        {
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }

        generatingGraph = true;
    }

    IEnumerator BuildGraph()
    {
        generatingGraph = false;
        dungeonGraph.Clear();
        nodePos.Clear();
        edges.Clear();

        foreach (var room in rooms)
        {
            Vector3 roomCenter = new Vector3(room.x + room.width / 2, 0, room.y + room.height / 2); //calculating the center of the room
            dungeonGraph.AddNode(room); //adding the room in a dictionary for placing a node
            nodePos.Add(roomCenter); //adding a node
            yield return new WaitForSeconds(timePerRoom);
        }

        foreach (var door in doors)
        {
            Vector3 doorCenter = new Vector3(door.x + door.width / 2, 0, door.y + door.height / 2); //calculating the center of the door
            dungeonGraph.AddNode(door); //adding the door in a dictionary for placing a node
            nodePos.Add(doorCenter); //adding a node
            yield return new WaitForSeconds(timePerRoom);
        }

        HashSet<RectInt> visited = new HashSet<RectInt>();
        Stack<RectInt> stack = new Stack<RectInt>();

        RectInt startNode = rooms[Random.Range(0, rooms.Count)];
        stack.Push(startNode);
        visited.Add(startNode);

        while (stack.Count > 0)
        {
            RectInt current = stack.Pop();
            Vector3 currentCenter = new Vector3(current.x + current.width / 2, 0, current.y + current.height / 2);

            foreach (var door in doors)
            {
                if (!current.Overlaps(door)) continue;

                Vector3 doorCenter = new Vector3(door.x + door.width / 2, 0, door.y + door.height / 2);

                foreach (var nextRoom in rooms)
                {
                    if (!nextRoom.Overlaps(door) || visited.Contains(nextRoom)) continue;

                    Vector3 nextRoomCenter = new Vector3(nextRoom.x + nextRoom.width / 2, 0, nextRoom.y + nextRoom.height / 2);

                    edges.Add((currentCenter, doorCenter));
                    yield return new WaitForSeconds(timePerRoom);

                    edges.Add((doorCenter, nextRoomCenter));
                    yield return new WaitForSeconds(timePerRoom);

                    visited.Add(nextRoom);
                    stack.Push(nextRoom);
                }
            }
        }
        RemoveDoors();
        graphDone = true;
    }

    IEnumerator FinalizeGraphAndFlood()
    {
        tilemap.GenerateTileMap();

        yield return null;

        tilemap.StartFloodFillFloor();

        yield return new WaitUntil(() => tilemap.FloodFillDone);

        tilemap.PlaceAssets();

        BuildNavMesh();
    }

    #endregion

    #region RoomSplit
    [Button]
    void SplitingVeriticaly()
    {
        List<int> validRooms = new List<int>();
        List<int> validSplits = new List<int>();

        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (int percentage in percentageSplit)
            {
                if (rooms[i].width * percentage / 10 >= minWidht  && (rooms[i].width - (rooms[i].width * percentage / 10)) >= minWidht)
                {
                    validRooms.Add(i);
                }
            }
        }

        if (validRooms.Count > 0)
        {
            int randomRoomY = validRooms[Random.Range(0, validRooms.Count)];

            RectInt roomY = rooms[randomRoomY];

            foreach (int percentage in percentageSplit)
            { 
                if (roomY.width * percentage / 10 >= minWidht && (roomY.width - roomY.width * percentage / 10) >= minWidht)
                {
                    validSplits.Add(percentage);
                }
            }

            int split = validSplits[Random.Range(0, validSplits.Count)];

            int width = (roomY.width * split) / 10;

            int splitX = roomY.x + width;

            RectInt left = new RectInt(roomY.x, roomY.y, width, roomY.height);
            RectInt right = new RectInt(splitX - overlap, roomY.y, roomY.width - left.width + overlap, roomY.height);

            rooms.RemoveAt(randomRoomY);
            rooms.Add(left);
            rooms.Add(right);
        }

        else
        {
            SplitingHorizontaly();
        }
    }

    [Button]
    void SplitingHorizontaly()
    {
        List<int> validRooms = new List<int>();
        List<int> validSplits = new List<int>();

        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (int percentage in percentageSplit)
            {
                if (rooms[i].height * percentage / 10 >= minHeight && (rooms[i].height - (rooms[i].height * percentage / 10)) >= minHeight)
                {
                    validRooms.Add(i);
                }
            }
        }

        if (validRooms.Count > 0)
        {
            int randomRoomX = validRooms[Random.Range(0, validRooms.Count)];

            RectInt roomX = rooms[randomRoomX];

            foreach (int percentage in percentageSplit)
            {
                if (roomX.height * percentage / 10 >= minHeight && (roomX.height - roomX.height * percentage / 10) >= minHeight)
                {
                    validSplits.Add(percentage);
                }
            }

            int split = validSplits[Random.Range(0, validSplits.Count)];

            int height = (roomX.height * split) / 10;

            int splitX = roomX.y + height;

            RectInt top = new RectInt(roomX.x, roomX.y, roomX.width, height);
            RectInt bottom = new RectInt(roomX.x, splitX - overlap, roomX.width, roomX.height - top.height + overlap);

            rooms.RemoveAt(randomRoomX);
            rooms.Add(top);
            rooms.Add(bottom);
        }

        else
        {
            SplitingVeriticaly();
        }
    }
    #endregion

    #region GraphVisualisation
    void OnDrawGizmos()
    {
        if (nodePos == null || edges == null) return;

        Gizmos.color = Color.red;
        foreach (var edge in edges)
        {
            Gizmos.DrawLine(edge.Item1, edge.Item2);
        }

        Gizmos.color = Color.cyan;
        foreach (var node in nodePos)
        {
            DrawCircle(node, 0.5f);
        }
    }

    void DrawCircle(Vector3 center, float radius)
    {
        int circleDivisions = 30;
        Vector3 previousCircle = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= circleDivisions; i++)
        {
            float angle = i * (2 * Mathf.PI / circleDivisions);
            Vector3 newCircle = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(previousCircle, newCircle);
            previousCircle = newCircle;
        }
    }
    #endregion

    bool RemoveRooms(out RectInt breakingRoom)
    {
        breakingRoom = new RectInt();

        int targetRemovals = Mathf.FloorToInt(rooms.Count * (removePercentage / 100f));
        int removals = 0;

        List<RectInt> copyRooms = rooms.Where(r => r != masterRoom).ToList(); //copy of all existing rooms

        int expectedRoomCount = rooms.Count;

        foreach (var room in copyRooms)
        {
            if (removals >= targetRemovals)
                break;

            rooms.Remove(room); //removes the room temporarily

            DungeonGraph<RectInt> testGraph = new DungeonGraph<RectInt>(); //temporary graph

            ///adds all rooms and doors as nodes (expect the removed once)
            foreach (var r in rooms) testGraph.AddNode(r);
            foreach (var door in doors) testGraph.AddNode(door);

            foreach (var door in doors)
            {
                var connectedRooms = rooms.Where(r => r.Overlaps(door)).ToList();
                if (connectedRooms.Count == 2)
                {
                    testGraph.AddEdge(connectedRooms[0], door);
                    testGraph.AddEdge(door, connectedRooms[1]);
                }
            }

            ///DFS from the masterRoom
            HashSet<RectInt> visitedRooms = new HashSet<RectInt>();
            Stack<RectInt> stack = new Stack<RectInt>();
            stack.Push(masterRoom);
            visitedRooms.Add(masterRoom);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var neighbor in testGraph.GetNeighbors(current))
                {
                    if (neighbor is RectInt nextRoom && rooms.Contains(nextRoom) && !visitedRooms.Contains(nextRoom))
                    {
                        visitedRooms.Add(nextRoom);
                        stack.Push(nextRoom);
                    }
                }
            }

            ///if a room is unreacheble, stop the algorithm
            if (removals >= rooms.Count - (rooms.Count * targetRemovals/100f))  
            {
                rooms.Add(room); //restore the removed room
                breakingRoom = room;
                return false;
            }

            removals++;
        }

        return true;
    }

    void RemoveDoors()
    {
        doors.RemoveAll(door =>
        {
            Vector3 center = new Vector3(door.x + door.width / 2, 0, door.y + door.height / 2);
            foreach (var edge in edges)
            {
                if (edge.Item1 == center || edge.Item2 == center)
                    return false;
            }
            return true;
        });

        nodePos.RemoveAll(pos =>
        {
            foreach (var edge in edges)
            {
                if (edge.Item1 == pos || edge.Item2 == pos)
                    return false;
            }
            return true;
        });

        Debug.Log("Unconnected doors are removed");
    }

    [Button]
    private void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}