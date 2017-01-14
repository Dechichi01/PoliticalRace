using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EventOnState : StateMachineBehaviour {

    public string functionName;
    [Range(0,1)]
    public float callTime;

    Character character;
    bool called = false;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        character = animator.GetComponentInParent<Character>();
        called = false;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    if (!called && stateInfo.normalizedTime > callTime)
        {
            called = true;
            System.Reflection.MethodInfo methodInfo = character.GetType().GetMethod(functionName);
            methodInfo.Invoke(character, null);
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

}
