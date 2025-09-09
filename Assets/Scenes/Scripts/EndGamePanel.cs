using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScreen");
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game.");

        Application.Quit();
    }
}