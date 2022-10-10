using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl control;
    public static GameState startGameState = GameState.Menu;
    public GameState state = GameState.Menu;
    public event Action<GameState> OnGameStateChanged;

    //public OptionManager options;
    //public DataManager data;
    //public SceneOrganizer organizer;
    //public EnemySpawner spawner;
    //public LevelSettings level;
    //public Castle castle;
    //public UnitPanel unit;
    //public ResourcePanel resource;
    //public Text // waveText;
    //public UIManager uiManager;

    public void Awake() {
        

        control = this;
        Debug.Log("make some way to inform the player why a tower isn't placed: too far from ship, not enough money, etc (only 2 i can think of rn");
        //Debug.LogError("fix unit spawn region; i think the region is ignored since the obj hasn't been created yet. may need to separate the spawnheight logic into its own class");
        Debug.Log("make the ship move up and down in a compound sin/cosin wave so that it looks more alive; maybe add unit avoidance?? might be more trouble than it's worth");
        Debug.Log("add powers (bottom right?). add more towers and units. make a menu - allow tower selection. stop energy when paused. convert the castle to a ship with flappy bird controls. change enemies to asteroids, enemy ships, etc. ignore what hits the wall behind you. make the game move on a treadmill with a continous background. add turrets to ship during downtime. ship trail particles. background treadmill. add a near miss mechanic which rewards money/resources (change to resources since you can mine when close) ");
        // get refs
        // data and options are not monobehaviours, so they are different
        //data = new DataManager();
        //data.Init();

        // retrieve previous options
        // todo
        //if (data.FileExists(options.fileName)) {
        // load saved options into option manager
        //}


        
        
    }

    public void Start() {
        // initialize ui elements - wait until after all awakes
        InitializeUIElements();
        
        // initialize state
        UpdateGameState(startGameState);
        
        
    }
     
    public void OnDestroy() {
        OnGameStateChanged = null;
    }

    public void InitializeUIElements() {
        
        //// wave counter

        // there should only be 1 "Wave Text"
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("Wave Text");
        if (canvases.Length == 1) {
            // waveText = canvases[0].GetComponent<Text>();
        }
        else {
            //Debug.LogError("Invalid # of objects tagged \"Wave Text\"");
            return;
        }

        // begin on wave 1
        // SetwaveText(1);
    }

    public void UpdateGameState(GameState newState) {
        state = newState;
        //Debug.Log("GameState: " + state);

        switch (state) {
            case GameState.Menu:
                break;
            case GameState.Preparation:
                break;
            case GameState.Combat:
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default:
                break;
        }
        
        OnGameStateChanged?.Invoke(state);
    }

    //public void SetwaveText(int wave) {
    //    if (// waveText != null && // waveText.isActiveAndEnabled) {
    //        // waveText.text = "Wave: " + wave;
    //    }
        
    //}

    
}

public enum GameState {
    Menu,
    Preparation,
    Combat,
    Victory,
    Defeat
}
