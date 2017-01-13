using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller3D))]
public class CharacterAnimController : MonoBehaviour {

    private Animator anim;
    private Controller3D controller;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller3D>();

    }

    public void PerformAction(Vector3 moveAmount, float fwdMovPercent)
    {
        anim.SetFloat("forward", fwdMovPercent);
        anim.SetBool("isJumping", moveAmount.y > 0);
        anim.SetBool("onAir", !controller.collisions.below);
        controller.Move(moveAmount);
    }

    public void Slide()
    {
        anim.SetTrigger("slide");
    }

}
