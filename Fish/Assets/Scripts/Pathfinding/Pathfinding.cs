using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform target;
    public Grid grid;

    // Nodes to be evaluated
    List<Node> open = new List<Node>();
    // Nodes already evaluated
    List<Node> closed = new List<Node>();
    // Path nodes
    List<Node> path = new List<Node>();
    // Starting node
    Node startNode;
    // Target node
    Node targetNode;

    public bool reset;
    
    // Start is called before the first frame update
    void Start()
    {
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        GeneratePath(); 
        grid.UpdatePath(path);

    }

    /* Update path costs
    void UpdateCosts(Vector2 start, Vector2 end)
    {
        foreach (Node n in grid.nodes)
            n.EvaluateCost(start, end);
    }
    */

    // Find where this object exists on the grid
    Node FindOnGrid(Transform t)
    {
        foreach (Node n in grid.nodes)
        {
            if (Mathf.Abs(t.position.y - n.position.y) < n.length / 2
                && Mathf.Abs(t.position.x - n.position.x) < n.length / 2)
            {
                return n;
            }
        }
        return null;
    }

    void GeneratePath()
    {
        // Set starting node based on this objects position
        startNode = FindOnGrid(transform);
        targetNode = FindOnGrid(target);

        // DEBUG - REMOVE
        if (startNode == null)
            Debug.Log("unable to find startNode");

        open = new List<Node>();
        closed = new List<Node>();
        // Place starting node in open set and path
        open.Add(startNode);
        path.Add(startNode);

        while (open.Count > 0)
        {
            Node currentNode = open[0];
            for (int i = 1; i < open.Count; ++i)
            {

                // Shorter path
                if (open[i].fCost < currentNode.fCost ||
                    open[i].fCost == currentNode.fCost && open[i].hCost < currentNode.hCost)
                    currentNode = open[i];

            }

            open.Remove(currentNode);
            closed.Add(currentNode);

            //Path has been found
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node n in currentNode.neighbours)
            {
                if (!n.isPathable || closed.Contains(n))
                    continue;

                float d = Vector2.Distance(currentNode.position, n.position);
                float pathCost = currentNode.gCost + d;

                if (!open.Contains(n) || pathCost < n.gCost)
                {
                    n.EvaluateCost(pathCost, targetNode.position);
                    
                    // Set parent to track path
                    n.parent = currentNode;

                    if (!open.Contains(n))
                        open.Add(n);
                }
            }
        }
    }

    // Retrace search to get path
    void RetracePath(Node start, Node end)
    {
        path = new List<Node>();
        Node currentNode = end;

        // Starting at end node, add parent nodes to path
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Reverse path 
        path.Reverse();

        // Visualise path
        grid.UpdatePath(path);

        // Send path to movement script
        GetComponent<FishA>().path = path;
    }
}
