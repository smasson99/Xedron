using UnityEngine;

namespace Game
{
    public class TransformCopier : MonoBehaviour
    {
        [Tooltip("The transform to copy.")] [SerializeField]
        private Transform targetTransform;

        [Header("Position")]
        [SerializeField]
        private bool copyXPosition = true;

        [SerializeField]
        private bool copyYPosition = true;

        [SerializeField]
        private bool copyZPosition = true;

        [Header("Rotation")]
        [SerializeField]
        private bool copyXRotation = true;

        [SerializeField]
        private bool copyYRotation = true;

        [SerializeField]
        private bool copyZRotation = true;


        private void LateUpdate()
        {
            transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);

            FixPosition();

            FixRotation();
        }

        private void FixPosition()
        {
            Vector3 position = transform.position;
            
            transform.position = new Vector3(!copyXPosition ? 0 : position.x,
                !copyYPosition ? 0 : position.y,
                !copyZPosition ? 0 : position.z);
        }

        private void FixRotation()
        {
            Vector3 eulerAngles = transform.eulerAngles;

            transform.eulerAngles = new Vector3(!copyXRotation ? 0 : eulerAngles.x,
                !copyYRotation ? 0 : eulerAngles.y,
                !copyZRotation ? 0 : eulerAngles.z);
        }
    }
}