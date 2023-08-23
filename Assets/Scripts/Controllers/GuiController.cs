using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuiController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerScoreText;

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
        _gameOverMenu.SetActive(true);
    }    

    private void UpdatePlayerScoreGraphic()
    {
        _playerScoreText.text = _playerScore.ToString();
    }
}
