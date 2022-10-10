using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    private void OnMouseUpAsButton() {

        EnemySpawner.spawner.StartWaves();
        UIManager.uiManager.OnPlay();

        // unique to play button
        GameControl.control.UpdateGameState(GameState.Combat);
    }
}
