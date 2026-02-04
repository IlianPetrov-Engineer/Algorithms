using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph<RectInt>
{
    private Dictionary<RectInt, List<RectInt>> adjacencyList;

    public DungeonGraph()
    {
        adjacencyList = new Dictionary<RectInt, List<RectInt>>();
    }

    public void Clear()
    {
        adjacencyList.Clear();
    }

    public void AddNode(RectInt node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<RectInt>();
        }
    }

    public void AddEdge(RectInt fromNode, RectInt toNode)
    {
        if (!adjacencyList.ContainsKey(fromNode))
        {
            AddNode(fromNode);
        }
        if (!adjacencyList.ContainsKey(toNode))
        {
            AddNode(toNode);
        }

        adjacencyList[fromNode].Add(toNode);
        adjacencyList[toNode].Add(fromNode);
    }

    public void RemoveNode(RectInt node)
    {
        if (!adjacencyList.ContainsKey(node)) return;

        // Remove this node from all adjacency lists
        foreach (var neighbor in adjacencyList[node])
        {
            adjacencyList[neighbor].Remove(node);
        }

        // Remove the node itself
        adjacencyList.Remove(node);
    }

    public bool IsPathBetween(RectInt from, RectInt to)
    {
        Stack<RectInt> stack = new Stack<RectInt>();
        HashSet<RectInt> visited = new HashSet<RectInt>();

        stack.Push(from);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.Equals(to)) return true;

            visited.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                    stack.Push(neighbor);
            }
        }

        return false;
    }

    public List<RectInt> GetNeighbors(RectInt node)
    {
        return adjacencyList.ContainsKey(node) ? new List<RectInt>(adjacencyList[node]) : new List<RectInt>();
    }

    public int GetNodeCount()
    {
        return adjacencyList.Count;
    }

    public void DFS(RectInt startRoom)
    {
        Stack<RectInt> stack = new Stack<RectInt>();
        HashSet<RectInt> visited = new HashSet<RectInt>();
        stack.Push(startRoom);

        while (stack.Count > 0)
        {
            RectInt currentRoom = stack.Pop();
            if (!visited.Contains(currentRoom))
            {
                visited.Add(currentRoom);
                Debug.Log($"Visited Room: {currentRoom}");

                foreach (var neighbor in GetNeighbors(currentRoom))
                {
                    stack.Push(neighbor);
                }
            }
        }
    }

    public Dictionary<RectInt, List<RectInt>> GetGraph()
    {
        return adjacencyList;
    }
}
