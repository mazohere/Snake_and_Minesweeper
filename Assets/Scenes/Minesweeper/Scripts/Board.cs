using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{

    // establishing gameobjects including every possible type of tile for the game with Num1, Num2, ect representing a tile with x amount of bombs in its vicinity.
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
    public Text timer;

    // Awake functions in unity are called whenever the gameobject the script is running on is initialised, regardless of whether or not the script is enabled.
    // private awake function which gets the games Tilemap (the area cells are allowed to be drawn in).
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // public cell drawing function. Public because it is called in another script.
    // starts by setting the width and height variables to the bounds of a parameter called "state", seen in the scripts this function is called in to be a list of the x and y co-ordinants and types of each cell.
    // then cycles through each dimensionally valid cell space using two for loops (one for the cell width and one for its height) and places a cell defined by the GetTile function using a SetTile function from the unity engine used for tilemapping.
    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);
        
        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    // private function for returning the type of cell in the parameters when the function is called.
    // short if statement to check the type of cell called; the tile being revealed opens more options for what it could be which are evaluated in the GetRevealedTile function for the sake of readability, if the player has not interacted with the tile it will be unknown and be tagged as such.
    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell);
        } else if (cell.flagged)
        {
            return tileFlag;
        }else
        {
            return tileUnknown;
        }
    }

    // private function for returning the type of revealed cell in the parameters.
    // the main things to understand here is the use of the "?" operator which works like this: condition ? consequent : alternative. Meaning if the cell is a mine the function evaluates whether it has exploded through the boolean variable "cell.exploded", if it has it returns "tileExploded" and if it hasn't it returns "tileMine". (a revealed cell being a non-exploded mine is only possible in a win state as that is when all cells are revealed).
    // if the revealed cell is a number that opens up even more possible types of cells, so the GetNumberTile is called.
    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded: tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    // private function for getting the number of a number tile.
    // literally just checks which number the cell is and returns that.
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }

    }
}
