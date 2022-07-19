using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript: MonoBehaviour
{
    public Text SnakeBestTime;
    public Text MinesweeperBestTime;

    void Update()
    {
        SnakeBestTime.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();

        MinesweeperBestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
    }
}