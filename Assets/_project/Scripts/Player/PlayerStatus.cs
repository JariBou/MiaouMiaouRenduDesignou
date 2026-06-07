using _project.Scripts.Alterables;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Player
{
    public class PlayerStatus : MonoBehaviour, IAlterable
    {
        [FormerlySerializedAs("NumAlterationsMax"), Header("Alterations"), SerializeField] 
        private int _numAlterationsMax = 10;

        // DEBUG
        [FormerlySerializedAs("debugAlteration"), Header("Debug"), SerializeField] 
        private Alteration _debugAlteration;

        [FormerlySerializedAs("debugTime"), SerializeField]
        private float _debugTime = 2.0f;

        [FormerlySerializedAs("doRandomDebugValues"), SerializeField]
        private bool _doRandomDebugValues;

        private readonly Alterable alterables = new();
        private float clock;

        // Update is called once per frame
        private void Update()
        {
            if (_debugTime > 0)
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
            if (_doRandomDebugValues) arg.amount = Random.Range(-5, 5);
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

        public Alterable GetAlterables()
        {
            return alterables;
        }

        private void DebugAlterations()
        {
            ReceiveAlteration(_debugAlteration);
        }
    }
}