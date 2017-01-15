using UnityEngine;
using System.Collections;

public class Money : Pickup {

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Destroy(gameObject);
    }

}
