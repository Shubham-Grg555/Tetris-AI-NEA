using System;
using System.Collections.Generic;
using UnityEngine;

public class easyAISpawnTetrominoes : spawnTetrominoBlueprint
{
    // variables for game logic and assiging tetrominoes and their information
    public GameObject[] arrayOfTetrominoes;
    public Queue<GameObject> queueOfNextTetrominoes;

    public GameObject currentTetromino;
    private GameObject currentTetrominoCopy;

    private Vector3 defaultSpawnLocation = new Vector3(30, 20, 0);
    private Vector3 OTetrominoSpawnLocation = new Vector3(30.5f, 20.5f, 0);

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
        currentTetromino = queueOfNextTetrominoes.Dequeue();
        spawnTetrominoInCorrectLocation(currentTetromino);
        tetrominoSelection(arrayOfTetrominoes, queueOfNextTetrominoes, pieces);
    }

    protected override void spawnTetrominoInCorrectLocation(GameObject currentTetromino)
    {
        // Only O tetromino has a different spawn location due to rotation points
        switch (currentTetromino.name)
        {
            case "I tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "J tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "L tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "O tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, OTetrominoSpawnLocation, Quaternion.identity);
                break;
            case "S tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "T tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
            case "Z tetromino easy AI":
                currentTetrominoCopy = Instantiate(currentTetromino, defaultSpawnLocation, Quaternion.identity);
                break;
        }
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
