using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update
    private float moveSpeed = 200.0f;
    [SerializeField] private Bullet bullet;

    [SerializeField] private float CHANGE_THIS_bulletSize;
    // Update is called once per frame
    void Start()
    {

        Destroy(gameObject, 0.4f);
    }
    void Update()
    {

        RaycastHit hit;
        Vector3 fromPosition = transform.position - transform.forward * CHANGE_THIS_bulletSize;
        int layermask1 = 1 << 8;
        int layermask2 = 1 << 9;
        int layermask = ~(layermask1 | layermask2);

        if (Physics.Raycast(fromPosition, transform.forward, out hit, moveSpeed * Time.fixedDeltaTime, layermask))
        {
            transform.position += transform.forward * hit.distance;
            HPTracker hpTracker = hit.collider.gameObject.GetComponent<HPTracker>();
            if (hpTracker != null) { hpTracker.GetHit(); }
            CollisionDetected();
        }

        transform.position += transform.forward * moveSpeed * (Time.deltaTime);


    }
    void CollisionDetected()
    {

        Destroy(gameObject);
    }

}
