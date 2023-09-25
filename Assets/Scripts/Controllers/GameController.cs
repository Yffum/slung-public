using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;
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


    // Google Play in-app reviews
    private static ReviewManager _reviewManager;
    private static PlayReviewInfo _playReviewInfo;


    public static void SaveUserData()
    {
        ES3.Save("UserData", UserData);
    }   
    
    public static void LoadUserData()
    {
        if (ES3.KeyExists("UserData"))
        {
            ES3.LoadInto("UserData", UserData);
        }    
        else
        {
            SaveUserData();
        }
    }    

    /// <summary>
    /// Prepare Google Play for an in-app review request
    /// </summary>
    public static IEnumerator PrepareReviewRequest()
    {
        // From: https://developer.android.com/guide/playcore/in-app-review/unity
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("Error preparing in-app review request");
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
    }

    /// <summary>
    /// Initiate Google Play's in-app review request
    /// </summary>
    public static IEnumerator RequestReview()
    {
        // From: https://developer.android.com/guide/playcore/in-app-review/unity
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("Error launching in-app review");
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow. 
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

        InitializeGooglePlayReviews();

        LoadUserData();

        Gui.UpdateHighScoreGraphic();

        if (UserData.IsMuted)
        {
            GameController.Sound.MuteAll();
        }

        GameController.Sound.PlayEndLevelSound();

        GameController.Gui.ActivatePostSplashCurtain();
    }

    private void InitializeMembers()
    {
        UserData = new UserData();

        Screen = GetComponent<ScreenController>().Init();
        Sound = GetComponent<SoundController>().Init();
        Spawn = GetComponent<SpawnController>().Init();
        Gui = GetComponent<GuiController>().Init();
    }

    private void InitializeGooglePlayReviews()
    {
        _reviewManager = new ReviewManager();
    }
}
