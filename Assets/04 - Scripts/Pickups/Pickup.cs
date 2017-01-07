using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Rigidbody))]
public abstract class Pickup : MonoBehaviour {

    void Start()
    {
        StartCoroutine("Rotate");
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0f, 80f * Time.deltaTime, 0f);
            yield return new WaitForFixedUpdate();
        }
    }
}


