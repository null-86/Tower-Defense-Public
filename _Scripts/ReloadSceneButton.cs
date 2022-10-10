using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour {

    public GameState state;

    public void ReloadScene() {

        // set static gamestate (will persist across reload)

        GameControl.startGameState = state;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
   
    }
}
