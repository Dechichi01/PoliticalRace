using UnityEngine;
using System.Collections;

public class WayPointsManager : MonoBehaviour
{
    // put the points from unity interface
    WayPoint[] wayPointList;

    int currentWayPoint = 0;
    WayPoint targetWayPoint;
    WayPoint previousTargetWayPoint;

    [HideInInspector]
    public float speed = 4f, rotationSpeed = 1f;

    float distanceBetweenWayPoints;

    [HideInInspector]
    public bool generatePathOnExit = false;
    public PathModule pathModule { private get; set; }

    private void Awake()
    {
        wayPointList = GetComponentsInChildren<WayPoint>();
        Initialize();
    }

    private void Initialize()
    {
        currentWayPoint = 0;
        previousTargetWayPoint = targetWayPoint = wayPointList[0];
        distanceBetweenWayPoints = 1f;
    }

    public SimpleTransform GetTranslateAmount(Transform Tobject, Vector3 position)
    {
        SimpleTransform simpleTrans = new SimpleTransform();
        float percent = Mathf.Clamp01(1 - Vector3.Distance(position, targetWayPoint.transform.position) / distanceBetweenWayPoints);
        simpleTrans.forwardVector = Vector3.Lerp(previousTargetWayPoint.transform.forward, targetWayPoint.transform.forward, percent);

        // move towards the target
        simpleTrans.position = Vector3.MoveTowards(position, targetWayPoint.transform.position, speed * Time.deltaTime);

        CheckForArrival(simpleTrans.position);

        return simpleTrans;
    }

    void CheckForArrival(Vector3 newPos)
    {        
        if (newPos == targetWayPoint.transform.position)
        {
            if (currentWayPoint == wayPointList.Length - 1)//Job finished, pass to another wayPointManager
            {
                Initialize();
                if (generatePathOnExit) MapGenerator.GetInstance().GeneratePath();
                GameManager.instance.ChangeWayPointManager();
                pathModule.Destroy(1f);
            }
            else
            {
                currentWayPoint++;
                previousTargetWayPoint = targetWayPoint;
                targetWayPoint = wayPointList[currentWayPoint];
                distanceBetweenWayPoints = Vector3.Distance(previousTargetWayPoint.transform.position, targetWayPoint.transform.position);
            }
        }
    }

}

public class SimpleTransform
{
    public Vector3 forwardVector, position;
}
