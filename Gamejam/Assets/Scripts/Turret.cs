using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float Range = 3f;
    public float Damage = 1f;
    public float FireRate = 1f;
    public int GoldCost = 50;

    private Ennemy m_Target;
    [SerializeField]
    private Transform m_CanonTip;
    private float m_LastFire;
    private LineRenderer m_Laser;

    private void Awake()
    {
        m_Laser = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        if(Time.time - m_LastFire >= 0.05f)
            m_Laser.enabled = false;

        // Out of range ?
        if (m_Target && Vector3.Distance(m_Target.transform.position, transform.position) > Range)
            m_Target = null;

        if(!m_Target)
        {
            m_Target = FindEnnemy();
        }
        if (m_Target)
        {
            transform.up = m_Target.transform.position - transform.position;

            // Fire !
            if(m_LastFire + (1/FireRate) <= Time.time)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        Debug.Log("Bang");
        m_LastFire = Time.time;
        m_Laser.SetPositions(new Vector3[] {m_CanonTip.position, m_Target.transform.position});
        m_Laser.enabled = true;
    }

    private Ennemy FindEnnemy()
    {
        Ennemy[] t_Ennemies = GameObject.FindObjectsOfType<Ennemy>();

        float t_NearestDist = float.PositiveInfinity;
        Ennemy t_NearestEnnemy = null;

        foreach(var e in t_Ennemies)
        {
            float t_Dist = Vector3.Distance(transform.position, e.transform.position);
            if(t_Dist < t_NearestDist && t_Dist <= Range)
            {
                t_NearestDist = t_Dist;
                t_NearestEnnemy = e;
            }
        }
        return t_NearestEnnemy;   
    }
}
