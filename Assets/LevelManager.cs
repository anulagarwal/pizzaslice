using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] int maxBurgers;
    [SerializeField] int currentBurgers;

    [SerializeField] int maxPizza;
    [SerializeField] int currentPizza;

    [SerializeField] int maxDonut;
    [SerializeField] int currentDonut;

    public static LevelManager Instance = null;


    void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItems(HexType ht)
    {
        switch (ht)
        {

        }
    }
}
