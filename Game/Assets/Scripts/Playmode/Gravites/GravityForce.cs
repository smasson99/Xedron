using UnityEngine;

namespace Game
{
    /// <summary>
    /// ECS Component that contains the properties required for the ECS Systems that handles gravity to work properly.
    /// </summary>
    public class GravityForce : MonoBehaviour
    {
        [Tooltip("The gravity force to apply to this GameObject")]
        [SerializeField]
        private float gravityForceFactor = 1.0f;

        public float GravityForceFactor
        {
            get => gravityForceFactor;
            private set => gravityForceFactor = value;
        }
    }
}