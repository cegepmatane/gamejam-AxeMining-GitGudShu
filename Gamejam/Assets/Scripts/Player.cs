using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action OnPlayerDeath;
    public static event Action onWin;

    public float speed = 5;
    public static int actionPerTurn = 3;
    public static int time = 0;
    public Grid grid;

    private bool isMoving = false;
    private int ores = 0;
    private bool _facingRight = true;
    private Vector3 _targetPos;
    private Vector3 _direction;
    private Vector3 _mouvement;
    private Tile _targetTile;
    private Vector2Int _targetPosGrid;
    private Animator m_anim;
    private SoundHandler _soundHandler;
    private BoxCollider2D _collider;

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

    private void Awake() {
        _collider = GetComponent<BoxCollider2D>();
        m_anim = GetComponent<Animator>();
        _soundHandler = GetComponent<SoundHandler>();
    }

    void Start()
    {
        transform.position = grid.GridToWorld(grid.WorldToGrid(transform.position));
        _targetPos = transform.position;
        oreText.text = " " + ores + " / " + grid.oreAmount;
    }

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
            if (Input.GetKeyDown(KeyCode.Space)) time++;
            _collider.enabled = true;
            m_anim.SetBool("isMoving", isMoving);
            Flip();
        }
        else {
            StartCoroutine(DisableColider());
            MoveTo(_targetPos);
        }
    }

    IEnumerator DisableColider() {
        if (_collider.enabled == true)
        {
            yield return new WaitForSeconds(0.05f);
            _collider.enabled = false;
        }
        else _collider.enabled = false;
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
        if (transform.position == a_targetPos)
        {
            isMoving = false;
        }
    }

    bool canMoveTo(Vector2Int a_gridPos) {
        _targetTile = grid.GetTile(a_gridPos);
        if (_targetTile.CompareTag("Ore"))
        {
            _soundHandler.PlayMine();
            m_anim.SetTrigger("Mine");
            _targetTile.GetComponent<OreTile>().Replace();
            getOre();
        }
        return (_targetTile.BaseCost == 0) ? false : true;
    }

    void getOre()
    {
        time++;
        ores++;
        oreText.text = " " + ores + " / " + grid.oreAmount;
        scoreText.text = "You won in " + time + " turns";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.CompareTag("Enemy"))
        {
            _soundHandler.PlayExplosion();
            Animator t_enemyAnim = gameObject.GetComponent<Animator>();
            t_enemyAnim.SetTrigger("Explode");
            m_anim.SetTrigger("Dead");
        }

        if(gameObject.CompareTag("Stairs") && grid.oreAmount == ores)
        {
            if (SceneManager.GetSceneByName("Level 3") == SceneManager.GetActiveScene())
            {
                Win();
            }
            else
            {
                _soundHandler.PlayStairs();
                SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
            }
        }
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
        _soundHandler.PlayDie();
        OnPlayerDeath?.Invoke();
    }

    public void Flip()
    {
        if (Input.GetAxisRaw("Horizontal") < 0 && _facingRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            _facingRight = false;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0 && !_facingRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            _facingRight = true;
        }
    }
}
