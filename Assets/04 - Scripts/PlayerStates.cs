using UnityEngine;
using System.Collections;

public class PlayerStates : MonoBehaviour {

    private Player player;

    public bool canPerformActions;

    //Player states
    public bool isMovingRight, isMovingLeft;
    public bool isJumping;
    public bool isSliding;
    public bool isPunchAttacking, isSlideAttacking;
    public bool isDownHillAttacking, isUpHillAttacking;
    public float startSlideTime;

    public bool isOnAir;

    public bool canAttackEnemy; //Tells if the player is close enough (and not to close) to attack the enemy. It's only set when the player enter the "Enemy Surroundings" trigger (see PlayerFronCollider for more info);

    public Vector3 previousPosition; //previous position of the player (before moving right or left). Used by PlayerSideColliderController;
    void Start()
    {
        player = GetComponent<Player>();
    }

    public void ResetActions()
    {
        isMovingLeft = isMovingRight = isJumping = isSliding = isPunchAttacking = isSlideAttacking = isDownHillAttacking = isUpHillAttacking = false;
    }
    public void ResetActions(bool exceptSliding)
    {
        isMovingLeft = isMovingRight = isJumping = isPunchAttacking = isSlideAttacking = isDownHillAttacking = isUpHillAttacking = false;
    }

    public void ResetAttacks()
    {
        isPunchAttacking = isSlideAttacking = isDownHillAttacking = isUpHillAttacking = false;
    }

    public bool IsExecutingAction()
    {
        return (isMovingRight || isMovingLeft || isJumping || isSliding || isPunchAttacking || isSlideAttacking || isDownHillAttacking || isUpHillAttacking);
    }
    public bool isPerformingAttack()
    {
        return (isPunchAttacking || isSlideAttacking || isDownHillAttacking || isUpHillAttacking);
    }

    public bool CanMoveRight(Transform playerT, PlayerPositions targetPositions, float xMovement)
    {
        if (!(Vector3.Dot(playerT.position, playerT.right) + xMovement > targetPositions.GroundRight + 0.2f) && canPerformActions) { return true; }
        return false;
    }

    public bool CanMoveLeft(Transform playerT, PlayerPositions targetPositions, float xMovement)
    {
        if (!(Vector3.Dot(playerT.position, playerT.right) - xMovement < targetPositions.GroundLeft - 0.2f) && canPerformActions) { return true; }
        return false;
    }

    public bool CanJump(bool isOnAir)
    {
        if (!isOnAir && canPerformActions) { return true; }
        return false;
    }

    public bool CanSlide()
    {
        if (canPerformActions) { return true; }
        return false;
    }

    public override string ToString()
    {
        return "PlayerActions: isMovingRight = " + isMovingRight +
            ", isMovingLeft = " + isMovingLeft +
            ", isJumping = " + isJumping +
            ", isSliding = " + isSliding + "\n";
    }
}
