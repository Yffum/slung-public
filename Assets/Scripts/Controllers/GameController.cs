using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Game { get; private set; }

    //------
    public static UserData UserData { get; private set; }
    //------
    public static ScreenController Screen { get; private set; }
    public static SoundController Sound { get; private set; }
    public static SpawnController Spawn { get; private set; }
    public static GuiController Gui { get; private set; }

    /*Serialize*/ public LevelController Level;

    public static void SaveUserData()
    {
        Debug.LogWarning("Saving is not implemented. Data not saved.");
        //ES3.Save("UserData", UserData);
    }   
    
    public static void LoadUserData()
    {
        Debug.LogWarning("Saving is not implemented. No data loaded.");
        /*
        if (ES3.KeyExists("UserData"))
        {
            ES3.LoadInto("UserData", UserData);
        }    
        else
        {
            SaveUserData();
        }
        */
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

        LoadUserData();

        Gui.UpdateHighScoreGraphic();

        if (UserData.IsMuted)
        {
            GameController.Sound.MuteAll();
        }

        GameController.Sound.PlayEndLevelSound();

        GameController.Gui.ActivatePostSplashCurtain();

        Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().SetTouchInputBounds();
    }

    private void InitializeMembers()
    {
        UserData = new UserData();

        Screen = GetComponent<ScreenController>().Init();
        Sound = GetComponent<SoundController>().Init();
        Spawn = GetComponent<SpawnController>().Init();
        Gui = GetComponent<GuiController>().Init();
    }
}
