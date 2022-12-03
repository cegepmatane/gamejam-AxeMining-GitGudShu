using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab_Ennemy;
    public float Interval = 2f;
    public Grid Grid;
    public Pathfinder Pathfinder;
    public Transform[] Objectives;

    private float m_LastSpawn;

    private void Update()
    {
        if(Time.time - m_LastSpawn >= Interval)
        {
            Vector3 t_SpawnPos = Grid.GridToWorld(Grid.WorldToGrid(transform.position));

            GameObject t_NewEnnemy = Instantiate(Prefab_Ennemy, t_SpawnPos, Quaternion.identity, transform);
            t_NewEnnemy.GetComponent<Ennemy>().Pathfinder = Pathfinder;
            t_NewEnnemy.GetComponent<Ennemy>().grid = Grid;
            t_NewEnnemy.GetComponent<Ennemy>().Objective = Objectives[Random.Range(0, Objectives.Length)];

            m_LastSpawn = Time.time;
        }
    }
}
