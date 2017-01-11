using UnityEngine;
using System.Collections;
using Utility;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    //Assigned in the inspector
    public PathModule startModule;
    public Module[] obstacles;
    public int iterations = 5;
    public int seed;

    System.Random prng;
    //Controlled by GameManager
    PathModule[] modules;
    Queue<PathModule> shuffledModules;
    public bool natureOnLeft = false, natureOnRight = false;

    List<PathModule> instantiedModules = new List<PathModule>();//used to get the last module and delete previous when in Game
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
        modules = FindObjectOfType<GameManager>().modulesWarmUp;
        ResetShuffledModulesQueue();

        //Create map holder and first module
        string holderName = "Generated Map";
        if (transform.FindChild(holderName) != null)
            DestroyImmediate(transform.FindChild(holderName).gameObject);

        generatedMap = new GameObject(holderName).transform;
        generatedMap.parent = transform;

        PathModule startingModule = startModule.Instantiate(transform.position, transform.rotation).GetComponent<PathModule>();
        startingModule.transform.parent = generatedMap;

        instantiedModules.Clear();
        instantiedModules.Add(startingModule);
        moduleVerifier = startingModule;
        startingModule.GenerateObstacles(this);
        GeneratePath();
    }

    public void GeneratePath(bool setup = false)
    {
        //Destroy past modules
        List<Module> modulesToDeactivate = new List<Module>();  
        if (!setup)
        {
            while (instantiedModules.Count > 4)
            {
                modulesToDeactivate.AddRange(instantiedModules[0].GetComponentsInChildren<Module>());
                instantiedModules.RemoveAt(0);
            }
        }

        for (int i = 0; i < modulesToDeactivate.Count; i++)
            modulesToDeactivate[i].Destroy();
        //

        moduleVerifier.playerEnterVerifier.SetActive(false);

        List<Connection> pendingConnections = instantiedModules[instantiedModules.Count - 1].GetEntranceAndExit();
        pendingConnections.RemoveAll(c => (c is Entrance));//Don't connect to the entrance of the first module

        for (int i = 0; i < iterations; i++)
        {
            List<Connection> newConnections = new List<Connection>();

            foreach (Connection connection in pendingConnections)
            {
                //Generate a module and it's obstacles
                PathModule newModule = GenerateModule(connection);
                if (i < 0.6 * iterations) moduleVerifier = newModule;
                instantiedModules.Add(newModule);

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

        moduleVerifier.playerEnterVerifier.SetActive(true);
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

    public Module GenerateObstacle(Connection connection)
    {
        string newTag = connection.GetRandomConnectTag();
        Module newModulePrefab = GetRandomWithTag(obstacles, newTag);
        return newModulePrefab.Instantiate().GetComponent<Module>();
    }

    public PathModule GenerateModule(Connection connection)
    {
        if (Application.isPlaying) GameManager.GetInstance().CheckForRestBlock();

        if (shuffledModules.Count == 0) ResetShuffledModulesQueue();
        PathModule newModulePrefab = shuffledModules.Dequeue();
        newModulePrefab.natureOnLeft = natureOnLeft;
        newModulePrefab.natureOnRight = natureOnRight;
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

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

}
