using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

/*    private Player player;
    private Transform playerT;

    private float dodgeVelocity;
    private float jumpVelocity;
    private float instantJumpVelocity;
    public float gravity = 9.8f;

    public float distanceInTurn = 0f;
    public Vector3 rotationPoint;
    private float velocityInTurn;
    public Vector3 newGroundCenter;
    public int targetAngle;

    private float fallVelocity = 0f;//Will define how fast the player will fall (i.e if he wants to slide right after jumping). Can simulate air resistance.
    private bool isPerformingAttack = false;
    // Use this for initialization
    void Start () {
        player = GetComponent<Player>();
        playerT = player.transform;
    }

    public void SetVelocitysBasedOnAnimation(Animation playerAnim, float xMovement, float jumpHeight, float jumpSpeed)
    {
        float animationEffect = playerAnim["jump"].speed * (jumpSpeed / 2);
        //Defining speeds for actions in the game
        dodgeVelocity = xMovement / (playerAnim["right"].length / playerAnim["right"].speed);
        jumpVelocity = jumpHeight / (playerAnim["jump"].length / animationEffect);
        instantJumpVelocity = jumpVelocity;
    }

    public void MovePlayerForward(PlayerStates playerState, float fwdVelocity)
    {
        if (!isPerformingAttack)
        {
            if (!playerState.IsExecutingAction() && !playerState.isOnAir)
            {
                player.anim.Play("run");
            }
            playerT.Translate(Vector3.forward * fwdVelocity * Time.deltaTime);
        }
    }

    public void ApplyGravity(ref PlayerStates playerState, ListQueue<PlayerActions> actionsQueue, float yGround)
    {
        if (playerState.isOnAir && !playerState.isJumping && !playerState.isPerformingAttack())
        {
            float fallRate = (actionsQueue.first == PlayerActions.SLIDE) ? 8f : 1f; //Fall 8 times faster if the player wants to slide
            fallVelocity += (gravity * fallRate * Time.fixedDeltaTime);

            playerT.Translate(Vector3.down * fallVelocity * Time.fixedDeltaTime);

            if (playerT.position.y <= yGround)
            {
                playerT.position = new Vector3(playerT.position.x, yGround, playerT.position.z);
                playerState.isOnAir = false;
                fallVelocity = 0f;
                player.ConsumeActionQueue(0.3f);
            }
        }
    }


    public void ExecutePlayerAction(ref PlayerStates playerState, float targetPosition, Animator playerAnim)
    {
        if (playerState.isMovingRight)//Move right
        {
            playerT.Translate(Vector3.right * dodgeVelocity * Time.fixedDeltaTime);
            if (Vector3.Dot(playerT.position, playerT.right) >= targetPosition)
            {
                playerT.Translate(Vector3.left * (Vector3.Dot(playerT.position, playerT.right) - targetPosition));
                playerState.isMovingRight = false;
            }
        }
        else if (playerState.isMovingLeft)
        {
            playerT.Translate(Vector3.left * dodgeVelocity * Time.fixedDeltaTime);

            if (Vector3.Dot(playerT.position,playerT.right) <= targetPosition)
            {
                playerT.Translate(Vector3.right*(targetPosition - Vector3.Dot(playerT.position, playerT.right)));
                playerState.isMovingLeft = false;
            }
        }
        else if (playerState.isJumping)
        {
            //Simulate air resistance until instantJumpVelocity is 30% of initial jumpVelocity
            if (instantJumpVelocity > 0.3f * jumpVelocity)
                instantJumpVelocity -= (instantJumpVelocity * 0.07f);//Simulate air resistance

            playerT.Translate(Vector3.up * instantJumpVelocity * Time.fixedDeltaTime);

            if (playerT.position.y >= targetPosition)
            {
                playerT.position = new Vector3(playerT.position.x, targetPosition, playerT.position.z);
                instantJumpVelocity = jumpVelocity;
                playerState.isJumping = false;
            }
        }
        else if (playerState.isSliding)
        {
            if (!playerAnim.IsPlaying("slide"))
            {
                playerState.isSliding = false;
                player.SetBoxCollider_Run();
            }
        }
        else if (!playerState.isSliding && playerState.isPunchAttacking && !isPerformingAttack)
        {
            StartCoroutine("PerformPunchAttack");
        }
        else if (playerState.isSlideAttacking && !isPerformingAttack)
        {
            StartCoroutine("PerformSlideAttack");
        }
        else if (playerState.isDownHillAttacking && !isPerformingAttack)
        {
            StartCoroutine("PerformDownHillAttack");
        }
        else if (playerState.isUpHillAttacking && !isPerformingAttack)
        {
            StartCoroutine("PerformUpHillAttack");
        }
            

    }

    IEnumerator PerformPunchAttack()
    {
        isPerformingAttack = true;
        player.anim.Stop();
        while (true)
        {
            if (!player.playerState.isPunchAttacking)
            {
                isPerformingAttack = false;
                break;
            }
            playerT.position = Vector3.MoveTowards(playerT.position, player.targetEnemy + Vector3.up*0.7f, 30f * Time.fixedDeltaTime);
            player.playerState.isOnAir = true;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator PerformSlideAttack()
    {
        isPerformingAttack = true;
        player.BuildSlide(0.2f);
        player.playerState.isSlideAttacking = true;
        while (true)
        {
            playerT.Translate(playerT.forward * 12f * Time.deltaTime);
            if (!player.playerState.isSlideAttacking)
            {
                isPerformingAttack = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator PerformDownHillAttack()
    {
        isPerformingAttack = true;
        player.anim.Stop();
        while (true)
        {
            if (!player.playerState.isDownHillAttacking)
            {
                isPerformingAttack = false;
                break;
            }
            playerT.position = Vector3.MoveTowards(playerT.position, player.targetEnemy - Vector3.forward*.5f, 30f * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator PerformUpHillAttack()
    {
        isPerformingAttack = true;
        player.anim.Stop();
        while (true)
        {
            if (!player.playerState.isUpHillAttacking)
            {
                isPerformingAttack = false;
                break;
            }
            playerT.position = Vector3.MoveTowards(playerT.position, player.targetEnemy - Vector3.forward * .5f + Vector3.up*0.5f, 30f * Time.fixedDeltaTime);
            player.playerState.isOnAir = true;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator TurnRight()
    {
        if (distanceInTurn == 0f) StopCoroutine("TurnPlayer");
        if (targetAngle == 0) targetAngle = 360;
        isPerformingAttack = true;
        float velocityInTurn = (player.fwdVelocity / distanceInTurn)*Mathf.Rad2Deg;

        while (true)
        {
            //float rotY = Mathf.SmoothDamp(playerT.rotation.eulerAngles.y, targetAngle + 60f, ref velocityInTurn, turnDistance/player.fwdVelocity);
            playerT.Rotate(0f, velocityInTurn * Time.deltaTime, 0f);
            //playerT.rotation = Quaternion.Euler(playerT.rotation.eulerAngles.x, rotY, playerT.rotation.eulerAngles.z);
            playerT.position = rotationPoint + Vector3.up - playerT.right * distanceInTurn;

            if ( (targetAngle != 360 && playerT.rotation.eulerAngles.y >= targetAngle) || (targetAngle == 360 && playerT.rotation.y <= 0))
            {
                playerT.rotation = Quaternion.Euler(playerT.rotation.eulerAngles.x, targetAngle, playerT.rotation.eulerAngles.z);
                player.targetPositions.SetAllPositions(Vector3.Dot(newGroundCenter, playerT.right), 2.4f, 0f);
                distanceInTurn = 0f;
                isPerformingAttack = false;
                player.playerState.canPerformActions = true;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator TurnLeft()
    {
        if (distanceInTurn == 0f) StopCoroutine("TurnPlayer");
        isPerformingAttack = true;
        float velocityInTurn = (player.fwdVelocity / distanceInTurn)*Mathf.Rad2Deg;

        while (true)
        {
            playerT.Rotate(0f, -velocityInTurn * Time.deltaTime, 0f);
            playerT.position = rotationPoint + Vector3.up + playerT.right * distanceInTurn;

            if (playerT.rotation.eulerAngles.y <= targetAngle + 10f)
            {
                playerT.rotation = Quaternion.Euler(playerT.rotation.eulerAngles.x, targetAngle, playerT.rotation.eulerAngles.z);
                player.targetPositions.SetAllPositions(Vector3.Dot(newGroundCenter, playerT.right), 2.4f, 0f);
                distanceInTurn = 0f;
                isPerformingAttack = false;
                player.playerState.canPerformActions = true;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
*/
}
