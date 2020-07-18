using UnityEditor;
using UnityEngine;

namespace CabbageSoft.Utilities.Drawers
{
    [CustomPropertyDrawer(typeof(SpriteWithPreview))]
    public class SpriteWithPreviewPropertyDrawer : PropertyDrawer
    {
        // Sprite
        private float spritePropertyHeight = EditorGUIUtility.singleLineHeight;

        // Toggle
        private float onlyTogglePropertyHeight = EditorGUIUtility.singleLineHeight;
        private float toggleOffsetX = 30f;

        // Preview
        private float previewPropertyHeight = 100f;
        private GUIStyle previewStyle = default;
        private Texture2D previewTexture = default;
        private bool isPreviewVisible = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect elementPos;

            if (previewStyle == null)
            {
                previewStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                {
                    alignment = TextAnchor.UpperRight,
                    fixedHeight = previewPropertyHeight
                };
            }

            EditorGUI.BeginProperty(position, label, property);

            // Sprite Property
            elementPos = new Rect(
                position.x,
                position.y,
                position.width,
                spritePropertyHeight
            );
            EditorGUI.PropertyField(elementPos, property);

            // The Toggle and the Preview are shown only if a Preview is available.
            previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
            if (previewTexture != null)
            {
                // Toggle
                elementPos = new Rect(
                    position.x + toggleOffsetX,
                    position.y + spritePropertyHeight,
                    position.width - toggleOffsetX,
                    onlyTogglePropertyHeight
                );
                isPreviewVisible = GUI.Toggle(elementPos, isPreviewVisible, "Preview");

                // Preview
                if (isPreviewVisible)
                {
                    elementPos = new Rect(
                        position.x,
                        position.y + spritePropertyHeight,
                        position.width,
                        position.height
                    );
                    GUI.Label(elementPos, previewTexture, previewStyle);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float serializedPropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            float previewAreaPropertyHeight = 0f;
            if (previewTexture != null)
            {
                if (isPreviewVisible)
                {
                    previewAreaPropertyHeight = previewPropertyHeight;
                }
                else
                {
                    previewAreaPropertyHeight = onlyTogglePropertyHeight;
                }
            }

            return serializedPropertyHeight + previewAreaPropertyHeight;
        }
    }
}