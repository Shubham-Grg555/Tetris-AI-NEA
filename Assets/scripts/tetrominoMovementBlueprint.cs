using UnityEngine;

public abstract class tetrominoMovementBlueprint : MonoBehaviour
{
    // variables used for the grid
    // 2 above the intended grid size, so that I spawn tetrminoes above the grid
    // so tetrominoes are not placed automatically if there is something at line 19
    protected static int gridHeight = 22;
    protected static int gridWidth = 10;
    protected static Transform[,] grid = new Transform[gridWidth, gridHeight];

    // variables used for mechanics e.g fall speed and line clear
    private int consecutiveLineClears = 0;
    protected bool isGameOver = false;
    protected static int score = 0;

    protected void moveLeft()
    {
        transform.position += Vector3.left;
        if (!validTetrominoMove())
        {
            transform.position += Vector3.right;
        }
    }

    protected void moveRight()
    {
        transform.position += Vector3.right;
        if (!validTetrominoMove())
        {
            transform.position += Vector3.left;
        }
    }

    protected void moveDown()
    {
        transform.position += Vector3.down;
        if (!validTetrominoMove())
        {
            transform.position += Vector3.up;
        }
    }

    protected void rotate()
    {
        transform.Rotate(Vector3.forward, 90f); // rotates by 90 degrees
        if (!validTetrominoMove())
        {
            transform.Rotate(Vector3.forward, -90f); // invalid move, so it rotates by -90 degrees
        }
    }

    protected void hardDrop()
    {
        while (validTetrominoMove())
        {
            transform.position += Vector3.down;
        }
        transform.position += Vector3.up;
    }

    // virtual as it does not need to be implemented by all the classes
    protected virtual bool validTetrominoMove()
    {
        bool isValidMove = true;

        // loops through each sprite square to make sure it is within the grid
        foreach (Transform tetrominoSquare in transform)
        {
            // rounds position of x and y values so they are not in an unexpected weird position e.g x = 1.19374
            int roundedX = Mathf.RoundToInt(tetrominoSquare.transform.position.x);
            int roundedY = Mathf.RoundToInt(tetrominoSquare.transform.position.y);
            if (roundedX < 0 || roundedX >= gridWidth || roundedY < 0 || roundedY >= gridHeight)
            {
                isValidMove = false;
                return isValidMove;
            }
            else if (grid[roundedX, roundedY] != null)
            {
                isValidMove = false;
                return isValidMove;
            }
        }
        return isValidMove;
    }

    // virtual as it does not need to be implemented by all the classes
    protected virtual void addTetrominoToGrid()
    {
        if (validTetrominoMove())
        {
            // loops through each sprite square and adds it into the grid
            foreach (Transform tetrominoSquare in transform)
            {
                // rounds position of x and y values so they are not in an unexpected weird position e.g x = 1.19374
                int roundedX = Mathf.RoundToInt(tetrominoSquare.transform.position.x);
                int roundedY = Mathf.RoundToInt(tetrominoSquare.transform.position.y);
                grid[roundedX, roundedY] = tetrominoSquare;
            }
        }
    }

    protected void checkForCompletedLine()
    {
        for (int row = gridHeight - 1; row >= 0; row--)
        {
            if (isCompleteLine(row))
            {
                clearLine(row);
                shiftLinesDown(row);
                scoreCalculator();
            }
        }
        scoreCalculator();
    }

    protected bool isCompleteLine(int row)
    {
        for (int col = 0; col < gridWidth; col++)
        {
            if (grid[col, row] == null)
            {
                return false;
            }
        }
        consecutiveLineClears++;
        return true;
    }

    protected void clearLine(int row)
    {
        for (int col = 0; col < gridWidth; col++)
        {
            Destroy(grid[col, row].gameObject);
            grid[col, row] = null;
        }
    }

    protected void shiftLinesDown(int row1)
    {
        for (int row = row1; row < gridHeight - 1; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                if (grid[col, row] != null && row != 0)
                {
                    grid[col, row - 1] = grid[col, row];
                    grid[col, row] = null;
                    grid[col, row - 1].transform.position += Vector3.down;
                }
            }
        }
    }

    protected void scoreCalculator()
    {
        switch (consecutiveLineClears)
        {
            case 1:
                score += 100;
                break;
            case 2:
                score += 300;
                break;
            case 3:
                score += 500;
                break;
            case 4:
                score += 800;
                break;
        }
        consecutiveLineClears = 0;
    }

    protected virtual bool checkIfGameOver()
    {
        // checks every square in the top two lines that are above the grid
        for (int aboveGrid = 20; aboveGrid < gridHeight - 1; aboveGrid++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                // only adds tetromino into the grid until it has stopped moving as there is a tetromino below
                // So the code can check if it is null at y value 20 or higher and return true to signify that the game is over
                // Can be done as the tetromino falling above the grid is not yet added into the grid
                if (grid[col, aboveGrid] != null)
                {
                    isGameOver = true;
                    return isGameOver;
                }
            }
        }
        return isGameOver;
    }
}
