using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
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
    public int frozenValue;


    [Header("Component References")]
    [SerializeField] public List<Tile> hexes;
    [SerializeField] public List<Tile> neighbors;
    [SerializeField] SpriteRenderer hexSprite;
    [SerializeField] public SpriteRenderer target;
    [SerializeField] public Animator fingerTap;


    [SerializeField] MeshRenderer hexMesh;
    [SerializeField] GameObject baseHex = null;
    [SerializeField] ParticleSystem baseVfx = null;
    [SerializeField] ParticleSystem sellVfx = null;
    [SerializeField] TextMeshPro frozenText = null;
    [SerializeField] Rigidbody rb = null;


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

        if (isHex)
        {
            rb = GetComponent<Rigidbody>();
        }

    }

    private void OnMouseDown()
    {
        if (GridManager.Instance.GetSelectionTile() != this && GridManager.Instance.canMove && GridManager.Instance.bomb && GetState() == TileType.Empty)
        {
            GridManager.Instance.PlaceBomb(this);
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
            if (to.GetNeighbor(i) != null && (to.GetNeighbor(i).GetState() == TileType.Empty || to.GetNeighbor(i).GetState() == TileType.Frozen))
            {


                Tile b = st.GetNeighbor(i);
                await b.transform.DOMove(new Vector3(to.GetNeighbor(i).transform.position.x, to.GetNeighbor(i).transform.position.y + GridManager.Instance.baseYOffset + (1 * (to.GetNeighbor(i).hexes.Count)), to.GetNeighbor(i).transform.position.z), 0.1f).OnComplete(() =>
                {
                    b.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);

                    to.GetNeighbor(i).AddHex(st.GetNeighbor(i), true);

                }).AsyncWaitForCompletion();
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
            st.transform.DOScale(GridManager.Instance.upScaleValue, 0.2f);

            this.AddHex(st, true);
        }).AsyncWaitForCompletion();
        VibrationManager.Instance.PlayHaptic();
        SoundManager.Instance.Play(Sound.Pop);
        GridManager.Instance.tempTiles.Add(this);
      await PlaceHex(st, this);
        GridManager.Instance.CheckForStack();
            
       // CheckForStack();        
    }


    public void ShiftHexesToTile()
    {
      
            //baseHex.SetActive(false);              
            hexes.Clear();
           // baseHex.SetActive(false);                     
                UpdateState(TileType.Empty);      
    }

    public async void BombThis(Vector3 point)
    {
        foreach(Tile t in hexes)
        {
            t.transform.parent = null;
            t.rb.isKinematic = false;
            t.rb.useGravity = true;
            t.rb.AddExplosionForce(GridManager.Instance.explosionForce, point, GridManager.Instance.explosionRadius, GridManager.Instance.explosionUpForce, ForceMode.Impulse);
            t.transform.DORotate(Random.rotation.eulerAngles, 1f, RotateMode.Fast).SetLoops(-1).SetEase(Ease.Linear);

        }
        List<GameObject> coins = new List<GameObject>();
        //Delay 2
        await Task.Delay(1000);
        foreach (Tile t in hexes)
        {
            GameObject g = CoinManager.Instance.SpawnCoin(t.transform.position);
            g.transform.DORotate(new Vector3(90, 0, 0), 0.4f);
            g.transform.DOScale(new Vector3(30, 30, 30), 0.4f);
           
            g.GetComponentInChildren<ParticleSystem>().Play();
            coins.Add(g);
            Destroy(t.gameObject);
        }

        foreach (GameObject g in coins)
        {
            g.transform.DOMove(GridManager.Instance.UICoinPos.position, 0.4f).OnComplete(() =>
            {
                LevelManager.Instance.AddItem(HexType.A);

                CoinManager.Instance.AddCoins(1);
                CoinManager.Instance.RemoveCoin(g);
            });
            await Task.Delay(150);
        }

        ShiftHexesToTile();
    }

    public void AddHex(Tile t, bool move)
    {
        //hexType = t.hexType;
        hexes.Add(t);

        if (GetState() == TileType.Frozen)
        {
            frozenValue--;

            hexes.Remove(t);
            Destroy(t.gameObject);
            frozenText.text = "" + frozenValue;
            UpdateState(TileType.Frozen);

            if (frozenValue <= 0)
            {
                UpdateState(TileType.Empty);
            }
           
        }
       
            if (t.isHex)
            {
                t.baseHex.SetActive(false);
            }
            t.transform.parent = transform;
            if (hexes.Count > 0 && state != TileType.Occupied)
            {
                hexType = t.hexType;
                UpdateState(TileType.Occupied);
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

    public int GetNeigborsWithHexes()
    {
        if (neighbors.FindAll(x => x != null && x.hexes.Count > 0)!=null)
        {
            return neighbors.FindAll(x => x != null && x.hexes.Count > 0).Count;
        }
        else
        {
            return 0;
        }
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
                hexMesh.materials[0].color = ColorManager.Instance.GetHexColor(hexes[0].hexType);

                hexMesh.materials[1].color = ColorManager.Instance.GetHexColor(hexes[0].hexType);
                print(ColorManager.Instance.GetHexColor(hexes[0].hexType));
                frozenText.gameObject.SetActive(false);

                if (baseHex != null && !isHex)
                {
                    baseHex.SetActive(true);
                }
                break;
            case TileType.Empty:
                hexMesh.materials[0].color = origColorOut;

                hexMesh.materials[1].color = origColor;
                frozenText.gameObject.SetActive(false);

                if (baseHex != null)
                {
                    baseHex.SetActive(false);
                }
                break;

            case TileType.Blocked:
                gameObject.SetActive(false);
                break;

            case TileType.Frozen:
                frozenText.gameObject.SetActive(true);
                break;

        }
    }

   
}
