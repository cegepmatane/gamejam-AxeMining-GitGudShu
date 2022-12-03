using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public bool isMoving = false;

    private Vector3 _targetPos;
    private Vector3 _direction;
    private Vector3 _mouvement;
    private Tile _targetTile;
    private Vector2Int _targetPosGrid;

    [SerializeField] private Grid _grid;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
        _targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) {
            if (Input.GetAxisRaw("Vertical") != 0) {
                _targetPosGrid = _grid.WorldToGrid(new Vector3(transform.position.x, transform.position.y + Input.GetAxisRaw("Vertical") * _grid.CellSize));
                if (canMoveTo(_targetPosGrid)) {
                    _targetPos = _grid.GridToWorld(_targetPosGrid);
                    isMoving = true;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") != 0) {
                _targetPosGrid = _grid.WorldToGrid(new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal") * _grid.CellSize, transform.position.y));
                if (canMoveTo(_targetPosGrid)) {
                    _targetPos = _grid.GridToWorld(_targetPosGrid);
                    isMoving = true;
                }
            }
        }
        else {
            MoveTo(_targetPos);
        }
    }

    void MoveTo(Vector3 a_targetPos) {
        _direction = a_targetPos - transform.position;
        _mouvement = _direction.normalized * speed * Time.deltaTime;
        if (_mouvement.magnitude >= _direction.magnitude) {
            transform.position = a_targetPos;
            isMoving = false;
        }
        else {
            transform.position += _mouvement;
        }
    }

    bool canMoveTo(Vector2Int a_gridPos) {
        _targetTile = _grid.GetTile(a_gridPos);
        Debug.Log(_targetTile.BaseCost);
        return (_targetTile.BaseCost == 0) ? false : true;
    }

}
