using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Mathematics;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel, mainMenuPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("I'm outta here !");
        Application.Quit();
    }

}

