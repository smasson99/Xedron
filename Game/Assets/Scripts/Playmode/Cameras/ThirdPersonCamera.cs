using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Tooltip("The player to follow.")] [SerializeField]
        private Transform targetTransform;

        [Tooltip("The distance to keep between the player and the camera.")] [SerializeField]
        private float distanceFromTarget = 5;

        [Tooltip("The translation speed for the camera.")] [SerializeField]
        private float translationSpeed = 10f;

        [Tooltip("The rotation speed for the camera.")] [SerializeField]
        private float rotationSpeed = 10f;

        [Tooltip("The rotation speed for the camera when auto-adjust.")] [SerializeField]
        private float autoAdjustRotationSpeed = 6f;

        [Header("Limits")]
        [Tooltip("The minimum rotation in degrees from the camera and the up of its follow target.")]
        [SerializeField]
        private float minUpRotation = 90;

        [Tooltip("The minimum rotation in degrees from the camera and the up of its follow target.")] [SerializeField]
        private float maxUpRotation = 150;

        private Vector2 leftJoysticDirection;
        private Vector3 rightJoysticDirection;

        private bool canAdjustCamera;
        private Coroutine adjustCameraCoroutine;

        private void Awake()
        {
            VerifyComponents();
        }

        private void VerifyComponents()
        {
            if (targetTransform == null)
            {
                throw new NullReferenceException(nameof(targetTransform) + "can't be null.");
            }
        }

        private void RotateCameraInRightJoysticDirection()
        {
            Vector3 targetVector3 = targetTransform.position - transform.position;
            float targetUpAngle = Vector3.Angle(targetVector3, targetTransform.up);

            if (targetUpAngle >= minUpRotation && targetUpAngle <= maxUpRotation)
            {
                transform.Translate(rightJoysticDirection * rotationSpeed * Time.deltaTime);
            }
            else
            {
                if (targetUpAngle < minUpRotation)
                {
                    if (rightJoysticDirection.y >= 0)
                    {
                        transform.Translate(rightJoysticDirection * rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        Vector2 newDirection = new Vector2(rightJoysticDirection.x, 0);
                        transform.Translate(newDirection * rotationSpeed * Time.deltaTime);
                    }
                }
                else if (targetUpAngle > maxUpRotation)
                {
                    if (rightJoysticDirection.y < 0)
                    {
                        transform.Translate(rightJoysticDirection * rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        Vector2 newDirection = new Vector2(rightJoysticDirection.x, 0);
                        transform.Translate(newDirection * rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }

        private void MoveCameraToTarget()
        {
            Vector3 targetVector = targetTransform.position - transform.position;

            targetVector.y = 0;

            if (Vector3.Distance(new Vector3(targetTransform.position.x, 0, targetTransform.position.z),
                    new Vector3(transform.position.x, 0, transform.position.z)) >= distanceFromTarget)
            {
                transform.Translate(targetVector * translationSpeed * Time.deltaTime, Space.World);
            }
        }

        private void MoveCamera(bool right)
        {
            if (right)
            {
                transform.Translate(Vector3.right * rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.left * rotationSpeed * Time.deltaTime);
            }
        }

        private void StopAdjustCamera()
        {
            canAdjustCamera = false;
            StopCoroutine(adjustCameraCoroutine);
            adjustCameraCoroutine = null;
        }

        private bool LeftJoysticIsUsed()
        {
            return leftJoysticDirection != Vector2.zero;
        }

        private bool RightJoysticIsUsed()
        {
            return rightJoysticDirection != Vector3.zero;
        }

        private void Update()
        {
            if (RightJoysticIsUsed())
            {
                
            }
        }

        private void LateUpdate()
        {
            transform.position = -targetTransform.forward * distanceFromTarget + targetTransform.position;
            
            transform.LookAt(targetTransform);
        }

        private IEnumerator AdjustCameraIn(float numberOfSeconds)
        {
            yield return new WaitForSeconds(numberOfSeconds);

            canAdjustCamera = true;
        }

        public void SetTargetTransform(Transform newTarget)
        {
            targetTransform = newTarget;
        }

        public void UpdateLeftJoysticDirection(Vector2 direction)
        {
            leftJoysticDirection = direction;
        }

        public void UpdateRightJoysticDirection(Vector2 direction)
        {
            rightJoysticDirection = new Vector3(direction.x, 0, direction.y);
        }
    }
}