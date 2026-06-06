using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private Alterable _Alterables;
    public float BaseAttack = 1f;
    public float BaseDefense = 10;
    [SerializeField] int NumAlterationsMax = 10;

    private float AlteredAttack = 0.0f;
    private float AlteredDefense = 0;


    // DEBUG
    [SerializeField] Alteration debugAlteration;
    float clock = 0;
    public float DebugTime = 2.0f;


    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock >= 2.0f)
        {
            clock = 0;
            DebugAlterations();
        }
    }

    private float GetFinalAttack(StatType type)
    {
        Debug.Log("Player attack calculating... ");
        AlteredAttack = _Alterables.GetFinalValue(BaseAttack, type);
        Debug.Log("Player attack was " + BaseAttack + " now is " + AlteredAttack + " after calculations");
        return AlteredAttack;
    }

    private float GetFinalDefense(StatType type)
    {
        Debug.Log("Player defense calculating... ");
        AlteredDefense = _Alterables.GetFinalValue(BaseDefense, type);
        Debug.Log("Player defense was " + BaseDefense + " now is " + Mathf.Floor(AlteredDefense) + " after calculations");
        return Mathf.Floor(AlteredDefense);
    }


    public void AddAlteration(Alteration arg)
    {
        if (_Alterables.NumAlterations() >= NumAlterationsMax)
            return;
        _Alterables.AddModifier(arg);
    }

    public Alterable getAlterables()
    {
        return _Alterables;
    }

    private void DebugAlterations()
    {
        if(debugAlteration != null)
        {
            AddAlteration(debugAlteration);
        }
    }
}
