using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUi;
    public GameObject crosshair;
    private Look playerLook;
    private Sway sway;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Resume();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        crosshair.SetActive(true);
        Time.timeScale = 1f;
        playerLook = GameObject.Find("Player").GetComponent<Look>();
        sway = GameObject.Find("Weapon").GetComponentInChildren<Sway>();
        playerLook.enabled = true;
        if (sway != null) sway.enabled = true;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUi.SetActive(true);
        crosshair.SetActive(false);
        Time.timeScale = 0f;
        playerLook = GameObject.Find("Player").GetComponent<Look>();
        sway = GameObject.Find("Weapon").GetComponentInChildren<Sway>();
        playerLook.enabled = false;
        if (sway != null) sway.enabled = false;
        isPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        FindObjectOfType<GameManager>().LoadMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
