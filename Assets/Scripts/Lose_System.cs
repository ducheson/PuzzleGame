using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lose_System : MonoBehaviour
{
    public GameObject loseUI;
    public GameObject panel; // Fade panel
    public float loseDelay = 1.5f;

    private Collider2D zoneCollider;
    private Dictionary<GameObject, float> insideTimers = new Dictionary<GameObject, float>();
    private HashSet<GameObject> hasLost = new HashSet<GameObject>();

    void Start()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider == null)
        {
            Debug.LogError("Lose_System: No Collider2D found on this GameObject!");
        }
        else if (!zoneCollider.isTrigger)
        {
            Debug.LogWarning("Lose_System: Zone collider should have 'Is Trigger' checked.");
        }

        if (panel != null)
            panel.SetActive(false); // Hide until lose
        if (loseUI != null)
            loseUI.SetActive(false); // Hide until after fade
    }

    void Update()
    {
        List<GameObject> currentlyInside = new List<GameObject>();

        ContactFilter2D filter = new ContactFilter2D { useTriggers = true };

        Collider2D[] results = new Collider2D[50];
        int count = zoneCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            Collider2D col = results[i];
            if (col != null && col.CompareTag("Prefab"))
            {
                GameObject prefabInstance = col.gameObject;

                if (!currentlyInside.Contains(prefabInstance))
                {
                    currentlyInside.Add(prefabInstance);

                    if (!insideTimers.ContainsKey(prefabInstance))
                        insideTimers[prefabInstance] = 0f;

                    if (!hasLost.Contains(prefabInstance))
                    {
                        insideTimers[prefabInstance] += Time.deltaTime;
                        if (insideTimers[prefabInstance] >= loseDelay)
                        {
                            PlayerLose(prefabInstance);
                        }
                    }
                }
            }
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (var tracked in insideTimers.Keys)
        {
            if (!currentlyInside.Contains(tracked))
                toRemove.Add(tracked);
        }
        foreach (var obj in toRemove)
            insideTimers.Remove(obj);
    }

    void PlayerLose(GameObject obj)
    {
        hasLost.Add(obj);
        StartCoroutine(SlowDownAndFade());
    }

    private IEnumerator SlowDownAndFade()
    {
        CanvasGroup cg = null;
        if (panel != null)
        {
            panel.SetActive(true); // Activate now
            cg = panel.GetComponent<CanvasGroup>();
            if (cg == null) cg = panel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }

        float duration = 2f;
        float elapsed = 0f;
        float startScale = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Slow time from 1 → 0
            Time.timeScale = Mathf.Lerp(startScale, 0f, t);

            // Fade alpha from 0 → 1
            if (cg != null)
                cg.alpha = t;

            yield return null;
        }

        // Ensure fully faded & time stopped
        Time.timeScale = 0f;
        if (cg != null) cg.alpha = 1f;

        // Show loseUI AFTER fade is done
        if (loseUI != null)
            loseUI.SetActive(true);
    }
}
