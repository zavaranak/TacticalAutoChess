using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private List<Node> nodes;
    private List<Edge> edges;

    public List<Node> Nodes => nodes;
    public List<Edge> Edges => edges;

    public Graph()
    {
        nodes = new List<Node>();
        edges = new List<Edge>();
    }

    public bool Adjacent(Node from, Node to)
    {
        foreach (Edge e in edges)
        {
            if (e.from == from && e.to == to)
                return true;
        }
        return false;
    }

    public List<Node> Neighbors(Node from)
    {
        List<Node> result = new();

        foreach (Edge e in edges)
        {
            if (e.from == from)
                result.Add(e.to);
        }
        return result;
    }

    public void AddNode(Vector3 worldPosition)
    {
        nodes.Add(new Node(nodes.Count, worldPosition));
    }

    public void AddEdge(Node from, Node to)
    {
        edges.Add(new Edge(from, to, 1));
    }

    public float Distance(Node from, Node to)
    {
        foreach (Edge e in edges)
        {
            if (e.from == from && e.to == to)
                return e.GetWeight();
        }

        return Mathf.Infinity;
    }


    public virtual List<Node> GetShortestPath(Node start, Node end)
    {
        List<Node> path = new();

        if (start == end)
        {
            path.Add(start);
            return path;
        }


        List<Node> unvisited = new();
        Dictionary<Node, Node> previous = new();

        Dictionary<Node, float> distances = new();
        Dictionary<Node, float> weightMap = new();

        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            unvisited.Add(node);
            distances.Add(node, float.MaxValue);
            weightMap.Add(node, float.MaxValue);
        }

        distances[start] = 0f;
        weightMap[start] = Vector3.Distance(start.worldPosition, end.worldPosition);
        while (unvisited.Count != 0)
        {
            unvisited = unvisited.OrderBy(node => distances[node]).ToList();
            Node current = unvisited[0];
            unvisited.Remove(current);

            if (current == end)
            {

                while (previous.ContainsKey(current))
                {
                    path.Insert(0, current);
                    current = previous[current];
                }

                path.Insert(0, current);
                break;
            }
            foreach (Node neighbor in Neighbors(current))
            {
                Edge edge = edges.Find(edge => edge.from == current && edge.to == neighbor);

                //float weight = edge.GetWeight();
                ////float realLength = Vector3.Distance(end.worldPosition, neighbor.worldPosition);

                //float alt = distances[current] + weight;
                //    if (alt < distances[neighbor])
                //    {
                //        distances[neighbor] = alt;
                //        previous[neighbor] = current;
                //    }



                float weight = edge.GetWeight();
                float heuristic = Vector3.Distance(end.worldPosition, neighbor.worldPosition);

                float alt = weightMap[current] + weight + heuristic;
                if (alt < distances[neighbor])
                {
                    weightMap[neighbor] = weightMap[current]+weight;
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }
        return path;
    }
}

public class Node
{
    public int index;
    public Vector3 worldPosition;

    private bool occupied = false;

    public Node(int index, Vector3 worldPosition)
    {
        this.index = index;
        this.worldPosition = worldPosition;
        occupied = false;
    }

    public void SetOccupied(bool val)
    {
        occupied = val;
    }

    public bool IsOccupied => occupied;
}

public class Edge
{
    public Node from;
    public Node to;

    private float weight;

    public Edge(Node from, Node to, float weight)
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }

    public float GetWeight()
    {
        if (to.IsOccupied)
            return Mathf.Infinity;

        return weight;
    }
}