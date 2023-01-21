using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTile : MonoBehaviour
{
    [Header("Component References")]

    [SerializeField] Material outMat;
    [SerializeField] Material inMat;
    [SerializeField] Color highlightColor;
    [SerializeField] Color highlightColorOut;

    [SerializeField] Color originalColorOut;
    [SerializeField] Color originalColorIn;


    private void Start()
    {
        outMat = GetComponent<MeshRenderer>().materials[0];
        inMat = GetComponent<MeshRenderer>().materials[1];
        originalColorOut = outMat.color;
        originalColorIn = inMat.color;

    }

    public void HighLight()
    {
        outMat.color = highlightColorOut;
        inMat.color = highlightColor;
    }

    public void Reset()
    {

        outMat.color = originalColorOut;
        inMat.color = originalColorIn;

    }
}

