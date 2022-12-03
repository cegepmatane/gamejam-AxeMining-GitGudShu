using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Grid))]
public class Pathfinder : MonoBehaviour
{
    private Grid m_Grid;
    private Node[,] m_nodes;


    private void Awake()
    {
        m_Grid = GetComponent<Grid>();
    }

    private void Start()
    {
        if (m_Grid == null)
        {
            Debug.LogError("Pathfinder is missing a Grid reference.");
            gameObject.SetActive(false);
            return;
        }

        InitNodes(m_Grid.ColumnCount, m_Grid.RowCount);
    }

    private void InitNodes(uint x, uint y)
    {
        m_nodes = new Node[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Tile t_Tuile = m_Grid.GetTile(new Vector2Int(i, j));

                if (t_Tuile == null)
                    continue;

                m_nodes[i, j] = new Node();
                m_nodes[i, j].Tile = t_Tuile;
            }
        }
    }

    public Path GetPath(Tile a_StartPoint, Tile a_EndTile, bool a_DiagonalAllowed = true)
    {
        if (a_StartPoint == null)
        {
            Debug.LogError("GetPath StartTile is null.");
            return null;
        }

        if (a_EndTile == null)
        {
            Debug.LogError("GetPath EndTile is null.");
            return null;
        }

        List<Node> t_openList = new List<Node>();
        List<Node> t_closedList = new List<Node>();    

        Node t_StartNode = m_nodes[a_StartPoint.x, a_StartPoint.y];
        Node t_EndNode = m_nodes[a_EndTile.x, a_EndTile.y];
        Node t_CurrentNode = t_StartNode;
        t_openList.Add(t_StartNode);

        do
        {
            if (t_openList.Count == 0)
            {
                return null;
            }

            uint minF = t_openList.Min(t => t.f);
            t_CurrentNode = t_openList.Find(t => t.f == minF);

            t_openList.Remove(t_CurrentNode);
            t_closedList.Add(t_CurrentNode);

            List<Node> t_neighbours = GetNeighbours(t_CurrentNode);
            foreach (Node t_Neighbour in t_neighbours)
            {
                if (t_Neighbour.Tile.BaseCost == 0 || t_closedList.Contains(t_Neighbour))
                    continue;

                uint t_NeighbourCost = 10u;

                // If diagonale
                if(t_Neighbour.Tile.x != t_CurrentNode.Tile.x && t_Neighbour.Tile.y != t_CurrentNode.Tile.y)
                {
                    if (!a_DiagonalAllowed)
                        continue;

                    t_NeighbourCost = 14u;
                }

                uint t_PathCost = t_CurrentNode.g + (t_NeighbourCost * t_Neighbour.Tile.BaseCost);

                if (!t_openList.Contains(t_Neighbour) || t_PathCost < t_Neighbour.g)
                {
                    t_Neighbour.g = t_PathCost;
                    t_Neighbour.h = Heuristic(t_Neighbour.Tile.x, t_Neighbour.Tile.y, a_EndTile.x, a_EndTile.y);
                    t_Neighbour.f = t_Neighbour.g + t_Neighbour.h;
                    t_Neighbour.Parent = t_CurrentNode;

                    if (!t_openList.Contains(t_Neighbour))
                        t_openList.Add(t_Neighbour);
                }
            }
        } while (t_CurrentNode.Tile != a_EndTile);

        if (t_EndNode.Parent != null)
        {
            Path t_path = new Path();

            t_CurrentNode = t_EndNode;
            while (t_CurrentNode != t_StartNode)
            {
                t_path.Checkpoints.Add(t_CurrentNode.Tile);
                t_CurrentNode = t_CurrentNode.Parent;
            }
            t_path.Checkpoints.Add(t_StartNode.Tile);
            t_path.Checkpoints.Reverse();

            return t_path;
        }

        return null;
    }

    private uint Heuristic(uint a_startX, uint a_startY, uint a_endX, uint a_endY)
    {
        uint dX = (uint)Math.Abs((int)a_startX - (int)a_endX);
        uint dY = (uint)Math.Abs((int)a_startY - (int)a_endY);

        return dX + dY;
    }

    private List<Node> GetNeighbours(Node a_node)
    {
        uint xMin, xMax, yMin, yMax;
        List<Node> t_list = new List<Node>();

        xMin = a_node.Tile.x == 0 ? a_node.Tile.x : a_node.Tile.x - 1;
        xMax = a_node.Tile.x == m_Grid.ColumnCount - 1 ? a_node.Tile.x : a_node.Tile.x + 1;
        yMin = a_node.Tile.y == 0 ? a_node.Tile.y : a_node.Tile.y - 1;
        yMax = a_node.Tile.y == m_Grid.RowCount - 1 ? a_node.Tile.y : a_node.Tile.y + 1;

        for (uint i = xMin; i <= xMax; i++)
            for (uint j = yMin; j <= yMax; j++)
                if(m_nodes[i, j] != null)
                    t_list.Add(m_nodes[i, j]);

        return t_list;
    }


    private class Node
    {
        public Tile Tile;
        public uint f, g, h;
        public Node Parent;
    }
}
