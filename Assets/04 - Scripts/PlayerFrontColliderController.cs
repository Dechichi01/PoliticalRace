using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class PlayerFrontColliderController : MonoBehaviour {

    /*
    private LevelManager levelManager;
    private GameController gameController;
    private BoxCollider boxCollider;
    private Character player;
    private PlayerController playerController;
    private PlayerStates playerState;
    private Animation playerAnim;
    private CameraController cameraController;

    private bool steppingUp = false;

    public void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        player = FindObjectOfType<Character>();
        playerController = obj.GetComponent<PlayerController>();
        playerState = player.playerState;
        gameController = player.GetComponent<GameController>();
        playerAnim = player.transform.Find("PlayerRotation/PlayerMesh/Prisoner").GetComponent<Animation>() as Animation;
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.CompareTag("Enemy"))
        {
        	//TODO: Dar uma olhada nesse 0.2, é o tempo para que o player possa matar o enemy
            if (playerState.isPerformingAttack()) {
                playerState.ResetAttacks();
                collider.GetComponent<EnemyController>().KillEnemy();
                return;
            }
            else
            {
                gameController.SetGameState(GameController.GameState.Death);
                player.ApplyDeath();
                Invoke("LoadLoseScene", 3f);
            }
        }
        if (collider.CompareTag("EnemySurroundings"))
        {
            player.targetEnemy = collider.transform.parent.transform.position;
            playerState.canAttackEnemy = true;
        }
        if (collider.CompareTag("Obstacle"))
        {            
            gameController.SetGameState(GameController.GameState.Death);
            player.ApplyDeath();
            Invoke("LoadLoseScene", 3f);
        }
        if (collider.CompareTag("TurnRight"))
        {
            playerState.canPerformActions = false;
            Transform afterTurn = collider.transform.GetChild(1).transform;
            Transform beforeTurn = collider.transform.GetChild(0).transform;

            Vector3 turnPoint = collider.transform.GetChild(2).transform.position;
                    
            float centerDistance = Vector3.Distance(beforeTurn.GetChild(0).transform.position, turnPoint);

            if (Vector3.Dot(player.transform.position, player.transform.right) <= player.targetPositions.GroundLeft + 0.2f)
                playerController.distanceInTurn = centerDistance + 2.4f;//left
            else if (Vector3.Dot(player.transform.position, player.transform.right) <= player.targetPositions.GroundCenter + 0.2f)
                playerController.distanceInTurn = centerDistance;
            else
                playerController.distanceInTurn = centerDistance - 2.4f;//right

            playerController.targetAngle = (int) ((player.transform.rotation * Quaternion.Euler(0f,90f,0f)).eulerAngles.y);
            playerController.newGroundCenter = afterTurn.position;
            playerController.rotationPoint = turnPoint;
            playerController.StartCoroutine("TurnRight");
        }
        if (collider.CompareTag("TurnLeft"))
        {
            playerState.canPerformActions = false;
            Transform afterTurn = collider.transform.GetChild(1).transform;
            Transform beforeTurn = collider.transform.GetChild(0).transform;

            Vector3 turnPoint = collider.transform.GetChild(2).transform.position;

            float centerDistance = Vector3.Distance(beforeTurn.GetChild(0).transform.position, turnPoint);

            if (Vector3.Dot(player.transform.position, player.transform.right) <= player.targetPositions.GroundLeft + 0.2f)
                playerController.distanceInTurn = centerDistance - 2.4f;//left
            else if (Vector3.Dot(player.transform.position, player.transform.right) <= player.targetPositions.GroundCenter + 0.2f)
                playerController.distanceInTurn = centerDistance;
            else
                playerController.distanceInTurn = centerDistance + 2.4f;//right

            playerController.targetAngle = (int) ((player.transform.rotation * Quaternion.Euler(0f, -90f, 0f)).eulerAngles.y); 
            playerController.newGroundCenter = afterTurn.position;
            playerController.rotationPoint = turnPoint;
            playerController.StartCoroutine("TurnLeft");
        }

    }

	void OnTriggerExit(Collider collider){
		if (collider.CompareTag("EnemySurroundings")){
            playerState.canAttackEnemy = false;
            //playerState.isAttacking = false;
        }
    }

    private void LoadLoseScene()
    {
        levelManager.LoadLevel("Lose");
    }

    public void TemporaryDisable()
    {
        gameObject.SetActive(false);
        Invoke("ActivateCollider", 0.2f);
    }

    private void ActivateCollider()
    {
        gameObject.SetActive(true);
    }*/
}
