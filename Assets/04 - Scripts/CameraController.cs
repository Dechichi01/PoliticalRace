using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private Transform tPlayerMesh;
    private Character player;

    private Transform tCamera;

    public Vector3 cameraOffset = new Vector3(0f, 2f, 4f);
    public float offsetAngleX = 12.6f;
    private float cameraY;

    private float bufferCameraY;
    // Use this for initialization
    void Start () {
        tCamera = GetComponent<Transform>();

        tPlayerMesh = FindObjectOfType<Character>().transform;
        player = FindObjectOfType<Character>();
        cameraY = tCamera.position.y;
	}
	
	void LateUpdate () {
        if (GameController.instance.GetGameState() != GameController.GameState.Running) { return; }

        float newPosX = tPlayerMesh.position.x;
        
        float newPosy = (player.playerState.isJumping || player.playerState.isOnAir)? tCamera.position.y : Mathf.Lerp(tCamera.position.y, tPlayerMesh.position.y + cameraOffset.y, Time.deltaTime * 7);
        float newPosz = Mathf.Lerp(tCamera.position.z, tPlayerMesh.position.z - cameraOffset.z, Time.deltaTime * 50);

        tCamera.position = new Vector3(newPosX, newPosy, newPosz);

       float distance = (Vector3.forward * cameraOffset.z + Vector3.up * cameraOffset.y).magnitude;

        Vector3 targetRot = tPlayerMesh.rotation.eulerAngles + Vector3.right*offsetAngleX;
        Vector3 thisRot = tCamera.rotation.eulerAngles;
        Quaternion destRot = Quaternion.Euler(targetRot.x, targetRot.y, thisRot.z);

        tCamera.rotation = Quaternion.Slerp(tCamera.rotation, destRot, 5f * Time.deltaTime);

        tCamera.position = tPlayerMesh.position - tCamera.forward * cameraOffset.z;
        tCamera.position = new Vector3(tCamera.position.x, newPosy, tCamera.position.z);
	}

    public float GetCameraToPlayerDistance() { return cameraOffset.z ; }


}
