using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textName;

    S_Alteration linkedAlteration;

    internal void LinkedAlteration(S_Alteration arg0)
    {
        linkedAlteration = arg0;
        textName.text = arg0.statType.ToString() + " : " + arg0.amount;
    }

    public int GetID()
    {
       // Debug.Log("GetID: " + linkedAlteration.ID);
        return linkedAlteration.ID;
    }
}

