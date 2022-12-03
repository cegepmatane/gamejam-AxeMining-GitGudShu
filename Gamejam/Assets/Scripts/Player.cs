using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action OnPlayerDeath;
    public static event Action onWin;

    public float speed = 5;
    public bool isMoving = false;
    //public int actionCounter = 0;
    public static int actionPerTurn = 3;
    public static int time = 0;
    public GameObject Tile_Wall;
    public Grid grid;

    private int ores = 0;
    private Vector3 _targetPos;
    private Vector3 _direction;
    private Vector3 _mouvement;
    private Tile _targetTile;
    private Vector2Int _targetPosGrid;
    private Animator m_anim;

    [SerializeField] private TextMeshProUGUI oreText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        OnPlayerDeath += DisablePlayerMovement;
        onWin += DisablePlayerMovement;
    }

    private void OnDisable()
    {
        OnPlayerDeath -= DisablePlayerMovement;
        onWin -= DisablePlayerMovement;
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.position = grid.GridToWorld(grid.WorldToGrid(transform.position));
        _targetPos = transform.position;
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) {
            if (Input.GetAxisRaw("Vertical") != 0) {
                _targetPosGrid = grid.WorldToGrid(new Vector3(transform.position.x, transform.position.y + Input.GetAxisRaw("Vertical") * grid.CellSize));
                if (canMoveTo(_targetPosGrid)) {
                    time++;
                    _targetPos = grid.GridToWorld(_targetPosGrid);
                    isMoving = true;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") != 0) {
                _targetPosGrid = grid.WorldToGrid(new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal") * grid.CellSize, transform.position.y));
                if (canMoveTo(_targetPosGrid)) {
                    
                    time++;
                    _targetPos = grid.GridToWorld(_targetPosGrid);
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
        m_anim.SetBool("isMoving",isMoving);
    }

    bool canMoveTo(Vector2Int a_gridPos) {
        _targetTile = grid.GetTile(a_gridPos);
        Debug.Log(_targetTile.BaseCost);
        if (IsOre(_targetPosGrid))
        {
            Debug.Log("Is ore");
            m_anim.SetTrigger("Mine");
            getOre();
            replaceOre(_targetPosGrid);
        }
        return (_targetTile.BaseCost == 0) ? false : true;
    }

    bool IsOre(Vector2Int a_gridPos)
    {
        _targetTile = grid.GetTile(a_gridPos);
        return _targetTile.CompareTag("Ore");
        
    }

    void replaceOre(Vector2Int a_gridPos)
    {
        //List<Tile> t_Tiles = _grid.GetComponentsInChildren<Tile>().ToList();
        Tile t_OldTile = grid.tiles.FirstOrDefault(t => a_gridPos == grid.WorldToGrid(t.transform.position));
        if (t_OldTile)
        {
            Debug.Log(t_OldTile.name);
            Destroy(t_OldTile.gameObject);
        }
        
        GameObject t_NewTile = Instantiate(Tile_Wall, grid.transform);
        t_NewTile.transform.position = grid.GridToWorld(a_gridPos);
        Sprite t_Sprite = t_NewTile.GetComponent<SpriteRenderer>().sprite;
        float t_NewScale = grid.CellSize / t_Sprite.bounds.size.x;
        t_NewTile.transform.localScale = new Vector3(t_NewScale, t_NewScale, t_NewScale);
        grid.tiles = grid.GetComponentsInChildren<Tile>().ToList();
    }

    void getOre()
    {
        ores++;
        oreText.text = "x" + ores;
        scoreText.text = "x" + ores;
    }

    private void DisablePlayerMovement()
    {
        m_anim.enabled = false;
        Time.timeScale = 0;
    }

    public void Win()
    {
        onWin?.Invoke();
    }

    public void Kill()
    {
        Debug.Log("Player dead");
        m_anim.SetTrigger("Dead");
        OnPlayerDeath?.Invoke();
    }
}
