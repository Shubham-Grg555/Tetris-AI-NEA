using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerTetrominoMovement : tetrominoMovementBlueprint
{
    // variables used for mechanics e.g fall speed, rotation and line clear
    private float fallSpeed = 0.8f;
    private static float previousTime;
    private playerSpawnTetrominoes playerSpawnTetrominoes;
    private static int numbOfHold = 0;

    // variables used for texts
    public TMP_Text scoreCounter;


    // Start is called before the first frame update
    private void Start()
    {
        GameObject getCorrectScoreCounterTextBox = GameObject.Find("playerScoreCounter");
        scoreCounter = getCorrectScoreCounterTextBox.GetComponent<TMP_Text>();
        playerSpawnTetrominoes = FindObjectOfType<playerSpawnTetrominoes>();
    }

    // Update is called once per frame
    private void Update()
    {
        scoreCounter.text = score.ToString();
        handleUserInput();

        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallSpeed / 10 : fallSpeed))
        {
            transform.position += Vector3.down;
            if (!validTetrominoMove())
            {
                transform.position += Vector3.up;
                addTetrominoToGrid();
                this.enabled = false;
                checkForCompletedLine();
                numbOfHold = 0;
                playerSpawnTetrominoes.spawnTetromino();
            }
            previousTime = Time.time;
        }

        increaseFallSpeedCheck();

        if (checkIfGameOver())
        {
            if (FindAnyObjectByType<easyAISpawnTetrominoes>() == true)
            {
                SceneManager.LoadScene("gameOverEasy");
            }
            else if (FindAnyObjectByType<mediumAISpawnTetrominoes>() == true)
            {
                SceneManager.LoadScene("gameOverMedium");
            }
            else if (FindAnyObjectByType<hardAIMovement>() == true)
            {
                SceneManager.LoadScene("gameOverHard");
            }
            else
            {
                SceneManager.LoadScene("gameOverSolo");
            }
        }
    }

    private void handleUserInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            moveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            moveRight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            moveDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            hardDrop();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && numbOfHold == 0)
        {
            hold();
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

    // ai is not built to use hold, since it should be a low skilled ai
    // so only need hold to be in the player tetromino movement
    private void hold()
    {
        numbOfHold = 1;
        playerSpawnTetrominoes.holdTetromino();
    }
}