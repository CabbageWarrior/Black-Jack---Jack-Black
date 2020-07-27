using UnityEngine;
using CabbageSoft.BlackJack.DeckManagement;

namespace CabbageSoft.BlackJack
{
    public class InputManager : MonoBehaviour
    {
        public enum InputLayerMask
        {
            Deck = 1 << 10,
            Card = 1 << 11
        }

        void Update()
        {
            if (GameManager.instance == null) return;
            if (GameManager.instance.DataManager == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray rayFromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayFromCamera, out RaycastHit hitFromCamera, 100, (int)InputLayerMask.Card))
                {
                    //Debug.Log("Card hit!");

                    Card hitCard = hitFromCamera.transform.GetComponent<Card>();
                    if (hitCard.IsDraggable)
                    {
                        hitCard.ClickEvent();
                        return;
                    }
                }

                if (Physics.Raycast(rayFromCamera, out _, 100, (int)InputLayerMask.Deck))
                {
                    //Debug.Log("Deck hit!");

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
