using System;
using UnityEngine;

namespace CustomCollision
{
    [Serializable]
    public enum ColliderShape
    {
        Box, Sphere
    }

    [Serializable]
    public enum BodyType
    {
        Static, Kinematic
    }

    public class ColliderInfo : MonoBehaviour
    {
        [SerializeField] protected Vector3 centerOffset;
        [SerializeField] protected float radius = 1;
        [SerializeField] protected ColliderShape colliderShape;
        [SerializeField] protected BodyType bodyType;

        // Returns "other" collider
        public Action<ColliderInfo> OnCollideEnter;
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            DrawColliderShape();
        }

        protected void DrawColliderShape()
        {
            switch(colliderShape)
            {
                case ColliderShape.Box: 
                    Gizmos.DrawWireCube(transform.position, Vector3.one * radius);
                break;

                case ColliderShape.Sphere:
                    Gizmos.DrawWireSphere(transform.position, radius);
                break;
            }
        }

        void Start()
        {
            CollisionSolver.RegisterCollider(this); // assign to simulation
        }

        public bool IsDynamic() => bodyType == BodyType.Kinematic;
        public ColliderShape GetColliderShape() => colliderShape;
    }
}
