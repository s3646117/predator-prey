using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Position
    public Vector2 position;
    // Length of edge
    public float length;
    // Layer containing obstacles
    public LayerMask obstacles;
    // Is node pathable?
    public bool isPathable;
    // Path costs
    public float gCost, hCost, fCost;
    // Neighbours
    public List<Node> neighbours;
    // Parent node
    public Node parent;

    public Node(Vector2 myPos, float myLength, LayerMask layer)
    {
        position = myPos;
        length = myLength;
        obstacles = layer;

        isPathable = IsPathable();

        neighbours = new List<Node>();
    }

    // Check if node is pathable
    bool IsPathable()
    {
        // Check for obstacles in this grid space
        if (Physics2D.OverlapBox(position, new Vector2(length, length), 0f, obstacles))
            return false;

        return true;
    }

    public void EvaluateCost(float pathCost, Vector2 end)
    {
        gCost = pathCost;
        hCost = Vector2.Distance(position, end);
        fCost = gCost + hCost;
    }

    public void AddNeighbour(Node n)
    {
        neighbours.Add(n);
    }

}
