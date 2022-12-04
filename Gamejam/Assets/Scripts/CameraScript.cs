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

    private float _gridWidth;
    private float _gridHeight;
    private float _cameraHalfWidth;
    private float _cameraHalfHeight;


    [SerializeField] private Grid grid;
    [SerializeField] private Transform target;

    void Update()
    {
        if (target == null) return;

        _gridHeight = grid.RowCount * grid.CellSize;
        _gridWidth = grid.ColumnCount * grid.CellSize;
        _cameraHalfHeight = Camera.main.orthographicSize;
        _cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;

        
        minX = Mathf.Min(minX, grid.transform.position.x);
        maxX = Mathf.Max(maxX, grid.transform.position.x + _gridWidth);
        minY = Mathf.Min(minY, grid.transform.position.y);
        maxY = Mathf.Max(maxY, grid.transform.position.y + _gridHeight);

        posY = Mathf.Max(Mathf.Min(target.position.y, maxY - _cameraHalfHeight), minY + _cameraHalfHeight);
        posX = Mathf.Max(Mathf.Min(target.position.x, maxX - _cameraHalfWidth), minX + _cameraHalfWidth);

        if (_gridWidth <= _cameraHalfWidth * 2) posX = grid.transform.position.x + _gridWidth / 2;
        if (_gridHeight <= _cameraHalfHeight * 2) posY = grid.transform.position.y + _gridHeight / 2;

        print(_gridHeight + " " + _cameraHalfHeight);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
