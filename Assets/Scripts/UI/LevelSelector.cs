using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The settings panel to toggle")]
    public GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SelectLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMainMenu()
    {   
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Settings Panel is not assigned in the inspector!");
        }
    }
}