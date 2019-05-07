using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pillar : MonoBehaviour
{
    [SerializeField] private float angle = 0;
    [SerializeField] private float distanceRaisePillar = 0;

    private new Rigidbody rigidbody = null;
    private Vector3 origin;
    private Vector3 target;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 origin, Vector3 target)
    {
        this.target = target;
        this.origin = origin;

        float angleTargetOrigin = GetAngleBetweenTargetAndOrigin();

        transform.rotation = Quaternion.Euler(0, angleTargetOrigin, -(90 - angle));
        transform.position = origin - transform.up * (transform.localScale.y);
    }

    public void Raise()
    {
        if (origin == null || target == null)
            throw new Exception("The origin and/or the target is not set. Please initialize the object. ");

        float v = FindVelocity(angle);

        StartCoroutine(RaisePillar(rigidbody, v));
    }

    private float FindVelocity(float angle)
    {
        Vector2 composant = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector3 vectorBetweenTargetAndOrigin = target - (origin);
        Vector2 distance = new Vector2(
                new Vector2(vectorBetweenTargetAndOrigin.x, vectorBetweenTargetAndOrigin.z).magnitude - composant.x * distanceRaisePillar,
                target.y - origin.y - Mathf.Pow(composant.y, 2) * distanceRaisePillar
            );

        //Substitute the time variable of the horizontal movement in the equation of the vertical movement.  
        //v * composant.x = distance.x / s => s = distance.x / (v * composant.x)
        //distance.y = v * composant.y * s + 0.5 * gravity.y * s ^ 2 => distance.y = v * composant.y * (distance.x / (v * composant.x)) + 0.5 * gravity.y * (distance.x / (v * composantX)) ^ 2
        //Then isolate v
        return Mathf.Sqrt((Physics.gravity.y / 2 * Mathf.Pow(distance.x, 2)) / ((distance.y - ((composant.y) * (distance.x / composant.x))) * Mathf.Pow(composant.x, 2)));
    }

    private float GetAngleBetweenTargetAndOrigin()
    {
        Vector2 vectorTargetOriginOnGround = new Vector2(target.x - origin.x, target.z - origin.z);

        if (target.z - origin.z < 0)
            return Mathf.Acos(Vector2.Dot(vectorTargetOriginOnGround.normalized, Vector2.right)) * Mathf.Rad2Deg;
        else
            return 360 - Mathf.Acos(Vector2.Dot(vectorTargetOriginOnGround.normalized, Vector2.right)) * Mathf.Rad2Deg;
    }

    private IEnumerator RaisePillar(Rigidbody pillarRigidbody, float velocity)
    {
        float distanceRaised = 0;
        while (distanceRaisePillar > distanceRaised)
        {
            //The end position of the pillar is not respected. Might give small error.

            pillarRigidbody.MovePosition(pillarRigidbody.position + pillarRigidbody.transform.up * velocity * Time.deltaTime);
            distanceRaised += (pillarRigidbody.transform.up * velocity * Time.deltaTime).magnitude;

            yield return new WaitForFixedUpdate();
        }
    }
}
