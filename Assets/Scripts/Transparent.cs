using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TransparentBack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private HashSet<Collider2D> overlappingPrefabs = new HashSet<Collider2D>();
    private Collider2D myCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        myCollider = GetComponent<Collider2D>();
        myCollider.isTrigger = true;

        StartCoroutine(DelayedInitialCheck());
    }

    IEnumerator DelayedInitialCheck()
    {
        // Wait one frame so Unity can resolve physics properly
        yield return null;

        Collider2D[] results = new Collider2D[10];
        int count = Physics2D.OverlapCollider(myCollider, new ContactFilter2D().NoFilter(), results);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = results[i];
            if (hit != null && hit != myCollider && hit.CompareTag("Prefab"))
            {
                overlappingPrefabs.Add(hit);
            }
        }

        if (overlappingPrefabs.Count > 0)
        {
            SetTransparent();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Prefab"))
        {
            overlappingPrefabs.Add(other);
            if (overlappingPrefabs.Count == 1)
            {
                SetTransparent();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Prefab"))
        {
            overlappingPrefabs.Remove(other);
            if (overlappingPrefabs.Count == 0)
            {
                SetOriginalColor();
            }
        }
    }

    private void SetTransparent()
    {
        Color transparentColor = originalColor;
        transparentColor.a = 0.6f;
        spriteRenderer.color = transparentColor;
    }

    private void SetOriginalColor()
    {
        spriteRenderer.color = originalColor;
    }
}
