using UnityEngine;
using System.Collections;

public class WayPointsManager : MonoBehaviour
{

    // put the points from unity interface
    Transform[] wayPointList;

    int currentWayPoint = 0;
    Transform targetWayPoint;
    Transform previousTargetWayPoint;

    [HideInInspector]
    public float speed = 4f, rotationSpeed = 1f;

    public float distanceBetweenWayPoints;

    private void Awake()
    {
        targetWayPoint = FindObjectOfType<Character>().transform;
        ChangeWayPointList();
    }

    public SimpleTransform GetTranslateAmount(Transform Tobject, Vector3 position)
    {
        SimpleTransform simpleTrans = new SimpleTransform();
        float percent = Mathf.Clamp01(1 - Vector3.Distance(position, targetWayPoint.position) / distanceBetweenWayPoints);
        simpleTrans.forwardVector = Vector3.Lerp(previousTargetWayPoint.forward, targetWayPoint.forward, percent);

        // move towards the target
        simpleTrans.position = Vector3.MoveTowards(position, targetWayPoint.position, speed * Time.deltaTime);

        CheckForArrival(simpleTrans.position);

        return simpleTrans;
    }

    void CheckForArrival(Vector3 newPos)
    {        
        if (newPos == targetWayPoint.position)
        {
            if (currentWayPoint == wayPointList.Length - 1)
            {
                ChangeWayPointList();
                currentWayPoint--;
            }

            currentWayPoint++;
            distanceBetweenWayPoints = Vector3.Distance(wayPointList[currentWayPoint].position, targetWayPoint.position);
            previousTargetWayPoint = targetWayPoint;
            targetWayPoint = wayPointList[currentWayPoint];
        }
    }

    void ChangeWayPointList()
    {
        wayPointList = FindObjectOfType<WayPoints>().wayPoints;
        currentWayPoint = 0;
        previousTargetWayPoint = targetWayPoint;
        targetWayPoint = wayPointList[0];
        distanceBetweenWayPoints = Vector3.Distance(previousTargetWayPoint.position, targetWayPoint.position);
    }
}

public class SimpleTransform
{
    public Vector3 forwardVector, position;
}
