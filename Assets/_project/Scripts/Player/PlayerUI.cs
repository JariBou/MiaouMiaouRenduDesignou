using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] UIBuffs Buffs;

    [SerializeField] public PlayerStatus player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Set player in PlayerUI");
            return;
        }
        if(Buffs)
            Buffs.Link(player);
    }
}
