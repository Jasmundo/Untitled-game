using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScreen");
    }
    public void GameWon()
    {
        SceneManager.LoadScene("YouWonScreen");
    }
}
