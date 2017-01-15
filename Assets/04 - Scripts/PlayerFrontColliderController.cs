using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class PlayerFrontColliderController : MonoBehaviour {

    Character player;

    private void Start()
    {
        player = transform.root.GetComponent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!player.playerState.dead && other.CompareTag("Obstacle"))
        {
            player.SCCenterSlide.y = .57f;
            player.Die();
        }
    }

}
