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
    public WayPointsManager wayPointsManager { private get; set; }

    private Vector3 FCSizeRun, FCCenterRun;
    private Vector3 FCSizeSlide, FCCenterSlide;
    private Vector3 SCSizeRun, SCCenterRun;
    private Vector3 SCSizeSlide, SCCenterSlide;

    Vector3 centralPathPosition;
    float targetXDisplacement, xDisplacement = 0f;
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

    float yMoveAmount;

    private void Start()
    {
        controller = GetComponent<Controller3D>();
        charAnimCtrl = GetComponent<CharacterAnimController>();
        playerState = new PlayerStates(transform.position.x, xMovement);
        swipeLogic = GetComponent<SwipeControls>();

        SetBoxColliders();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        centralPathPosition = transform.position;
    }

    private void Update()
    {
        ProcessInput();
        ConsumeActionQueue();
        ProcessMovement();

        wayPointsManager.speed = fwdVelocity;
        SimpleTransform simpleTrans = wayPointsManager.GetTranslateAmount(transform, centralPathPosition);
        transform.forward = simpleTrans.forwardVector;
        centralPathPosition = simpleTrans.position;
        transform.position = new Vector3(centralPathPosition.x, transform.position.y, centralPathPosition.z) + transform.right * xDisplacement;

        charAnimCtrl.PerformAction(yMoveAmount, fwdVelocity/maxFwdVelocity);
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
                    if (playerState.CanMoveRight(xDisplacement, xMovement))
                    {
                        targetXDisplacement += xMovement;
                        playerState.canPerformActions = false;
                        playerState.isMovingRight = true;
                    }
                    break;
                case (PlayerActions.LEFT):
                    if (playerState.CanMoveLeft(xDisplacement, xMovement))
                    {
                        targetXDisplacement -= xMovement;
                        playerState.canPerformActions = false;
                        playerState.isMovingLeft = true;
                    }
                    break;
                case (PlayerActions.JUMP):
                    playerState.isSliding = false;
                    jump = true;
                    break;
                case (PlayerActions.SLIDE):
                    StopCoroutine(CheckForSlide());
                    StartCoroutine(CheckForSlide());
                    break;
            }
        }
    }

    void ProcessMovement()
    {
        //Horizontal movement
        if (playerState.isMovingRight)
        {
            xDisplacement += dodgeSpeed * Time.deltaTime;
            if (xDisplacement > targetXDisplacement)
            {
                xDisplacement = targetXDisplacement;
                playerState.isMovingRight = false;
                playerState.canPerformActions = true;
            }
        }
        else if (playerState.isMovingLeft)
        {
            xDisplacement -= dodgeSpeed * Time.deltaTime;
            if (xDisplacement < targetXDisplacement)
            {
                xDisplacement = targetXDisplacement;
                playerState.isMovingRight = false;
                playerState.canPerformActions = true;
            }
        }

        yMoveAmount += gravity * Time.deltaTime * Time.deltaTime;

        if (controller.collisions.below) yMoveAmount = -0.015f;
        playerState.isOnAir = !controller.collisions.below;

        if (jump)
        {
            jump = false;
            yMoveAmount = jumpVelocity * Time.deltaTime;
        }

    }

    IEnumerator CheckForSlide()
    {
        charAnimCtrl.Slide();
        while (playerState.isOnAir)
        {
            yMoveAmount += 1.5f*gravity * Time.deltaTime * Time.deltaTime;
            yield return null;
        }
        SetBoxCollider_Slide();
    }

    private void SetBoxColliders()
    {
        FCCenterRun = frontCollider.center;
        FCSizeRun = frontCollider.size;
        FCCenterSlide = new Vector3(frontCollider.center.x, 0.5f, frontCollider.center.z);
        FCSizeSlide = new Vector3(frontCollider.size.x, 0.5f, frontCollider.size.z);

        SCCenterRun = sidesCollider.center;
        SCSizeRun = sidesCollider.size;
        SCCenterSlide = new Vector3(sidesCollider.center.x, 0.5f, sidesCollider.center.z);
        SCSizeSlide = new Vector3(sidesCollider.size.x, 0.5f, sidesCollider.size.z);
    }

    public void SetBoxCollider_Slide()
    {
        frontCollider.size = FCSizeSlide;
        frontCollider.center = FCCenterSlide;

        sidesCollider.size = SCSizeSlide;
        sidesCollider.center = SCCenterSlide;

    }

    //This should be called by the Slide animations after it finishes
    public void SetBoxCollider_Run()
    {
        frontCollider.size = FCSizeRun;
        frontCollider.center = FCCenterRun;

        sidesCollider.size = SCSizeRun;
        sidesCollider.center = SCCenterRun;
    }
}


[System.Serializable]
public class PlayerStates
{

    public bool canPerformActions = true;
    public bool canJump = true;

    //Player states
    public bool isMovingRight, isMovingLeft;
    public bool isJumping;
    public bool isSliding;
    public bool isPunchAttacking, isSlideAttacking;
    public bool isDownHillAttacking, isUpHillAttacking;
    public float startSlideTime;

    public bool isOnAir;

    [HideInInspector]
    public bool canAttackEnemy; //Tells if the player is close enough (and not to close) to attack the enemy. It's only set when the player enter the "Enemy Surroundings" trigger (see PlayerFronCollider for more info);

    private PlayerPositions targetPositions;
    [HideInInspector]
    public Vector3 previousPosition; //previous position of the player (before moving right or left). Used by PlayerSideColliderController;

    public PlayerStates(float groundCenter, float xMovement)
    {
        targetPositions = new PlayerPositions(groundCenter, xMovement);
    }

    public PlayerStates()
    {

    }

    public void Copy(PlayerStates states)
    {
        canPerformActions = states.canPerformActions;
        canJump = states.canJump;
        isMovingRight = states.isMovingRight;
        isMovingLeft = states.isMovingLeft;
        isJumping = states.isJumping;
        isSliding = states.isSliding;
        isPunchAttacking = states.isPunchAttacking;
        isSlideAttacking = states.isSlideAttacking;
        isUpHillAttacking = states.isUpHillAttacking;
        isDownHillAttacking = states.isDownHillAttacking;
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

    public bool CanMoveRight(float horizontalDisplacement, float xMovement)
    {
        return horizontalDisplacement < xMovement;
    }

    public bool CanMoveLeft(float horizontalDisplacement, float xMovement)
    {
        return horizontalDisplacement > -xMovement;
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

