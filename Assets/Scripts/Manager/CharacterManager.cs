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

    public Player Player
    {
        get { return _player;}
        set {_player = value;}
    }
    private Player _player;

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
}
