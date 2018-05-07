using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Ray rayFromCamera;
            RaycastHit hitFromCamera;

            rayFromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayFromCamera, out hitFromCamera, 100, (int)InputLayerMask.Card))
            {
                //Debug.Log("Card hit!");

                if (hitFromCamera.transform.GetComponent<Card>().IsDraggable)
                {
                    hitFromCamera.transform.GetComponent<Card>().ClickEvent();
                    return;
                }
            }

            if (Physics.Raycast(rayFromCamera, out hitFromCamera, 100, (int)InputLayerMask.Deck))
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
