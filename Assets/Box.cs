using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Box : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] List<Transform> points;
    [SerializeField] public Vector3 origScale;


    [Header("Component References")]
    [SerializeField] public List<Transform> food;
    [SerializeField] public TextMeshPro sign;







    // Start is called before the first frame update
    void Start()
    {       
        UpdateSign();
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
    public void UpdateSign()
    {
        sign.text = LevelManager.Instance.GetRemaining()+"";
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
        z = new Vector3(z.x, z.y + GridManager.Instance.baseYOffset, z.z);
        return z;
    }

    public void Sanitize()
    {
        foreach(Transform t in food)
        {
            Destroy(t.gameObject);
        }
        food.Clear();
        UpdateSign();
    }
}
