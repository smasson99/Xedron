using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private Rigidbody body;
    
    private static float KNOCKBACK_VELOCITY = 5;
    private static float KNOCKBACK_DISTANCE = 5;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            float time = KNOCKBACK_DISTANCE / KNOCKBACK_VELOCITY;

            float vi = -Physics.gravity.y * time * time / 2;

            body.velocity = new Vector3(-transform.forward.x * KNOCKBACK_VELOCITY, vi, -transform.forward.z * KNOCKBACK_VELOCITY);
        }
    }
}
