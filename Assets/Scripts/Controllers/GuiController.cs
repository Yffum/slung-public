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

    /// <summary>
    /// The canvas group for "Pull &" and the down arrow in the start menu
    /// </summary>
    [SerializeField] private CanvasGroup _pullInstructions;

    [SerializeField] private GameObject _gameOverMenu;

    private int _playerScore = 0;

    public GuiController Init()
    {
        UpdatePlayerScoreGraphic();

        return this;
    }

    /// <summary>
    /// Give player a point, update _playerScoreText, and trigger animation 
    /// to expand _playerScoreText as feedback.
    /// </summary>
    public void IncrementPlayerScore()
    {
        _playerScore++;

        UpdatePlayerScoreGraphic();

        // trigger animation
        _playerScoreText.gameObject.GetComponent<Animator>().SetTrigger("Expand");
    }

    /// <summary>
    /// Enables _playerScoreText which automatically triggers fade in animation
    /// </summary>
    public void EnablePlayerScore()
    {
        _playerScoreText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Trigger animation which fades out and then disables
    /// </summary>
    public void DisablePlayerScore()
    {
        // trigger animation with event which disables self
        _playerScoreText.gameObject.GetComponent<Animator>().SetTrigger("Disable");
    }

    /// <summary>
    /// This event should only be called by the animation event after playing the
    /// disable animation. To active this animation, use DisablePlayerScore()
    /// </summary>
    public void EventDisablePlayerScore()
    {
        _playerScoreText.gameObject.SetActive(false);
    }

    //Task: Add check for new highscore==========///////////////////////////////////////
    /// <summary>
    /// Activate game over menu fade in animation and update final score text
    /// </summary>
    public void OpenGameOverMenu()
    {
        _finalPlayerScoreText.text = _playerScore.ToString();

        _gameOverMenu.SetActive(true);
    }

    private void Update()
    {
        AdjustPullInstructionsTransparency();
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

    /// <summary>
    /// Updates the _playerScoreText graphic to correspond to the value of _playerScore
    /// </summary>
    private void UpdatePlayerScoreGraphic()
    {
        _playerScoreText.text = _playerScore.ToString();
    }
}
