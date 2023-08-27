using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : GenericObject
{
    public void UnpauseLevelAndDisableSelf()
    {
        GameController.Game.Level.UnpauseLevel();
        DisableSelf();
    }
}
