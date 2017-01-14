using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class Controller3D : MonoBehaviour {

    public LayerMask collisionMask;
    public CollisionInfo collisions;

    public float skinWidth = 0.015f;

    public float yGround;

    public void Move(float yMoveAmount)
    {
        collisions.Reset();

        if (yMoveAmount <= 0)
            VerticalCollisions(ref yMoveAmount);

        transform.Translate(Vector3.up*yMoveAmount);
    }

    void VerticalCollisions(ref float yMoveAmount)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Abs(yMoveAmount), collisionMask))
        {
            collisions.below = true;
            yMoveAmount = -(hit.distance - skinWidth);
            yGround = hit.point.y + skinWidth;
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

