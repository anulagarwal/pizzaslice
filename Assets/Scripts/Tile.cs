using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
[System.Serializable]
public class Food
{
    public Transform item;
    public HexType ht;
}

[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] int maxHex;
    public HexCoordinates coordinates;
    public bool canPlace = false;
    public bool isHex = false;


    [SerializeField] TileType state;
    [SerializeField] public HexType hexType;
    public Vector3 origPos;
    public Color origColor;



    [Header("Component References")]
    [SerializeField] public List<Tile> hexes;
    [SerializeField] public List<Tile> neighbors;
    [SerializeField] SpriteRenderer hexSprite;
    [SerializeField] MeshRenderer hexMesh;
    [SerializeField] GameObject baseHex = null;






    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
        if(hexMesh!=null)
        origColor = hexMesh.material.color;
        if (baseHex != null && !isHex)
        {
            baseHex.SetActive(false);
        }
        else if(baseHex != null && isHex)
        {
            baseHex.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

        //if mouse button (left hand side) pressed instantiate a raycast
        if (Input.GetMouseButton(0) && GridManager.Instance.GetSelectionTile()!=null)
        {
           /* //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider == GetComponent<Collider>())
                {
                    GridManager.Instance.SetEnteredTile(this);
                    canPlace = GridManager.Instance.CompareSelectedToEnteredTile();
                }
            }
            else
            {
                canPlace = false;
                GridManager.Instance.SetEnteredTile(null);
            }*/
        }
    }

    #region Mouse Selection

    public bool CanBePlace()
    {
        return canPlace;
    }

    public void SetCanPlace(bool v)
    {
        canPlace = v;
    }

    public  void PlaceHex()
    {
    }

    public void HideBase()
    {
        if(baseHex!=null)
        {
            baseHex.SetActive(false);
        }
    }
    public async void PlaceHexes(Tile st)
    {
        List<Tile> tempSelect = new List<Tile>();
        List<Task> task = new List<Task>();

        GridManager.Instance.tempTiles.Clear();

        await st.transform.DOMove(new Vector3(transform.position.x, transform.position.y + GridManager.Instance.baseYOffset + (1 * (hexes.Count) ), transform.position.z), 0.1f).OnComplete(() => {

            this.AddHex(st, true);
        }).AsyncWaitForCompletion();
        st.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
        VibrationManager.Instance.PlayHaptic();
        SoundManager.Instance.Play(Sound.Pop);
        GridManager.Instance.tempTiles.Add(this);
        tempSelect.Add(this);
        foreach (int i in st.GetNeighborIndex())
        {
            if (this.GetNeighbor(i) != null && this.GetNeighbor(i).GetState() == TileType.Empty)
            {
                
                
                Tile b = st.GetNeighbor(i);
                await b.transform.DOMove(new Vector3(GetNeighbor(i).transform.position.x, GetNeighbor(i).transform.position.y + GridManager.Instance.baseYOffset + (1 * (GetNeighbor(i).hexes.Count)), GetNeighbor(i).transform.position.z), 0.1f).OnComplete(() => {

                    this.GetNeighbor(i).AddHex(st.GetNeighbor(i), true);
                }).AsyncWaitForCompletion();
                b.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
                VibrationManager.Instance.PlayHaptic();
                SoundManager.Instance.Play(Sound.Pop);
                GridManager.Instance.tempTiles.Add(this.GetNeighbor(i));

               
                if (st.GetNeighbor(i).GetNeighborIndex().Count > 0)
                {
                    foreach (int x in st.GetNeighbor(i).GetNeighborIndex())
                    {

                        if (this.GetNeighbor(i).GetNeighbor(x) != null && this.GetNeighbor(i).GetNeighbor(x).GetState() == TileType.Empty)
                        {
                            Tile t = st.GetNeighbor(i).GetNeighbor(x);
                            await t.transform.DOMove(new Vector3(this.GetNeighbor(i).GetNeighbor(x).transform.position.x, this.GetNeighbor(i).GetNeighbor(x).transform.position.y + GridManager.Instance.baseYOffset + (1 * (GetNeighbor(i).GetNeighbor(x).hexes.Count)), this.GetNeighbor(i).GetNeighbor(x).transform.position.z), 0.1f).OnComplete (()=> {
                                this.GetNeighbor(i).GetNeighbor(x).AddHex(st.GetNeighbor(i).GetNeighbor(x), true);
                            }).AsyncWaitForCompletion();
                            t.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
                            GridManager.Instance.tempTiles.Add(this.GetNeighbor(i).GetNeighbor(x));
                            VibrationManager.Instance.PlayHaptic();
                            SoundManager.Instance.Play(Sound.Pop);
                        }

                    }
                }
            }           
        }
        GridManager.Instance.CheckForStack();
            
       // CheckForStack();        
    }


    public void ShiftHexesToTile()
    {
      
            baseHex.SetActive(false);              
            hexes.Clear();
           // baseHex.SetActive(false);                     
                UpdateState(TileType.Empty);      

    }


    public void AddHex(Tile t, bool move)
    {
        //hexType = t.hexType;
        hexes.Add(t);
        if (t.isHex)
        {
            t.baseHex.SetActive(false);
        }
        t.transform.parent = transform;
        if (move)
        {
           
        }
        if (hexes.Count > 0)
        {
            UpdateState(TileType.Occupied);
//            baseHex.SetActive(true);
        }        
    }

   /* public void CheckForStack()
    {
        foreach (Tile tile in neighbors)
        {
            if (tile != null)
            {
                if (tile.hexes.Count > 0)
                {
                    if (tile.hexes[0].hexType == hexes[0].hexType)
                    {

                        foreach (Tile t in tile.neighbors)
                        {
                            if (t != null && t!=tile)
                            {
                                if (t.hexes.Count > 0)
                                {
                                    if (t.hexes[0].hexType == tile.hexes[0].hexType)
                                    {
                                        t.ShiftHexesToTile(tile);
                                        //Check if tile has hexes, if so, teleport all to this tile
                                        //  tile.transform.DOMove(new Vector3(transform.position.x, t.transform.position.y * (1 * hexes.Count * GridManager.Instance.yOffsetTile), transform.position.z), 0.2f);
                                    }
                                }
                            }
                        }
                        tile.ShiftHexesToTile(this);


                        //Check if tile has hexes, if so, teleport all to this tile
                        //  tile.transform.DOMove(new Vector3(transform.position.x, t.transform.position.y * (1 * hexes.Count * GridManager.Instance.yOffsetTile), transform.position.z), 0.2f);
                    }
                }
            }
        }
    }*/
    #endregion

    #region neighbors
    public Tile GetNeighbor(int index)
    {
        return neighbors[index];
    }

    public List<Tile> GetNeighbors()
    {
        return neighbors;
    }

    public List<int> GetNeighborIndex()
    {
        List<int> i = new List<int>();
        foreach(Tile t in neighbors)
        {
            if (t != null)
            {
                i.Add(neighbors.IndexOf(t));
            }
        }
        return i;
    }
    #endregion

    #region Highlight

   

    public void Highlight(Color c)
    {
        //hexSprite.color = c;
        hexMesh.material.color = c;
    }

    public void DeHighlight()
    {
      // hexSprite.color = Color.white;
        hexMesh.material.color = origColor;
    }


    #endregion
    public void SetNeighbor(Tile cell)
    {
        neighbors.Add(cell);
        //cell.SetNeighbor(this);
    }


    public void CopyTiles()
    {

    }

    public void MergeHex()
    {

    }


    public void AddHex(Hex h)
    {
        hexes.Add(h);
    }

    public void RemoveHex(Hex h)
    {
        hexes.Remove(h);
    }

    public void SellHexes()
    {
        hexType = hexes[0].hexType;             
        if (LevelManager.Instance.currentPizza >= LevelManager.Instance.maxPizza)
        {
            GameManager.Instance.WinLevel();
        }
        
        
        hexes.Clear();
        
        UpdateState(TileType.Empty);
        baseHex.SetActive(false);

    }

   
    public TileType GetState()
    {
        return state;
    }
    public void UpdateState(TileType t)
    {
        state = t;
        switch (t)
        {
            case TileType.Occupied:
                Highlight(origColor);
                if (baseHex != null && !isHex)
                {
                    baseHex.SetActive(true);
                }
                break;
            case TileType.Empty:
                Highlight(origColor);
                if (baseHex != null)
                {
                    baseHex.SetActive(false);
                }
                break;


        }
    }

   
}
