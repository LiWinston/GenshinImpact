// using UI;
// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(IconManager))]
// public class IconManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         IconManager iconManager = (IconManager)target;
//
//         if (GUILayout.Button("Show Icon Dictionary"))
//         {
//             foreach (var pair in IconManager.IconDictionary)
//             {
//                 EditorGUILayout.LabelField(pair.Key.ToString(), pair.Value.ToString());
//             }
//         }
//     }
// }