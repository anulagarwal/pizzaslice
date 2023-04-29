using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]

public class PatternObject
{
    public PatternType pt;
    public GameObject obj;
}
[System.Serializable]

public class TileObject
{
    public TileType t;
    public GameObject g;
}

[System.Serializable]
public class SpawnTile
{
    public PatternType pattern;
    public TileType tile1;
    public TileType tile2;
    public TileType tile3;
}
public class SelectionManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] int maxBox;
    [SerializeField] int spawnIndex;
    [SerializeField] float highlightDelay;
    [SerializeField] float startTime;
    [SerializeField] bool isHighlight;

    [Header("Component References")]
    [SerializeField] List<TileObject> tileObjects;
    [SerializeField] List<PatternObject> PatternObjects;


    [SerializeField] List<GameObject> tiles;
    [SerializeField] List<Transform> boxPoints;
    [SerializeField] public List<GameObject> spawnedTiles;
    List<Tweener> tweens = new List<Tweener>();


    public static SelectionManager Instance = null;


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
        for(int i =0; i< maxBox; i++)
        {
            Spawn(boxPoints.Find(x => x.childCount == 1));
        }
        isHighlight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTime + highlightDelay < Time.time && !isHighlight)
        {

            isHighlight = true;


            if (CheckForSpace(boxPoints[0].GetChild(1).gameObject))
            {
                tweens.Add(boxPoints[0].GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.green, 0.6f).SetLoops(-1, LoopType.Yoyo));
            }

            if (CheckForSpace(boxPoints[1].GetChild(1).gameObject))
            {
               tweens.Add(boxPoints[1].GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.green, 0.6f).SetLoops(-1, LoopType.Yoyo));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {            
                boxPoints[0].GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                boxPoints[1].GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;            
        }
    }


    public void Spawn(Transform t)
    {
        GameObject g = Instantiate(tiles[spawnIndex], t.position, Quaternion.identity);
        g.transform.SetParent(t);
        spawnedTiles.Add(g);
        spawnIndex++;
        if(spawnIndex == tiles.Count)
        {
            spawnIndex = 0;
        }
        startTime = Time.time;
        isHighlight = false;
        foreach (Tweener tw in tweens)
        {
            tw.Kill();
        }

    }

    public bool CheckForSpace()
    {
        bool canPlace = false;
        foreach(GameObject g in spawnedTiles)
        {
            SelectionTile t = g.GetComponent<SelectionTile>();
            foreach(Tile b in GridManager.Instance.GetCells())
            {
                canPlace = GridManager.Instance.CompareSelectedToEnteredTile(t.primaryTile, b);
                if (canPlace)
                {
                    break;
                }
            }
            if (canPlace)
            {
                break;
            }
        }
        return canPlace;
    }
    public bool CheckForSpace(GameObject g)
    {
        bool canPlace = false;
       
            SelectionTile t = g.GetComponent<SelectionTile>();
            foreach (Tile b in GridManager.Instance.GetCells())
            {
                canPlace = GridManager.Instance.CompareSelectedToEnteredTile(t.primaryTile, b);
                if (canPlace)
                {
                    break;
                }
            }           
        
        return canPlace;
    }
    public void ActiveTiles(bool isActive)
    {
        foreach(GameObject g in spawnedTiles)
        {
            g.SetActive(isActive);
        }
    }

    public void SelectionHighlight(GameObject g)
    {
        boxPoints.Find(x => x.GetChild(1).gameObject == g).GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
    }
    public void DeSelectionHighlight(GameObject g)
    {
        boxPoints.Find(x => x.GetChild(1).gameObject == g).GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void RemoveTile(GameObject g)
    {
        Spawn(boxPoints.Find(x => x.GetChild(1).gameObject == g));
        g.transform.SetParent(null);

        //Destroy(g);
        spawnedTiles.Remove(g);
    }


}
