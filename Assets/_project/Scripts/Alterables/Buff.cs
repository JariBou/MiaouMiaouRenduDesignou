using TMPro;
using UnityEngine;

namespace _project.Scripts.Alterables
{
    public class Buff : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textShown;

        private Alteration linkedAlteration;

        internal void LinkedAlteration(Alteration arg0)
        {
            linkedAlteration = arg0;
            string middle = " ";
            Color color = Color.red;

            if (arg0.amount > 0)
            {
                middle = " +";
                color = Color.green;
            }

            textShown.color = color;
            textShown.text = arg0.statType + middle + arg0.amount;
        }

        public int GetID()
        {
            // Debug.Log("GetID: " + linkedAlteration.ID);
            return linkedAlteration.ID;
        }
    }
}