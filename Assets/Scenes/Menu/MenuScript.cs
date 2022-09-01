using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript: MonoBehaviour
{
    // establishing variables.
    public Text SnakeBestTime;
    public Text MinesweeperBestTime;

    // public update to constantly (when the scene is running) set each highscore to their player prefs. This has to be an update function because scores are subject to change even on the menu screen.
    public void Update()
    {
        SnakeBestTime.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();

        MinesweeperBestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
    }
}