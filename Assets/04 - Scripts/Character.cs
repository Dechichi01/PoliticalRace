using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller3D))]
[RequireComponent(typeof(CharacterAnimController))]
[RequireComponent(typeof(SwipeControls))]
public class Character : MonoBehaviour {

    public PlayerStates playerState;
    private Controller3D controller;
    private CharacterAnimController charAnimCtrl;
    private PlayerFrontColliderController frontColliderController;
    private PlayerSideColliderController sidesColliderController;
    private SwipeControls swipeLogic;

    float targetPosition;
    bool jump = false;

    private float xMovement = 2.4f; //Amount of x movement done when dodging
    public float dodgeSpeed = 8f;
    public float fwdVelocity = 12f;
    public float maxFwdVelocity = 22f;
    [Range(1, 3)]
    public float jumpHeight = 2.2f; //How high the player can jump
    public float timeToJumpApex = .4f;

    private float jumpVelocity, gravity;


    public BoxCollider frontCollider;
    public BoxCollider sidesCollider;

    ListQueue<PlayerActions> actionsQueue = new ListQueue<PlayerActions>();

    Vector3 moveAmount;
    
    private void Start()
    {
        controller = GetComponent<Controller3D>();
        charAnimCtrl = GetComponent<CharacterAnimController>();
        playerState = new PlayerStates(transform.position.x, xMovement);
        swipeLogic = GetComponent<SwipeControls>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        ProcessInput();
        ConsumeActionQueue();
        ProcessMovement();

        charAnimCtrl.PerformAction(moveAmount, fwdVelocity/maxFwdVelocity);
    }

    private void ProcessInput()
    {
        SwipeControls.SwipeDirection direction = swipeLogic.GetSwipeDirection();

        //IF PC INPUT
        if (direction == SwipeControls.SwipeDirection.Null)
        {
            if (Input.GetKeyDown(KeyCode.D))
                direction = SwipeControls.SwipeDirection.Right;
            else if (Input.GetKeyDown(KeyCode.A))
                direction = SwipeControls.SwipeDirection.Left;
            else if (Input.GetKeyDown(KeyCode.W))
                direction = SwipeControls.SwipeDirection.Jump;
            else if (Input.GetKeyDown(KeyCode.S))
                direction = SwipeControls.SwipeDirection.Duck;
        }

        if (actionsQueue.Count<2)
        {
            switch(direction)
            {
                case SwipeControls.SwipeDirection.Right:
                    actionsQueue.Enqueue(PlayerActions.RIGHT);
                    break;
                case SwipeControls.SwipeDirection.Left:
                    actionsQueue.Enqueue(PlayerActions.LEFT);
                    break;
                case SwipeControls.SwipeDirection.Jump:
                    actionsQueue.Enqueue(PlayerActions.JUMP);
                    break;
                case SwipeControls.SwipeDirection.Duck:
                    actionsQueue.Enqueue(PlayerActions.SLIDE);
                    break;
            }
        }                
    }

    private void ConsumeActionQueue()
    {
        if (playerState.canPerformActions && actionsQueue.Count > 0)
        {
            PlayerActions action = actionsQueue.Dequeue();
            playerState.ResetActions(true);
            switch(action)
            {
                case (PlayerActions.RIGHT):
                    if (playerState.CanMoveRight(transform, xMovement))
                    {
                        playerState.canPerformActions = false;
                        targetPosition = Vector3.Dot(transform.position, transform.right) + xMovement;
                        playerState.isMovingRight = true;
                    }
                    break;
                case (PlayerActions.LEFT):
                    if (playerState.CanMoveLeft(transform, xMovement))
                    {
                        playerState.canPerformActions = false;
                        targetPosition = Vector3.Dot(transform.position, transform.right) - xMovement;
                        playerState.isMovingLeft = true;
                    }
                    break;
                case (PlayerActions.JUMP):
                    playerState.isSliding = false;
                    jump = true;
                    playerState.isJumping = true;
                    playerState.isOnAir = true;
                    break;
                case (PlayerActions.SLIDE):
                    if (!playerState.isOnAir)
                    {
                        playerState.isSliding = true;
                    }
                    break;
            }
        }
    }

