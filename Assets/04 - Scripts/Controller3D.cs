using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class Controller3D : MonoBehaviour {

    public LayerMask collisionMask;
    public CollisionInfo collisions;

    public float skinWidth = 0.015f;

    public void Move(Vector3 moveAmount)
    {
        collisions.Reset();

        if (moveAmount.y <= 0)
            VerticalCollisions(ref moveAmount);

        Debug.Log(collisions.below);
        transform.Translate(moveAmount);
    }

    void VerticalCollisions(ref Vector3 moveAmount)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, .5f, collisionMask);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Abs(moveAmount.y), collisionMask))
        {
            collisions.below = true;
            moveAmount.y = -(hit.distance - skinWidth);
        }
    }
    
    [System.Serializable]
    public struct CollisionInfo
    {
        public bool left, right, below;

        public void Reset()
        {
            left = right = below = false;
        }
    }
}

