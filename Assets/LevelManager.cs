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
        UIManager.Instance.UpdateObjective(HexType.A, 0);
        UIManager.Instance.UpdateObjective(HexType.B, 0);
        UIManager.Instance.UpdateObjective(HexType.C, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(HexType ht)
    {
        switch (ht)
        {
            case HexType.A:
                currentPizza++;
                break;

            case HexType.B:
                currentBurgers++;
                break;

            case HexType.C:
                currentDonut++;
                break;
        }

        UIManager.Instance.UpdateObjective(HexType.A, (float)currentPizza/(float)maxPizza);
        UIManager.Instance.UpdateObjective(HexType.B, (float)currentDonut / (float)maxDonut);
        UIManager.Instance.UpdateObjective(HexType.C, (float)currentBurgers / (float)maxBurgers);

        //Send Checkmark on UI for all completed
        //Also show text to show remaining items to be filled
        if(currentPizza>= maxPizza)
        {

        }

        if(currentDonut>=maxDonut)
        {

        }
        if (currentBurgers >= maxBurgers)
        {

        }

        if(currentBurgers >= maxBurgers && currentDonut >= maxDonut && currentPizza >= maxPizza)
        {
            GameManager.Instance.WinLevel();            
            //Disable all inputs
        }
    }
}
