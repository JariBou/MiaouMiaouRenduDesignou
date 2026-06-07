using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _project.Scripts.Alterables
{
    [Serializable]
    public class Alterable
    {
        private List<Alteration> alterations = new();
        private int counter = 1;

        public UnityAction<Alteration> onAddAlteration, onRemoveAlteration;
        private CancellationTokenSource source;

        public void SetUpNewSource()
        {
            source = new CancellationTokenSource();
        }

        public int NumAlterations()
        {
            return alterations.Count;
        }

        public void AddModifier(Alteration inAlteration)
        {
            if (!IsValidAlteration(inAlteration))
            {
                Debug.LogWarning("Invalid Alteration");
                return;
            }

            counter += 1;
            inAlteration.ID = counter;
            if (alterations.Exists(alteration => alteration.ID == inAlteration.ID))
            {
                Debug.Log("Modifier with id " + inAlteration.ID + " already exists");
                return;
            }

            Debug.Log("Adding alteration: " + inAlteration);
            alterations.Add(inAlteration);
            if (inAlteration.timer > 0)
            {
                Debug.Log("With timer of: " + inAlteration.timer);
                if (source == null || source.IsCancellationRequested) SetUpNewSource();
                CancellationToken cancellationToken = source.Token;
                _ = ModifierTimerAsync(inAlteration, cancellationToken);
            }

            onAddAlteration?.Invoke(inAlteration);
        }

        private bool IsValidAlteration(Alteration inAlteration)
        {
            if (inAlteration.ID < 0) return false;

            return true;
        }

        private async Awaitable ModifierTimerAsync(Alteration alteration, CancellationToken inToken)
        {
            await Awaitable.WaitForSecondsAsync(alteration.timer, inToken);
            Debug.Log("Timer done");
            if (alterations.Exists(itemAlteration => itemAlteration.ID == alteration.ID)) RemoveModifier(alteration);
        }

        public void CancelAll()
        {
            source.Cancel();
            source.Dispose();
        }

        public void RemoveModifier(Alteration inAlteration)
        {
            alterations.Remove(inAlteration);
            onRemoveAlteration?.Invoke(inAlteration);
        }

        public void RemoveLastModifier()
        {
            onRemoveAlteration?.Invoke(alterations.Last());
            alterations.Remove(alterations.Last());
        }

        public void RemoveRandomModifier()
        {
            int i = Random.Range(0, alterations.Count);
            onRemoveAlteration?.Invoke(alterations[i]);
            alterations.RemoveAt(i);
        }


        public float GetFinalValue(float inValue, StatType type)
        {
            foreach (Alteration item in alterations.Where(n => n.statType == type))
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
}