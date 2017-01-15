using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
public abstract class Pickup : PlaceableItem {

    private void Update()
    {
        transform.Rotate(0f, 80f * Time.deltaTime, 0f);
    }
}


