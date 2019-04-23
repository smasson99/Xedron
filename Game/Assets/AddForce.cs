using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    [SerializeField] private Transform effectZone;
    [SerializeField] private float angle;
    [SerializeField] private float distanceRaisePillar;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject pillarPrefab;

    private bool pillarRaised = false;
    void Update()
    {
        if(Input.anyKey && !pillarRaised)
        {
            float angleTargetOrigin = GetAngleBetweenTargetAndOrigin();
                    Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject pillar = Instantiate(pillarPrefab, effectZone.position, Quaternion.Euler(0, angleTargetOrigin, -(90 - angle)));
            Rigidbody pillarRigidbody = pillar.GetComponent<Rigidbody>();
            pillarRigidbody.position -= pillarRigidbody.transform.up * (pillar.transform.localScale.y);


            float v = FindVelocityToGetToTarget(angle);

            pillarRaised = true;

            StartCoroutine(RaisePillar(pillarRigidbody, v));
        }
    }

    private float FindVelocityToGetToTarget(float angle)
    {
        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        Vector3 vector = target.position - (effectZone.position);
        float distanceX = new Vector2(vector.x, vector.z).magnitude - direction.x * distanceRaisePillar;
        float distanceY = target.position.y - effectZone.position.y - direction.y * distanceRaisePillar;

        float a = direction.y * (distanceX / direction.x);
        float b = (distanceY - a) / -4.9f;
        float c = b / Mathf.Pow(distanceX, 2);
        float d = c * Mathf.Pow(direction.x, 2);
        float v2 = 1 / d;
        float v = Mathf.Sqrt(v2);
        return v;
    }

    private float GetAngleBetweenTargetAndOrigin()
    {
        Vector2 vectorTargetOriginOnGround = new Vector2(target.position.x - effectZone.position.x, target.position.z - effectZone.position.z);

        if (target.position.z - effectZone.position.z < 0)
            return  Mathf.Acos(Vector2.Dot(vectorTargetOriginOnGround.normalized, Vector2.right)) * Mathf.Rad2Deg;
        else
            return  360 - Mathf.Acos(Vector2.Dot(vectorTargetOriginOnGround.normalized, Vector2.right)) * Mathf.Rad2Deg;
    }

    private IEnumerator RaisePillar(Rigidbody pillarRigidbody , float velocity)
    {
        float distanceRaised = 0;
        Vector3 posStart = pillarRigidbody.position;

        while (distanceRaisePillar > distanceRaised)
        {
            if(distanceRaised + velocity * Time.deltaTime > distanceRaisePillar)
            {
                float missingDistanceToRaisePillar = distanceRaisePillar - (pillarRigidbody.position - posStart).magnitude;

                pillarRigidbody.MovePosition(pillarRigidbody.position + pillarRigidbody.transform.up * missingDistanceToRaisePillar);

                distanceRaised += (pillarRigidbody.transform.up * missingDistanceToRaisePillar).magnitude;
            }
            else
            {
                pillarRigidbody.MovePosition(pillarRigidbody.position + pillarRigidbody.transform.up * velocity * Time.deltaTime);
                distanceRaised += (pillarRigidbody.transform.up * velocity * Time.deltaTime).magnitude;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
