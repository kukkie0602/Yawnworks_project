using System.Collections.Generic; 

[System.Serializable]
public class SettingsData
{
    public float musicVolume;

    public SettingsData()
    {
        musicVolume = 1f;
    }
}

[System.Serializable]
public class HighScoresData
{
    public List<string> levelNames = new List<string>();
    public List<int> scores = new List<int>();
    public List<int> coins = new List<int>();
}