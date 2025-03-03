using NaughtyAttributes;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    public RectInt masterRoom = new RectInt(0, 0, 100, 50);

    public List<RectInt> rooms = new List<RectInt>();

    void Start()
    {
        rooms.Add(masterRoom);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var room in rooms)
        {
            if (rooms.Count < 16)
            {
                SplitingVeriticaly();
                SplitingHorizontaly();
            }
            
            AlgorithmsUtils.DebugRectInt(room, Color.green);
        }
    }

    IEnumerator GeneratingRooms() 
    { 
        yield return new WaitForSeconds(0.5f);
        SplitingVeriticaly();
        SplitingHorizontaly();
    }

    [Button]
    void SplitingVeriticaly()
    { 
        List<RectInt> newRooms = new List<RectInt>();

        foreach (RectInt room in rooms)
        {
            int splitX = room.x + room.width / 2;

            RectInt left = new RectInt(room.x, room.y, room.width / 2, room.height);
            RectInt right = new RectInt(splitX, room.y, room.width - left.width, room.height);

            newRooms.Add(left);
            newRooms.Add(right);
        }

        rooms = newRooms;
    }

    [Button]
    void SplitingHorizontaly()
    {
        List<RectInt> finalRooms = new List<RectInt>();

        foreach (RectInt room in rooms)
        {
            int splitY = room.y + room.height / 2;

            RectInt top = new RectInt(room.x, room.y, room.width, room.height / 2);
            RectInt bottom = new RectInt(room.x, splitY, room.width, room.height - top.height);

            finalRooms.Add(top);
            finalRooms.Add(bottom);
        }

        rooms = finalRooms;
    }
}
