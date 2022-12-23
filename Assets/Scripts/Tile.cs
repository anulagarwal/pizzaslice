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
        if(baseHex!=null)
        baseHex.SetActive(false);

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
    public List<Tile> PlaceHexes(Tile st)
    {
        List<Tile> tempSelect = new List<Tile>();

        this.AddHex(st,true);
        tempSelect.Add(this);
        foreach (int i in st.GetNeighborIndex())
        {
            if (this.GetNeighbor(i) != null && this.GetNeighbor(i).GetState() == TileType.Empty)
            {
                this.GetNeighbor(i).AddHex(st.GetNeighbor(i),true);
                tempSelect.Add(this.GetNeighbor(i));
                if (st.GetNeighbor(i).GetNeighborIndex().Count > 0)
                {
                    foreach (int x in st.GetNeighbor(i).GetNeighborIndex())
                    {
                        if (this.GetNeighbor(i).GetNeighbor(x) != null && this.GetNeighbor(i).GetNeighbor(x).GetState() == TileType.Empty)
                        {
                            this.GetNeighbor(i).GetNeighbor(x).AddHex(st.GetNeighbor(i).GetNeighbor(x),true);
                            tempSelect.Add(this.GetNeighbor(i).GetNeighbor(x));
                        }

                    }
                }
            }           
        }
        return tempSelect;

       // CheckForStack();
        
    }

    public void ShiftHexesToTile(Tile t)
    {
        var sequence = DOTween.Sequence();
        for(int i = hexes.Count-1; i>=0; i--)
        {
            t.AddHex(hexes[i],false);
            sequence.AppendInterval(0.05f).Append(hexes[i].transform.DOMove(new Vector3(t.transform.position.x, t.transform.position.y + (1 * t.hexes.Count * GridManager.Instance.yOffsetTile), t.transform.position.z), 0.2f));
            SoundManager.Instance.Play(Sound.Pop);

        }
        sequence.OnComplete(()=>
        {

            hexes.Clear();
           // baseHex.SetActive(false);
            
                print("selll1");

            if (t.hexes.Count >= 5)
            {
                print("selll");
                t.SellHexes();
            }
        });
                UpdateState(TileType.Empty);

    }


    public void AddHex(Tile t, bool move)
    {
        //hexType = t.hexType;
        hexes.Add(t);
        t.transform.parent = transform;
        if (move)
        {
            t.transform.DOMove(new Vector3(transform.position.x, transform.position.y + (1 * hexes.Count * GridManager.Instance.yOffsetTile), transform.position.z), 0.2f).OnComplete(() =>
            {
                SoundManager.Instance.Play(Sound.Pop);
            });
            t.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
        }
        if (hexes.Count > 0)
        {
            UpdateState(TileType.Occupied);
//            baseHex.SetActive(true);
        }        
    }

    public void CheckForStack()
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
    }
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
        //Remove all hexes - teleport vfx to ui space
        var sequence = DOTween.Sequence();

        hexType = hexes[0].hexType;

        for (int i = hexes.Count - 1; i >= 0; i--)
        {
            sequence.AppendInterval(0.15f).Append(hexes[i].transform.DOMove(UIManager.Instance.GetItemPos(hexType), 0.3f));
            LevelManager.Instance.AddItem(hexType);
        }
        sequence.OnComplete(() =>
        {
            foreach (Tile h in hexes)
            {
                Destroy(h.gameObject);
            }


        });
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
                break;
            case TileType.Empty:
                Highlight(origColor);
                break;


        }
    }

   
}
