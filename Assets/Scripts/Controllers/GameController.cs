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
    public static LevelController Level { get; private set; }
    public static GuiController Gui { get; private set; }


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
        Level = GetComponent<LevelController>().Init();
        Gui = GetComponent<GuiController>().Init();
    }
}
