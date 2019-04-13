using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float Gravity => Mathf.Abs(Physics.gravity.y);

    [SerializeField]
    private float gravityMultiplier = 1.0f;

    private ProjectileVisual projectileVisual;
    private Rigidbody rigidbody;
    private float GravityValue => Gravity * gravityMultiplier;

    public float GravityMultiplier
    {
        get => gravityMultiplier;
        set => gravityMultiplier = value;
    }

    private void Awake()
    {
        GetComponents();
        VerifyComponents();
    }

    private void GetComponents()
    {
        rigidbody = GetComponent<Rigidbody>();

        projectileVisual = GetComponent<ProjectileVisual>();
    }

    private void VerifyComponents()
    {
        if (projectileVisual is null)
        {
            throw new NullReferenceException(nameof(projectileVisual));
        }

        if (rigidbody is null)
        {
            throw new NullReferenceException(nameof(rigidbody));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        rigidbody.isKinematic = true;
        
        transform.SetParent(other.transform);
    }

    public void Shoot(Vector3 targetPosition, float firingAngle)
    {
        Vector3 position = transform.position;

        float targetDistance = Vector3.Distance(position, targetPosition);

        float projectile_Velocity = targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / GravityValue);

        float velocityX = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float velocityY = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        transform.rotation = Quaternion.LookRotation(targetPosition - position);

        rigidbody.isKinematic = false;
        rigidbody.velocity = transform.TransformVector(new Vector3(0, velocityY, velocityX));
    }
}