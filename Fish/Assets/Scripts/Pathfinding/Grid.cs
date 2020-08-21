using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int cols = 10;
    public int rows = 10;
    public float nodeLength = 1f;
    public List<Node> nodes = new List<Node>();
    public LayerMask obstacles;
    List<Node> path = new List<Node>();
    
    // Start is called before the first frame update
    void Start()
    {
        BuildGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void BuildGrid()
    {
        for(int y = 0; y < rows; ++y)
        {
            for(int x = 0; x < cols; ++x)
            {
                float posX = x * nodeLength + nodeLength / 2 - cols * nodeLength / 2;
                float posY = y * nodeLength + nodeLength / 2 - rows * nodeLength / 2;
                Node n = new Node(new Vector2(posX, posY), nodeLength, obstacles);
                nodes.Add(n);
                
            }
        }

        AssociateNodes();
    }

    // Set neighbouring nodes
    void AssociateNodes()
    {
        for (int y = 0; y < rows; ++y)
        {
            for (int x = 0; x < cols; ++x)
            {
                // Find node within list
                int n = y * cols + x;

                // Test top edge
                if(y < rows - 1)
                {
                    // Top
                    nodes[n].AddNeighbour(nodes[n + cols]);
                    
                    // Test left edge
                    if (x > 0)
                    {
                        // Left
                        nodes[n].AddNeighbour(nodes[n - 1]);
                        // Top left
                        nodes[n].AddNeighbour(nodes[n + cols - 1]);

                        // Test right edge
                        if(x < cols - 1)
                        {
                            // Right
                            nodes[n].AddNeighbour(nodes[n + 1]);
                            // Top right
                            nodes[n].AddNeighbour(nodes[n + cols + 1]);
                        }
                    }
                }
                
                //Test bottom edge
                if (y > 0)
                {
                    // Bottom
                    nodes[n].AddNeighbour(nodes[n - cols]);

                    // Test left edge
                    if (x > 0)
                    {
                        // NOTE: Already added left
                        // Bottom left
                        nodes[n].AddNeighbour(nodes[n - cols - 1]);

                        // Test right edge
                        if (x < cols - 1)
                        {
                            // NOTE: Already added right
                            // Bottom right
                            nodes[n].AddNeighbour(nodes[n - cols + 1]);
                        }
                    }
                }
            }
        }
    }

    public void UpdatePath(List<Node> newPath)
    {
        path = newPath;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            if (nodes[i].isPathable)
                Gizmos.color = Color.clear;
            else
                Gizmos.color = Color.red;

            if (path.Contains(nodes[i]))
                Gizmos.color = Color.black;

            Gizmos.DrawCube(nodes[i].position, new Vector2(nodeLength, nodeLength));
        }
               
    }
}
