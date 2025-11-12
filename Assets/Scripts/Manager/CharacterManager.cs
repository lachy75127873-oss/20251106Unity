using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager instance;

    public static CharacterManager Instance
    {
        get
        {
            if (instance == null) instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();
            
            return instance;
        }
    }
    
    public Player Player{get;set;}
    public System.Action<Player> OnPlayerReady; // awake 순서 정렬용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    
    public void RegisterPlayer(Player player)
    {
        Player = player;
        OnPlayerReady?.Invoke(player);
    }
    
}
