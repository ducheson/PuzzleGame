using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PolygonCollider2D))]
public class ShrinkPolygonColliderEditor : Editor
{
    private const float shrinkFactor = 0.975f;
    private const float expandFactor = 1.05f;

    void OnEnable()
    {
        PolygonCollider2D poly = (PolygonCollider2D)target;
        BackupOriginal(poly);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PolygonCollider2D poly = (PolygonCollider2D)target;

        EditorGUILayout.Space();

        // Shrink button
        if (GUILayout.Button("Shrink Collider (−2.5%)"))
        {
            ApplyShrink(poly, shrinkFactor);
        }

        // Expand button
        if (GUILayout.Button("Expand Collider (+5%)"))
        {
            ApplyShrink(poly, expandFactor);
        }

        // Reset button
        if (GUILayout.Button("Reset to Original Collider"))
        {
            ResetCollider(poly);
        }
    }

    void ApplyShrink(PolygonCollider2D poly, float factor)
    {
        Vector2[] original = GetOriginalPoints(poly);
        if (original == null) return;

        Vector2[] result = new Vector2[original.Length];

        for (int i = 0; i < original.Length; i++)
        {
            Vector2 prev = original[(i - 1 + original.Length) % original.Length];
            Vector2 curr = original[i];
            Vector2 next = original[(i + 1) % original.Length];

            Vector2 toPrev = (curr - prev).normalized;
            Vector2 toNext = (next - curr).normalized;

            Vector2 normal1 = new Vector2(toPrev.y, -toPrev.x);
            Vector2 normal2 = new Vector2(toNext.y, -toNext.x);

            Vector2 direction = (normal1 + normal2).normalized;
            if (factor < 1f)
                direction = -direction;

            float offset = Mathf.Abs(1f - factor) * 0.5f;
            result[i] = curr + direction * offset;
        }

        Undo.RecordObject(poly, "Modify Polygon Collider");
        poly.points = result;
        EditorUtility.SetDirty(poly);
    }

    void ResetCollider(PolygonCollider2D poly)
    {
        Vector2[] original = GetOriginalPoints(poly);
        if (original != null)
        {
            Undo.RecordObject(poly, "Reset Polygon Collider");
            poly.points = original;
            EditorUtility.SetDirty(poly);
        }
    }

    void BackupOriginal(PolygonCollider2D poly)
    {
        PolygonColliderBackup backup = poly.GetComponent<PolygonColliderBackup>();
        if (backup == null)
            backup = poly.gameObject.AddComponent<PolygonColliderBackup>();

        if (backup.originalPoints == null || backup.originalPoints.Length == 0)
        {
            backup.originalPoints = poly.points;
            EditorUtility.SetDirty(backup);
        }
    }

    Vector2[] GetOriginalPoints(PolygonCollider2D poly)
    {
        PolygonColliderBackup backup = poly.GetComponent<PolygonColliderBackup>();
        if (backup != null && backup.originalPoints != null && backup.originalPoints.Length > 0)
            return backup.originalPoints;

        Debug.LogWarning("No original collider shape found.");
        return null;
    }
}