    void ProcessMovement()
    {
        moveAmount.z = fwdVelocity*Time.deltaTime;
        moveAmount.x = 0;

        //Horizontal movement
        if (playerState.isMovingRight)
        {
            float lateralMoveAmount = dodgeSpeed * Time.deltaTime;
            float finalLateralPos = Vector3.Dot(transform.position + Vector3.right * lateralMoveAmount, transform.right);
            if (finalLateralPos >= targetPosition)
            {
                lateralMoveAmount = Mathf.Abs(targetPosition - Vector3.Dot(transform.position, transform.right));
                playerState.isMovingRight = false;
                playerState.canPerformActions = true;
            }
            moveAmount += Vector3.right * lateralMoveAmount;
        }
        else if (playerState.isMovingLeft)
        {
            float lateralMoveAmount = dodgeSpeed * Time.deltaTime;
            float finalLateralPos = Vector3.Dot(transform.position + Vector3.left * lateralMoveAmount, transform.right);
            if (finalLateralPos <= targetPosition)
            {
                lateralMoveAmount = Mathf.Abs(targetPosition - Vector3.Dot(transform.position, transform.right));
                playerState.isMovingLeft = false;
                playerState.canPerformActions = true;
            }
            moveAmount += Vector3.left * lateralMoveAmount;
        }

        //Vertical Movement
        playerState.isOnAir = !controller.collisions.below;

        if (controller.collisions.below)
        {
            moveAmount.y = 0f;
        }
        moveAmount.y += gravity * Time.deltaTime * Time.deltaTime;

        if (jump && !playerState.isOnAir)
        {
            jump = false;
            moveAmount.y = jumpVelocity * Time.deltaTime;
        }

    }

}


[System.Serializable]
public class PlayerStates
{

    public bool canPerformActions = true;

    //Player states
    public bool isMovingRight, isMovingLeft;
    public bool isJumping;
    public bool isSliding;
    public bool isPunchAttacking, isSlideAttacking;
    public bool isDownHillAttacking, isUpHillAttacking;
    public float startSlideTime;

    public bool isOnAir;

    public bool canAttackEnemy; //Tells if the player is close enough (and not to close) to attack the enemy. It's only set when the player enter the "Enemy Surroundings" trigger (see PlayerFronCollider for more info);

    PlayerPositions targetPositions;
    public Vector3 previousPosition; //previous position of the player (before moving right or left). Used by PlayerSideColliderController;

    public PlayerStates(float groundCenter, float xMovement)
    {
        targetPositions = new PlayerPositions(groundCenter, xMovement);
    }

    public void ResetActions()
    {
        isMovingLeft = isMovingRight = isJumping = isSliding = isPunchAttacking =
            isSlideAttacking = isDownHillAttacking = isUpHillAttacking = false;
    }
    public void ResetActions(bool exceptSliding)
    {
        isMovingLeft = isMovingRight = isJumping = isPunchAttacking = isSlideAttacking =
            isDownHillAttacking = isUpHillAttacking = false;
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

    public bool CanMoveRight(Transform playerT, float xMovement)
    {
        if (!(Vector3.Dot(playerT.position, playerT.right) + xMovement > targetPositions.GroundRight + 0.2f)) { return true; }
        return false;
    }

    public bool CanMoveLeft(Transform playerT, float xMovement)
    {
        if (!(Vector3.Dot(playerT.position, playerT.right) - xMovement < targetPositions.GroundLeft - 0.2f)) { return true; }
        return false;
    }

    public bool CanJump()
    {
        if (!isOnAir) { return true; }
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

    public struct PlayerPositions
    {
        public float GroundCenter;
        public float GroundRight;
        public float GroundLeft;

        public PlayerPositions(float _GroundCenter, float xMovement)
        {
            GroundCenter = _GroundCenter;
            GroundRight = GroundCenter + xMovement;
            GroundLeft = GroundCenter - xMovement;
        }

        public void SetAllPositions(float _GroundCenter, float xMovement)
        {
            GroundCenter = _GroundCenter;
            GroundRight = GroundCenter + xMovement;
            GroundLeft = GroundCenter - xMovement;
        }

        public override string ToString()
        {
            return "PlayerPositions: GroundCenter = " + GroundCenter +
                ", GroundLeft = " + GroundLeft +
                ", GroundRight = " + GroundRight;
        }

    }
}

public enum PlayerActions
{
    NONE,
    RIGHT, LEFT,
    JUMP, SLIDE,
    PUNCH_ATK, SLIDE_ATK,
    UPHILL_ATK, DOWNHILL_ATK
}

