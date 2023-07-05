using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class HPTracker : MonoBehaviour
{
    private UnityEvent gotHit;
    private float HP = 100;
    private float timeOfDeath;

    [SerializeField] private Vector3 spawnPoint;

    void Start()
    {
        if (gotHit == null)
            gotHit = new UnityEvent();
        gotHit.AddListener(LoseHP);
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            transform.position = spawnPoint;
            timeOfDeath = Time.time;
            HP = 100;
        }
    }

    void LoseHP()
    {
        HP -= 10;
    }

    public void GetHit()
    {
        gotHit.Invoke();
    }
}
