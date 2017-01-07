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

    private GameState gameState;
    
	// Use this for initialization
	void Start () {
        gameState = GameState.Entrance;	
	}

    public GameState GetGameState() { return gameState; }
    public void SetGameState(GameState state) { gameState = state; }
	
	// Update is called once per frame
	void Update () {
	
	}
}
