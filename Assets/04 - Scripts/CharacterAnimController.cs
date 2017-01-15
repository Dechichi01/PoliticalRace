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

    public void PerformAction(float yMoveAmount, float fwdMovPercent)
    {
        anim.SetFloat("forward", fwdMovPercent);
        anim.SetBool("isJumping", yMoveAmount > 0);
        anim.SetBool("onAir", !controller.collisions.below);
        controller.Move(yMoveAmount);
    }

    public void Slide()
    {
        anim.SetTrigger("slide");
    }

    public void Die()
    {
        anim.Play("Die", 0);
        //anim.SetTrigger("die");
    }

}
