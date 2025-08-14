using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rigidbody2D))]
public class Rigidbody2DMassEditor : Editor
{
    private bool autoMassEnabled = true;
    private float baseDensity = 1.0f;

    public override void OnInspectorGUI()
    {
        Rigidbody2D rb = (Rigidbody2D)target;

        // Auto-mass calculator UI
        EditorGUILayout.LabelField("Custom Mass Calculator", EditorStyles.boldLabel);
        autoMassEnabled = EditorGUILayout.Toggle("Auto-Calculated Mass", autoMassEnabled);

        if (autoMassEnabled)
        {
            baseDensity = EditorGUILayout.FloatField("Base Density", baseDensity);

            Transform t = rb.transform;
            float adjustedArea = 4f * t.localScale.x * t.localScale.y;
            float calculatedMass = baseDensity * adjustedArea;

            EditorGUILayout.LabelField("Calculated Mass", calculatedMass.ToString("F3"));

            if (GUILayout.Button("Apply Calculated Mass"))
            {
                Undo.RecordObject(rb, "Set Calculated Mass");
                rb.mass = calculatedMass;
                EditorUtility.SetDirty(rb);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rigidbody2D Built-in Properties", EditorStyles.boldLabel);

        // Use DrawDefaultInspector instead of accessing internal names
        DrawDefaultInspector(); // Avoids relying on internal SerializedProperty names
    }
}