using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{

    // public functions for loading snake and minesweeper, reseting each of the scores, and quiting the application. They have all been set to their corresponding buttons in the unity engine.
    public void OnSnakeButtonClick()
    {
        SceneManager.LoadScene("Snake");
    }

    public void OnMinesweeperButtonClick()
    {
        SceneManager.LoadScene("Minesweeper");
    }

    public void OnSnakeResetClick()
    {
        PlayerPrefs.SetInt("SnakeHighScore", 0);
    }

    public void OnMinesweeperResetClick()
    {
        PlayerPrefs.SetString("MinesweeperBestTime", "00:00");
        PlayerPrefs.SetFloat("MinesweeperBestTimeTechnical", 0f);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
