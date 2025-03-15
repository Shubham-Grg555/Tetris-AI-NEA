using System;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawnTetrominoes : spawnTetrominoBlueprint
{
    // variables for game logic and assiging tetrominoes and their information
    public GameObject[] arrayOfTetrominoes;
    public Queue<GameObject> queueOfNextTetrominoes;

    // variables used for the current, held or next tetrmonio
    private GameObject currentTetromino;
    private GameObject currentTetrominoCopy;
    private static GameObject heldTetrominoCopy;
    private static GameObject[] heldTetromino = new GameObject[2];
    private static GameObject nextTetromino;
    private static GameObject nextTetrominoCopy;
    public GameObject[] arrayOfTetrominoesWithNoMovement;


    // spawn locations
    private Vector3 defaultSpawnLocation = new Vector3(4, 20, 0);
    private Vector3 OTetrominoSpawnLocation = new Vector3(4.5f, 20.5f, 0);
    private Vector3 nextTetrominoSpawnLocation = new Vector3(12, 17, 0);
    private Vector3 heldTetrominoSpawnLocation = new Vector3(-3, 17, 0);

    protected char[] pieces = new char[] { 'I', 'J', 'L', 'O', 'S', 'T', 'Z', '8' };

    // Start is called before the first frame update
    private void Start()
    {
        yateShuffle(pieces);
        queueOfNextTetrominoes = new Queue<GameObject>(1);
        for (int i = 0; i < 6; i++)
        {
            tetrominoSelection(arrayOfTetrominoes, queueOfNextTetrominoes, pieces);
        }
        spawnTetromino();
    }

    public override void spawnTetromino()
    {
        {
            currentTetromino = queueOfNextTetrominoes.Dequeue();
            spawnTetrominoInCorrectLocation(currentTetromino);
            showNextTetromino();
            tetrominoSelection(arrayOfTetrominoes, queueOfNextTetrominoes, pieces);
        }
    }

    // Don't need to show next tetromino for ai, so method is only added here
    protected void showNextTetromino()
    {
        {
            if (nextTetromino == null) // means that the game has just started
            {
                nextTetromino = getTetrominoWithNoMovement(queueOfNextTetrominoes.Peek());
                // displayed the next tetromino and stores it into a variable, so it can be destroyed later
                nextTetrominoCopy = Instantiate(nextTetromino, nextTetrominoSpawnLocation, Quaternion.identity);
            }
            else
            {
                Destroy(nextTetrominoCopy.gameObject);
                nextTetromino = getTetrominoWithNoMovement(queueOfNextTetrominoes.Peek());
                nextTetrominoCopy = Instantiate(nextTetromino, nextTetrominoSpawnLocation, Quaternion.identity);
            }
        }
    }

    // Don't need to show held tetromino for ai, so method is only added here
    public void holdTetromino()
    {
        // check to see if there are any tetrominoes being held
        if (heldTetromino[0] == null)
        {
            // stores tetromino into the array
            heldTetromino[0] = getTetrominoWithNoMovement(currentTetromino);
            heldTetrominoCopy = Instantiate(heldTetromino[0], heldTetrominoSpawnLocation, Quaternion.identity);

            // destroys the current tetromino in the grid, so it gets the next tetromino in the queue
            Destroy(currentTetrominoCopy);
            spawnTetromino();
        }
        else
        {
            // temporarily stores the old held tetromino (the one that will go back into the grid)
            heldTetromino[1] = getTetrominoWithMovement(heldTetromino[0]);
            heldTetromino[0] = getTetrominoWithNoMovement(currentTetromino);

            // destroys the tetromino in the grid and hold tetromino space
            Destroy(heldTetrominoCopy);
            Destroy(currentTetrominoCopy);

            // spawns the tetromino that was in the hold tetromino space back into the grid
            spawnTetrominoInCorrectLocation(heldTetromino[1]);
            // spawns the tetromino that was in the grid into the hold tetromino space
            heldTetrominoCopy = Instantiate(heldTetromino[0], heldTetrominoSpawnLocation, Quaternion.identity);
            heldTetromino[1] = null;
        }
    }

    protected override void spawnTetrominoInCorrectLocation(GameObject currentTetromino)
    {
        // Only O tetromino has a different spawn location due to rotation points
        switch (currentTetromino.name)
        {
            case "I tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "J tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "L tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "O tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, OTetrominoSpawnLocation, Quaternion.identity);
                break;
            case "S tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "T tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "Z tetromino":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
        }
    }

    private GameObject getTetrominoWithNoMovement(GameObject nextTetromino)
    {
        switch (nextTetromino.name)
        {
            case "I tetromino":
                return arrayOfTetrominoesWithNoMovement[0];
            case "J tetromino":
                return arrayOfTetrominoesWithNoMovement[1];
            case "L tetromino":
                return arrayOfTetrominoesWithNoMovement[2];
            case "O tetromino":
                return arrayOfTetrominoesWithNoMovement[3];
            case "S tetromino":
                return arrayOfTetrominoesWithNoMovement[4];
            case "T tetromino":
                return arrayOfTetrominoesWithNoMovement[5];
            case "Z tetromino":
                return arrayOfTetrominoesWithNoMovement[6];
        }
        return null;
    }

    private GameObject getTetrominoWithMovement(GameObject nextTetromino)
    {
        switch (nextTetromino.name)
        {
            case "I tetromino (no movement) Variant":
                return arrayOfTetrominoes[0];
            case "J tetromino (no movement) Variant":
                return arrayOfTetrominoes[1];
            case "L tetromino (no movement) Variant":
                return arrayOfTetrominoes[2];
            case "O tetromino (no movement) Variant":
                return arrayOfTetrominoes[3];
            case "S tetromino (no movement) Variant":
                return arrayOfTetrominoes[4];
            case "T tetromino (no movement) Variant":
                return arrayOfTetrominoes[5];
            case "Z tetromino (no movement) Variant":
                return arrayOfTetrominoes[6];
        }
        return null;
    }

    protected override void enqueTetromino(char tetromino)
    {
        switch (tetromino)
        {
            case 'I':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[0]);
                break;
            case 'J':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[1]);
                break;
            case 'L':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[2]);
                break;
            case 'O':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[3]);
                break;
            case 'S':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[4]);
                break;
            case 'T':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[5]);
                break;
            case 'Z':
                queueOfNextTetrominoes.Enqueue(arrayOfTetrominoes[6]);
                break;
            default:
                throw new Exception("Invalid tetris piece chosen");
        }
    }
}