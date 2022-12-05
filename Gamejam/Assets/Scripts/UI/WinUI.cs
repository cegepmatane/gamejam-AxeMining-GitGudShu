using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    public GameObject winMenu;

    private void OnEnable()
    {
        Player.onWin += EnableWinMenu;
    }

    private void OnDisable()
    {
        Player.onWin -= EnableWinMenu;
    }
    public void EnableWinMenu()
    {
        winMenu.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Player.time = 0;
        SceneManager.LoadScene(0);
    }
}
