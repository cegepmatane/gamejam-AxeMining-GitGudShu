using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float Speed = 10f;
    public int nbActionToMove = 3;
    public Pathfinder Pathfinder;
    public Grid grid;
    public Transform Objective;
    public bool canMove = false;

    private Path m_Path;
    private Animator m_Anim;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        transform.position = grid.GridToWorld(grid.WorldToGrid(transform.position));
    }

    // Update is called once per frame
    void Update()
    {
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

            Vector3 t_Direction = t_TargetPos - transform.position;
            Vector3 t_Deplacement = t_Direction.normalized * Time.deltaTime * Speed;
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

                if (m_Path.Checkpoints.Count == 1)
                {
                    m_Anim.SetTrigger("Explode");
                    StartCoroutine(WaitBeforeDestruction());
                    Destroy(gameObject);
                }
            }
        }
        if(Player.time % nbActionToMove == 0 && !canMove)
        {
            Player.time++;
            CalculatePath();
            canMove = true;
        }

        m_Anim.SetBool("isMoving", canMove);
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

    private void OnDrawGizmosSelected()
    {
        if (m_Path == null)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < m_Path.Checkpoints.Count - 1; i++)
        {
            Gizmos.DrawLine(m_Path.Checkpoints[i].transform.position, m_Path.Checkpoints[i + 1].transform.position);
        }
    }

    IEnumerator WaitBeforeDestruction()
    {
        yield return new WaitForSeconds(2.0f);
    }
}
