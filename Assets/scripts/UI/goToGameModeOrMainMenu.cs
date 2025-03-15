using UnityEngine;
using UnityEngine.SceneManagement;

public class goToGameModeOrMainMenu : MonoBehaviour
{
    public void goToSoloGameMode()
    {
        SceneManager.LoadScene("soloGameMode");
    }

    public void goToEasyGameMode()
    {
        SceneManager.LoadScene("easyAIGameMode");
    }

    public void goToMediumGameMode()
    {
        SceneManager.LoadScene("mediumAIGameMode");
    }

    public void goToHardGameMode()
    {
        SceneManager.LoadScene("hardAIGameMode");
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
