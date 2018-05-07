using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSlot : MonoBehaviour
{
    public Player player;
    public SpriteRenderer deckHighlighter;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SwitchHighlighterOff();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (player.CurrentState == Player.State.WaitingForCard)
        {
            if (other.CompareTag("ValidCard"))
            {
                Card card = other.GetComponent<Card>();

                if (PlayersManager.currentPlayer == null)
                {
                    PlayersManager.currentPlayer = player;
                }

                if (card.IsDragging)
                {
                    if (PlayersManager.currentPlayer == player)
                    {
                        deckHighlighter.enabled = true;
                    }
                }
                else
                {
                    if (!card.IsUsed && PlayersManager.currentPlayer == player)
                    {
                        player.GetCard(card);
                        other.tag = "Card";

                        PlayersManager.currentPlayer = null;
                    }
                    SwitchHighlighterOff();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ValidCard"))
        {
            SwitchHighlighterOff();

            if (PlayersManager.currentPlayer == player)
            {
                PlayersManager.currentPlayer = null;
            }
        }
    }

    private void SwitchHighlighterOff()
    {
        deckHighlighter.enabled = false;
    }
}
