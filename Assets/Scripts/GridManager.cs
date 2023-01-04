using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
	[Header("Grid Generator Attributes")]
	public int width = 6;
	public int height = 6;

	public Tile cellPrefab;

	List<Tile> cells = new List<Tile>();

	[Header("Attributes")]
	[SerializeField] Tile selectedTile;
	[SerializeField] Tile enteredTile;
	[SerializeField] public float yOffsetTile;
	[SerializeField] public float baseYOffset;

	[SerializeField] public float upScaleValue;

	[SerializeField] public List<Tile> tempTiles;

	[Header("Component References")]
	[SerializeField] Transform fromBox;
	[SerializeField] Transform toBox;
	[SerializeField] Vector3 centerBox;


	[SerializeField] public Box box;
	public static GridManager Instance = null;


	void Awake()
	{
		Application.targetFrameRate = 100;
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		Instance = this;
		//cells = new Tile[height * width];

		for (int z = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				cells.Add(CreateCell(x, z));
			}
		}
		SetNeighbors();
		centerBox = box.transform.position;
		box.transform.position = fromBox.position;
	}

	Tile CreateCell(int x, int z)
	{
		Vector3 position;

		position.x = x * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);

		Tile cell = Instantiate<Tile>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.gameObject.name = "Tile " + cells.Count;

		return cell;
	}

	public void SetNeighbors()
	{
		foreach (Tile t in cells)
		{
			int b = t.coordinates.X;
			int z = t.coordinates.Z;

			if (cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z + 1) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z + 1));
			}
			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}
			
			if (cells.Find(x => x.coordinates.X == b + 1 && x.coordinates.Z == z) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b + 1 && x.coordinates.Z == z));
			}
			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}

			if (cells.Find(x => x.coordinates.X == b + 1 && x.coordinates.Z == z-1) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b + 1 && x.coordinates.Z == z-1));
			}
			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}


			if (cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z - 1) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z - 1));
			}
			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}


			if (cells.Find(x => x.coordinates.X == b - 1 && x.coordinates.Z == z ) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b - 1 && x.coordinates.Z == z ));
			}

			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}


			if (cells.Find(x => x.coordinates.X == b - 1 && x.coordinates.Z == z+1) != null)
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(cells.Find(x => x.coordinates.X == b - 1 && x.coordinates.Z == z+1));
			}
			else
			{
				cells.Find(x => x.coordinates.X == b && x.coordinates.Z == z).SetNeighbor(null);
			}

			
					}
		

	}

	
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
        }
    }

    #region Entered Tiles
    public void SetEnteredTile(Tile t)
    {
		enteredTile = t;
        if (t = null)
        {
			CleanSelection();
        }
    }

	public List<Tile> GetCells()
    {
		return cells;
    }
	public Tile GetEnteredTile()
    {
		return enteredTile;
    }
	public Tile GetSelectionTile()
    {
		return selectedTile;
    }
	public void SetSelectedTile(Tile t)
	{
		selectedTile = t;
	}

	public bool PlaceTile()
    {
		if (enteredTile.CanBePlace())
		{

			 enteredTile.PlaceHexes(selectedTile);
			GameManager.Instance.AddMove(1);
			//tempTiles = enteredTile.CalculatePlacedHexes(selectedTile);
			return true;
		}
		return false;
	}
	public void DeselectTile()
    {        
		selectedTile = null;
		enteredTile = null;
		CleanSelection();
    }

	public async void CheckForStack()
    {
		box.transform.position = fromBox.position;
		foreach (Tile tile in tempTiles)
		{
            if (tile.hexes.Count > 0)
			{
				foreach (Tile n in tile.GetNeighbors())
				{
					if (n != null)
					{
						if (n.hexes.Count > 0 && tile.hexes.Count>0)
						{
							if (n.hexes[0].hexType == tile.hexes[0].hexType)
							{
								for (int i = n.hexes.Count - 1; i >= 0; i--)
								{
									
									await n.hexes[i].transform.DOMove(new Vector3(tile.transform.position.x, tile.transform.position.y + GridManager.Instance.baseYOffset + (1 * (tile.hexes.Count) * GridManager.Instance.yOffsetTile), tile.transform.position.z), 0.2f).AsyncWaitForCompletion();

									tile.AddHex(n.hexes[i], false);
									n.hexes[i].HideBase();

									VibrationManager.Instance.PlayHaptic();
									SoundManager.Instance.Play(Sound.Pop);
								}

								

								foreach (Tile t in n.neighbors)
								{
									if (t != null)
									{
										if (t.hexes.Count > 0 && n.hexes.Count>0)
										{
											if (t.hexes[0].hexType == n.hexes[0].hexType && t != tile)
											{

												for (int i = t.hexes.Count - 1; i >= 0; i--)
												{
													
													await t.hexes[i].transform.DOMove(new Vector3(n.transform.position.x, n.transform.position.y + GridManager.Instance.baseYOffset + (1 * (n.hexes.Count) * GridManager.Instance.yOffsetTile), n.transform.position.z), 0.2f).AsyncWaitForCompletion();
													n.AddHex(t.hexes[i], false);
													t.hexes[i].HideBase();
													VibrationManager.Instance.PlayHaptic();
													SoundManager.Instance.Play(Sound.Pop);
												}
												t.ShiftHexesToTile();												
											}
										}
									}
								}
								if (n.hexes.Count >= 5)
								{
									SelectionManager.Instance.ActiveTiles(false);
									box.transform.position = fromBox.position;
									await box.transform.DOMove(centerBox, 0.5f).AsyncWaitForCompletion();
									 //await box.transform.DOScale(new Vector3(1, 1, 1), 0.7f).AsyncWaitForCompletion();

									for (int i = n.hexes.Count - 1; i >= 0; i--)
									{
										await n.hexes[i].transform.DOPunchScale(n.hexes[i].transform.localScale*1.5f, 0.05f).AsyncWaitForCompletion();
									}
									for (int i = n.hexes.Count - 1; i >= 0; i--)
									{
										VibrationManager.Instance.PlayHaptic();
										LevelManager.Instance.AddItem(n.hexes[0].hexType);

										await n.hexes[i].transform.DOMove(box.GetPosition(), 0.2f).OnComplete(() => {
											box.AddFood(n.hexes[i].transform);
										}).AsyncWaitForCompletion();
									}
									n.SellHexes();
									await Task.Delay(1500);
									//await box.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f).AsyncWaitForCompletion();
									await box.transform.DOMove(toBox.position, 0.5f).AsyncWaitForCompletion();
									box.Sanitize();
									SelectionManager.Instance.ActiveTiles(true);

								}

								n.ShiftHexesToTile();
							}
						}
					}
				}

				if (tile.hexes.Count >= 5)
				{
					SelectionManager.Instance.ActiveTiles(false);
					//await box.transform.DOScale(new Vector3(1, 1, 1), 0.7f).AsyncWaitForCompletion();
					box.transform.position = fromBox.position;
					await box.transform.DOMove(centerBox, 0.5f).AsyncWaitForCompletion();
					for (int i = tile.hexes.Count - 1; i >= 0; i--)
					{
						await tile.hexes[i].transform.DOPunchScale(tile.hexes[i].transform.localScale * 1.5f, 0.05f).AsyncWaitForCompletion();
					}

					for (int i = tile.hexes.Count - 1; i >= 0; i--)
					{
						VibrationManager.Instance.PlayHaptic();
						LevelManager.Instance.AddItem(tile.hexes[0].hexType);
						await tile.hexes[i].transform.DOMove(box.GetPosition(), 0.2f).OnComplete(() => {
							box.AddFood(tile.hexes[i].transform);
						}).AsyncWaitForCompletion();

					}
					tile.SellHexes();
					await Task.Delay(1500);

					//await box.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f).AsyncWaitForCompletion();
					await box.transform.DOMove(toBox.position, 0.5f).AsyncWaitForCompletion();
					box.Sanitize();
					SelectionManager.Instance.ActiveTiles(true);

				}

			}
			
		}

		
        if (!SelectionManager.Instance.CheckForSpace())
        {
			foreach(GameObject g in SelectionManager.Instance.spawnedTiles)
            {
				g.transform.DOShakePosition(1f, 0.5f);
				//Initiate fail state
            }
			GameManager.Instance.LoseLevel();
		}


		if (LevelManager.Instance.currentPizza >= LevelManager.Instance.maxPizza)
		{
			GameManager.Instance.WinLevel();
		}

	}
	public void CleanSelection()
    {
		foreach (Tile t in cells)
		{
			if (t.GetState() != TileType.Occupied)
				t.DeHighlight();
		}
	}

    #endregion

    public bool CompareSelectedToEnteredTile()
    {
		CleanSelection();
		bool isPossible = false;
		List<Tile> tempSelect = new List<Tile>();
		if (enteredTile.GetState() == TileType.Empty)
		{
			isPossible = true;
			foreach (int i in selectedTile.GetNeighborIndex())
			{
				if (enteredTile.GetNeighbor(i) != null && enteredTile.GetNeighbor(i).GetState() == TileType.Empty)
				{
					isPossible = true;
					tempSelect.Add(enteredTile.GetNeighbor(i));
					foreach (int x in selectedTile.GetNeighbor(i).GetNeighborIndex())
					{
						if (enteredTile.GetNeighbor(i).GetNeighbor(x) != null && enteredTile.GetNeighbor(i).GetNeighbor(x).GetState() == TileType.Empty)
						{
							isPossible = true;
							tempSelect.Add(enteredTile.GetNeighbor(i).GetNeighbor(x));
						}
						else
						{
							isPossible = false;
						}
					}
				}
				else
				{
					isPossible = false;
				}
			}
		}
        if (isPossible)
        {
			enteredTile.Highlight(Color.green);
			foreach(Tile t in tempSelect)
            {
				t.Highlight(Color.green);
            }
        }
		return isPossible;
    }

	public bool CompareSelectedToEnteredTile(Tile g, Tile e)
	{
		CleanSelection();
		bool isPossible = false;
		if (e.GetState() == TileType.Empty)
		{
			isPossible = true;
			foreach (int i in g.GetNeighborIndex())
			{
				if (e.GetNeighbor(i) != null && e.GetNeighbor(i).GetState() == TileType.Empty)
				{
					isPossible = true;
					foreach (int x in g.GetNeighbor(i).GetNeighborIndex())
					{
						if (e.GetNeighbor(i).GetNeighbor(x) != null && e.GetNeighbor(i).GetNeighbor(x).GetState() == TileType.Empty)
						{
							isPossible = true;
						}
						else
						{
							isPossible = false;
						}
					}
				}
				else
				{
					isPossible = false;
				}
			}
		}
		
		return isPossible;
	}

	public void DeselectAll()
    {
		foreach (Tile t in cells)
		{
			t.DeHighlight();
		}
	}
}
