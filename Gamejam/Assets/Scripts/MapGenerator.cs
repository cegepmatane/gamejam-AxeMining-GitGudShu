using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int minRow = 10;
    public int maxRow = 20;
    public int minCol = 10;
    public int maxCol = 20;

    [SerializeField] private Grid _grid;

    // Start is called before the first frame update
    void Start()
    {
        _grid.RowCount = (uint)Random.Range(minRow, maxRow);
        _grid.ColumnCount = (uint)Random.Range(minCol, maxCol);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
