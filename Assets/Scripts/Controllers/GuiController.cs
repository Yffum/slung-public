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

    [SerializeField] private GameObject _gameOverMenu;

    private int _playerScore = 0;

    public GuiController Init()
    {
        UpdatePlayerScoreGraphic();

        return this;
    }

    public void IncrementPlayerScore()
    {
        // increment
        _playerScore++;

        UpdatePlayerScoreGraphic();

        // trigger animation
        _playerScoreText.gameObject.GetComponent<Animator>().SetTrigger("Expand");
    }

    public void OpenGameOverMenu()
    {
        _finalPlayerScoreText.text = _playerScore.ToString();

        _gameOverMenu.SetActive(true);
    }    

    private void UpdatePlayerScoreGraphic()
    {
        _playerScoreText.text = _playerScore.ToString();
    }
}
