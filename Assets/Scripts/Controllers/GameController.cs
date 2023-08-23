using System.Collections;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Game { get; private set; }
    //------
    public static ScreenController Screen { get; private set; }
    public static SpawnController Spawn { get; private set; }
    public static GuiController Gui { get; private set; }

    /*Serialize*/ public LevelController Level;

    public void StartLevel()
    {
        Level.gameObject.SetActive(true);
    }    

    public void StopLevel()
    {
        Level.gameObject.GetComponent<LevelController>().CleanUpLevel();
    }

    private void Awake()
    {
        // make GameController singleton
        if (Game == null)
        {
            Game = this;
        }
        else if (Game != this)
        {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject);

        InitializeMembers();
    }

    private void InitializeMembers()
    {
        Screen = GetComponent<ScreenController>().Init();
        Spawn = GetComponent<SpawnController>().Init();
        Gui = GetComponent<GuiController>().Init();
    }
}
