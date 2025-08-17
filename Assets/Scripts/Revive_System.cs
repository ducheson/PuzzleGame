using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Needed for Button & Image

public class Revive_System : MonoBehaviour
{
    private ResultMenu resultMenu;
    public Button reviveButton; // Assign in Inspector
    public TextMeshProUGUI extraLiveText;
    public TextMeshProUGUI reviveText;
    public Material disabledMaterial; // Assign a material for gray-out effect
    private Material originalMaterial; // Store the original button material

    public bool hasWatchedAd = false;
    private bool hasRevived = false; // Prevent multiple revives

    void Start()
    {
        resultMenu = ResultMenu.FindAnyObjectByType<ResultMenu>();

        if (reviveButton != null)
        {
            Image btnImage = reviveButton.GetComponent<Image>();
            if (btnImage != null)
                originalMaterial = btnImage.material; // save original material
        }

        if (Ads_Manager.Instance != null)
        {
            Ads_Manager.Instance.OnRewardEarned += OnRewardEarned;
            Ads_Manager.Instance.OnAdClosed += OnAdClosed;
        }
        else
        {
            Debug.LogError("Ads_Manager not found in scene!");
        }
    }

    private void OnDestroy()
    {
        if (Ads_Manager.Instance != null)
        {
            Ads_Manager.Instance.OnRewardEarned -= OnRewardEarned;
            Ads_Manager.Instance.OnAdClosed -= OnAdClosed;
        }
    }

    private void OnAdClosed()
    {
        Debug.Log("Ad closed.");

        if (!hasWatchedAd)
        {
            Debug.Log("No reward earned – skipping revive.");
            return;
        }

        if (hasRevived)
        {
            Debug.Log("Revive already used – skipping.");
            return;
        }

        // Delay so SDK restores timescale first
        StartCoroutine(DelayPauseAfterAd());
    }

    private IEnumerator DelayPauseAfterAd()
    {
        yield return null; // wait 1 frame
        Time.timeScale = 0f;
        Debug.Log("Time paused after ad closed.");
    }

    private void OnRewardEarned()
    {
        if (hasRevived)
        {
            Debug.Log("Revive already used – ignoring reward.");
            return;
        }

        Debug.Log("Player earned revive reward!");
        hasWatchedAd = true;

        // Show revive text, hide extra live text
        if (extraLiveText != null) extraLiveText.gameObject.SetActive(false);
        if (reviveText != null) reviveText.gameObject.SetActive(true);
    }

    public void Revive()
    {
        if (hasWatchedAd)
        {
            resultMenu.HideImmediateMenu();
            StartCoroutine(ReviveAfterDelay());
        }
    }

    private IEnumerator ReviveAfterDelay()
    {
        Debug.Log("Reviving player after ad.");

        yield return null;

        Prefab[] allPrefabs = GameObject.FindObjectsByType<Prefab>(FindObjectsSortMode.None);

        var targetPrefabs = new System.Collections.Generic.List<Prefab>();
        foreach (Prefab p in allPrefabs)
        {
            int index = Game_System.Instance.spawnObjects.IndexOf(p.originPrefab);
            if (index >= 0 && index <= 2)
                targetPrefabs.Add(p);
        }

        int prefabCount = targetPrefabs.Count;

        float scaleFactor = 2f; 
        float totalTime = 2f + Mathf.Log10(prefabCount / 5f + 1f) * scaleFactor;

        float delayPerPrefab = Mathf.Max(0.05f, totalTime / Mathf.Max(1, prefabCount));

        Debug.Log($"Removing {prefabCount} prefabs over {totalTime:F2} seconds (delay {delayPerPrefab:F2}s each).");

        foreach (Prefab p in targetPrefabs)
        {
            Destroy(p.gameObject);
            yield return new WaitForSecondsRealtime(delayPerPrefab);
        }

        Time.timeScale = 1f;

        hasRevived = true;

        // Disable revive button and set material
        if (reviveButton != null)
        {
            reviveButton.interactable = false;

            Image btnImage = reviveButton.GetComponent<Image>();
            if (btnImage != null && disabledMaterial != null)
                btnImage.material = disabledMaterial;
        }

        hasWatchedAd = false;
    }

    public void ResetRevive()
    {
        hasRevived = false;

        // Re-enable revive button
        if (reviveButton != null)
        {
            reviveButton.interactable = true;

            Image btnImage = reviveButton.GetComponent<Image>();
            if (btnImage != null && originalMaterial != null)
                btnImage.material = originalMaterial;
        }

        // Reset UI state
        if (extraLiveText != null) extraLiveText.gameObject.SetActive(true);
        if (reviveText != null) reviveText.gameObject.SetActive(false);
    }
}
