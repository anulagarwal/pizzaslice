using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectionTile : Tile
{
    [Header("Attributes")]
    [SerializeField] Vector3 origPos;

    [Header("Component References")]
    [SerializeField] Tile primaryTile;

    private void Start()
    {
        origPos = transform.position;
    }

    private void Update()
    {
        if (GridManager.Instance.GetSelectionTile() == primaryTile)
        {
            //if mouse button (left hand side) pressed instantiate a raycast
            if (Input.GetMouseButton(0))
            {
                //create a ray cast and set it to the mouses cursor position in game
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                }
            }
        }
    }
    #region Mouse events

    private void OnMouseDown()
    {
        if (GridManager.Instance.GetSelectionTile() != primaryTile)
        {
            GridManager.Instance.SetSelectedTile(primaryTile);
        }
    }

    private void OnMouseUp()
    {
        if (GridManager.Instance.GetSelectionTile() == primaryTile)
        {
            if (GridManager.Instance.PlaceTile())
            {
                SelectionManager.Instance.RemoveTile(gameObject);
                Vector3 targPos = GridManager.Instance.GetEnteredTile().transform.position;
                Vector3 pos = new Vector3(targPos.x, transform.position.y, targPos.z);

                GridManager.Instance.CheckForStack();
               // transform.DOMove(pos,0.3f);
            }
            else
            {
                transform.DOMove(origPos, 0.25f);
            }


            //If can place, destroy this from selection manager and spawn new

            GridManager.Instance.DeselectTile();
            //transform.position = origPos;

        }
    }

    private void OnMouseDrag()
    {
       
    }

    #endregion



    #region Neighbors
   /* public List<int> GetNeighborIndex()
    {
        List<int> i = new List<int>();
        foreach (Tile t in neighbors)
        {
            if (t != null)
            {
                i.Add(neighbors.IndexOf(t));
            }
        }
        return i;
    }
    public Tile GetNeighbor(int index)
    {
        return neighbors[index];
    }

    public List<Tile> GetNeighbors()
    {
        return neighbors;
    }
   */
    #endregion
}
