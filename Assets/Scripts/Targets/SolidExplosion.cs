using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidExplosion : MonoBehaviour
{
    /// <summary>
    /// Note: This method is called by an animation event. Unfreeze the game and open game over menu
    /// </summary>
    public void OpenGameOverMenu()
    {
        // open game over menu
        GameController.Gui.OpenGameOverMenu();
        GameController.Game.Level.CleanUpLevel();
    }

}
