using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidExplosion : MonoBehaviour
{
    /// <summary>
    /// This method is called by an animation event and opens the game over menu
    /// </summary>
    public void OpenGameOverMenu()
    {
        this.gameObject.SetActive(false);

        GameController.Gui.OpenGameOverMenu();
    }
}
