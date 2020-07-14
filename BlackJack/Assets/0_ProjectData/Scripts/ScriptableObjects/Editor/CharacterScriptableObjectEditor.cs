using UnityEngine;
using UnityEditor;

namespace CabbageSoft.ScriptableObjects
{
    [CustomEditor(typeof(CharacterScriptableObject))]
    [CanEditMultipleObjects]
    public class CharacterScriptableObjectEditor : Editor
    {
        SerializedProperty characterNameProp;
        SerializedProperty portraitSpriteProp;
        SerializedProperty frontFaceSpriteProp;

        SerializedProperty riskPercentageProp;
        SerializedProperty riskCalcMinValueProp;

        void OnEnable()
        {
            // Setup the SerializedProperties
            characterNameProp = serializedObject.FindProperty("characterName");
            portraitSpriteProp = serializedObject.FindProperty("portraitSprite");
            frontFaceSpriteProp = serializedObject.FindProperty("frontFaceSprite");

            riskPercentageProp = serializedObject.FindProperty("riskPercentage");
            riskCalcMinValueProp = serializedObject.FindProperty("riskCalcMinValue");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // Show the custom GUI controls
            DrawProperties();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperties()
        {
            // Rendering Identity parameters
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(characterNameProp);
            DrawAssetWithPreview(portraitSpriteProp);
            DrawAssetWithPreview(frontFaceSpriteProp);

            // Rendering Statistics parameters
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);

            EditorGUILayout.Slider(riskPercentageProp, 0f, 100f, new GUIContent("Risk Percentage"));
            EditorGUILayout.IntSlider(riskCalcMinValueProp, 0, 20, new GUIContent("Risk Calc. Min Value"));
        }

        private void DrawAssetWithPreview(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property);
            Texture2D texture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
            if (texture != null)
            {
                GUILayout.Label(texture);
                EditorGUILayout.Space();
            }
        }
    }
}
