using UnityEngine;

public class GraphTester : MonoBehaviour
{
    void Start()
    {
        GraphTest<string> graph = new GraphTest<string>();
        graph.AddNode("B");
        graph.AddNode("C");
        graph.AddNode("D");
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");
        Debug.Log("Graph Structure:");
        //graph.PrintGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
