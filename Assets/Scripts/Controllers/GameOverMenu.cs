using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : GenericObject
{
    /// <summary>
    /// This method is called by an animation event after the game over menu has sufficiently closed.
    /// Make the start menu interactable.
    /// </summary>
    public void EnableStartMenu()
    {
        GameController.Gui.OpenStartMenu();
    }
}
