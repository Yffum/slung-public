using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuiController : MonoBehaviour
{
    /// <summary>
    /// The player's current score, displayed in level
    /// </summary>
    [SerializeField] private TextMeshProUGUI _playerScoreText;

    /// <summary>
    /// The score displayed in the _gameOverMenu
    /// </summary>
    [SerializeField] private TextMeshProUGUI _finalPlayerScoreText;

    [SerializeField] private TextMeshProUGUI _playerHighScoreText;

    /// <summary>
    /// The canvas group for "Pull &" and the down arrow in the start menu
    /// </summary>
    [SerializeField] private CanvasGroup _pullInstructions;

    [SerializeField] private GameObject _postSplashCurtain;


    [SerializeField] private GameObject _startUpperMenu;
    [SerializeField] private GameObject _startCenterMenu;
    [SerializeField] private GameObject _startLowerMenu;

    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _highScoreNotification;
    [SerializeField] private GameObject _finalScoreSubtitleText;

    [SerializeField] private GameObject _levelHUD;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseButton;

    public GuiController Init()
    {
        UpdatePlayerScoreGraphic();

        return this;
    }

    public void ActivatePostSplashCurtain()
    {
        _postSplashCurtain.SetActive(true);
    }

    /// <summary>
    /// Give player a point, update _playerScoreText, and trigger animation 
    /// to expand _playerScoreText as feedback.
    /// </summary>
    public void IncrementScoreGraphic()
    {

        UpdatePlayerScoreGraphic();

        // trigger animation
        _playerScoreText.gameObject.GetComponent<Animator>().SetTrigger("Expand");
    }

    /*
    /// <summary>
    /// This event should only be called by the animation event after playing the
    /// disable animation. To active this animation, use DisablePlayerScore()
    /// </summary>
    public void EventDisablePlayerScore()
    {
        _playerScoreText.gameObject.SetActive(false);
    }
    */

    /// <summary>
    /// This method is called by the GameOverMenu object during its animation, so that there is a delay (dictated by GamOverMenu's
    /// animation) before the user can intercat with the start menu
    /// </summary>
    public void OpenStartMenu()
    {
        // allow user to "pull & release"
        GameController.Game.Level.EnableUserInput();
    }

    public void CloseStartMenu()
    {
        _startUpperMenu.GetComponent<Animator>().SetTrigger("Disable");
        _startCenterMenu.GetComponent<Animator>().SetTrigger("Disable");
        _startLowerMenu.GetComponent<Animator>().SetTrigger("Disable");
    }

    /// <summary>
    /// Activate game over menu fade in animation and update final score text
    /// </summary>
    public void OpenGameOverMenu()
    {
        int score = GameController.Game.Level.PlayerScore;

        // conform final score text
        _finalPlayerScoreText.text = score.ToString();

        // if high score, update graphic and save data
        if (score > GameController.UserData.HighScore)
        {
            GameController.UserData.HighScore = score;
            _playerHighScoreText.text = score.ToString();

            // activate crown image to notify player
            _highScoreNotification.SetActive(true);
            _finalScoreSubtitleText.SetActive(false);
        }
        else
        {
            // deactivate crown if not high score
            _highScoreNotification.SetActive(false);
            _finalScoreSubtitleText.SetActive(true);
        }

        GameController.UserData.PlayCount++;

        GameController.SaveUserData();

        // open menu
        _gameOverMenu.SetActive(true);

        _startCenterMenu.gameObject.SetActive(false);
        _startLowerMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate _gameOverMenu's close animation which contains an event that indirectly triggers OpenStartMenu
    /// </summary>
    public void CloseGameOverMenu()
    {
        _gameOverMenu.GetComponent<Animator>().SetTrigger("Close");

        // disable SolidExplosion (which is acting as the _gameOverMenu background) because the animation uses its own background
        GameController.Game.Level.SolidExplosion.SetActive(false);

        // ready start menu
        _startUpperMenu.SetActive(true);
        _startUpperMenu.GetComponent<Animator>().SetTrigger("Enable");
        _startCenterMenu.SetActive(true);
        _startCenterMenu.GetComponent<Animator>().SetTrigger("Enable");
        _startLowerMenu.SetActive(true);
        _startLowerMenu.GetComponent<Animator>().SetTrigger("Enable");
    }

    public void EnableLevelHUD()
    {
        _levelHUD.SetActive(true);
        UpdatePlayerScoreGraphic();
    }

    public void DisableLevelHUD()
    {
        _levelHUD.GetComponent<Animator>().SetTrigger("Disable");
    }

    public void EnablePauseButton()
    {
        _pauseButton.SetActive(true);
    }

    public void DisablePauseButton()
    {
        _pauseButton.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        _pauseMenu.SetActive(true);

        DisablePauseButton();
    }

    public void ClosePauseMenu()
    {
        GameController.Sound.PlayBlipSound();

        _pauseMenu.GetComponent<Animator>().SetTrigger("Disable");
    }

    /// <summary>
    /// Updates the _playerScoreText graphic to correspond to the value of _playerScore
    /// </summary>
    public void UpdatePlayerScoreGraphic()
    {
        _playerScoreText.text = GameController.Game.Level.PlayerScore.ToString();
    }

    public void UpdateHighScoreGraphic()
    {
        _playerHighScoreText.text = GameController.UserData.HighScore.ToString();
    }

    private void Update()
    {
        // if level isn't running, adjust _pullInstructions transparency with pouch displacement
        if (!GameController.Game.Level.IsRunning)
        {
            AdjustPullInstructionsTransparency();
        }
    }

    /// <summary>
    /// (Called every Update) Adjust the transparency of _pullInstructions based on the distance
    /// of the slingshot pouch to its resting spot
    /// </summary>
    private void AdjustPullInstructionsTransparency()
    {
        // get pouch dispalcement from slingshot handler (accessed through level controller)
        float pouchDisplacement = GameController.Game.Level.SlingshotInputHandler.GetComponent<SlingshotInputHandler>().GetPouchDisplacement();
        float alphaDifference = pouchDisplacement / 60f;
        if (alphaDifference > 1)
        {
            alphaDifference = 1;
        }
        _pullInstructions.alpha = 1 - alphaDifference;
    }
}
