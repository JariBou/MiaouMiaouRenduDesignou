using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textShown;

    S_Alteration linkedAlteration;

    internal void LinkedAlteration(S_Alteration arg0)
    {
        linkedAlteration = arg0;
        string middle = " ";
        Color color = Color.red;

        if (arg0.amount > 0)
        {
            middle = " +";
            color = Color.green;
        }
        textShown.color = color;
        textShown.text = arg0.statType.ToString() + middle + arg0.amount;
    }

    public int GetID()
    {
       // Debug.Log("GetID: " + linkedAlteration.ID);
        return linkedAlteration.ID;
    }
}

