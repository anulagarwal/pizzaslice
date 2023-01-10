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
    public Color origColorOut;
    public Color occupiedColor;





    [Header("Component References")]
    [SerializeField] public List<Tile> hexes;
    [SerializeField] public List<Tile> neighbors;
    [SerializeField] SpriteRenderer hexSprite;
    [SerializeField] MeshRenderer hexMesh;
    [SerializeField] GameObject baseHex = null;
    [SerializeField] ParticleSystem baseVfx = null;
    [SerializeField] ParticleSystem sellVfx = null;








    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
        if(hexMesh!=null)
       // origColor = hexMesh.material.color;
        if (baseHex != null && !isHex)
        {
            baseHex.SetActive(false);
        }
        else if(baseHex != null && isHex)
        {
            baseHex.SetActive(true);
                hexMesh.materials[0].color = ColorManager.Instance.GetHexColor(hexType);
                hexMesh.materials[1].color = ColorManager.Instance.GetHexColor(hexType);

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

    public void PlayBaseVFX()
    {
        baseVfx.Play();
    }

    public void PlaySellVFX()
    {
        sellVfx.Play();
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

    public async Task PlaceHex(Tile st, Tile to)
    {

        foreach (int i in st.GetNeighborIndex())
        {
            if (to.GetNeighbor(i) != null && to.GetNeighbor(i).GetState() == TileType.Empty)
            {


                Tile b = st.GetNeighbor(i);
                await b.transform.DOMove(new Vector3(to.GetNeighbor(i).transform.position.x, to.GetNeighbor(i).transform.position.y + GridManager.Instance.baseYOffset + (1 * (to.GetNeighbor(i).hexes.Count)), to.GetNeighbor(i).transform.position.z), 0.1f).OnComplete(() =>
                {

                    to.GetNeighbor(i).AddHex(st.GetNeighbor(i), true);

                }).AsyncWaitForCompletion();
                b.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
                VibrationManager.Instance.PlayHaptic();
                SoundManager.Instance.Play(Sound.Pop);
                GridManager.Instance.tempTiles.Add(to.GetNeighbor(i));

                if (st.GetNeighbor(i).GetNeighborIndex().Count > 0)
                {
                    foreach (int x in st.GetNeighbor(i).GetNeighborIndex())
                    {
                       await PlaceHex(st.GetNeighbor(i), to.GetNeighbor(i));
                    }

                }
            }
        }


    }

        public async void PlaceHexes(Tile st)
        {
        GridManager.Instance.tempTiles.Clear();
        await st.transform.DOMove(new Vector3(transform.position.x, transform.position.y + GridManager.Instance.baseYOffset + (1 * (hexes.Count) ), transform.position.z), 0.1f).OnComplete(() => {

            this.AddHex(st, true);
        }).AsyncWaitForCompletion();
        st.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);
        VibrationManager.Instance.PlayHaptic();
        SoundManager.Instance.Play(Sound.Pop);
        GridManager.Instance.tempTiles.Add(this);
      await PlaceHex(st, this);
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
            hexType = t.hexType;
            
            occupiedColor = ColorManager.Instance.GetHexColor(hexType);

            UpdateState(TileType.Occupied);
            //            baseHex.SetActive(true);
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
        hexMesh.materials[1].color = c;
    }

    public void DeHighlight()
    {
      // hexSprite.color = Color.white;
        hexMesh.materials[1].color = origColor;
        hexMesh.materials[0].color = origColorOut;

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
        hexes.Clear();        
        UpdateState(TileType.Empty);
        baseHex.SetActive(false);
    }
    public void SellHex(Tile h)
    {
       hexes[hexes.FindIndex(x=>x==h)] =null;
        if (hexes.Count == 0)
        {
            hexes.Clear();
            UpdateState(TileType.Empty);
            baseHex.SetActive(false);
        }
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
                //Highlight(occupiedColor);
                hexMesh.materials[0].color = ColorManager.Instance.GetHexColor(hexType);

                hexMesh.materials[1].color = ColorManager.Instance.GetHexColor(hexType);

                if (baseHex != null && !isHex)
                {
                    baseHex.SetActive(true);
                }
                break;
            case TileType.Empty:
                Highlight(origColor);
                hexMesh.materials[1].color = origColor;

                if (baseHex != null)
                {
                    baseHex.SetActive(false);
                }
                break;

            case TileType.Blocked:
                gameObject.SetActive(false);
                break;

        }
    }

   
}
