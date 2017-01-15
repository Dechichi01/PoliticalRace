using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public enum GameState
    {
        Paused,
        Running,
        Entrance,
        Death
    }

    private static GameController _instance;
    public static GameController instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    private GameState gameState;
    
	// Use this for initialization
	void Start () {
        gameState = GameState.Running;	
	}

    public GameState GetGameState() { return gameState; }
    public void SetGameState(GameState state) { gameState = state; }
	
	// Update is called once per frame
	void Update () {
	
	}
}
