using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class Controller3D : MonoBehaviour {

    public BoxCollider boxColl;
    public LayerMask collisionMask;
    public CollisionInfo collisions;

    public float skinWidth = 0.015f;

    public void Move(float yMoveAmount)
    {
        collisions.Reset();

        if (yMoveAmount != 0)
            VerticalCollisions(ref yMoveAmount);

        transform.Translate(Vector3.up*yMoveAmount);
    }

    void VerticalCollisions(ref float yMoveAmount)
    {
        float directionY = Mathf.Sign(yMoveAmount);
        float rayLength = Mathf.Abs(yMoveAmount) + skinWidth;
        Vector3 rayOrigin = boxColl.bounds.center + new Vector3(0f, -boxColl.bounds.extents.y, .3f);

        RaycastHit hit;
        Debug.DrawRay(rayOrigin, Vector3.down * Mathf.Abs(yMoveAmount), Color.red);
        if (Physics.Raycast(rayOrigin, Vector3.up*directionY, out hit, rayLength, collisionMask))
        {
            if (hit.normal != Vector3.up) collisions.climbingSlope = true;
            collisions.below = directionY == -1;
            yMoveAmount = (hit.distance - skinWidth)*directionY;
        }
    }
    
    [System.Serializable]
    public struct CollisionInfo
    {
        public bool left, right, below;
        public bool climbingSlope;

        public void Reset()
        {
            left = right = below = climbingSlope = false;
        }
    }
}

