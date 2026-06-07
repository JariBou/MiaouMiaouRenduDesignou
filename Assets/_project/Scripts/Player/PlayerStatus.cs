using _project.Scripts.Player;
using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, I_Alterable
{
    private Alterable _Alterables = new Alterable();
    [SerializeField] int NumAlterationsMax = 10;

    // DEBUG
    [SerializeField] S_Alteration debugAlteration;
    float clock = 0;
    public float DebugTime = 2.0f;


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
        if (_Alterables.NumAlterations() >= NumAlterationsMax)
            return;
        _Alterables.AddModifier(arg);
    }
}
