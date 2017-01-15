using UnityEngine;
using System.Collections;
using Utility;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    //Assigned in the inspector
    public PathModule startModule;
    public PlaceableItem[] obstacles;
    public PlaceableItem[] pickUps;
    public int iterations = 5;
    public int seed;

    System.Random prng;

    PathModule[] modules;//Controlled by GameManager
    Queue<PathModule> shuffledModules;
    public bool natureOnLeft = false, natureOnRight = false;
    PRD prdNatLeft, prdNatRight;

    PathModule moduleVerifier;//module that will trigger new instantiations when player passes on
    PathModule lastModule;
    Transform generatedMap;//Holder for the path created

    //Singleton
    private static MapGenerator instance;
    public static MapGenerator GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
        prng = new System.Random(seed);
    }

    private void Start()
    {
        //GenerateMap();
    }

    public void GenerateMap()
    {
        prng = new System.Random(seed);
        prdNatLeft = new PRD(.5f);
        prdNatRight = new PRD(.5f);
        RandomChangeSideEnvVariables();

        modules = FindObjectOfType<GameManager>().modulesPerState[0].modules;
        ResetShuffledModulesQueue();

        //Create map holder and first module
        string holderName = "Generated Map";
        if (transform.FindChild(holderName) != null)
            DestroyImmediate(transform.FindChild(holderName).gameObject);

        generatedMap = new GameObject(holderName).transform;
        generatedMap.parent = transform;

        PathModule startingModule = startModule.Instantiate(transform.position, transform.rotation).GetComponent<PathModule>();
        startingModule.transform.parent = generatedMap;

        lastModule = startingModule;
        moduleVerifier = startingModule;
        startingModule.natureOnRight = natureOnRight;
        startingModule.natureOnLeft = natureOnLeft;
        startingModule.GenerateObstacles(this);
        GeneratePath();
    }

    public void GeneratePath()
    {
        List<Connection> pendingConnections = lastModule.GetEntranceAndExit();
        pendingConnections.RemoveAll(c => (c is Entrance));//Don't connect to the entrance of the last module

        for (int i = 0; i < iterations; i++)
        {
            List<Connection> newConnections = new List<Connection>();

            foreach (Connection connection in pendingConnections)
            {
                //Generate a module and it's obstacles
                PathModule newModule = GenerateModule(connection);
                if (i < 0.6 * iterations) moduleVerifier = newModule;
                lastModule = newModule;

                //Random change in side enviroment (road/nature) if module is a turn
                if (newModule is TurnLeftModule || newModule is TurnRightModule) RandomChangeSideEnvVariables();
                newModule.natureOnLeft = natureOnLeft;
                newModule.natureOnRight = natureOnRight;

                //Get entrance and exits and match
                List<Connection> newModuleConnections = newModule.GetEntranceAndExit();
                Connection connectionToMatch = GetConnection(newModuleConnections);
                MatchConnections(connection, connectionToMatch);

                newModule.GenerateObstacles(this);

                newConnections.AddRange(newModuleConnections.FindAll(c => c != connectionToMatch));
                newModule.transform.parent = generatedMap;
            }

            pendingConnections = newConnections;
        }
    }

    public void SetModulesArray(PathModule[] newModules)
    {
        modules = newModules;
        shuffledModules = new Queue<PathModule>(Randomness.ShuffledArray(modules, prng));
    }

    void ResetShuffledModulesQueue()
    {
        shuffledModules = new Queue<PathModule>(Randomness.ShuffledArray(modules, prng));
    }

    public PlaceableItem GenerateObstacle(Connection connection)
    {
        string newTag = connection.GetRandomConnectTag();
        Module newModulePrefab = GetRandomWithTag(obstacles, newTag);
        return newModulePrefab.Instantiate().GetComponent<PlaceableItem>();
    }

    public PlaceableItem GeneratePickup(Connection connection)
    {
        string newTag = connection.GetRandomConnectTag();
        PlaceableItem newModulePrefab = pickUps[0];
        return newModulePrefab.Instantiate().GetComponent<PlaceableItem>();
    }

    public PathModule GenerateModule(Connection connection)
    {
        //Game flow
        if (Application.isPlaying) GameManager.GetInstance().CheckGameState();

        if (shuffledModules.Count == 0) ResetShuffledModulesQueue();

        PathModule newModulePrefab = shuffledModules.Dequeue();

        return newModulePrefab.Instantiate().GetComponent<PathModule>();
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
            else return connections[prng.Next(0, connections.Count)];
        }
    }

    public void MatchConnections(Connection oldExit, Connection newExit)
    {
        Transform newModule = newExit.transform.root;
        Vector3 forwardVectorToMatch = -oldExit.transform.forward;
        float angleToRotate = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, angleToRotate);
        Vector3 translation = oldExit.transform.position - newExit.transform.position;
        newModule.position += translation;
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

    private void RandomChangeSideEnvVariables()
    {
        natureOnLeft = prdNatLeft.CheckOccurrence(Random.value);
        natureOnRight = prdNatRight.CheckOccurrence(Random.value);
    }

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

}
