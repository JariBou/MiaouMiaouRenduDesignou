using _project.Scripts.Alterables;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [FormerlySerializedAs("Buffs"), SerializeField]
        private UIBuffs _buffs;

        [FormerlySerializedAs("player"), SerializeField]
        private PlayerStatus _player;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (_player == null)
            {
                Debug.LogWarning("Set player in PlayerUI");
                return;
            }

            if (_buffs) _buffs.Link(_player);
        }
    }
}