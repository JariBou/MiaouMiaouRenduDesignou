using _project.Scripts.Alterable;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Player
{
    public class PlayerStatus : MonoBehaviour, IAlterable
    {
        [FormerlySerializedAs("NumAlterationsMax"),SerializeField] private int _numAlterationsMax = 10;

        // DEBUG
        [FormerlySerializedAs("debugAlteration"),SerializeField] private Alteration _debugAlteration;
        [FormerlySerializedAs("DebugTime")] public float debugTime = 2.0f;
        private readonly Alterable.Alterable alterables = new();
        private float clock;


        // Update is called once per frame
        private void Update()
        {
            if (debugTime > 0)
            {
                clock += Time.deltaTime;
                if (clock >= 2.0f)
                {
                    clock = 0;
                    DebugAlterations();
                }
            }
        }

        public void ReceiveAlteration(Alteration arg)
        {
            if (alterables.NumAlterations() >= _numAlterationsMax) return;

            alterables.AddModifier(arg);
        }

        internal int GetFinalStat(int inStat, StatType type)
        {
            int finalStat = 0;
            Debug.Log("Player " + type + " calculating... ");
            finalStat = (int)Mathf.Floor(alterables.GetFinalValue(inStat, type));
            Debug.Log("Player " + type + " was " + inStat + " now is " + finalStat + " after calculations");
            return finalStat;
        }

        public Alterable.Alterable GetAlterables()
        {
            return alterables;
        }

        private void DebugAlterations()
        {
            ReceiveAlteration(_debugAlteration);
        }
    }
}