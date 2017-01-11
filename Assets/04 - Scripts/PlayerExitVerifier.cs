using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerExitVerifier : MonoBehaviour {

    public PathModule pathModule;
    public bool generatePath = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pathModule.Destroy();
            if (generatePath) MapGenerator.GetInstance().GeneratePath();
        }
    }

}
