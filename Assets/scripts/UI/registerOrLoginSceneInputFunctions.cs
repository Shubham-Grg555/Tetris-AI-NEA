using UnityEngine;
using UnityEngine.SceneManagement;

public class regOrLoginButtonFunctions : MonoBehaviour
{
    public void goToRegisterScene()
    {
        SceneManager.LoadScene("register");
    }

    public void goToLoginScene()
    {
        SceneManager.LoadScene("login");
    }
}
