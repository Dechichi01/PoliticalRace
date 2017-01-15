using UnityEngine;
using Utility;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour {

    [Range(0, 1)]
    public float restProbability = 0.1f;
    public int numberOfRestBlocks = 2;

    PRD restPRD;
    int blocksGeneratedInState = 0;

    public GameState gameState = GameState.WarmUp;

    public ModulesPerState[] modulesPerState;

    public PlaceableItem[] roadSideProps;
    public PlaceableItem[] natureSideProps;

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
        SetGameState(GameState.WarmUp);
        mapGen.iterations = 1;
        mapGen.GenerateMap();

        SetGameState(GameState.Calibrate);
        mapGen.iterations = 3;
        mapGen.GeneratePath();

        SetGameState(GameState.Reward);
        mapGen.iterations = 1;
        mapGen.GeneratePath();
    }

    /*
    * Randonly change to RestBlocks based on a Pseudo Random Distribution
    * change back after the number of rest blocks was instantiated
    */
    public void CheckGameState()
    {
        blocksGeneratedInState++;
        switch(gameState)
        {
            case (GameState.WarmUp):
                break;
            case (GameState.Calibrate):
                break;
            case (GameState.Reward):
                if (blocksGeneratedInState >= 3)
                {
                    SetGameState(GameState.Challenges);
                }
                break;
            case (GameState.Challenges):
                if (restPRD.GetProbability() > Random.value)
                {
                    restPRD.ResetTries();
                    SetGameState(GameState.Rest);
                }
                break;
            case (GameState.Rest):
                if (blocksGeneratedInState >= numberOfRestBlocks)
                {
                    SetGameState(GameState.Challenges);
                }
                break;
        }
    }

    void SetGameState(GameState state)
    {
        gameState = state;
        blocksGeneratedInState = 0;

        for (int i = 0; i < modulesPerState.Length; i++)
        {
            if (modulesPerState[i].state == gameState) mapGen.SetModulesArray(modulesPerState[i].modules);
        }
    }

    public void ChangeWayPointManager()
    {
        player.wayPointsManager = wayPointsManagerQueue.Dequeue();
    }

    public enum GameState
    {
        WarmUp = 0, Calibrate = 1, Reward = 2, Challenges = 3, Rest = 4
    }

    [System.Serializable]
    public struct ModulesPerState
    {
        public GameState state;
        public PathModule[] modules;
    }
}
