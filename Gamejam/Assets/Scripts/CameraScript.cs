using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private float minY;
    private float minX;
    private float maxY;
    private float maxX;
    private float posX;
    private float posY;

    [SerializeField] private Grid grid;
    [SerializeField] private Transform target;

    void Update()
    {
        if (target == null) return;
        
        minX = Mathf.Min(minX, grid.transform.position.x);
        maxX = Mathf.Max(maxX, grid.transform.position.x + grid.ColumnCount * grid.CellSize);
        minY = Mathf.Min(minY, grid.transform.position.y);
        maxY = Mathf.Max(maxY, grid.transform.position.y + grid.RowCount * grid.CellSize);

        posY = Mathf.Max(Mathf.Min(target.position.y, maxY - Camera.main.orthographicSize), minY + Camera.main.orthographicSize);
        posX = Mathf.Max(Mathf.Min(target.position.x, maxX - Camera.main.aspect * Camera.main.orthographicSize), minX + Camera.main.aspect * Camera.main.orthographicSize);
        transform.position = new Vector3(posX, posY, transform.position.z);

        print(maxX);
    }
}
