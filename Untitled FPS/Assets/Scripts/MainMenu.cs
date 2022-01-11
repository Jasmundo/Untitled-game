using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject instructions;
    public GameObject menu;
    void Start()
	{
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
	private void Update()
	{

    }
	public void StartGame() 
    {
        FindObjectOfType<GameManager>().StartGame();
    }
    public void LoadMainMenu()
    {
        FindObjectOfType<GameManager>().LoadMainMenu();
    }
    public void Instructions()
    {
        instructions.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void Back()
    {
        menu.SetActive(true);
        this.gameObject.SetActive(false);    
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
