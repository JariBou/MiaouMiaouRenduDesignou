using System;

//[CreateAssetMenu(fileName = "Alteration", menuName = "ScriptableObjects/Alterable", order = 1)]

namespace _project.Scripts.Alterables
{
    [Serializable]
    public struct Alteration : IEquatable<Alteration>
    {
        public int ID;
        public AlterationType type;
        public StatType statType;
        public float amount;
        public float timer;
        public string name;

        public bool Equals(Alteration other)
        {
            return ID == other.ID && type == other.type && statType == other.statType && amount.Equals(other.amount) && timer.Equals(other.timer) &&
                   name == other.name;
        }

        public override bool Equals(object obj)
        {
            return obj is Alteration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, (int)type, (int)statType, amount, timer, name);
        }
    }
}