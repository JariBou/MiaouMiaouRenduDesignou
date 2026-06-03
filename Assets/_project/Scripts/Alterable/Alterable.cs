using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Alterable 
{
    List<Alteration> Alterations;
    CancellationTokenSource Source;
    public void SetUpNewSource()
    {
        Source = new CancellationTokenSource();
    }

    public void AddModifier(Alteration InAlteration)
    {
        if(Alterations.Exists(alteration => alteration.ID == InAlteration.ID))
        {
            Debug.Log("Modifier with id " + InAlteration.ID + " already exists");
            return;
        }
        Alterations.Add(InAlteration);
        if(InAlteration.timer > 0)
        {
            if (Source ==  null || Source.IsCancellationRequested)
                SetUpNewSource();
            CancellationToken cancellationToken = Source.Token;
            _ = ModifierTimerAsync(InAlteration, cancellationToken);
        }
    }

    async Awaitable ModifierTimerAsync(Alteration alteration, CancellationToken inToken)
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

    public void RemoveModifier(Alteration InAlteration)
    {
        Alterations.Remove(InAlteration);
    }

    public void RemoveLastXModifier()
    {
        Alterations.Remove(Alterations.Last());
    }

    public void RemoveRandomModifier()
    {
        int i = UnityEngine.Random.Range(0, Alterations.Count);
        Alterations.RemoveAt(i);
    }


    public float GetFinalValue(float inValue)
    {
        foreach (var item in Alterations)
        {
            switch (item.Type)
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
        return inValue + amount;
    }

    private float MultiplicationAlteration(float inValue, float amount)
    {
        return inValue * amount;
    }
}

public struct Alteration
{
    public float ID;
    public AlterationType Type;
    public float amount;
    public float timer;
}

public enum AlterationType
{
    Addition,
    Multiplication,
}