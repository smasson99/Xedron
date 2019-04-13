using System;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    private Rigidbody rigidbody;
    private bool isMoving = true;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        if (rigidbody is null)
        {
            throw new NullReferenceException(nameof(rigidbody));
        }
    }

    public void UpdateRotation()
    {
        transform.LookAt(rigidbody.velocity);
    }

    private void Update()
    {
        if (isMoving)
        {
            UpdateRotation();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        isMoving = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isMoving = true;
    }
}
