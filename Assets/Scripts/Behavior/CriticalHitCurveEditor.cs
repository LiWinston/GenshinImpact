using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CriticalHitCurve))]
public class CriticalHitCurveEditor : Editor
{
    private SerializedProperty curvePointsProperty;
    private CriticalHitCurve curve;

    private void OnEnable()
    {
        curve = (CriticalHitCurve)target;
        curvePointsProperty = serializedObject.FindProperty("curvePoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Critical Hit Curve Points:");

        for (int i = 0; i < curvePointsProperty.arraySize; i++)
        {
            SerializedProperty curvePoint = curvePointsProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(curvePoint.FindPropertyRelative("level"), GUIContent.none);
            EditorGUILayout.PropertyField(curvePoint.FindPropertyRelative("chance"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Point"))
        {
            curve.curvePoints.Add(new CriticalHitCurvePoint());
        }

        if (GUILayout.Button("Remove Last Point") && curve.curvePoints.Count > 0)
        {
            curve.curvePoints.RemoveAt(curve.curvePoints.Count - 1);
        }

        serializedObject.ApplyModifiedProperties();
    }
}