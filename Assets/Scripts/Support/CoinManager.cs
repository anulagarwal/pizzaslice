using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CoinManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] GameObject coin;
    [SerializeField] List<GameObject> spawnedCoins;
    [Header("Attributes")]
    [SerializeField] int startCoins;
    [SerializeField] int currentCoins;

    [Header("Rewards")]
    [SerializeField] int levelReward;
    public static CoinManager Instance = null;

    private void Awake()
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
        startCoins = PlayerPrefs.GetInt("coins", startCoins);
        currentCoins = startCoins;
        AddCoins(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentCoins()
    {
        return currentCoins;
    }
    #region Level Rewards

    public void AddToLevelReward(int v)
    {
        levelReward += v;
        UIManager.Instance.UpdateLevelReward(levelReward);
    }

    public void MultiplyLevelReward(int v)
    {
        levelReward *= v;
        UIManager.Instance.UpdateLevelReward(levelReward);
    }

    #endregion

    public GameObject SpawnCoin(Vector3 pos)
    {
        GameObject g = Instantiate(coin, pos, Quaternion.identity);
        spawnedCoins.Add(g);
        return g;
    }

    public void RemoveCoin(GameObject g)
    {
        spawnedCoins.Remove(g);
        Destroy(g);
    }
    #region Coin Getter Setter
    public void AddCoins(int v)
    {
        currentCoins += v;
        PlayerPrefs.SetInt("coins", currentCoins);
        UIManager.Instance.UpdateCurrentCoins(currentCoins);
    }

    public bool SubtractCoins(int v)
    {
        if (currentCoins - v > 0)
        {
            currentCoins -= v;
            PlayerPrefs.SetInt("coins", currentCoins);
            UIManager.Instance.UpdateCurrentCoins(currentCoins);

            return true;
        }
        else
        {
            return false;
        }

    }

    #endregion

}
