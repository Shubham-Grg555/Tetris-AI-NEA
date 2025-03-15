using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class easyAIMovement : AITetrominoMovementBlueprint
{
    // variables used for mechanics e.g fall speed, rotation and line clear
    private float fallSpeed = 0.8f;
    private static float previousTime;
    protected easyAISpawnTetrominoes spawnEasyAITetrominoes;
    private new bool isGameOver = false;

    // variables used for texts
    TMP_Text scoreCounter;

    private void Start()
    {
        GameObject getCorrectScoreCounterTextBox = GameObject.Find("easyAIScoreCounter");
        scoreCounter = getCorrectScoreCounterTextBox.GetComponent<TMP_Text>();
        spawnEasyAITetrominoes = FindObjectOfType<easyAISpawnTetrominoes>();

        StartCoroutine(startAI());
    }

    private void Update()
    {
        if (Time.time - previousTime > fallSpeed)
        {
            transform.position += Vector3.down;
            if (!validTetrominoMove())
            {
                transform.position += Vector3.up;
                addTetrominoToGrid();
                checkForCompletedLine();
                spawnEasyAITetrominoes.spawnTetromino();
            }
            previousTime = Time.time;
        }

        increaseFallSpeedCheck();

        if (checkIfGameOver())
        {
            SceneManager.LoadScene("gameWonEasy");
        }
    }

    private void increaseFallSpeedCheck()
    {
        float currentTime = Time.time;
        switch (currentTime)
        {
            case > 180:
                fallSpeed = 0.5f;
                break;
            case > 150:
                fallSpeed = 0.55f;
                break;
            case > 120:
                fallSpeed = 0.6f;
                break;
            case > 90:
                fallSpeed = 0.65f;
                break;
            case > 60:
                fallSpeed = 0.7f;
                break;
            case > 30:
                fallSpeed = 0.75f;
                break;
        }
    }

    protected override IEnumerator startAI()
    {
        while (true)
        {
            GameObject currentTetromino = spawnEasyAITetrominoes.currentTetromino;
            Tuple<Vector3, int> bestLocationAndRotation;

            bestLocationAndRotation = getBestLocationAndRotation(currentTetromino);

            float waitTime = waitRandomAmountOfTime();
            // makes ai slower and reduces tetromino placing speed
            yield return new WaitForSeconds(waitTime);

            // Get the best location and rotation from the tuple
            Vector3 bestLocation = bestLocationAndRotation.Item1;
            int bestRotation = bestLocationAndRotation.Item2;

            MoveTetromino(bestLocation, bestRotation);
            spawnEasyAITetrominoes.spawnTetromino();

            waitTime = waitRandomAmountOfTime();
            // makes ai slower and reduces tetromino placing speed
            yield return new WaitForSeconds(waitTime);
        }
    }

    protected override float waitRandomAmountOfTime()
    {
        float randomFloat = UnityEngine.Random.Range(1.5f, 5.0f);
        return randomFloat;
    }

    public override float calculateScoreForSimulatedPosition()
    {
        // high height weight, so it will avoid stacking up a lot of lines, so unlikely to clear a lot of lines
        float heightWeight = -0.5f;
        float linesWeight = 1.0f;
        float holesWeight = -0.125f; // much more likely to have holes
        float bumpinessWeight = -0.075f; // don't care about bumpiness a lot

        int maximumLineHeight = calculateMaximumLineHeight();
        int totalLineClears = getLineClears();
        int holes = getHoles();
        int bumpiness = calculateBumpiness();

        float score = maximumLineHeight * heightWeight +
                      totalLineClears * linesWeight +
                      holes * holesWeight +
                      bumpiness * bumpinessWeight;

        return score;
    }

    protected override bool checkIfGameOver()
    {
        // checks every square in the top two lines that are above the grid
        for (int aboveGrid = 20; aboveGrid < gridHeight - 1; aboveGrid++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                // only adds tetromino into the grid until it has stopped moving as there is a tetromino below
                // So the code can check if it is null at y value 20 or higher and return true to signify that the game is over
                // Can be done as the tetromino falling above the grid is not yet added into the grid
                if (aiGrid[col, aboveGrid] != null)
                {
                    isGameOver = true;
                    return isGameOver;
                }
            }
        }
        return isGameOver;
    }
}