using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // establish the box collider for the engine to act as a guide for where the food can appear. Declaring the "Food" variable isn't needed because since this script is running on the food gameobject in the engine, it can be called using "this.transform".
    public BoxCollider2D gridArea;

    // private start function always called when the script begins to run.
    // calls RandomizePosition function because the functions operations are needed at times other than startup.
    private void Start()
    {
        RandomizePosition();
    }

    // private randomize position function for randomizing the foods position.
    // defines "bounds" as the bounds of the gridArea established earlier, basically just the game area visually seen in the engine.
    // defines private floats x and y as random values between the minimum and maximum values of the game area.
    // sets the "Food" gameobject to those random values.
    private void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);

    }

    // private collision detection function for the "Food" gameobject.
    // because the scoring is calculated in the "Snake" script, the only thing the food has to do is randomize where it is when it detects collision, meaning whether it collides with the gameobject tagged as "Player" or the ones tagged with "Obstacle", it should randomize itself. It does so with the "Obstacle" tags to prevent spawning in the snake segments.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "Obstacle")
        {
            RandomizePosition();
        }
    }
}