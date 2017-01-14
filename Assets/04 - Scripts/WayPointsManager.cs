using UnityEngine;
using System.Collections;

public class WayPointsManager : MonoBehaviour
{

    // put the points from unity interface
    Transform[] wayPointList;

    int currentWayPoint = 0;
    Transform targetWayPoint;

    [HideInInspector]
    public float speed = 4f, rotationSpeed = 1f;

    // Use this for initialization
    void Start()
    {
        ChangeWayPointList();
    }

    public SimpleTransform GetTranslateAmount(Transform Tobject)
    {
        SimpleTransform simpleTrans = new SimpleTransform();
        //simpleTrans.forwardVector = Vector3.Lerp(Tobject.forward, targetWayPoint.forward, 1 - Vector3.Distance(Tobject.position, targetWayPoint.position));
        simpleTrans.forwardVector = Vector3.RotateTowards(Tobject.forward, targetWayPoint.position - Tobject.position, 0.5f * Time.deltaTime, 0.0f);

        // move towards the target
        simpleTrans.position = Vector3.MoveTowards(Tobject.position, targetWayPoint.position, speed * Time.deltaTime);

        CheckForArrival(simpleTrans.position);

        return simpleTrans;
    }

    void CheckForArrival(Vector3 newPos)
    {        
        if (newPos == targetWayPoint.position)
        {
            if (currentWayPoint == wayPointList.Length - 1)
                ChangeWayPointList();

            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint];
        }

        Debug.Log(currentWayPoint);
    }

    void ChangeWayPointList()
    {
        wayPointList = FindObjectOfType<WayPoints>().wayPoints;
        currentWayPoint = 0;
        targetWayPoint = wayPointList[0];
    }
}

public class SimpleTransform
{
    public Vector3 forwardVector, position;
}
