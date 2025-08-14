using UnityEngine;

public class Prefab : MonoBehaviour
{
    [HideInInspector]
    public GameObject originPrefab;

    private bool hasMerged = false;

    private void OnEnable()
    {
        hasMerged = false; // Reset on reuse/spawn
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMerged) return;

        Prefab otherPrefab = collision.gameObject.GetComponent<Prefab>();
        if (otherPrefab == null || otherPrefab.hasMerged) return;

        if (otherPrefab.originPrefab != originPrefab || GetInstanceID() >= otherPrefab.GetInstanceID())
            return;

        hasMerged = true;
        otherPrefab.hasMerged = true;

        Vector3 midpoint = (transform.position + otherPrefab.transform.position) / 2f;

        Destroy(gameObject);
        Destroy(otherPrefab.gameObject);

        if (Game_System.Instance != null)
        {
            int currentIndex = Game_System.Instance.spawnObjects.IndexOf(originPrefab);
            int nextIndex = currentIndex + 1;

            Game_System.Instance.SpawnNext(nextIndex, midpoint);
        }
    }
}
