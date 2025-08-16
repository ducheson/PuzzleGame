using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_System : MonoBehaviour
{
    public static Game_System Instance;

    public List<GameObject> spawnObjects = new List<GameObject>();
    public Transform spawnPoint;
    public float spawnCooldown = 0.5f;
    private bool canSpawn = true;
    private float randomRotation;

    private GameObject currentPrefab;
    private GameObject nextPrefab;

    private Point_System pointSystem;

    private Preview preview;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        pointSystem = Point_System.Instance;
        preview = Preview.FindAnyObjectByType<Preview>();

        GenerateNextPrefab();
        GenerateNextPrefab();
    }

    public void Spawn()
    {
        if (!canSpawn || spawnPoint == null)
        {
            return;
        }

        GameObject instance = Instantiate(currentPrefab, spawnPoint.position, Quaternion.identity);
        instance.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        instance.tag = "Prefab";

        Prefab prefabScript = instance.GetComponent<Prefab>();
        if (prefabScript != null)
            prefabScript.originPrefab = currentPrefab;

        GenerateNextPrefab();
        StartCoroutine(SpawnCooldown());
    }

    private void GenerateNextPrefab()
    {
        if (spawnObjects.Count == 0)
        {
            Debug.LogWarning("Spawn objects list is empty!");
            return;
        }

        currentPrefab = nextPrefab;

        int maxIndex = Mathf.Min(4, spawnObjects.Count);
        int randomIndex = UnityEngine.Random.Range(0, maxIndex);
        nextPrefab = spawnObjects[randomIndex];

        randomRotation = UnityEngine.Random.Range(0f, 360f);
        preview.CreateCurrentPreviewInstance(currentPrefab, randomRotation);
    }

    private IEnumerator SpawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
    }

    public GameObject SpawnNext(int index, Vector3 position)
    {
        if (spawnObjects.Count == 0 || index < 0 || index >= spawnObjects.Count)
        {
            string reason = spawnObjects.Count == 0 ? "spawnObjects list is empty" : $"invalid index ({index})";
            Debug.LogWarning($"Cannot SpawnNext: {reason}");
            return null;
        }

        float randomMergeRotation = UnityEngine.Random.Range(0f, 360f);
        GameObject instance = Instantiate(spawnObjects[index], position, Quaternion.identity);
        instance.transform.rotation = Quaternion.Euler(0f, 0f, randomMergeRotation);

        instance.tag = "Prefab";

        Prefab prefabScript = instance.GetComponent<Prefab>();
        if (prefabScript != null)
            prefabScript.originPrefab = spawnObjects[index];

        pointSystem.UpdatePoint(index);

        return instance;
    }
    
    public void ResetGame()
    {
        // Find all objects with the tag "Prefab" and destroy them
        GameObject[] existingFruits = GameObject.FindGameObjectsWithTag("Prefab");
        foreach (GameObject fruit in existingFruits)
        {
            Destroy(fruit);
        }

        // Optionally, reset current and next prefabs
        currentPrefab = null;
        nextPrefab = null;

        // Regenerate next prefab for preview
        GenerateNextPrefab();
        GenerateNextPrefab();
    }
}