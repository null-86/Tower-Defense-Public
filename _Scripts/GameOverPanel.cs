using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public float height = 7;
    public float width = 10;
    public TextMeshPro resultsText;
    public Canvas parentCanvas;
    

    public void Start()
    {

        if (resultsText == null) {
            resultsText = GameObject.FindGameObjectWithTag("Results Text").GetComponent<TextMeshPro>();
        }

        GameControl.control.OnGameStateChanged += OnGameStateChanged;

        // default off - handled by initial ongamestatechanged call
        //HideMenu();
    }

    public void OnDestroy() {
        GameControl.control.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state) {
        
        if (state == GameState.Victory) {
            Debug.Log("maybe fix this. uimanager may handle this");
            //parentCanvas.enabled = true;
            if (resultsText != null) {
                resultsText.text = "Victory";
            }
            
        }
        else if (state == GameState.Defeat) {
            Debug.Log("maybe fix this. uimanager may handle this");
            //parentCanvas.enabled = true;
            if (resultsText != null) {
                resultsText.text = "Defeat";
            }
        } else {
            //HideMenu();
        }
    }

    //private void HideMenu() {
    //    parentCanvas.enabled = false;
    //}
}
