using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void PressOKFromGameOver()
    {
        GameController.Gui.CloseGameOverMenu();

        GameController.Sound.PlayBlipSound();

        if (GameController.UserData.PlayCount > 6)
        {
            UnityEngine.iOS.Device.RequestStoreReview();
        }
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

        GameController.Sound.PlayBlipSound();
    }

    public void PressReadyFromPauseMenu()
    {
        GameController.Gui.ClosePauseMenu();
    }
}
