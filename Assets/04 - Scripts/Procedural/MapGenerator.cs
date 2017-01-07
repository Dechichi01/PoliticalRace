using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Module[] modules;
    public Module startModule;

    public int iterations = 5;

    public void GenerateMap()
    {
        //Create map holder and first module
        string holderName = "Generated Map";
        if (transform.FindChild(holderName) != null)
            DestroyImmediate(transform.FindChild(holderName).gameObject);

        Transform generatedMap = new GameObject(holderName).transform;
        generatedMap.parent = transform;

        Module startingModule = (Module) Instantiate(startModule, transform.position, transform.rotation);
        startingModule.transform.parent = generatedMap;

        //
        startingModule.GenerateObstacles(this);
        List<Connection> pendingConnections = startingModule.GetEntranceAndExit();
        pendingConnections.RemoveAll(c => (c is Entrance));//Don't connect to the entrance of the first module

        Module currentModule = startingModule;

        for (int i = 0; i < iterations; i++)
        {
            List<Connection> newConnections = new List<Connection>();

            foreach(Connection connection in pendingConnections)
            {
                //Get a random tag of a object this connection can connect to
                Module newModule = GenerateModule(connection);
                newModule.GenerateObstacles(this);//Generate all obstacles in this Module
                List<Connection> newModuleConnections = newModule.GetEntranceAndExit();
                Connection connectionToMatch = GetConnection(newModuleConnections);
                MatchConnections(connection, connectionToMatch);
                newConnections.AddRange(newModuleConnections.FindAll(c => c != connectionToMatch));
                newModule.transform.parent = generatedMap;
            }

            pendingConnections = newConnections;
        }

        Module[] instantiatedModules = generatedMap.GetComponentsInChildren<Module>();
        Debug.Log(instantiatedModules.Length);
    }

    public Module GenerateModule(Connection connection)
    {
        string newTag = connection.GetRandomConnectTag();
        Module newModulePrefab = GetRandomWithTag(modules, newTag);
        return Instantiate(newModulePrefab);
    }

    //Returns a random connection from a list of connections, priority is default connection always
    private Connection GetConnection(List<Connection> connections)
    {
        Connection defaultConnection = connections.Find(c => c.isDefaultConnection);

        if (defaultConnection) return defaultConnection;
        else return connections[Random.Range(0, connections.Count)];
    }

    public void MatchConnections(Connection oldExit, Connection newExit)
    {
        Transform newModule = newExit.transform.root;
        Vector3 forwardVectorToMatch = -oldExit.transform.forward;
        float angleToRotate = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, angleToRotate);
        Vector3 translation = oldExit.transform.position - newExit.transform.position;
        newModule.transform.position += translation;
    }

    private Module GetRandomWithTag(Module[] modules, string tagToMatch)
    {
        List<Module> matchingModules = new List<Module>();
        for (int i = 0; i < modules.Length; i++)
        {
            if (modules[i].Tag == tagToMatch)
                matchingModules.Add(modules[i]);            
        }
        return matchingModules[Random.Range(0, matchingModules.Count)];
    }

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

}
