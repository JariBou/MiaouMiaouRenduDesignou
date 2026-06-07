using UnityEngine;

[CreateAssetMenu(fileName = "Alteration", menuName = "ScriptableObjects/Alterable", order = 1)]
public class Alteration : ScriptableObject
{
    public int ID = 0;
    public Sprite icon;
    public AlterationType type;
    public StatType statType;
    public float amount;
    public float timer;
}
