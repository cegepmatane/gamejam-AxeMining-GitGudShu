using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float Speed = 5f;
    public int nbActionToMove = 3;
    public Pathfinder Pathfinder;
    public Grid grid;
    public Transform Objective;
    public bool canMove;
    public bool canGenerate;

    private int _addCost = 500;
    private bool _isMoving;
    private Path m_Path;
    private Animator m_Anim;
    private Vector2Int m_NextGridPos;

    void Start()
    {
        transform.position = grid.GridToWorld(grid.WorldToGrid(transform.position));
        m_Anim = GetComponent<Animator>();
        canMove = true;
        canGenerate = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (canMove)
        {
            if (m_Path == null)
            {
                CalculatePath();
            }
            if (m_Path == null)
            {
                return;
            }
            Vector3 t_TargetPos = m_Path.Checkpoints[1].transform.position;
            //m_LastGridPos = new Vector2Int((int)m_Path.Checkpoints[0].x, (int)m_Path.Checkpoints[0].y);
            Vector3 t_Direction = t_TargetPos - transform.position;
            Vector3 t_Deplacement = t_Direction.normalized * Time.deltaTime * Speed;
            TurnOffDanger(grid.WorldToGrid(transform.position));
            if (t_Deplacement.magnitude >= t_Direction.magnitude) // on dépasse le checkpoint
            {
                transform.position = t_TargetPos;
            }
            else
            {
                transform.position += t_Deplacement;
            }


            if (transform.position == t_TargetPos)
            {
                CalculatePath();
                canMove = false;
                
                if (m_Path == null) return;
            }
        }
        if(Player.time % nbActionToMove == 0 && !canMove)
        {
            Player.time++;
            CalculatePath();
            canMove = true;
            
        }
        TurnOnDanger(grid.WorldToGrid(transform.position));
        m_Anim.SetBool("isMoving", canMove);

        */

        if(Player.time % nbActionToMove == 0 && canMove)
        {
            if (m_Path == null)
            {
                CalculatePath();
            }
            if (m_Path == null)
            {
                return;
            }
            //TurnOffDanger(grid.WorldToGrid(transform.position));
            Vector3 t_TargetPos = m_Path.Checkpoints[1].transform.position;
            //m_LastGridPos = new Vector2Int((int)m_Path.Checkpoints[0].x, (int)m_Path.Checkpoints[0].y);
            Vector3 t_Direction = t_TargetPos - transform.position;
            Vector3 t_Deplacement = t_Direction.normalized * Time.deltaTime * Speed;
            if (t_Deplacement.magnitude >= t_Direction.magnitude) // on dépasse le checkpoint
            {
                transform.position = t_TargetPos;
                canMove = false;
            }
            else
            {
                _isMoving = true;
                transform.position += t_Deplacement;
            }
            if (transform.position == t_TargetPos)
            {
                _isMoving = false;
                canMove = false;
                TurnOnDanger(grid.WorldToGrid(transform.position));
                canGenerate = true;
                grid.GetTile(grid.WorldToGrid(transform.position)).BaseCost += (uint)_addCost;
            }
        }
        if (Player.time % nbActionToMove == nbActionToMove - 2)
        {
            TurnOnDanger(grid.WorldToGrid(transform.position));
        }
        if(Player.time % nbActionToMove == nbActionToMove-1 && canGenerate)
        {
            CalculatePath();
            m_NextGridPos = new Vector2Int((int)m_Path.Checkpoints[1].x, (int)m_Path.Checkpoints[1].y);
            grid.GetTile(m_NextGridPos).BaseCost -= (uint)_addCost;
            TurnOffDanger(grid.WorldToGrid(transform.position));
            TurnOnDanger(m_NextGridPos);
            canMove = true;
            canGenerate = false;
            grid.GetTile(grid.WorldToGrid(transform.position)).BaseCost -= (uint)_addCost;
        }
        m_Anim.SetBool("isMoving", _isMoving);
    }

    void TurnOnDanger(Vector2Int a_GridPos)
    {

        //Debug.Log("TurnOnDanger : " + a_GridPos.x + " - " + a_GridPos.y);
        Tile _actualTile = grid.GetTile(a_GridPos);
        uint _x = _actualTile.x;
        uint _y = _actualTile.y;
        TurnOnDangerTile(_x, _y);
        TurnOnDangerTile(_x - 1, _y);
        TurnOnDangerTile(_x, _y - 1);
        TurnOnDangerTile(_x + 1, _y);
        TurnOnDangerTile(_x, _y + 1);
    }

    void TurnOnDangerTile(uint a_x, uint a_y)
    {
        Tile _actualTile = grid.GetTile(new Vector2Int((int)a_x,(int)a_y));
        Material _material = _actualTile.gameObject.GetComponent<SpriteRenderer>().material;
        Debug.Log(_material.name);
        //if(_material.name == "Ground")
            _material.SetInteger("_isDanger", 1);
    }

    void TurnOffDanger(Vector2Int a_GridPos)
    {
        //Debug.Log("TurnOffDanger : " + a_GridPos.x + " - " + a_GridPos.y);
        Tile _actualTile = grid.GetTile(a_GridPos);
        uint _x = _actualTile.x;
        uint _y = _actualTile.y;
        TurnOffDangerTile(_x, _y);
        TurnOffDangerTile(_x - 1, _y);
        TurnOffDangerTile(_x, _y - 1);
        TurnOffDangerTile(_x + 1, _y);
        TurnOffDangerTile(_x, _y + 1);
    }

    void TurnOffDangerTile(uint a_x, uint a_y)
    {
        Tile _actualTile = grid.GetTile(new Vector2Int((int)a_x, (int)a_y));
        Material _material = _actualTile.gameObject.GetComponent<SpriteRenderer>().material;
        //if (_material.name == "Ground")
            _material.SetInteger("_isDanger", 0);
    }

    public void SetPath(Path a_Path)
    {
        m_Path = a_Path;

    }

    private void CalculatePath()
    {
        Tile t_StartTile = grid.GetTile(grid.WorldToGrid(transform.position));
        Tile t_EndTile = grid.GetTile(grid.WorldToGrid(Objective.position));

        m_Path = Pathfinder.GetPath(t_StartTile, t_EndTile, false);
    }

    private void OnDrawGizmosSelected() {
        if (m_Path == null)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < m_Path.Checkpoints.Count - 1; i++) {
            Gizmos.DrawLine(m_Path.Checkpoints[i].transform.position, m_Path.Checkpoints[i + 1].transform.position);
        }
    }

    public void Explodes()
    {
        Destroy(gameObject);
    }

}
