using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Needed for Button & Image

public class Revive_System : MonoBehaviour
{
    public GameObject loseUI;
    public GameObject panel;
    public Button reviveButton; // Assign in Inspector
    public Material disabledMaterial; // Assign a material for gray-out effect

    private bool hasWatchedAd = false;
    private bool hasRevived = false; // Prevent multiple revives

    private void Awake()
    {
        if (Ads_Manager.Instance != null)
        {
            Ads_Manager.Instance.OnRewardEarned += OnRewardEarned;
            Ads_Manager.Instance.OnAdClosed += OnAdClosed;
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

    private void OnRewardEarned()
    {
        if (hasRevived)
        {
            Debug.Log("Revive already used – ignoring reward.");
            return;
        }

        Debug.Log("Player earned revive reward!");
        hasWatchedAd = true;
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

        StartCoroutine(ReviveAfterDelay());
    }

    private IEnumerator ReviveAfterDelay()
    {
        Debug.Log("Reviving player after ad.");

        if (loseUI != null)
        {
            loseUI.SetActive(false);
            panel.SetActive(false);
        }

        yield return null; // Wait one frame so ad SDK restores timeScale

        Time.timeScale = 0f;
        
        Prefab[] allPrefabs = GameObject.FindObjectsByType<Prefab>(FindObjectsSortMode.None);

        var targetPrefabs = new System.Collections.Generic.List<Prefab>();
        foreach (Prefab p in allPrefabs)
        {
            int index = Game_System.Instance.spawnObjects.IndexOf(p.originPrefab);
            if (index >= 0 && index <= 2)
                targetPrefabs.Add(p);
        }

        int prefabCount = targetPrefabs.Count;

        // Logarithmic scaling: grows slower as count increases
        // Base case: 5 prefabs = 2 sec
        // Formula: 2f + log10(prefabCount / 5f + 1) * scale
        float scaleFactor = 2f; // How fast it grows
        float totalTime = 2f + Mathf.Log10(prefabCount / 5f + 1f) * scaleFactor;

        // Ensure at least 0.05 sec delay per prefab
        float delayPerPrefab = Mathf.Max(0.05f, totalTime / Mathf.Max(1, prefabCount));

        Debug.Log($"Removing {prefabCount} prefabs over {totalTime:F2} seconds (delay {delayPerPrefab:F2}s each).");

        foreach (Prefab p in targetPrefabs)
        {
            Destroy(p.gameObject);
            yield return new WaitForSecondsRealtime(delayPerPrefab);
        }

        Time.timeScale = 1f;

        Debug.Log("Removed all prefabs with index 0–2 and resumed game.");

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
}
