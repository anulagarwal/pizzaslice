using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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


    [Header("Component References")]
    [SerializeField] public List<Tile> hexes;
    [SerializeField] public List<Tile> neighbors;
    [SerializeField] SpriteRenderer hexSprite;
    [SerializeField] MeshRenderer hexMesh;





    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        //if mouse button (left hand side) pressed instantiate a raycast
        if (Input.GetMouseButton(0) && GridManager.Instance.GetSelectionTile()!=null)
        {
            //create a ray cast and set it to the mouses cursor position in game
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

            }
        }
    }

    #region Mouse Selection

    public bool CanBePlace()
    {
        return canPlace;
    }

    public List<Tile> PlaceHexes(Tile st)
    {
        List<Tile> tempSelect = new List<Tile>();

        this.AddHex(st);
        tempSelect.Add(this);
        foreach (int i in st.GetNeighborIndex())
        {
            if (this.GetNeighbor(i) != null && this.GetNeighbor(i).GetState() == TileType.Empty)
            {
                this.GetNeighbor(i).AddHex(st.GetNeighbor(i));
                tempSelect.Add(this.GetNeighbor(i));
                if (st.GetNeighbor(i).GetNeighborIndex().Count > 0)
                {
                    foreach (int x in st.GetNeighbor(i).GetNeighborIndex())
                    {
                        if (this.GetNeighbor(i).GetNeighbor(x) != null && this.GetNeighbor(i).GetNeighbor(x).GetState() == TileType.Empty)
                        {
                            this.GetNeighbor(i).GetNeighbor(x).AddHex(st.GetNeighbor(i).GetNeighbor(x));
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
        foreach(Tile h in hexes)
        {
            t.AddHex(h);
            h.transform.DOMove(new Vector3(t.transform.position.x, t.transform.position.y + (1 * t.hexes.Count * GridManager.Instance.yOffsetTile), t.transform.position.z), 0.2f);
        }
        hexes.Clear();
        if (hexes.Count == 0)
        {
            UpdateState(TileType.Empty);
        }
    }
    

    public void AddHex(Tile t)
    {
        //hexType = t.hexType;
        hexes.Add(t);
        t.transform.DOMove(new Vector3(transform.position.x, t.transform.position.y, transform.position.z), 0.2f);       
        if (hexes.Count > 0)
        {
            UpdateState(TileType.Occupied);
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
        hexSprite.color = c;
        hexMesh.material.color = c;
    }

    public void DeHighlight()
    {
        hexSprite.color = Color.white;
        hexMesh.material.color = Color.white;

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
        //After empty, change tile type to empty
        var sequence = DOTween.Sequence();

        hexType = hexes[0].hexType;
        foreach (Tile h in hexes)
        {
            sequence.Append(h.transform.DOMove(UIManager.Instance.GetItemPos(hexType), 0.2f));
            LevelManager.Instance.AddItem(hexType);

        }

        sequence.OnComplete(() =>
        {
            foreach (Tile h in hexes)
            {
                Destroy(h.gameObject);
            }
        });
        hexes.Clear();
        
        UpdateState(TileType.Empty);

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
                Highlight(ColorManager.Instance.GetHexColor(hexes[0].hexType));
                break;
            case TileType.Empty:
                Highlight(Color.white);
                break;


        }
    }
}
