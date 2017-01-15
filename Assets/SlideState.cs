using UnityEngine;
using System.Collections;

public class SlideState : StateMachineBehaviour {

    Character player;

    RangeFloat startSlide, finishSlide;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = animator.GetComponentInParent<Character>();
        startSlide = new RangeFloat(0f, .24f);
        finishSlide = new RangeFloat(.46f, 1f);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    if (stateInfo.normalizedTime >= startSlide.start && stateInfo.normalizedTime <= startSlide.end)
        {
            float percent = (stateInfo.normalizedTime - startSlide.start) / (startSlide.end - startSlide.start);
            player.frontCollider.size = Vector3.Lerp(player.FCSizeRun, player.FCSizeSlide, percent);
            player.frontCollider.center = Vector3.Lerp(player.FCCenterRun, player.FCCenterSlide, percent);

            player.sidesCollider.size = Vector3.Lerp(player.SCSizeRun, player.SCSizeSlide, percent);
            player.sidesCollider.center = Vector3.Lerp(player.SCCenterRun, player.SCCenterSlide, percent);
        }
        else if (stateInfo.normalizedTime >= finishSlide.start && stateInfo.normalizedTime <= finishSlide.end)
        {
            float percent = (stateInfo.normalizedTime - finishSlide.start) / (finishSlide.end - finishSlide.start);
            player.frontCollider.size = Vector3.Lerp(player.FCSizeSlide, player.FCSizeRun, percent);
            player.frontCollider.center = Vector3.Lerp(player.FCCenterSlide, player.FCCenterRun, percent);

            player.sidesCollider.size = Vector3.Lerp(player.SCSizeSlide, player.SCSizeRun, percent);
            player.sidesCollider.center = Vector3.Lerp(player.SCCenterSlide, player.SCCenterRun, percent);
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

    public struct RangeFloat {
        public float start, end;

        public RangeFloat(float _start, float _end) { start = _start; end = _end; }
    }
}
