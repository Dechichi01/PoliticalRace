using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Module[] modules;
    public Module startModule;

    public int iterations = 5;

    List<Module> instantiedModules = new List<Module>();
    Module moduleVerifier;
    Module lastModule;
    Transform generatedMap;

    private static MapGenerator instance;
    public static MapGenerator GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        //Create map holder and first module
        string holderName = "Generated Map";
        if (transform.FindChild(holderName) != null)
            DestroyImmediate(transform.FindChild(holderName).gameObject);

        generatedMap = new GameObject(holderName).transform;
        generatedMap.parent = transform;

        Module startingModule = (Module) Instantiate(startModule, transform.position, transform.rotation);
        startingModule.transform.parent = generatedMap;

        //
        instantiedModules.Clear();
        instantiedModules.Add(startingModule);
        moduleVerifier = startingModule;
        startingModule.GenerateObstacles(this);
        GeneratePath();
    }

    public void GeneratePath()
    {
        while (instantiedModules.Count > 3)
        {
            Destroy(instantiedModules[0].gameObject);
            instantiedModules.RemoveAt(0);
        }

        moduleVerifier.playerEnterVerifier.SetActive(false);

        List<Connection> pendingConnections = instantiedModules[instantiedModules.Count - 1].GetEntranceAndExit();
        pendingConnections.RemoveAll(c => (c is Entrance));//Don't connect to the entrance of the first module

        for (int i = 0; i < iterations; i++)
        {
            List<Connection> newConnections = new List<Connection>();

            foreach (Connection connection in pendingConnections)
            {
                //Generate a module and it's obstacles
                Module newModule = GenerateModule(connection);
                newModule.GenerateObstacles(this);
                if (i < 0.6 * iterations) moduleVerifier = newModule;
                instantiedModules.Add(newModule);

                //Get entrance and exits and match
                List<Connection> newModuleConnections = newModule.GetEntranceAndExit();
                Connection connectionToMatch = GetConnection(newModuleConnections);
                MatchConnections(connection, connectionToMatch);

                newConnections.AddRange(newModuleConnections.FindAll(c => c != connectionToMatch));
                newModule.transform.parent = generatedMap;
            }

            pendingConnections = newConnections;
        }

        moduleVerifier.playerEnterVerifier.SetActive(true);
        Debug.Log(moduleVerifier.name + ", " + moduleVerifier.name);
    }

    public Module GenerateModule(Connection connection)
    {
        string newTag = connection.GetRandomConnectTag();
        Module newModulePrefab = GetRandomWithTag(modules, newTag);
        return Instantiate(newModulePrefab);
    }

    //Returns a random connection from a list of connections, priority to entrance, then default connection
    private Connection GetConnection(List<Connection> connections)
    {
        Connection entrance = connections.Find(c => c is Entrance);
        if (entrance) return entrance;
        else
        {
            Connection defaultConnection = connections.Find(c => c.isDefaultConnection);
            if (defaultConnection) return defaultConnection;
            else return connections[Random.Range(0, connections.Count)];
        }
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
