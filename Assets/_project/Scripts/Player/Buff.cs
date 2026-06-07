using _project.Scripts.Alterable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Player
{
    public class Buff : MonoBehaviour
    {
        [FormerlySerializedAs("textName"),SerializeField] private TextMeshProUGUI _textName;

        private Alteration linkedAlteration;

        internal void LinkedAlteration(Alteration arg0)
        {
            linkedAlteration = arg0;
            _textName.text = arg0.statType + " : " + arg0.amount;
        }

        public int GetID()
        {
            // Debug.Log("GetID: " + linkedAlteration.ID);
            return linkedAlteration.ID;
        }
    }
}