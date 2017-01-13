using UnityEngine;
using System.Collections;

public class AngryPeople : MonoBehaviour {

    /*
	private Camera mainCamera;
	public GameObject playerGO;
	private Player player;
	private PlayerStates playerState;
	//private Animation playerAnim;
	private Transform playerT;
	private float yGround;
	private float followingDist; //Varies in a 1.7-2.9 range
	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		player = playerGO.GetComponent<Player>();
		playerState = playerGO.GetComponent<PlayerStates>();
		playerT = playerGO.GetComponent<Transform>();
		yGround = player.yGround;
		followingDist = 1.7f;
	}
	
	// Update is called once per frame
	void Update () {
		if (yGround - player.yGround <= - 1.5)//player is going up, we don't follow
		{
			
		}
		else 
		{
			yGround = player.yGround;
		}
		for (int i =0; i<transform.childCount;i++){
			Animation childAnim = transform.GetChild(i).GetComponent<Animation>();
			if (!childAnim.IsPlaying("run"))
				childAnim.Play("run");
		}

	}

    void LateUpdate()
    {
        float newPosx = Mathf.Lerp(transform.position.x, playerT.position.x, 10*Time.deltaTime);
        float newPosy = Mathf.Lerp(transform.position.y, yGround, 8*Time.deltaTime);
        float newPosz = Mathf.Lerp(transform.position.z, playerT.position.z - followingDist, 50 * Time.deltaTime);
        transform.position = new Vector3(newPosx, newPosy, newPosz);
        transform.up = playerT.up;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
        }
    }*/
}
