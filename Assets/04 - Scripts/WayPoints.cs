using UnityEngine;
using System.Collections;

public class WayPoints : MonoBehaviour
{

    // put the points from unity interface
    public Transform[] wayPointList;

    public int currentWayPoint = 0;
    Transform targetWayPoint;

    [HideInInspector]
    public float speed = 4f;

    // Use this for initialization
    void Start()
    {
        ChangeWayPointList();
    }

    public Vector3 GetForwadMoveAmount(Vector3 position)
    {
        Vector3 moveAmount = Vector3.MoveTowards(position, targetWayPoint.position, speed * Time.deltaTime) - position;

        if (moveAmount == targetWayPoint.position)
        {
            if (currentWayPoint == wayPointList.Length - 1)
                ChangeWayPointList();

            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint];
        }

        return moveAmount;
    }

    public Vector3 GetRotationAmount(Transform Tobject)
    {
        return Vector3.RotateTowards(Tobject.forward, targetWayPoint.position - Tobject.position, speed * Time.deltaTime, 0.0f) - Tobject.rotation.eulerAngles;
    }

    void ChangeWayPointList()
    {

    }
}