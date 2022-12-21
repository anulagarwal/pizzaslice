using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexColor
{
    public HexType ht;
    public Color col;
}
public class ColorManager : MonoBehaviour
{

    [SerializeField] List<HexColor> hc;
    public static ColorManager Instance = null;


    void Awake()
    {
        Application.targetFrameRate = 100;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

    }


   public Color GetHexColor(HexType h)
    {
        return hc.Find(x => x.ht == h).col;
    }
}
