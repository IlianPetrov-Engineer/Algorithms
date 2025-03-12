using NaughtyAttributes;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    public int overlap;

    public int maxRooms;

    public int minWidht;

    public int minHeight;

    public float timePerRoom;

    public RectInt masterRoom = new RectInt(0, 0, 100, 50);

    public List<RectInt> rooms = new List<RectInt>();

    public bool splitHorizontaly = false;

    void Start()
    {
        rooms.Add(masterRoom);

        int random = Random.Range(0, 2);

        if (random == 0)
        {
            splitHorizontaly = true;
        }

        else if (random != 0)
        {
            splitHorizontaly = false;
        }

        StartCoroutine(GeneratingRooms());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var room in rooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.green);

            if (room.width < minWidht * 2 || room.height < minHeight * 2)
            {
                StopCoroutine(GeneratingRooms());
            }
        }
    }

    IEnumerator GeneratingRooms()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(timePerRoom);

            if (rooms.Count < maxRooms)
            {
                if (splitHorizontaly)
                {
                    SplitingHorizontaly();
                    splitHorizontaly = false;
                    Debug.Log("SplitHorizontaly complete");
                }

                else if (splitHorizontaly != true)
                {
                    SplitingVeriticaly();
                    splitHorizontaly = true;
                    Debug.Log("SlpitVericaly complete");
                }
            }
        }
    }

    [Button]
    void SplitingVeriticaly()
    {
        int randomRoomY = Random.Range(0, rooms.Count);

        RectInt room = rooms[randomRoomY];

        if (room.width > minWidht * 2)
        {
            int splitX = room.x + room.width / 2;

            RectInt left = new RectInt(room.x + overlap, room.y, room.width / 2 - overlap, room.height);
            RectInt right = new RectInt(splitX - overlap, room.y, room.width - left.width + overlap, room.height);

            rooms.RemoveAt(randomRoomY);
            rooms.Add(left);
            rooms.Add(right);

            //if (randomRoomY == 0)
            //{
            //    return;
            //}
        }

        else
        {
            Debug.LogError("MinWidht is reached");
            return;
        }
    }

    [Button]
    void SplitingHorizontaly()
    {
        int randomRoomX = Random.Range(0, rooms.Count);

        RectInt roomX = rooms[randomRoomX];

        if (roomX.height > minHeight * 2)
        {
            int splitY = roomX.y + roomX.height / 2;

            RectInt top = new RectInt(roomX.x, roomX.y + overlap, roomX.width, roomX.height / 2 - overlap);
            RectInt bottom = new RectInt(roomX.x, splitY - overlap, roomX.width, roomX.height - top.height + overlap);

            rooms.RemoveAt(randomRoomX);
            rooms.Add(top);
            rooms.Add(bottom);

            //if (randomRoomX == 0)
            //{
            //    return;
            //}
        }

        else
        {
            Debug.LogError("MinWidht is reached");
            return;
        }
    }
}
