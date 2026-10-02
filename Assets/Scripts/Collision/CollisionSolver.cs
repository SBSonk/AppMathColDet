using System.Collections.Generic;
using UnityEngine;

namespace CustomCollision
{
    public class CollisionSolver : MonoBehaviour
    {
        static CollisionSolver Instance;

        [Header("PhysicsConfig")]
        [SerializeField, Tooltip("Distance to start culling collisions.")] float minDistanceFiltering = 2.5f;

        // TODO: switch to fixed arr when necessary
        List<ColliderInfo> _activeColliders = new List<ColliderInfo>();

        void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // this is a bad idea and i shouldve done ecs lmao
        void FixedUpdate()
        {
            // TODO: do spatial grid filtering shi for bigger scens
            for (int i = 0; i <_activeColliders.Count; i++)
            {
                var sourceCollider = _activeColliders[i];
                if (!sourceCollider.IsDynamic()) continue; // ignore static objs

                for (int o = 0; o < _activeColliders.Count; o++)
                {
                    if (o == i) continue; // skip self

                    var otherCollider = _activeColliders[o];

                    float dist = Vector3.Distance(sourceCollider.transform.position, otherCollider.transform.position);

                    if (dist > minDistanceFiltering) continue;

                    // check for collisions based on type
                    HandleCollision(sourceCollider, otherCollider);
                }
            }
        }

        void HandleCollision(ColliderInfo a, ColliderInfo b)
        {
            var aColShape = a.GetColliderShape();
            var bColShape = b.GetColliderShape();
            if (aColShape == ColliderShape.Box)
            {
                if (bColShape == ColliderShape.Box)
                {
                    
                }

                if (bColShape == ColliderShape.Sphere)
                {
                    
                }
            }

            if (aColShape == ColliderShape.Sphere)
            {
                
            }
        }

        // /// <summary>
        // /// Creates and registers a collider to the physics solver.
        // /// </summary>
        // /// <param name="g">GameObject to assign to.</param>
        // /// <param name="colliderShape">Shape of the collider.</param>
        // /// <returns>The resulting colliderInfo component for assigning values.</returns>
        // public static ColliderInfo AssignCollider(GameObject g, ColliderShape colliderShape)
        // {
        //     // Dont allow multiple colliders on object because I dont wanna handle all that
        //     if (g.TryGetComponent<ColliderInfo>(out _)) return null;

        //     ColliderInfo result;
        
        //     switch (colliderShape)
        //     {
        //         case ColliderShape.Box: 
        //             g.AddComponent<BoxColliderInfo>();
        //             break;

        //         case ColliderShape.Sphere:
        //             g.AddComponent<SphereColliderInfo>();
        //             break;
        //     }

        //     result = g.GetComponent<ColliderInfo>();

        //     // TODO: should do err handling here
        //     return result;
        // } 
    
        public static bool RegisterCollider(ColliderInfo col)
        {
            if (Instance._activeColliders.Contains(col)) return false;

            Instance._activeColliders.Add(col);
            return true;
        }
    }
}