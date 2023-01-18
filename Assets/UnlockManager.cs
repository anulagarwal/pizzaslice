using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Unlock
{
    public Image unlockImage;
    public UnlockType ut;
    public string name;
}

public class UnlockManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] int unlockIndex;


    [Header("Component References")]
    [SerializeField] List<Unlock> unlocks;

    public static UnlockManager Instance = null;


    void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        //cells = new Tile[height * width];

    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void UpdateUnlock(Unlock u)
    {

    }
}
