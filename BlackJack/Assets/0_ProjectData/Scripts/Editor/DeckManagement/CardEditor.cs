using UnityEditor;
using UnityEngine;

namespace CabbageSoft.BlackJack.DeckManagement
{
    [CustomEditor(typeof(Card))]
    public class CardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Card myCard = (Card)target;
            if (GUILayout.Button("Show"))
            {
                myCard.Toggle(true);
            }
            if (GUILayout.Button("Hide"))
            {
                myCard.Toggle(false);
            }
        }
    }
}
