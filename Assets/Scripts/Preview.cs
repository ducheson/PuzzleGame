using System.Collections;
using UnityEngine;

public class Preview : MonoBehaviour
{
    private GameObject previewInstance;
    public Transform spawnPoint;

    public void CreateCurrentPreviewInstance(GameObject currentPrefab, float randomRotation)
    {
        if (spawnPoint == null || currentPrefab == null)
            return;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(currentPrefab, spawnPoint.position, Quaternion.identity);

        previewInstance.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        Vector3 fixedPosition = previewInstance.transform.position;
        fixedPosition.z = 0;
        previewInstance.transform.position = fixedPosition;

        previewInstance.transform.SetParent(spawnPoint);

        Vector3 targetScale = previewInstance.transform.localScale;
        previewInstance.transform.localScale = Vector3.zero;

        StartCoroutine(AnimateScale(previewInstance.transform, targetScale, 0.5f));

        DestroyImmediate(previewInstance.GetComponent<Rigidbody2D>());

        // 🔧 Disable all colliders (including children)
        Collider2D[] colliders = previewInstance.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        previewInstance.GetComponent<Prefab>().enabled = false;
    }

    private IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            if (target == null) yield break; // ✅ Exit if target was destroyed

            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            target.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        if (target != null) // ✅ Check again before final assignment
            target.localScale = targetScale;
    }
}
