using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MuteRay : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.PlaySound("Infect");
        GameController.G.TextView("Можно использовать только верхние подсвеченные лучи");
    }
}
