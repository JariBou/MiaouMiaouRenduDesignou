using UnityEngine;

//[CreateAssetMenu(fileName = "Alteration", menuName = "ScriptableObjects/Alterable", order = 1)]

[System.Serializable]
public struct S_Alteration
{
    public int ID;
    public AlterationType type;
    public StatType statType;
    public float amount;
    public float timer;
    public string name;
}
