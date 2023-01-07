using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] List<Transform> points;
    [SerializeField] public Vector3 origScale;


    [Header("Component References")]
    [SerializeField] public List<Transform> food;
    [SerializeField] public Transform boxTop;
    [SerializeField] public List<Transform> coinStack;
    [SerializeField] public List<Vector3> coinPos;
    [SerializeField] public Transform box;






    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform t in coinStack)
        {
            coinPos.Add(t.localPosition);
            
        }
        origScale = box.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFood(Transform t)
    {
        int currentLevel = food.Count;
        int i = 0;
        if (currentLevel >= points.Count)
        {
            int newId = currentLevel % points.Count;
            if (newId == 0)
            {
                newId = 0;
            }
            i = newId;
        }
        else
        {
            i = currentLevel;
        }

        t.transform.parent = points[i];

        food.Add(t);
    }

    
    public Vector3 GetPosition()
    {
        int currentLevel = food.Count;
        int i = 0;
        if (currentLevel >= points.Count)
        {
            int newId = currentLevel % points.Count;
            if (newId == 0)
            {
                newId = 0;
            }
            i = newId;
        }
        else
        {
            i = currentLevel;
        }


        Vector3 z = Vector3.zero;
        z = points[i].position;

        return z;
    }

    public void Sanitize()
    {
        foreach(Transform t in food)
        {
            Destroy(t.gameObject);
        }
        food.Clear();
        foreach (Transform t in coinStack)
        {
            t.localScale = Vector3.zero;
            t.localPosition = coinPos[coinStack.FindIndex(x=>x==t)];
            
        }
        box.localScale = origScale;
    }
}
