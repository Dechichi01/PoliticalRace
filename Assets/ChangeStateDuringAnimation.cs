using UnityEngine;
using System.Collections;

public class ChangeStateDuringAnimation : StateMachineBehaviour {

    Character character;
    public PlayerStates states;

    [Range(0,1)]
    public float changeTime;
    public bool revert = false;
    [Range(0, 1)]
    public float revertTime = 1;
    PlayerStates previousStates;

    bool changed = false, reverted = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponentInParent<Character>();
        previousStates = new PlayerStates();
        previousStates.Copy(character.playerState);

        changed = reverted = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!changed && stateInfo.normalizedTime > changeTime)
        {
            changed = true;
            character.playerState.Copy(states);
        }
        if (!reverted && revert && stateInfo.normalizedTime > revertTime)
        {
            reverted = true;
            character.playerState.Copy(previousStates);
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
