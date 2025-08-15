using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Data_Manager : MonoBehaviour
{
    public static Data_Manager Instance;

    private List<int> latestScore = new List<int>();
    private const string SCORE_LIST_KEY = "LatestScoreList";

    private List<float> latestTime = new List<float>();
    private const string TIME_LIST_KEY = "LatestTimeList";

    private List<int> highestScore = new List<int>();
    private const string HIGHEST_SCORE_KEY = "HighestScoreList";

    void Awake()
    {
        Instance = this;
        LoadData();
    }

    public void SaveCurrentScore()
    {
        int currentPoint = Point_System.Instance.GetCurrentPoint();
        float currentTime = Time_System.Instance.GetCurrentTime();

        if (currentPoint == 0) return;

        // Save latest scores
        latestScore.Add(currentPoint);
        if (latestScore.Count > 20)
            latestScore.RemoveAt(0);
        PlayerPrefs.SetString(SCORE_LIST_KEY, string.Join(",", latestScore));

        // Save latest times (as float with 2 decimal places)
        latestTime.Add(currentTime);
        if (latestTime.Count > 20)
            latestTime.RemoveAt(0);
        PlayerPrefs.SetString(TIME_LIST_KEY, string.Join(",", latestTime.Select(t => t.ToString("F2"))));

        // Save highest scores
        highestScore.Add(currentPoint);
        highestScore = highestScore
            .OrderByDescending(s => s)
            .Take(5)
            .ToList();
        PlayerPrefs.SetString(HIGHEST_SCORE_KEY, string.Join(",", highestScore));

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        // Load latest scores
        if (PlayerPrefs.HasKey(SCORE_LIST_KEY))
        {
            string[] entries = PlayerPrefs.GetString(SCORE_LIST_KEY).Split(',');
            latestScore.Clear();
            foreach (var entry in entries)
            {
                if (int.TryParse(entry, out int score))
                    latestScore.Add(score);
            }
        }

        // Load latest times
        if (PlayerPrefs.HasKey(TIME_LIST_KEY))
        {
            string[] entries = PlayerPrefs.GetString(TIME_LIST_KEY).Split(',');
            latestTime.Clear();
            foreach (var entry in entries)
            {
                if (float.TryParse(entry, out float time))
                    latestTime.Add(time);
            }
        }

        // Load highest scores
        if (PlayerPrefs.HasKey(HIGHEST_SCORE_KEY))
        {
            string[] entries = PlayerPrefs.GetString(HIGHEST_SCORE_KEY).Split(',');
            highestScore.Clear();
            foreach (var entry in entries)
            {
                if (int.TryParse(entry, out int score))
                    highestScore.Add(score);
            }

            highestScore = highestScore
                .OrderByDescending(s => s)
                .Take(5)
                .ToList();
        }
    }

    public List<int> GetScoreHistory()
    {
        return new List<int>(latestScore);
    }

    public List<float> GetTimeHistory()
    {
        return new List<float>(latestTime);
    }

    public List<int> GetTopScores()
    {
        return new List<int>(highestScore);
    }

    public void ClearHistory()
    {
        latestScore.Clear();
        latestTime.Clear();
        highestScore.Clear();

        PlayerPrefs.DeleteKey(SCORE_LIST_KEY);
        PlayerPrefs.DeleteKey(TIME_LIST_KEY);
        PlayerPrefs.DeleteKey(HIGHEST_SCORE_KEY);
        PlayerPrefs.Save();
    }
}