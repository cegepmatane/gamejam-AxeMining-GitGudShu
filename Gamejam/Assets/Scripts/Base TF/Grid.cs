using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid : MonoBehaviour
{
    public float CellSize = 1;
    public uint RowCount = 10;
    public uint ColumnCount = 10;
    public Color GridColor = Color.white;
    public List<Tile> tiles;
    public int oreAmount;

#if UNITY_EDITOR
    [Space]
    [Header("Grid Editor")]

    public GameObject[] AvailableTiles;
    public uint SelectedTile;
#endif

    private void Awake()
    {
        tiles = GetComponentsInChildren<Tile>().ToList();

        foreach(Tile t_Tile in tiles)
        {
            Vector2Int t_GridPos = WorldToGrid(t_Tile.transform.position);
            t_Tile.x = (uint)t_GridPos.x;
            t_Tile.y = (uint)t_GridPos.y;
        }
    }

    private void Start()
    {
        oreAmount = GetComponentsInChildren<OreTile>().ToList().Count;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GridColor;

        for (int i = 0; i < ColumnCount + 1; i++)
        {
            float t_Length = CellSize * RowCount;
            Vector3 t_D = transform.position + (Vector3.right * i * CellSize);
            Vector3 t_A = t_D + (Vector3.up * t_Length);

            Gizmos.DrawLine(t_D, t_A);
        }

        for (int i = 0; i < RowCount + 1; i++)
        {
            float t_Length = CellSize * ColumnCount;
            Vector3 t_D = transform.position + (Vector3.up * i * CellSize);
            Vector3 t_A = t_D + (Vector3.right * t_Length);

            Gizmos.DrawLine(t_D, t_A);
        }
    }

    public Vector2Int WorldToGrid(Vector3 a_WorldPos)
    {
        Vector3 t_Diff = a_WorldPos - transform.position;
        int t_PosX = Mathf.FloorToInt(t_Diff.x / CellSize);
        int t_PosY = Mathf.FloorToInt(t_Diff.y / CellSize);

        // Exceptions
        if (t_PosX < 0 || t_PosX >= ColumnCount)
            throw new GridException("Out of grid!");
        else if (t_PosY < 0 || t_PosY >= RowCount)
            throw new GridException("Out of grid!");

        return new Vector2Int(t_PosX, t_PosY);
    }

    public Vector3 GridToWorld(Vector2Int a_GridPos)
    {
        // Exceptions
        if (a_GridPos.x < 0 || a_GridPos.x >= ColumnCount)
            throw new GridException("Out of grid!");
        else if (a_GridPos.y < 0 || a_GridPos.y >= RowCount)
            throw new GridException("Out of grid!");

        float t_PosX = (a_GridPos.x * CellSize) + (0.5f * CellSize);
        float t_PosY = (a_GridPos.y * CellSize) + (0.5f * CellSize);

        return new Vector3(t_PosX, t_PosY, 0) + transform.position;
    }

    public Tile GetTile(Vector2Int a_GridPos)
    {
        return tiles.FirstOrDefault(t => t.x == a_GridPos.x && t.y == a_GridPos.y);
    }
}
