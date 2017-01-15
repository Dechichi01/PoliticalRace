using UnityEngine;
using Utility;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour {

    public PathModule[] modulesWarmUp;
    public PathModule[] modulesCalibrate;
    public PathModule[] modulesReward;
    public PathModule[] modulesChallenges;
    public PathModule[] modulesRest;

    [Range(0, 1)]
    public float restProbability = 0.1f;
    public int numberOfRestBlocks = 2;

    PRD restPRD;
    int restBlocksCounter = 0;

    public GameState gameState = GameState.Setup;

    Character player;
    MapGenerator mapGen;
    [HideInInspector]
    public Queue<WayPointsManager> wayPointsManagerQueue = new Queue<WayPointsManager>();

    //Singleton
    public static GameManager instance;
    public static GameManager GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
        restPRD = new PRD(restProbability);
        player = FindObjectOfType<Character>();
    }

    private void Start()
    {
        mapGen = MapGenerator.GetInstance();
        GenerateStartingPath();
        player.wayPointsManager = wayPointsManagerQueue.Dequeue();
    }

    void GenerateStartingPath()
    {
        mapGen.iterations = 1;
        mapGen.SetModulesArray(modulesWarmUp);

        mapGen.GenerateMap();

        mapGen.iterations = 3;
        mapGen.SetModulesArray(modulesCalibrate);

        mapGen.GeneratePath(true);

        mapGen.SetModulesArray(modulesReward);
        mapGen.GeneratePath(true);

        mapGen.iterations = 5;
        mapGen.SetModulesArray(modulesChallenges);
        mapGen.GeneratePath(true);

        gameState = GameState.Challenges;
    }

    /*
    * Randonly change to RestBlocks based on a Pseudo Random Distribution
    * change back after the number of rest blocks was instantiated
    */
    public void CheckForRestBlock()
    {
        if (gameState == GameState.Rest)
        {
            restBlocksCounter++;
            if (restBlocksCounter >= numberOfRestBlocks)
            {
                restBlocksCounter = 0;
                gameState = GameState.Challenges;
                mapGen.SetModulesArray(modulesChallenges);
            }
        }
        else if (gameState == GameState.Challenges && restPRD.GetProbability() > Random.value)
        {
            restPRD.ResetTries();
            mapGen.SetModulesArray(modulesRest);
            gameState = GameState.Rest;
        }
    }

    public void ChangeWayPointManager()
    {
        player.wayPointsManager = wayPointsManagerQueue.Dequeue();
    }

    public enum GameState
    {
        Setup, Challenges, Rest
    }
}
