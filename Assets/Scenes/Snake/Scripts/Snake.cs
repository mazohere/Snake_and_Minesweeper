using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public int initialSize = 4;
    private string notAllowed = "left";
    private string directionName = "right";
    public int highScore = 0;
    public int score = 0;
    public Text highScoreText;
    public Text scoreText;
    public Text gameOverHighScoreText;
    public GameObject gameOverVisuals;
    public GameObject pauseVisuals;
    private bool gameOver;
    private bool escapeKeyDisable = false;

    private void Start()
    {
        ResetState();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            directionName = "up";
            if (directionName != notAllowed)
            {
                _direction = Vector2.up;
                notAllowed = "down";
            }
            
                
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            directionName = "down";
            if (directionName != notAllowed)
            {
                _direction = Vector2.down;
                notAllowed = "up";
            }
            
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            directionName = "left";
            if (directionName != notAllowed)
            {
                _direction = Vector2.left;
                notAllowed = "right";
            }

        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            directionName = "right";
            if (directionName != notAllowed)
            {
                _direction = Vector2.right;
                notAllowed = "left";
            }
            
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escapeKeyDisable)
            {
                if (!gameOver)
                {
                    gameOver = true;
                    pauseVisuals.SetActive(true);
                    SnakeScoreCalculate();
                } else if (gameOver)
                {
                    ContinueButton();
                }
            }
        }
    }

    private void FixedUpdate()
    {

        if (!gameOver)
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

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void ResetState()
    {
        gameOver = false;
        gameOverVisuals.SetActive(false);
        escapeKeyDisable = false;
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

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Food")
        {
            Grow();
            score++;
            print(score);
            scoreText.text = score.ToString();
        } else if (other.tag == "Obstacle")
        {
            GameOver();
        }
        
    }

    private void GameOver()
    {
        gameOver = true;
        gameOverVisuals.SetActive(true);
        escapeKeyDisable = true;

        SnakeScoreCalculate();
    }

    public void TryAgainButton()
    {
        gameOverVisuals.SetActive(false);
        ResetState();
    }

    public void BackToMenuButton()
    {
        gameOverVisuals.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void ContinueButton()
    {
        gameOver = false;
        pauseVisuals.SetActive(false);
    }

    public void SnakeScoreCalculate()
    {
        highScore = PlayerPrefs.GetInt("SnakeHighScore");

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("SnakeHighScore", highScore);
        }

        print("high score: " + highScore);
        highScoreText.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();
        gameOverHighScoreText.text = (PlayerPrefs.GetInt("SnakeHighScore")).ToString();

        score = 0;
        scoreText.text = score.ToString();
    }

}
