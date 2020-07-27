using UnityEngine;
using CabbageSoft.BlackJack.DeckManagement;

namespace CabbageSoft.BlackJack
{
    public class InputManager : MonoBehaviour
    {
        /// <summary>
        /// Input layer mask.
        /// </summary>
        public enum InputLayerMask
        {
            Deck = 1 << 10,
            Card = 1 << 11
        }

        /// <inheritdoc/>
        void Update()
        {
            if (GameManager.instance == null) return;
            if (GameManager.instance.DataManager == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray rayFromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Checking Card click.
                if (Physics.Raycast(rayFromCamera, out RaycastHit hitFromCamera, 100, (int)InputLayerMask.Card))
                {
                    Card hitCard = hitFromCamera.transform.GetComponent<Card>();
                    if (hitCard.IsDraggable)
                    {
                        hitCard.ClickEvent();
                        return;
                    }
                }

                // Checking Deck click.
                if (Physics.Raycast(rayFromCamera, out _, 100, (int)InputLayerMask.Deck))
                {
                    if (GameManager.StaticDeckRef != null)
                    {
                        GameManager.StaticDeckRef.ClickEvent();
                    }
                    return;
                }
            }
        }
    }
}
