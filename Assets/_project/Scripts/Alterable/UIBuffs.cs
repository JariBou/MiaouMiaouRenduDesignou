using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBuffs : MonoBehaviour
{
    [SerializeField] Buff prefab;

    List<Buff> children = new List<Buff>();
    private PlayerStatus _player;

    public bool Link(PlayerStatus player)
    {
        Debug.Log("Linking UI with player");
        if(player == null) 
            return false;
        Debug.Log("Link valid");
        _player = player;

        _player.getAlterables().OnAddAlteration += AddUIBuff;
        _player.getAlterables().OnRemoveAlteration += RemoveUIBuff;

        return true;

    }

    private void OnDisable()
    {
        _player.getAlterables().OnAddAlteration -= AddUIBuff;
        _player.getAlterables().OnRemoveAlteration -= RemoveUIBuff;
    }
    private void AddUIBuff(S_Alteration arg0)
    {
        Debug.Log("Add buff to UI");
        var tmp = Instantiate(prefab, transform);
        tmp.LinkedAlteration(arg0);
        children.Add(tmp);

    }

    private void RemoveUIBuff(S_Alteration arg0)
    {
        Debug.Log("Remove buff to UI");
        var child = children.Find(n => n.GetID() == arg0.ID);
        if (child == null)
            return;
        children.Remove(child);
        Destroy(child.gameObject);
    }
}
