using UnityEngine;

namespace Game
{
    public class TransformCopier : MonoBehaviour
    {
        [Tooltip("The transform to copy.")] [SerializeField]
        private Transform targetTransform;

        [Header("Position")] 
    
        [SerializeField] private bool copyXPosition = true;
        [SerializeField] private bool copyYPosition = true;
        [SerializeField] private bool copyZPosition = true;
    
        [Header("Rotation")]
    
        [SerializeField] private bool copyXRotation = true;
        [SerializeField] private bool copyYRotation = true;
        [SerializeField] private bool copyZRotation = true;


        private void LateUpdate()
        {
            transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);

            FixPosition();

            FixRotation();
        }
    
        private void FixPosition()
        {
            Vector3 position = transform.position;
        
            if (!copyXPosition)
                transform.position = new Vector3(0, position.y, position.z);
        
            if (!copyYPosition)
                transform.position = new Vector3(position.x, 0, position.z);
        
            if (!copyZPosition)
                transform.position = new Vector3(position.x, position.y, 0);
        }

        private void FixRotation()
        {
            Quaternion rotation = transform.rotation;
        
            if (!copyXRotation)
                transform.rotation = new Quaternion(0, rotation.y, rotation.z, rotation.w);
        
            if (!copyYRotation)
                transform.rotation = new Quaternion(rotation.x, 0, rotation.z, rotation.w);
        
            if (!copyZRotation)
                transform.rotation = new Quaternion(rotation.x, rotation.y, 0, rotation.w);
        }
    }
}
