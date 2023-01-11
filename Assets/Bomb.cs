using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;


public class Bomb : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] public Tile t;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceOnTile(Tile tile)
    {
        t = tile;
        _=Explode();
    }

   async Task Explode()
    {
        transform.DOPunchScale(transform.localScale * 1.4f, 1f).SetLoops(6, LoopType.Restart);
        GetComponent<MeshRenderer>().material.DOColor(Color.red, 1f).SetLoops(6, LoopType.Restart);
        await Task.Delay(2000);

        ps.Play();
        GetComponent<MeshRenderer>().enabled = false;
        await Task.Delay(150);
        Destroy(gameObject, 2f);

        GridManager.Instance.BombSurroundingTiles(t);
    }
}
