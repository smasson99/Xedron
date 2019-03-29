using Unity.Entities;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// ECS System that handles the gravity effect on its entities.
    /// </summary>
    public class GravitySystem : ComponentSystem
    {
        private struct GravityEntitiesFilter
        {
            public GravityForce gravityForce;
            public CharacterController characterController;
        }

        protected override void OnUpdate()
        {
            foreach (GravityEntitiesFilter entity in GetEntities<GravityEntitiesFilter>())
            {
                entity.characterController.SimpleMove(new Vector3(0, -entity.gravityForce.GravityForceFactor, 0));
            }
        }
    }
}