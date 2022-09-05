using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript: MonoBehaviour
{
    // establishing variables.
    public Text SnakeBestTime;
    public Text MinesweeperBestTime;

    // public update to constantly (when the scene is running) set each highscore textbox to their player prefs. This has to be an update function because scores are subject to change even on the menu screen.
    // switch statements used as a fast way of checking whether or not the playerprefs for each highscore exist, and setting them to default if they don't. This is to ensure the scores wont be blank for the first time a new player starts the game.
    public void Update()
    {
        switch (PlayerPrefs.HasKey("SnakeHighScore"))
        {
            case true: 
                SnakeBestTime.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();
                break;

            case false: 
                PlayerPrefs.SetInt("SnakeHighScore", 0);
                break;
        }

        switch (PlayerPrefs.HasKey("MinesweeperBestTime"))
        {
            case true: 
                MinesweeperBestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
                break;

            case false: 
                PlayerPrefs.SetString("MinesweeperBestTime", "00:00");
                break;
        }
    }
}