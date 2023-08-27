using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void PressOKFromGameOver()
    {
        GameController.Gui.CloseGameOverMenu();

        GameController.Sound.PlayBlipSound();
    }

    public void PressMute()
    {
        GameController.Sound.MuteAll();
    }   
    
    public void PressUnmute()
    {
        GameController.Sound.UnmuteAll();
    }

    public void PressPause()
    {
        GameController.Game.Level.PauseLevel();
    }

    public void PressReadyFromPauseMenu()
    {
        GameController.Gui.ClosePauseMenu();
    }
}
