using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AITetrominoMovementBlueprint : tetrominoMovementBlueprint
{
    protected static Transform[,] simulatedGrid = new Transform[gridWidth, gridHeight];
    protected static Transform[,] aiGrid = new Transform[gridWidth, gridHeight];
    protected int maxRotationsForSimulation = 3;

    protected abstract IEnumerator startAI();

    protected abstract float waitRandomAmountOfTime();

    protected void MoveTetromino(Vector3 position, int rotation)
    {
        Vector3 startingPosition = transform.position;
        for (int i = 0; i < rotation; i++)
        {
            rotate();
        }
        // checks if the idea position is right of the current position
        // So it knows where and how far to move the tetromino to the desired location
        if (transform.position.x < position.x + 26)
        {
            // adjusting by 26, so everything is in relation to the grid
            for (int i = 0; i < position.x - (Math.Round(startingPosition.x) - 26); i++)
            {
                moveRight();
            }
        }
        else // moves it left
        {
            for (int i = 0; i < (Math.Round(startingPosition.x) - 26) - position.x; i++)
            {
                moveLeft();
            }
        }
        hardDrop();
        addTetrominoToGrid();
        checkForCompletedLine();
    }

    protected Tuple<Vector3, int> getBestLocationAndRotation(GameObject currentTetromino)
    {
        Vector3 originalPosition = currentTetromino.transform.position;
        Quaternion originalRotation = currentTetromino.transform.rotation;
        Dictionary<Vector3, int> possiblePositionsAndRotation = new Dictionary<Vector3, int>();

        possiblePositionsAndRotation = simulateAllPositionsAndRotations(originalPosition, originalRotation, currentTetromino);

        float highestScore = float.NegativeInfinity;
        Vector3 bestOverallLocation = Vector3.zero;
        int bestOverallRotation = 0;

        foreach (KeyValuePair<Vector3, int> keyValuePair in possiblePositionsAndRotation)
        {
            Vector3 position = keyValuePair.Key;
            int rotation = keyValuePair.Value;

            rotateSimulatedTetromino(rotation, currentTetromino);

            currentTetromino.transform.position = position;

            // Add in the simulated tetromino into the simulated grid
            foreach (Transform children in currentTetromino.transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                // can sometimes round too high or low
                Tuple<int, int> fixedRoundedValues = fixPotentialRoundingIssue(roundedX, roundedY);
                roundedX = fixedRoundedValues.Item1;
                roundedY = fixedRoundedValues.Item2;

                simulatedGrid[roundedX, roundedY] = children;
            }

            float score = calculateScoreForSimulatedPosition();

            // remove the simulated tetromino into the simulated grid
            foreach (Transform children in currentTetromino.transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                // same potential rounding issue
                Tuple<int, int> fixedRoundedValues = fixPotentialRoundingIssue(roundedX, roundedY);
                roundedX = fixedRoundedValues.Item1;
                roundedY = fixedRoundedValues.Item2;

                // Remove the simulated tetromino the array
                simulatedGrid[roundedX, roundedY] = null;
            }

            if (score > highestScore)
            {
                highestScore = score;
                bestOverallLocation = position;
                bestOverallRotation = rotation;
            }
            // resets position and rotation of the simulated tetromino
            currentTetromino.transform.position = originalPosition;
            currentTetromino.transform.rotation = originalRotation;
        }
        return Tuple.Create(bestOverallLocation, bestOverallRotation);
    }

    private Dictionary<Vector3, int> simulateAllPositionsAndRotations(Vector3 originalPosition, Quaternion originalRotation, GameObject currentTetromino)
    {
        Dictionary<Vector3, int> possiblePositionsAndRotation = new Dictionary<Vector3, int>();

        // iterate for loop until all rotations have been tested for their position
        for (int rotationNumber = 0; rotationNumber < maxRotationsForSimulation; rotationNumber++)
        {
            rotateSimulatedTetromino(rotationNumber, currentTetromino);
            Vector3[] possiblePositionsWithCurrentRotation = getAllPossiblePositions(currentTetromino, originalPosition, originalRotation);
            foreach (Vector3 validPosition in possiblePositionsWithCurrentRotation)
            {
                // ensures that there are no duplicates in the dictionary
                if (!possiblePositionsAndRotation.ContainsKey(validPosition))
                {
                    // adds all the positions and rotations possible into the dictionary
                    possiblePositionsAndRotation.Add(validPosition, rotationNumber);
                }
            }
            // reseting position and rotation of tetromino, so it can simulate more positions and rotations
            currentTetromino.transform.position = originalPosition;
            currentTetromino.transform.rotation = originalRotation;
        }
        return possiblePositionsAndRotation;
    }

    private void rotateSimulatedTetromino(int rotation, GameObject currentTetromino)
    {
        switch (rotation)
        {
            case 1:
                currentTetromino.transform.Rotate(Vector3.forward, 90f);
                break;
            case 2:
                currentTetromino.transform.Rotate(Vector3.forward, 180f);
                break;
            case 3:
                currentTetromino.transform.Rotate(Vector3.forward, 270f);
                break;
            default:
                break;
        }
    }

    private Vector3[] getAllPossiblePositions(GameObject currentTetromino, Vector3 originalPosition, Quaternion originalRotation)
    {
        List<Vector3> possiblePositions = new List<Vector3>();

        // simulating all possible positions the tetromino can be in and removing the ones that are invalid
        for (int x = 0; x < gridWidth; x++)
        {
            currentTetromino.transform.position = new Vector3(x, currentTetromino.transform.position.y);
            while (aiGrid[x, Mathf.RoundToInt(currentTetromino.transform.position.y + 1)] == null)
            {
                currentTetromino.transform.position += Vector3.down;
                if (!isSimulatedPositionValid(currentTetromino))
                {
                    currentTetromino.transform.position += Vector3.up;
                    break;
                }
            }
            Vector3 endPosition = new Vector3(x, currentTetromino.transform.position.y, 0);
            // Check if the rotated position is valid
            if (isSimulatedPositionValid(currentTetromino))
            {
                possiblePositions.Add(endPosition);
            }
            // reseting position and rotation of tetromino, so it can simulate more positions and rotations
            currentTetromino.transform.position = originalPosition;
            currentTetromino.transform.rotation = originalRotation;
        }

        return possiblePositions.ToArray();
    }

    private bool isSimulatedPositionValid(GameObject currentTetromino)
    {
        // Check if the position is within the grid bounds and not occupied by other blocks
        foreach (Transform tetrominoSquare in currentTetromino.transform)
        {
            int roundedX = Mathf.RoundToInt(currentTetromino.transform.position.x);
            int roundedY = Mathf.RoundToInt(currentTetromino.transform.position.y);
            if (roundedX < 0 || roundedX >= gridWidth || roundedY < 0 || roundedY >= gridHeight)
            {
                return false;
            }

            try
            {
                if (simulatedGrid[roundedY, roundedX] != null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        return true;
    }

    private Tuple<int, int> fixPotentialRoundingIssue(int roundedX, int roundedY)
    {
        if (roundedX < 0)
        {
            roundedX = 0;
        }
        else if (roundedX > 9)
        {
            roundedX = 9;
        }

        if (roundedY < 0)
        {
            roundedY = 0;
        }
        else if (roundedY > 9)
        {
            roundedY = 9;
        }
        return Tuple.Create(roundedX, roundedY);
    }

    public abstract float calculateScoreForSimulatedPosition();

    public int calculateMaximumLineHeight()
    {
        int maximumLineHeight = 0;

        for (int col = 0; col < gridWidth; col++)
        {
            for (int row = 0; row < gridHeight - 1; row++)
            {
                if (simulatedGrid[col, row] != null)
                {
                    // Finds the highest row height after adding in the simulated tetromino and returns it
                    maximumLineHeight = Math.Max(maximumLineHeight, (gridHeight - 1) - row);
                    break;
                }
            }
        }
        return maximumLineHeight;
    }

    public int getLineClears()
    {
        int linesCleared = 0;
        // Checks if adding the simulated tetromino will clear any lines
        for (int row = 0; row < gridHeight - 1; row++)
        {
            bool lineClear = true;
            for (int col = 0; col < gridWidth; col++)
            {
                if (simulatedGrid[col, row] == null)
                {
                    lineClear = false;
                }
            }
            if (lineClear)
            {
                linesCleared++;
            }
        }
        return linesCleared;
    }

    public int getHoles()
    {
        int holesCount = 0;

        // Loop through each column of the grid
        for (int x = 0; x < gridWidth; x++)
        {
            bool foundBlock = false;
            for (int y = 0; y < gridHeight - 1; y++)
            {
                // Check if there's a block in the current cell of the grid
                if (simulatedGrid[x, y] != null)
                {
                    foundBlock = true;
                }
                // If there's no block in the current cell and we've already found a block above,
                // it means there's a hole in this column
                else if (foundBlock)
                {
                    holesCount++;
                }
            }
        }

        return holesCount;
    }

    public int calculateBumpiness()
    {
        int bumpinessCount = 0;

        // Loop through each column of the grid
        // gridWidth -1 as I can't compared the bumpiness of something outside the grid
        for (int x = 0; x < gridWidth - 1; x++)
        {
            // Calculate the height of the current column
            int currentColumnHeight = getColumnHeight(x);

            // Calculate the height of the next column
            int nextColumnHeight = getColumnHeight(x + 1);

            // Calculate the absolute difference in height between adjacent columns and add it to bumpiness
            bumpinessCount += Mathf.Abs(currentColumnHeight - nextColumnHeight);
        }

        return bumpinessCount;
    }

    public int getColumnHeight(int x)
    {
        int maxHeight = 0;

        // Loop through each row of the specified column
        for (int y = 0; y < gridHeight - 1; y++)
        {
            // Check if the cell in the grid is occupied by a block
            if (simulatedGrid[x, y] != null)
            {
                // Update the maximum height if the current cell is occupied
                maxHeight = Mathf.Max(maxHeight, y);
            }
        }

        return maxHeight;
    }

    protected override void addTetrominoToGrid()
    {
        if (validTetrominoMove())
        {
            // loops through each sprite square and adds it into the grid
            foreach (Transform tetrominoSquare in transform)
            {
                // rounds position of x and y values so they are not in an unexpected weird position e.g x = 1.19374
                int roundedX = Mathf.RoundToInt(tetrominoSquare.transform.position.x);
                int roundedY = Mathf.RoundToInt(tetrominoSquare.transform.position.y);
                roundedX -= 26;
                aiGrid[roundedX, roundedY] = tetrominoSquare;
                simulatedGrid[roundedX, roundedY] = tetrominoSquare;
                this.enabled = false;
            }
        }
    }

    protected override bool validTetrominoMove()
    {
        bool isValidMove = true;
        // loops through each sprite square to make sure it is within the grid
        foreach (Transform tetrominoSquare in transform)
        {
            // rounds position of x and y values so they are not in an unexpected weird position e.g x = 1.19374
            int roundedX = Mathf.RoundToInt(tetrominoSquare.transform.position.x);
            int roundedY = Mathf.RoundToInt(tetrominoSquare.transform.position.y);
            roundedX -= 26; // -26 from roundedX, as the ai is on the right side of the screen so I can just minus 26 from roundedX
            if (roundedX < 0 || roundedX >= gridWidth || roundedY < 0 || roundedY >= gridHeight)
            {
                isValidMove = false;
                return isValidMove;
            }
            else if (aiGrid[roundedX, roundedY] != null)
            {
                isValidMove = false;
                return isValidMove;
            }
        }
        return isValidMove;
    }
}
