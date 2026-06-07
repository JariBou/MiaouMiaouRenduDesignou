using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Alterable 
{
    List<S_Alteration> Alterations = new List<S_Alteration>();
    CancellationTokenSource Source;

    public UnityAction<S_Alteration> OnAddAlteration,OnRemoveAlteration;
    int counter = 1;

    public void SetUpNewSource()
    {
        Source = new CancellationTokenSource();
    }
    public int NumAlterations()
    {
        return Alterations.Count;
    }
    public void AddModifier(S_Alteration InAlteration)
    {
        if (!IsValidAlteration(InAlteration))
        {
            Debug.LogWarning("Invalid Alteration");
            return;
        }
        counter += 1;
        InAlteration.ID = counter;
        if(Alterations.Exists(alteration => alteration.ID == InAlteration.ID))
        {
            Debug.Log("Modifier with id " + InAlteration.ID + " already exists");
            return;
        }
        Debug.Log("Adding alteration: " + InAlteration);
        Alterations.Add(InAlteration);
        if(InAlteration.timer > 0)
        {
            Debug.Log("With timer of: " + InAlteration.timer);
            if (Source ==  null || Source.IsCancellationRequested)
                SetUpNewSource();
            CancellationToken cancellationToken = Source.Token;
            _ = ModifierTimerAsync(InAlteration, cancellationToken);
        }
        OnAddAlteration?.Invoke(InAlteration);
    }

    private bool IsValidAlteration(S_Alteration inAlteration)
    {
        if(inAlteration.ID < 0) return false;
        return true;
    }

    async Awaitable ModifierTimerAsync(S_Alteration alteration, CancellationToken inToken)
    {
        await Awaitable.WaitForSecondsAsync(alteration.timer, inToken);
        Debug.Log("Timer done");
        if(Alterations.Exists(item_alteration => item_alteration.ID == alteration.ID))
        {
            RemoveModifier(alteration);
        }
        
    }
    
    public void CancelAll()
    {
        Source.Cancel();
        Source.Dispose();
    }

    public void RemoveModifier(S_Alteration InAlteration)
    {
        Alterations.Remove(InAlteration);
        OnRemoveAlteration?.Invoke(InAlteration);
    }

    public void RemoveLastModifier()
    {
        OnRemoveAlteration?.Invoke(Alterations.Last());
        Alterations.Remove(Alterations.Last());
    }

    public void RemoveRandomModifier()
    {
        int i = UnityEngine.Random.Range(0, Alterations.Count);
        OnRemoveAlteration?.Invoke(Alterations[i]);
        Alterations.RemoveAt(i);
    }


    public float GetFinalValue(float inValue, StatType type)
    {
        foreach (var item in Alterations.Where(n => n.statType == type))
        {
            switch (item.type)
            {
                case AlterationType.Addition:
                    inValue = AdditionAlteration(inValue, item.amount);
                    break;
                case AlterationType.Multiplication:
                    inValue = MultiplicationAlteration(inValue, item.amount);
                    break;
                default:
                    Debug.Log("Modifier with id " + item.ID + " doesnt have a proper modifier");
                    break;
            }
        }
        return inValue;
    }

    private float AdditionAlteration(float inValue, float amount)
    {
        float result = inValue + amount;
        Debug.Log("Addition: " + inValue + " + " + amount + " = " + result);
        return result;
    }

    private float MultiplicationAlteration(float inValue, float amount)
    {
        float result = inValue * amount;
        Debug.Log("Multiplication: " + inValue + " * " + amount + " = " + result);
        return result;
    }
}
public enum AlterationType
{
    Addition,
    Multiplication,
}

public enum StatType
{
    Attack,
    Def,
}