using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (PlayerStates))]
[RequireComponent (typeof (SwipeControls))]
public class Player : MonoBehaviour {
    /*
    
    private float xMovement = 2.4f; //Amount of x movement done when dodging
    public float yGround; //y coordinate to make player "step" on the ground.
    public LayerMask groundMask;
    private SwipeControls swipeLogic;
    private CameraController cameraController;

    private Vector3 FCSizeRun, FCCenterRun;
    private Vector3 FCSizeSlide, FCCenterSlide;
    private Vector3 SCSizeRun, SCCenterRun;
    private Vector3 SCSizeSlide, SCCenterSlide;

    public PlayerStates playerState;
    public PlayerPositions targetPositions = new PlayerPositions(); //Set the boundaries for the player movement (6 positions)
    public float targetPosition; //Store the current target position for the player (one of the 6 possibles in the PlayerPositions struct)
    public Vector3 targetEnemy; //Target Enemy position. Set by PlayerFronColliderController when enter EnemySurroundings BoxCollider

    public float dodgeSpeed = 8f;
    public float jumpSpeed = 8f;
    public float fwdVelocity = 12f;
    [Range(1, 3)]
    public float jumpHeight = 2.2f; //How high the player can jump

    public ListQueue<PlayerActions> actionsQueue = new ListQueue<PlayerActions>();


    // Use this for initialization
    void Start () {
        playerState = GetComponent<PlayerStates>() as PlayerStates;
        playerState.canPerformActions = true;
        playerController = GetComponent<PlayerController>() as PlayerController;
        cameraController = Camera.main.GetComponent<CameraController>();
        playerT = transform;

        frontColliderController = transform.Find("Colliders/FrontCollider").GetComponent<PlayerFrontColliderController>();
        sidesColliderController = transform.Find("Colliders/SideColliders").GetComponent<PlayerSideColliderController>();

        frontCollider = transform.Find("Colliders/FrontCollider").GetComponent<BoxCollider>() as BoxCollider;
        sidesCollider = transform.Find("Colliders/SideColliders").GetComponent<BoxCollider>() as BoxCollider;

        anim = GetComponent<Animator>();

        swipeLogic = transform.GetComponent<SwipeControls>() as SwipeControls;

        //FindGround(Vector3.zero);
        yGround = 1;
        Vector3 groundCenter = new Vector3(0f, yGround, playerT.position.z);
        targetPositions.SetAllPositions(groundCenter.x, xMovement, jumpHeight);//Will set all 6 possible final positions for the player based on the Center position, xMovement and jumpHeight
        playerState.isOnAir = false;
       
        SetBoxColliders();

        StartCoroutine("playEntrance");
    }
	
	// Update is called once per frame
	void Update () {
        playerController.ApplyGravity(ref playerState, actionsQueue, yGround);

        if (GameController.instance.GetGameState() != GameController.GameState.Running) { return; }
        ProcessInput();
        ConsumeActionQueue();

        playerController.MovePlayerForward(playerState, fwdVelocity);
        playerController.ExecutePlayerAction(ref playerState, targetPosition, anim);

	}

    void FixedUpdate()
    {
        if (GameController.instance.GetGameState() != GameController.GameState.Running) { return; }
        FindGround(Vector3.zero);
    }

    private void FindGround(Vector3 offset)
    {
        RaycastHit hit;

        if (Physics.Raycast(playerT.position + offset, Vector3.down, out hit, 30f, groundMask))
        {
            float potentialYGround = hit.point.y + playerHeight / 2f;

            if (playerT.up != hit.normal)//Climbing/Descending slope
                playerT.forward = Quaternion.Euler(playerT.forward.z*90f, 0f, -playerT.forward.x*90f) * hit.normal;

            if (yGround != potentialYGround)
                playerState.isOnAir = true;

            yGround = potentialYGround;
        }
    }

    public void ApplyDeath()
    {
        playerState.ResetActions();
        actionsQueue = new ListQueue<PlayerActions>();
        frontColliderController.gameObject.SetActive(false);
        sidesColliderController.gameObject.SetActive(false);
        playerState.isOnAir = false;
        Invoke("EnableOnAir", 0.3f);
        anim.Stop();
        anim.Play("death");
    }

    private void EnableOnAir() { playerState.isOnAir = true; } //Just so we can use Invoke

    private void ProcessInput()
    {
        SwipeControls.SwipeDirection direction = swipeLogic.GetSwipeDirection();

        if (actionsQueue.Count < 2)
        {
            if ((Input.GetKeyDown(KeyCode.D) || direction == SwipeControls.SwipeDirection.Right))
                actionsQueue.Enqueue(PlayerActions.RIGHT);
            else if ((Input.GetKeyDown(KeyCode.A) || direction == SwipeControls.SwipeDirection.Left))
                actionsQueue.Enqueue(PlayerActions.LEFT);
            else if ((Input.GetKeyDown(KeyCode.W) || direction == SwipeControls.SwipeDirection.Jump) && playerState.CanJump(playerState.isOnAir))
            {
                if (playerState.canAttackEnemy && !playerState.isPerformingAttack())
                {
                    if (Vector3.Angle((targetEnemy - playerT.position), Vector3.forward) > 20f)
                        actionsQueue.Enqueue(PlayerActions.UPHILL_ATK);
                    else if ((targetEnemy.z - playerT.position.z) >= 2.5f)
                        actionsQueue.Enqueue(PlayerActions.PUNCH_ATK);
                }
                else
                    actionsQueue.Enqueue(PlayerActions.JUMP);
            }
            else if ((Input.GetKeyDown(KeyCode.S) || direction == SwipeControls.SwipeDirection.Duck) && playerState.CanSlide())
            {
                if (playerState.canAttackEnemy && !playerState.isPerformingAttack())
                {
                    if (playerState.isOnAir && Vector3.Angle((targetEnemy - playerT.position), Vector3.forward) > 20f)
                        actionsQueue.Enqueue(PlayerActions.DOWNHILL_ATK);
                    else
                        actionsQueue.Enqueue(PlayerActions.SLIDE_ATK);
                }
                else
                    actionsQueue.Enqueue(PlayerActions.SLIDE);
            }
        }      
    }

    public void ConsumeActionQueue()
    {
        if (actionsQueue.Count > 0)
        {
            if (actionsQueue.first == PlayerActions.SLIDE && playerState.isOnAir) return;
            if (playerState.isSliding && (Time.time - playerState.startSlideTime) < 0.4f) return;
            if (playerState.isJumping || playerState.isMovingLeft || playerState.isMovingRight) return;

            PlayerActions playerAction = actionsQueue.Dequeue();
            switch (playerAction)
            {
                case (PlayerActions.RIGHT):
                    BuildMoveRight();
                    break;
                case (PlayerActions.LEFT):
                    BuildMoveLeft();
                    break;
                case (PlayerActions.JUMP):
                    BuildJump();
                    break;
                case (PlayerActions.SLIDE):
                    BuildSlide();
                    break;
                case (PlayerActions.PUNCH_ATK):
                    if ((Time.time - lastAttackTime) > attackLatency)
                    {
                        lastAttackTime = Time.time;
                        BuildPunchAttack();
                    }
                    break;
                case (PlayerActions.SLIDE_ATK):
                    if ((Time.time - lastAttackTime) > attackLatency)
                    {
                        lastAttackTime = Time.time;
                        BuildSlideAttack();
                    }
                    break;
                case (PlayerActions.DOWNHILL_ATK):
                    if ((Time.time - lastAttackTime) > attackLatency)
                    {
                        lastAttackTime = Time.time;
                        BuildDownHillAttack();
                    }
                    break;
                case (PlayerActions.UPHILL_ATK):
                    if ((Time.time - lastAttackTime) > attackLatency)
                    {
                        lastAttackTime = Time.time;
                        BuildUpHillAttack();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void BuildMoveLeft()
    {
        playerState.ResetActions(true);

        if (!playerState.CanMoveLeft(playerT, targetPositions, xMovement)) return;

        targetPosition = Vector3.Dot(playerT.position, playerT.right) - xMovement;
        playerState.previousPosition = playerT.position;
        playerState.isMovingLeft = true;
    }

    public void BuildMoveRight()
    {
        playerState.ResetActions(true);

        if (!playerState.CanMoveRight(playerT, targetPositions, xMovement)) return;

        targetPosition = Vector3.Dot(playerT.position, playerT.right) + xMovement;
        playerState.previousPosition = playerT.position;

        playerState.isMovingRight = true;
    }

    public void BuildJump()
    {
        playerState.ResetActions();

        targetPosition = playerT.position.y + jumpHeight;
        anim.Play("jump");

        playerState.isJumping = true;
        playerState.isOnAir = true;
    }

    public void BuildSlide()
    {
        playerState.ResetActions();

        if (playerState.isOnAir)
            return;

        Invoke("SetBoxCollider_Slide", 0.2f);//Wait for the player to get to the ground to change box collider size

        anim.Stop();
        anim.SetTrigger("slide");

        playerState.startSlideTime = Time.time;

        playerState.isSliding = true;
    }

    public void BuildPunchAttack()
    {

        playerState.ResetActions();

        if (playerState.canAttackEnemy && (targetEnemy.z - playerT.position.z) >= 2.5f)
            playerState.isPunchAttacking = true;
    }

    public void BuildSlideAttack()
    {
        playerState.ResetActions();

        if (playerState.canAttackEnemy && (targetEnemy.z - playerT.position.z) >= 2.5f)
            Invoke("EnableSlideAttacking", 0.15f);
    }

    public void BuildDownHillAttack()
    {
        playerState.ResetActions();

        if (playerState.canAttackEnemy && playerState.isOnAir && Vector3.Angle((targetEnemy - playerT.position), Vector3.forward) > 20f)
            playerState.isDownHillAttacking = true;
    }

    public void BuildUpHillAttack()
    {
        playerState.ResetActions();

        if (playerState.canAttackEnemy && Vector3.Angle((targetEnemy - playerT.position), Vector3.forward) > 20f)
            playerState.isUpHillAttacking = true;
    }

    public void EnableSlideAttacking() //Just so we can use Invoke
    {
        playerState.isSlideAttacking = true;
    }

    private void SetBoxColliders()
    {
        FCCenterRun = frontCollider.center;
        FCSizeRun = frontCollider.size;
        FCCenterSlide = new Vector3(frontCollider.center.x, -0.5f, frontCollider.center.z);
        FCSizeSlide = new Vector3(frontCollider.size.x, 0.5f, frontCollider.size.z);

        SCCenterRun = sidesCollider.center;
        SCSizeRun = sidesCollider.size;
        SCCenterSlide = new Vector3(sidesCollider.center.x, -0.5f, sidesCollider.center.z);
        SCSizeSlide = new Vector3(sidesCollider.size.x, 0.5f, sidesCollider.size.z);
    }

    public void SetBoxCollider_Slide()
    {
        frontCollider.size = FCSizeSlide;
        frontCollider.center = FCCenterSlide;

        sidesCollider.size = SCSizeSlide;
        sidesCollider.center = SCCenterSlide;

    }

    public void SetBoxCollider_Run()
    {
        frontCollider.size = FCSizeRun;
        frontCollider.center = FCCenterRun;

        sidesCollider.size = SCSizeRun;
        sidesCollider.center = SCCenterRun;

    }

    IEnumerator playEntrance()
    {
        Camera mainCamera = Camera.main;

        playerT.position = new Vector3(playerT.position.x, mainCamera.transform.position.y + 0.65f, mainCamera.transform.position.z - 4f);

        float maxDeltaY = 0.6f;
        float Voy = 4.5f;
        float g = Mathf.Pow(Voy, 2) / (2 * maxDeltaY);
        float timeToMax = Voy / g;
        float Vy = Voy;
        float t = Time.time;

        float deltaZToMax = mainCamera.GetComponent<CameraController>().GetCameraToPlayerDistance();
        float Vzc = deltaZToMax / timeToMax;
        float zInMax = playerT.position.z + deltaZToMax;

        float deltaZToFinish = Mathf.Abs(Mathf.Abs(mainCamera.transform.position.z + deltaZToMax) - Mathf.Abs(zInMax));
        float Voz = Vzc;
        float Vfz = 0.2f * Voz;
        float res = (Mathf.Pow(Voz, 2) - Mathf.Pow(Vfz, 2)) / (2 * deltaZToFinish);
        float timeToFinish = (Voz - Vfz) / res;
        float Vz = Voz;

        anim.Play("jump");

        while (playerT.position.y > yGround)
        {
            yield return new WaitForFixedUpdate();
            Vy = Vy - g * Time.fixedDeltaTime;

            if (Vz > Vfz)
            {
                Vz = (playerT.position.z > mainCamera.transform.position.z - 0.3f) ? (Vz - res * Time.fixedDeltaTime) : Vzc;
            }
            else
            {
                Vz = Vfz;
            }
            playerT.Translate((Vector3.up * Vy + Vector3.forward * Vz) * Time.fixedDeltaTime);
        }
        GameController.instance.SetGameState(GameController.GameState.Running);
        StopCoroutine("playEntrance");
    }
    */
}



