using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowards : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform target;

    private void Update()
    {
        Vector3 targetDirection = target.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = rotation;
    }
}
