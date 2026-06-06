using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textName;
    [SerializeField] Image image;

    Alteration linkedAlteration;

    internal void LinkedAlteration(Alteration arg0)
    {
        linkedAlteration = arg0;
        image.sprite = arg0.icon;
        textName.text = arg0.name;
    }

    public int GetID()
    {
        Debug.Log("GetID: " + linkedAlteration.ID);
        return linkedAlteration.ID;
    }
}

