using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;

public class HistoryUI : MonoBehaviour
{
    [Header("Latest Score")]
    public Transform content_Latest;
    public GameObject prefab_Latest;

    [Header("Highest Score")]
    public Transform content_Highest;
    public GameObject prefab_Highest;

    [Header("Others")]
    public GameObject panel;
    
    public Sprite[] rankSprites;

    public void Start()
    {
        panel.SetActive(false);
    }

    private void UpdateHistory()
    {
        // Clear existing items
        foreach (Transform child in content_Latest)
        {
            Destroy(child.gameObject);
        }

        List<int> scores = Data_Manager.Instance.GetScoreHistory();
        List<float> times = Data_Manager.Instance.GetTimeHistory();

        for (int i = 0; i < scores.Count; i++)
        {
            GameObject item = Instantiate(prefab_Latest, content_Latest);

            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in texts)
            {
                if (text.gameObject.name == "Index")
                    text.text = (i + 1).ToString(); // 1 to 10
                else if (text.gameObject.name == "Score")
                    text.text = $"Score: {scores[scores.Count - 1 - i]}";
                else if (text.gameObject.name == "Time")
                {
                    float timeValue = times[times.Count - 1 - i];

                    int hours = (int)(timeValue / 3600);
                    int minutes = (int)((timeValue % 3600) / 60);
                    int seconds = (int)(timeValue % 60);
                    text.text = $"{hours:00}:{minutes:00}:{seconds:00}";
                }
            }
        }
    }

    private void UpdateHighest()
    {
        // Clear existing items
        foreach (Transform child in content_Highest)
        {
            Destroy(child.gameObject);
        }

        List<int> topScores = Data_Manager.Instance.GetTopScores();
        int maxItems = 5;

        for (int i = 0; i < maxItems; i++)
        {
            GameObject item = Instantiate(prefab_Highest, content_Highest);
            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in texts)
            {
                if (text.gameObject.name == "Score")
                {
                    if (i < topScores.Count)
                    {
                        text.text = $"Score: {topScores[i]}";
                        text.gameObject.SetActive(true);
                    }
                    else
                    {
                        text.gameObject.SetActive(false); // No score to show
                    }
                }
            }

            // Always show the rank image if we have a sprite for it
            Transform imageTransform = item.transform.Find("Image");
            if (imageTransform != null)
            {
                Image image = imageTransform.GetComponent<Image>();

                if (i < rankSprites.Length)
                {
                    image.sprite = rankSprites[i];
                    imageTransform.gameObject.SetActive(true);
                }
                else
                {
                    imageTransform.gameObject.SetActive(false); // No sprite available
                }
            }
        }
    }

    public void OpenHistory()
    {
        if (!panel.activeSelf)
        {
            UpdateHistory();
            UpdateHighest();
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
