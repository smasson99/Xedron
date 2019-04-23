using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LeapingMinion : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private float gravity;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravity = Physics.gravity.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GlobalComponents"))
        {
            Debug.Log("Position: " + rigidbody.position);
            rigidbody.isKinematic = true;
            Destroy(this);
        }

    }

    private void Update()
    {

        //Debug.Log(rigidbody.velocity);
    }

    public void Leap(Vector3 target, float velocity)
    {
        // Find the distance between target and object (u)
        float distanceBetweenTargetAndMinion = Vector3.Distance(transform.position, target);

        // Find the time to get the object with the velocity set 
        //s = u / (u/s)
        float timeTakingToTravel = distanceBetweenTargetAndMinion / velocity;

        // Find the Y velocity  - Divide by 2 because... I dont know O.o
        // u/s = (u/s^2) * s
        float velocityToGetToTheTarget = gravity * timeTakingToTravel / 2;

        // Find the height of the target
        // u
        float diffHeight = target.y - transform.position.y;

        // Find the Y to get the hight (u/s) = u / s
        float velocityToBeAtRightHeight = diffHeight / timeTakingToTravel;

        // Add both velocity
        float velocityY = -velocityToGetToTheTarget + velocityToBeAtRightHeight;

        // Find velocity for x and z
        Vector3 velocityXZ = (target - transform.position).normalized * velocity;

        // Set the velocity
        rigidbody.velocity = new Vector3(velocityXZ.x, velocityY, velocityXZ.z);


        Debug.Log($"Leaping on {target}. ");
    }
}
