using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Powerup
{
    public PowerupType pt;
    public int cost;
}
public class PowerupManager : MonoBehaviour
{

    public static PowerupManager Instance = null;

    private void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    [Header("Component References")]
    [SerializeField] List<Powerup> powerups;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPowerupCost(PowerupType pt)
    {
        return powerups.Find(x => x.pt == pt).cost;
    }

    public void UsePowerup(PowerupType pt)
    {
        CoinManager.Instance.SubtractCoins(powerups.Find(x => x.pt == pt).cost);
    }
}
