using UnityEngine;
using System.Collections;

public class PlayerSideColliderController : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) Debug.Log("Gotcha");
    }
}
