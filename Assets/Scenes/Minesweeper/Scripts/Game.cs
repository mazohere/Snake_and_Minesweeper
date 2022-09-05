using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    // establishing both public and private variables for use both in this script, other scripts, and the engine itself with gameobjects such as textboxes and sprites.
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
    private bool timer_increase = false;

    // these three functions all run at basically the same time with very slight variations for the sake of timing and validating engine specific values. (OnValidate runs at the start or when a value in the engine changes; Awake runs before the script runs; Start runs when the script runs).
    // on startup: make sure the amount of mines doesn't excede the amount of tiles on the board, which would result in a arithmetic error; sets the board variable to the component type of the board gameobject for readability reasons; runs the NewGame function.
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

    // private function for starting a new game.
    // first sets relevent variables to a new game state.
    // then calls the GenerateCells, Mines, Numbers, and Draw functions to create the board.
    private void NewGame()
    {
        timer = 0;
        timer_increase = false;
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        timeDisplay.text = (string.Format("{0:00}:{1:00}", minutes, seconds));
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

    // private function for generating empty cells.
    // nested for loops which cycle through every possible cell position on the board and create an empty cell in that position, some of these will be changed later but the order of generation had to go empty -> mines -> numbers because each relies on the one before it to generate anything.
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

    // private function for generating mines on a set amount of empty cells.
    // first a for loop to cycle through the set amount of mines which in this case is 32.
    // the function then chooses a random x and y co-ordinant between the minimum and maximum bounds of the game.
    // then there is some logic to make sure the mine doesn't generate on an already placed mine and that it doesn't move out of bounds if it has generated on an already placed mine.
    // if the while loop determines the mine is in a valid spot, the "state" list will set those co-ordinants type to "Mine".
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

    // private function for generating numbers on empty cells.
    // nested for loops which cycle through every cell position on the board.
    // if the cell is already a mine the for loop should continue with its next itteration.
    // otherwise, set the cell's number value (established in the struct in the public "cells" script) to the return value of the "CountMines" function at the current parameters.
    // if the number of mines around the cell is more than 0, set the cell's type to "Number".
    // then set the current cell to the "state" list's current x and y co-ordinants.
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

    // private function for counting the mines around a cell
    // some logic to record the number value every cell around the cell being investigated without recording the cell itself, using nested for loops with x and y co-ordinants limited to one space adjacent to the current cell.
    // set the x and y values to the current cells x and y values plus the adjacent values which will either be a positive or negative 1 (or a 0).
    // use the x and y values as co-ordinants in the "GetCell" function to find the cell, then check if that cell is a "Mine" type. If it is, add one to the count variable.
    // the "CountMines" function then returns the current mine count for that cell.
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

    // private update function, called every frame the script is running. Used to look for button presses and to check if the game is over or has been paused.
    // constant if statement looking for key presses. If "R" is pressed the "NewGame" function should run. If "Escape" is pressed and the pause and gameOver booleans are not active, pause, along with the pause gameobjects, become active. If "Escape" is pressed while the pause boolean is active, the Continue function runs, meaning the pause boolean and it's gameobjects become inactive again.
    // another if statement checking if the "gameOver" and "pause" booleans. If both are false the timer, using a "deltaTime" variable built into the engine, continues to tick and gets converted into a readable time for the textbox display using the "FloorToInt" and "Format" C# functions. The script also continues to look for mouse buttons, with mouse button 0 (left click), triggering a reveal and the timer_increase boolean value to be set to true, marking the moment the player reveals a cell as the moment the timer will start to increase, and mouse button 1 (right click), triggering a flag.
    // if either the "gameOver" or "pause" booleans become true, the timer and mouse button detection are disabled until they become false again.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameOver && !pause)
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
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            } else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
                timer_increase = true;
            }

            if (timer_increase)
            {
                timer += Time.deltaTime;

                float minutes = Mathf.FloorToInt(timer / 60);
                float seconds = Mathf.FloorToInt(timer % 60);
                timeDisplay.text = (string.Format("{0:00}:{1:00}", minutes, seconds));
            }
        }
    }

    // private function for when the player flags a cell.
    // first use some unity engine features to detect where on the screen the mouse is, then set the "cell" variable to the cell being hovered over by the mouse.
    // if the cell type is invalid (out of bounds) or already revealed, do nothing.
    // otherwise set the flagged boolean in "cell" to the opposite of itself to facilitate unflagging, set the current co-ordinantes of the "state" list to the new cell, and re-run the Draw function to redraw the board with the current cells value changed.
    // also set the flagsRemaining textbox to the current flag count minus 1 and subtract the inherant flagCount variable by one, or do the opposite if a flagged cell is changed into a non-flagged cell.
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
        if (cell.flagged)
        {
            flagsRemaining.text = (flagCount - 1).ToString();
            flagCount--;
        }
        else if (!cell.flagged)
        {
            flagsRemaining.text = (flagCount + 1).ToString();
            flagCount++;
        }
    }

    // private function for revealing cells.
    // same thing as before with the unity features to find the mouses location and set the current cell to that cell.
    // if the current cell type is either invalid (out of bounds), revealed, or flagged, do nothing.
    // otherwise, a switch statement begins. If a Mine has been revealed, run the "Explode" function. If an "Empty" has been revealed, run the "Flood" function and check if the game has been won with the "CheckWinCondition" function. If it is neither of those things it would have to be a "Number", so the "revealed" boolean for the current cell is set to true, the "state" list is given the cells co-ordinants, and the win condition is checked.
    // assuming no game end conditions have been met, the board is then re-drawn with the cell's current type now being displayed.
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

    // private function for the board to explode if the player reveals a Mine.
    // first set values to a game over state, with the gameOver boolean being set to true, the gameOverVisuals being set to active, and the gameOverBestTime textbox being set to the current playerprefs for the best game time, meaning the current gametime wont beat the highscore even if it was less time.
    // then set the current cell's revealed and exploded boolean tags to true.
    // then a nested for loop which cycles through all cells, checks if they're mines, and if they are sets them to revealed.
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

    // private semi-recursive function for flooding all empty cells adjacent to an empty cell the player revealed, and any empty cells adjacent to those.
    // if the cell is revealed or is type Mine or Invalid, do nothing (this becomes important on subsequent itterations of the function).
    // if it isn't it must be either Empty or Number, in which case they are revealed and their position is catalogued in the "state" list at the specific co-ordinants.
    // if the cell has been flagged at this stage it has been flagged incorrectly and the function is about to reveal a non-bomb cell, meaning the flag count should go back up.
    // if the cell is Empty, the function repeats on every cell directly adjacent to the current cell. (diagonal cells are not flooded only because they aren't in the original game for balancing reasons).
    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.flagged)
        {
            flagsRemaining.text = (flagCount + 1).ToString();
            flagCount++;
        }

        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position. x - 1, cell.position.y));
            Flood(GetCell(cell.position. x + 1, cell.position.y));
            Flood(GetCell(cell.position. x, cell.position.y - 1));
            Flood(GetCell(cell.position. x, cell.position.y + 1));
        }
    }

    // private function to check if the game is won.
    // cycles through the gameboard with nested for loops, if there are any cells which are not type "Mine" and have not been revealed yet, do nothing.
    // otherwise, the game is won. gameOver boolean is set to true (there really only needs to be one gameover boolean regardless of if its a win or a lose), gameWinVisuals are set to active, and if the current time is better (less) than the recorded best time, the best recorded time is set to the current time. (It looks a lot more complicated than just that but thats only because I had to mess around with float and string converters to get the playerprefs to recognise the data type).
    // it then cycles through all cells again, flagging the unflagged mines.
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y < height; y++)
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

    // private function for getting a cell at specific x and y co-ordinants on the board.
    // if the "IsValid" function determines the co-ordinants are indeed valid, GetCell returns the "state" list at the current co-ordinants.
    // otherwise, the cell is not valid and a new cell is returned.
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

    // private function for determining the validity of the x and y co-ordinants in its parameters, because it is a boolean function, it only returns "true" or "false".
    // small amount of logic to check whether or not the selected cell is in a valid place.
    // if it is the function returns "true" if not it returns "false".
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // four public scripts which run whenever the button they are tied to is pressed (which is the reason these buttons still work even in a gameover state where mouse button detection has been disabled).
    // the try again button runs the NewGame function.
    // the back to menu button sets the gameovervisuals and pausevisuals gameobject's active value to false and loads the "Menu" scene.
    // the continue button sets the "pause" boolean and the pausevisuals gameobject's active value to false.
    // the quit game button quits the application.
    public void TryAgainButton()
    {
        NewGame();
    }

    public void BackToMenuButton()
    {
        pauseVisuals.SetActive(false);
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