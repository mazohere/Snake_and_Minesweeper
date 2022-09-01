using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    // establishing variables for use both in the code and in the graphical unity engine for things like textboxes ect.
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public int initialSize = 4;
    private string notAllowed = " ";
    private string directionName = " ";
    public int highScore = 0;
    public int score = 0;
    public Text highScoreText;
    public Text scoreText;
    public Text gameOverHighScoreText;
    public GameObject gameOverVisuals;
    public GameObject pauseVisuals;
    private bool gameOver;
    private bool escapeKeyDisable = false;
    private bool move = false;

    // (start functions in unity are hardcoded to run once on startup)
    // start function which calls the "resetState" function, useful to minimise re-writting code as the "resetState" function is also called when the "Try Again" button is pressed.The function has to be a private function because "Start" functions unrelated to this exist elsewhere in the code.
    private void Start()
    {
        ResetState();
    }

    // (Update functions in unity are hardcoded to run continiously every frame until the scene ends)
    // Update function which checks every frame for a key press for either up, down, left, right, (we can think of these as the directional keys) or escape (escape condition explained later). When one of the directional keys is pressed, and the direction is not whatever the notAllowed variable is, the direction variable the snake object uses is changed to the Vector2 equivilant and the notAllowed variable is changed to the opposite of the directional key, this is to prevent the player from changing the snakes direction to the opposite of its current direction - i.e the snake is going right and the player changes it to left - which would cause an instant and unfair falestate as the snake would collide with itself. The move boolean is also activated after any directional key is presssed, which enables the snake to move and gets set to false during a reset state, this is to prevent the snake from moving as soon as the game begins.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            move = true;
            directionName = "up";
            if (directionName != notAllowed)
            {
                _direction = Vector2.up;
                notAllowed = "down";
            }
            
                
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            move = true;
            directionName = "down";
            if (directionName != notAllowed)
            {
                _direction = Vector2.down;
                notAllowed = "up";
            }
            
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move = true;
            directionName = "left";
            if (directionName != notAllowed)
            {
                _direction = Vector2.left;
                notAllowed = "right";
            }

        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move = true;
            directionName = "right";
            if (directionName != notAllowed)
            {
                _direction = Vector2.right;
                notAllowed = "left";
            }
            
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // The escape key in these games is used as a pause button, meaning that we first check if pausing is allowed using the "escapeKeyDisable" boolean (if the player dies a gameover screen which will be explained later pops up, when this happens the pause button should not be functional because there is nothing to pause), if it is we check separately if the "gameOver" boolean - which stops the timer and the snake moving - is false, this has to be a seperate check because the pause button sets "gameOver" to true while activated so the snake and timer don't continue. If "gameOver" is false the game object "pauseVisuals" gets set to active, this includes a screen block, and the buttons, "continue", "back to menu", and "quit game", which do what you would expect and will be explained later. If the escape key is pressed again gameOver will be true, meaning it will run the "ContinueButton" function which sets "gameOver" to false again and continues the game, meaning if the escape key is pressed while already paused, it will unpause the game.
            if (!escapeKeyDisable)
            {
                if (!gameOver)
                {
                    gameOver = true;
                    pauseVisuals.SetActive(true);
                } else if (gameOver)
                {
                    ContinueButton();
                }
            }
        }
    }

    // (FixedUpdate functions in unity are functions which continiously run with a set amount of ticks (set in the unity editor itself) instead of once ever frame like the normal Update function)
    // first check to see if the gameOver boolean is false as if it were true the snake should not move, then adjust each segment to move to the space the following segment is currently occupying, creating a domino effect of continuous movement. Then it transforms the top segment to the newest x and y directions, typically these will be the same as what they were the previous itteration but either way the top segment's direction will change to match them.
    private void FixedUpdate()
    {

        if (!gameOver && move)
        {
            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
            }

            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + _direction.x,
                Mathf.Round(this.transform.position.y) + _direction.y,
                0.0f
            );
        }
    }

    // Private grow function which calls on the segmentPrefab (the array - called Lists in unity - holding all segments of the snake), and adds a new segment positioned after the last segment in the list.
    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 2].position;

        _segments.Add(segment);
    }

    // Private reset state function which resets every aspect of the game besides the global highscore.
    // First set all "gameOver" variables to false with move also being set to false to prevent the snake from moving instantly and notAllowed being set to blank to prevent its value from the previous round staying.
    // Then destroy both the game objects in the "_segments" list (which is to say the entire snake), then clear the list of now empty indexs. This is only necessary for successive itterations as the first time the game is played the list will already be blank
    // The initial snake segment which this script is running on is then added to the "_segments" list and set to the center of the scene, with the amount of snake segments needed to match the "initialSize" int value (set in the engine itself) minus 1 for the already present top segment then being added to the list.
    private void ResetState()
    {
        gameOver = false;
        gameOverVisuals.SetActive(false);
        escapeKeyDisable = false;
        move = false;
        notAllowed = " ";
        SnakeScoreCalculate();

        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        _segments.Clear();
        _segments.Add(this.transform);

        this.transform.position = Vector3.zero;

        for (int i = 1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
        }
    }

    // private collision detection function running whenever the snake colides with another gameobject.
    // first check to see if move boolean is true as if it was false the snake would be in its reset state meaning it would be constantly colliding with itself thereby triggering a gameover.
    // if the snake collides with any gameobject tagged as "Food" (these exist in another script), the grow function is called, the score int value is incremented by one, and the new score value is displayed on the scoreText textbox.
    // if the snake collides with any gameobject tagged as "Obstacle" (the walls and the other snake segments), the gameOver function is called.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (move)
        {
            if (other.tag == "Food")
            {
                Grow();
                score++;
                scoreText.text = score.ToString();
            } 
            else if (other.tag == "Obstacle")
            {
                GameOver();
            }
        }
    }

    // private game over function
    // sets the gameOver boolean to true to stop the timer and snake from moving
    // sets the gameOverVisuals (a screenblock and buttons for "try again", "back to menu", and "quit game") to active, meaning they are visable and interactive.
    // sets the escapeKeyDisable boolean to true to prevent pausing while the gameOverVisuals are already active
    // calls the SnakeScoreCalculate function as a more efficient way of calculating the score as the function used everytime score calculation is required.
    private void GameOver()
    {
        gameOver = true;
        gameOverVisuals.SetActive(true);
        escapeKeyDisable = true;

        SnakeScoreCalculate();
    }

    // public try again function, called when the "try again" button is pressed. The function is public because it's tied to a gameobject outside this script.
    // sets the gameOverVisuals' active value to false then runs the ResetState function.
    public void TryAgainButton()
    {
        gameOverVisuals.SetActive(false);
        ResetState();
    }

    // Public back to menu function, called when the "back to menu" button is pressed.
    // sets the gameOverVisuals' active value to false then loads the "Menu" scene through the Unity "SceneManager" operator.
    public void BackToMenuButton()
    {
        gameOverVisuals.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    // public quit game function, called when the "quit game" button is pressed.
    // Quits the game (not just snake but the whole game, exits the application).
    public void QuitGameButton()
    {
        Application.Quit();
    }

    // public continue function, called when the "continue" button is pressed.
    // sets the gameOver boolean to false, sets the pauseVisuals' active to false as this button is shown on the pause screen.
    public void ContinueButton()
    {
        gameOver = false;
        pauseVisuals.SetActive(false);
    }

    // public snake game score calculation function, called whenever the global score needs to be calculated.
    // first sets highScore int value to the "SnakeHighScore" global playerprefs value for conciseness.
    // then, if the score the player achieved on this itteration of the game is higher than the current high score, both "highScore" and the high score playerpref are set to the current score.
    // then the highscore and gameoverhighscore textboxes are set to the playerprefs global highscore converted to a string.
    // finally the score value and textbox is set to 0 for the next round.
    public void SnakeScoreCalculate()
    {
        highScore = PlayerPrefs.GetInt("SnakeHighScore");

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("SnakeHighScore", highScore);
        }

        highScoreText.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();
        gameOverHighScoreText.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();

        score = 0;
        scoreText.text = score.ToString();
    }
}