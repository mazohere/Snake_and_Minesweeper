using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    public int flagCount;

    private Board board;
    private Cell[,] state;
    private bool gameOver;
    private float timer;
    public Text flagsRemaining;
    public Text timeDisplay;
    public Text bestTime;
    public Text gameOverBestTime;
    public Text gameWinBestTime;
    public Text gameWinTime;
    public GameObject gameOverVisuals;
    public GameObject pauseVisuals;
    public GameObject gameWinVisuals;
    private bool pause = false;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        timer = 0;
        flagCount = mineCount;
        flagsRemaining.text = flagCount.ToString();
        bestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
        gameOverVisuals.SetActive(false);
        gameWinVisuals.SetActive(false);

        state = new Cell[width, height];
        gameOver = false;

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        Camera.main.transform.position = new Vector3(width / 2f, height /2f, -10f);

        board.Draw(state);
    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;

            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].type = Cell.Type.Mine;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }
                
                cell.number = CountMines(x, y);

                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }
                
                state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                pause = true;
                pauseVisuals.SetActive(true);
            } else if (pause)
            {
                ContinueButton();
            }
        }

        if (!gameOver && !pause)
        {
            timer += Time.deltaTime;

            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);
            timeDisplay.text = (string.Format("{0:00}:{1:00}", minutes, seconds));
            
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            } else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }

        
    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
        flagsRemaining.text = (flagCount - 1).ToString();
        flagCount--;
        
    }

    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        switch (cell.type)
        {
            case Cell.Type.Mine: Explode(cell); break;
            case Cell.Type.Empty: Flood(cell);  CheckWinCondition(); break;
            default: cell.revealed = true; state[cellPosition.x, cellPosition.y] = cell; CheckWinCondition(); break;
        }

        board.Draw(state);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        gameOver = true;

        gameOverVisuals.SetActive(true);
        gameOverBestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
        
        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position. x - 1, cell.position.y));
            Flood(GetCell(cell.position. x + 1, cell.position.y));
            Flood(GetCell(cell.position. x, cell.position.y - 1));
            Flood(GetCell(cell.position. x, cell.position.y + 1));
        }
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y< height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        Debug.Log("Winner!");
        gameOver = true;

        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (PlayerPrefs.GetFloat("MinesweeperBestTimeTechnical") > timer || PlayerPrefs.GetFloat("MinesweeperBestTimeTechnical") == 0f)
        {
            PlayerPrefs.SetFloat("MinesweeperBestTimeTechnical", timer);
            PlayerPrefs.SetString("MinesweeperBestTime", time);
            bestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");
        }

        gameWinVisuals.SetActive(true);
        gameWinTime.text = time;
        gameWinBestTime.text = PlayerPrefs.GetString("MinesweeperBestTime");


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        } else
        {
            return new Cell();
        }
    }

    private bool IsValid(int x, int y)
    {
        return  x >= 0 && x < width && y >= 0 && y < height;
    }

    public void TryAgainButton()
    {
        NewGame();
    }

    public void BackToMenuButton()
    {
        gameOverVisuals.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    public void ContinueButton()
    {
        pause = false;
        pauseVisuals.SetActive(false);
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}