using UnityEngine;
using System.Collections;

public class PlayerEnterVerifier : MonoBehaviour {

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) MapGenerator.GetInstance().GeneratePath();
    }
}
