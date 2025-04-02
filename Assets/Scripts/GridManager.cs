using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : Manager<GridManager>
{
    public Tilemap grid;

    Graph graph;
    Dictionary<Team, int> startPositionPerTeam;


    //public Node GetFreeNode(Team forTeam)
    //{
    //    int startIndex = startPositionPerTeam[forTeam];
    //    int currentIndex = startIndex;

    //    while (graph.Nodes[currentIndex].IsOccupied)
    //    {
    //        if (startIndex == 0)
    //        {
    //            currentIndex++;
    //            if (currentIndex == graph.Nodes.Count)
    //                return null;
    //        }
    //        else
    //        {
                
    //            currentIndex--;
    //            if (currentIndex == -1)
    //                return null;
    //        }

    //    }
    //    if (forTeam == Team.Team2)
    //    {
    //        GridManager.Instance.toIndex =currentIndex;
    //    }
    //    return graph.Nodes[currentIndex];
    //}

    private new void Awake()
    {
        base.Awake();
        InitializeGraph();
        startPositionPerTeam = new Dictionary<Team, int>();
        startPositionPerTeam.Add(Team.Team1, 0);
        startPositionPerTeam.Add(Team.Team2, graph.Nodes.Count - 1);
        GameManager.Instance.InstantiateUnits();
    }

    //debug
    public int fromIndex = 0;
    public int toIndex = 10;


    private void InitializeGraph()
    {
        graph = new Graph();

        BoundsInt bounds = grid.cellBounds;
        Vector3 gridOffset = grid.transform.position;

        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int localPosition = new Vector3Int(x, y, 0);
                if (grid.HasTile(localPosition))
                {
                    Vector3 worldPosition = grid.CellToWorld(localPosition) + gridOffset;
                    graph.AddNode(worldPosition);
                }
            }
        }

        var allNodes = graph.Nodes;
        foreach (Node from in allNodes)
        {
            foreach (Node to in allNodes)
            {
                if (from == to) continue;

                if (Vector3.Distance(from.worldPosition, to.worldPosition) < 1.1f) 
                {
                    graph.AddEdge(from, to);
                }
            }
        }
    }


    public List<Node> GetPath(Node from, Node to)
    {
        if(from==null || to == null)
        {
            return null;
        }

        return graph.GetShortestPath(from, to);
    }

    public List<Node> GetNeighbors(Node node)
    {
        return this.graph.Neighbors(node);
    }

    public Node GetNodeByIndex(int index)
    {
        return graph.Nodes[index];
    }

    private void OnDrawGizmos()
    {
        if (graph == null)
            return;

        var allEdges = graph.Edges;
        if (allEdges == null)
            return;

        foreach (Edge e in allEdges)
        {
            Debug.DrawLine(e.from.worldPosition, e.to.worldPosition, Color.black, 100);
        }

        var allNodes = graph.Nodes;
        if (allNodes == null)
            return;



        for (int i = 0; i < allNodes.Count; i++)
        {
            Node n = allNodes[i];
            if (n == null) continue;

            // Draw node sphere
            Gizmos.color = n.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(n.worldPosition, 0.1f);

            // Draw node label
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontSize = 12;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            // Offset the label slightly above the node
            Vector3 labelPosition = n.worldPosition + Vector3.up * 0.2f;

            #if UNITY_EDITOR
            UnityEditor.Handles.Label(labelPosition, i.ToString(), labelStyle);
            #endif
        }


        if (fromIndex >= allNodes.Count || toIndex >= allNodes.Count)
            return;

        List<Node> path = graph.GetShortestPath(allNodes[fromIndex], allNodes[toIndex]);
        if (path.Count > 1)
        {
            //Debug.Log(path.Count);
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].worldPosition, path[i].worldPosition, Color.red, 10);
            }
        }
    }
}

