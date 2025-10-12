using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGameManager : MonoBehaviour
{
    public void KeepPlaying()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game.");

        Application.Quit();
    }
}