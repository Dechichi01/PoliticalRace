using UnityEngine;
using System.Collections;

public class Money : Pickup {

	void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
    }

}
