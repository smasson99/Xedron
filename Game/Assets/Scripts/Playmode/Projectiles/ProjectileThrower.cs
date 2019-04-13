using System.Collections;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject projectilePrefab;
    
    [Header("Configuration")]
    [SerializeField]
    private float firedAngle = 1;
    
    private GameObject projectile;

    public Transform testTarget;

    private void InstantiateProjectile(Vector3 targetPosition)
    {
        projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position;
        
        projectile.GetComponent<Projectile>()?.Shoot(targetPosition, firedAngle);
    }

    public void Shoot(Vector3 targetPosition)
    {
        InstantiateProjectile(targetPosition);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(testTarget.position);
        }
    }
}
