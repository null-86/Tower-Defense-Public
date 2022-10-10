using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneToLoad;
    public bool reloadCurrentScene = true;

    public void OnLoadScene() {
        if (reloadCurrentScene) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else {
            if (SceneManager.GetSceneByName(sceneToLoad).IsValid()) {
                SceneManager.LoadScene(sceneToLoad);
            } else {
                Debug.LogError("LoadScene is misconfigured; Unable to Load");
            }
        }
        
    }
}
