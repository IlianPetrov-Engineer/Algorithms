using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    public int overlap;
    public int maxRooms;
    public int minWidht;
    public int minHeight;
    public float timePerRoom;

    public bool automaticGeneration = false;

    public enum DungeonSize { Mini, Small, Medium, Large, Custom };

    [Tooltip("Mini - 50x50, Small - 100x100, Medim - 150x150, Large - 200x200, Custom - write your own positives values")]
    public DungeonSize dungeonSize;

    public RectInt masterRoom;
    public List<RectInt> rooms = new List<RectInt>();

    private Coroutine coroutine;

    void Start()
    {
        DungeonSizeGenerator(dungeonSize);

        rooms.Add(masterRoom);

        if (automaticGeneration)
        {
            coroutine = StartCoroutine(GeneratingRooms());
        }

        if (masterRoom.width < 0 || masterRoom.height < 0)
        {
            StopCoroutine(coroutine);
            Debug.LogError("Input positive values for wigth and height");
        }
    }

    void Update()
    {
        bool allRoomsTooSmall = true;

        foreach (var room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.green);

            if (room.width > minWidht * 2 || room.height > minHeight * 2)
            {
                allRoomsTooSmall = false;
            }
        }

        if (allRoomsTooSmall && coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            Debug.Log("Coroutine is stopped");
        }
    }

    void DungeonSizeGenerator(DungeonSize dungeonSize)
    {
        switch (dungeonSize)
        {
            case DungeonSize.Mini:
                masterRoom = new RectInt(0, 0, 50, 50);
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
                yield break;
            }
        }
    }

    [Button]
    void SplitingVeriticaly()
    {
        List<int> validRooms = new List<int>();

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].width > minWidht * 2)
            {
                validRooms.Add(i);
            }
        }

        if (validRooms.Count > 0)
        {
            int randomRoomY = validRooms[Random.Range(0, validRooms.Count)];

            RectInt roomY = rooms[randomRoomY];

            int splitX = roomY.x + roomY.width / 2;

            RectInt left = new RectInt(roomY.x /*+ overlap*/, roomY.y, roomY.width / 2 /*- overlap*/, roomY.height);
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

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].height > minHeight * 2)
            {
                validRooms.Add(i);
            }
        }

        if (validRooms.Count > 0)
        {
            int randomRoomX = validRooms[Random.Range(0, validRooms.Count)];

            RectInt roomX = rooms[randomRoomX];

            int splitY = roomX.y + roomX.height / 2;

            RectInt top = new RectInt(roomX.x, roomX.y /*+ overlap*/, roomX.width, roomX.height / 2 /*- overlap*/);
            RectInt bottom = new RectInt(roomX.x, splitY - overlap, roomX.width, roomX.height - top.height + overlap);

            rooms.RemoveAt(randomRoomX);
            rooms.Add(top);
            rooms.Add(bottom);
        }

        else
        {
            SplitingVeriticaly();
        }
    }
}
