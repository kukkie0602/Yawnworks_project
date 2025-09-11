using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string SETTINGS_FILE_NAME = "/settings.json";
    private static readonly string HIGHSCORES_FILE_NAME = "/highscores.json";


    public static void SaveSettings(SettingsData data)
    {
        string path = Application.persistentDataPath + SETTINGS_FILE_NAME;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Settings saved to: " + path);
    }

    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + SETTINGS_FILE_NAME;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);
            return data;
        }
        else
        {
            return new SettingsData();
        }
    }


    public static void SaveHighScores(HighScoresData data)
    {
        string path = Application.persistentDataPath + HIGHSCORES_FILE_NAME;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static HighScoresData LoadHighScores()
    {
        string path = Application.persistentDataPath + HIGHSCORES_FILE_NAME;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HighScoresData data = JsonUtility.FromJson<HighScoresData>(json);
            return data;
        }
        else
        {
            return new HighScoresData();
        }
    }
}