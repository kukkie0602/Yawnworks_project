using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelectorScene");
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game.");

        Application.Quit();
    }
}