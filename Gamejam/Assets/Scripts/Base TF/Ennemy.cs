using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public float Speed = 10f;
    public Pathfinder Pathfinder;
    public Grid Grid;
    public Transform Objective;

    private Path m_Path;

    private void Update()
    {
        if (m_Path == null)
            CalculatePath();

        if (m_Path == null)
            return;

        Vector3 t_TargetPos = m_Path.Checkpoints[1].transform.position;
        Vector3 t_Direction = t_TargetPos - transform.position;
        Vector3 t_Deplacement = t_Direction.normalized * Speed * Time.deltaTime;

        // On dépasse notre checkpoint
        if (t_Deplacement.magnitude >= t_Direction.magnitude)
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

            if (m_Path == null)
                return;

            if (m_Path.Checkpoints.Count == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CalculatePath()
    {
        Tile t_StartTile = Grid.GetTile(Grid.WorldToGrid(transform.position));
        Tile t_EndTile = Grid.GetTile(Grid.WorldToGrid(Objective.position));

        m_Path = Pathfinder.GetPath(t_StartTile, t_EndTile, false);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Path == null)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < m_Path.Checkpoints.Count - 1; i++)
        {
            Gizmos.DrawLine(m_Path.Checkpoints[i].transform.position, m_Path.Checkpoints[i + 1].transform.position);
        }
    }
}
