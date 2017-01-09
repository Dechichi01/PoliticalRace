using UnityEngine;
using System.Collections.Generic;
using Utility;
using System.Collections;

public class PathModule : Module {

    public GameObject playerEnterVerifier;
    public Connection sideEnviromentConnection;

    Module[] sideProps;
    public Module[] natureSideProps, roadSideProps;
    public GameObject natureSideRight, natureSideLeft;
    public GameObject roadSideRight, roadSideLeft;

    public bool natureOnRight = false, natureOnLeft = false;

    Queue<Module> shuffledSideProps;

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

    void GenerateSideEnviroment(MapGenerator mapGen)
    {
        //Enable appropriate sides
        if (natureOnRight) natureSideRight.SetActive(true);
        else roadSideRight.SetActive(true);
        if (natureOnLeft) natureSideLeft.SetActive(true);
        else roadSideLeft.SetActive(true);
        //

        sideProps = natureOnRight ? natureSideProps : roadSideProps;
        shuffledSideProps = new Queue<Module>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));

        Vector3 startPos = GetComponentInChildren<Entrance>().transform.position;
        startPos.x += GetComponent<BoxCollider>().bounds.extents.x;
        Vector3 endPos = GetComponentInChildren<Exit>().transform.position;
        endPos.x = startPos.x;

        GenerateSideProps(mapGen, startPos, endPos);

        //Generate left side
        sideProps = natureOnLeft ? natureSideProps : roadSideProps;
        shuffledSideProps = new Queue<Module>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));

        sideEnviromentConnection.transform.Rotate(Vector3.up * 180f);
        
        startPos.x -= GetComponent<BoxCollider>().bounds.size.x;
        endPos.x = startPos.x;

        GenerateSideProps(mapGen, startPos, endPos);

        sideEnviromentConnection.transform.Rotate(Vector3.up * 180f);
    }

    void GenerateSideProps(MapGenerator mapGen, Vector3 currentPos, Vector3 endPos)
    {
        while (currentPos.z < endPos.z)
        {
            Module currentProp = Instantiate(GetModuleFromQueue());
            currentPos.z += currentProp.bc.bounds.extents.z * 1.25f;
            if (currentPos.z + currentProp.bc.bounds.extents.z *1.25f > endPos.z)
            {
                DestroyImmediate(currentProp.gameObject);
                break;
            }
            sideEnviromentConnection.transform.position = currentPos;
            mapGen.MatchConnections(sideEnviromentConnection, currentProp.GetConnections()[0]);
            currentProp.transform.parent = transform;

            currentPos.z += currentProp.bc.bounds.extents.z*1.25f;
        }
    }

    Module GetModuleFromQueue()
    {
        if (shuffledSideProps.Count == 0) shuffledSideProps = new Queue<Module>(Randomness.ShuffledArray(sideProps, Random.Range(0, 5000)));
        return shuffledSideProps.Dequeue();
    }
}
