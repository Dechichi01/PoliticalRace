using UnityEngine;
using System.Collections.Generic;
using Utility;
using System.Collections;

[RequireComponent(typeof(WayPointsManager))]
public class PathModule : Module {

    public Connection sideEnviromentConnection;

    public Vector3[] envPoints_R, envPoints_L;

    public SideEnviroment[] natureSideProps, roadSideProps;
    public GameObject natureSideRight, natureSideLeft;
    public GameObject roadSideRight, roadSideLeft;

    [HideInInspector]
    public WayPointsManager wayPointsManager;
    SideEnviroment[] sideProps;
    Queue<SideEnviroment> shuffledSideProps;

    [HideInInspector]
    public bool natureOnRight = false, natureOnLeft = false;

    public override void Reuse(Vector3 position, Quaternion rotation)
    {
        base.Reuse(position, rotation);
        if (wayPointsManager == null)
        {
            wayPointsManager = GetComponent<WayPointsManager>();
            wayPointsManager.pathModule = this;
        }

        GameManager.instance.wayPointsManagerQueue.Enqueue(wayPointsManager);
    }

    public List<Connection> GetObstacleConnections()
    {
        List<Connection> connections = new List<Connection>(GetComponentsInChildren<Connection>());
        connections.RemoveAll(c => !(c is ObstacleConnection));
        return connections;
    }

    public List<Connection> GetEntranceAndExit()
    {
        List<Connection> connections = new List<Connection>(GetComponentsInChildren<Connection>());
        connections.RemoveAll(c => (!(c is Entrance) && !(c is Exit)));
        return connections;
    }

    public void GenerateObstacles(MapGenerator mapGen)
    {
        List<Connection> obstacleConnections = GetObstacleConnections();

        foreach (Connection connection in obstacleConnections)
        {
            //Get a random tag of a object this connection can connect to
            Module newModule = mapGen.GenerateObstacle(connection);
            List<Connection> newModuleConnections = newModule.GetConnections();
            mapGen.MatchConnections(connection, newModuleConnections[0]);
            newModule.transform.parent = transform;
        }

        GenerateSideEnviroment(mapGen);
    }

    protected void GenerateSideEnviroment(MapGenerator mapGen)
    {
        //Enable appropriate sides
        natureSideRight.SetActive(natureOnRight);
        natureSideLeft.SetActive(natureOnLeft);
        roadSideRight.SetActive(!natureOnRight);
        roadSideLeft.SetActive(!natureOnLeft);
        //

        sideProps = natureOnRight ? natureSideProps : roadSideProps;
        shuffledSideProps = new Queue<SideEnviroment>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));

        for (int i = 0; i < envPoints_R.Length - 1; i++)
        {
            Vector3 startPos = transform.TransformPoint(envPoints_R[i]);
            Vector3 endPos = transform.TransformPoint(envPoints_R[i + 1]);
            Vector3 incrementDir = (endPos - startPos).normalized;
            sideEnviromentConnection.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0f, -90f, 0f)*incrementDir);
            GenerateSideProps(mapGen, startPos, endPos, incrementDir);
        }

        //Generate left side
        sideProps = natureOnLeft ? natureSideProps : roadSideProps;
        shuffledSideProps = new Queue<SideEnviroment>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));

        sideEnviromentConnection.transform.Rotate(Vector3.up * 180f);

        for(int i = 0; i < envPoints_L.Length - 1; i++)
        {
            Vector3 startPos = transform.TransformPoint(envPoints_L[i]);
            Vector3 endPos = transform.TransformPoint(envPoints_L[i + 1]);
            Vector3 incrementDir = (endPos - startPos).normalized;
            sideEnviromentConnection.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0f, 90f, 0f) * incrementDir);
            GenerateSideProps(mapGen, startPos, endPos, incrementDir);
        }

        sideEnviromentConnection.transform.Rotate(Vector3.up * 180f);
    }

    protected void GenerateSideProps(MapGenerator mapGen, Vector3 currentPos, Vector3 endPos, Vector3 incrementDir)
    {
        while (Vector3.Dot(currentPos,incrementDir) < Vector3.Dot(endPos,incrementDir))
        {
            SideEnviroment currentProp = GetModuleFromQueue().Instantiate().GetComponent<SideEnviroment>();
            sideEnviromentConnection.transform.position = currentPos;
            mapGen.MatchConnections(sideEnviromentConnection, currentProp.GetConnections()[0]);
            currentProp.transform.parent = transform;

            float increment = Vector3.Dot(currentProp.bc.bounds.size, incrementDir) *
                (1 + Random.Range(currentProp.minDistFactor, currentProp.maxDistFactor));
            currentPos += Mathf.Abs(increment) * incrementDir;
        }
    }

    protected SideEnviroment GetModuleFromQueue()
    {
        if (shuffledSideProps.Count == 0) shuffledSideProps = new Queue<SideEnviroment>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));
        return shuffledSideProps.Dequeue();
    }

    public void DestroyAll(float delay = 0)
    {
        if (delay != 0)
            StartCoroutine(DestroyAllWithDelay(delay));
        else
        {
            Module[] modulesToDestroy = GetComponentsInChildren<Module>();
            for (int i = 0; i < modulesToDestroy.Length; i++)
                modulesToDestroy[i].Destroy();
        }
    }

    IEnumerator DestroyAllWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyAll();
    }

    protected void OnDrawGizmos()
    {
        DrawCross(envPoints_R);
        DrawCross(envPoints_L);
    }

    protected void DrawCross(Vector3[] points)
    {
        if (points != null)
        {
            Gizmos.color = Color.red;
            float size = 2f;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 globalWaypointPos = points[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.forward * size, globalWaypointPos + Vector3.forward * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
