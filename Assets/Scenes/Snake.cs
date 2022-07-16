using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            
        }
    }

    private void FixedUpdate()
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

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void ResetState()
    {
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

        if (score > highScore)
        {
            highScore = score;
        }

        print("high score: " + highScore);

        score = 0;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Food")
        {
            Grow();
            score++;
            print(score);
        } else if (other.tag == "Obstacle")
        {
            ResetState();
        }
        
    }

}
