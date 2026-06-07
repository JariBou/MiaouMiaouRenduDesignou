using System.Collections.Generic;
using _project.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Alterable
{
    public class UIBuffs : MonoBehaviour
    {
        [FormerlySerializedAs("prefab"),SerializeField] private Buff _prefab;
        private PlayerStatus player;

        private readonly List<Buff> children = new();

        private void OnDisable()
        {
            player.GetAlterables().onAddAlteration -= AddUIBuff;
            player.GetAlterables().onRemoveAlteration -= RemoveUIBuff;
        }

        public bool Link(PlayerStatus playerStatus)
        {
            Debug.Log("Linking UI with player");
            if (playerStatus == null) return false;

            Debug.Log("Link valid");
            this.player = playerStatus;

            this.player.GetAlterables().onAddAlteration += AddUIBuff;
            this.player.GetAlterables().onRemoveAlteration += RemoveUIBuff;

            return true;
        }

        private void AddUIBuff(Alteration arg0)
        {
            Debug.Log("Add buff to UI");
            Buff tmp = Instantiate(_prefab, transform);
            tmp.LinkedAlteration(arg0);
            children.Add(tmp);
        }

        private void RemoveUIBuff(Alteration arg0)
        {
            Debug.Log("Remove buff to UI");
            Buff child = children.Find(n => n.GetID() == arg0.ID);
            if (child == null) return;

            children.Remove(child);
            Destroy(child.gameObject);
        }
    }
}