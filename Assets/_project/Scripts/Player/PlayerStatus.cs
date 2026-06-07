using _project.Scripts.Player;
using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, I_Alterable
{
    private Alterable _Alterables = new Alterable();
    [Header("Alterations")]
    [SerializeField] int NumAlterationsMax = 10;

    // DEBUG
    [Header("Debug")]
    [SerializeField] S_Alteration debugAlteration;
    float clock = 0;
    public float DebugTime = 2.0f;
    [SerializeField] bool doRandomDebugValues;

    // Update is called once per frame
    void Update()
    {
        if(DebugTime > 0)
        {
            clock += Time.deltaTime;
            if (clock >= 2.0f)
            {
                clock = 0;
                DebugAlterations();
            }
        }
    }

    internal int GetFinalStat(int inStat, StatType type)
    {
        int finalStat = 0;
        Debug.Log("Player " + type.ToString() + " calculating... ");
        finalStat = (int)Mathf.Floor(_Alterables.GetFinalValue(inStat, type));
        Debug.Log("Player " + type.ToString() + " was " + inStat + " now is " + finalStat + " after calculations");
        return finalStat;
    }

    public Alterable getAlterables()
    {
        return _Alterables;
    }

    private void DebugAlterations()
    {
        ReceiveAlteration(debugAlteration);
    }

    public void ReceiveAlteration(S_Alteration arg)
    {
        if (doRandomDebugValues)
            arg.amount = UnityEngine.Random.Range(-5, 5);
        if (_Alterables.NumAlterations() >= NumAlterationsMax)
            return;
        _Alterables.AddModifier(arg);
    }
}
