using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameStateButton : MonoBehaviour
{
    public GameState targetState;
    public void ChangeGameState() {
        GameControl.control.UpdateGameState(targetState);
    }
}
