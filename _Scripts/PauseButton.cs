using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private void OnMouseUpAsButton() {
        EnemySpawner.spawner.PauseWaves();
        UIManager.uiManager.OnPause();

    }
}
