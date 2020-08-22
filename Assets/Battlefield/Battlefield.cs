using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]

public class Battlefield : MonoBehaviour
{

	private static Battlefield instance;
	public static Battlefield Instance
	{
		get	{ return instance; }
	}
	public List<Cell> cellData;
	private Mesh hexMesh;
	[SerializeField]
	private int width, bredth;

	public void Awake()
	{
		instance = this;
	}

#if UNITY_EDITOR
	public void Initialize(int width, int bredth)
	{
		this.width = width;
		this.bredth = bredth;
		tag = "Battlefield";

		cellData = new List<Cell>(width * bredth);
	}
	#endif
	public void RenderCells(Material defaultMaterial = null)
	{
		hexMesh = new Mesh()
		{
			vertices = CellConfig.Instance.Vertices,
			triangles = new int[]
				{
				2,1,0,
				2,0,5,
				2,5,3,
				3,5,4
				}

		};
		hexMesh.name = "HexCell";
		cellData.OrderBy(cell => Cell.Point2Index(cell.point, width));
		cellData.ForEach(cell => RenderCell(cell, defaultMaterial));		
	}

	private void RenderCell(Cell cell, Material defaultMaterial)
	{
		cell.transform.SetParent(this.transform.Find("Cells"));

		var x = cell.point.x * CellConfig.Instance.width * 0.75f;
		var z = cell.point.y * CellConfig.Instance.height + (cell.point.x % 2 == 0 ? CellConfig.Instance.height * 0.5f : 0);
		cell.transform.position = new Vector3(x, 0, z);
		cell.transform.localScale *= 0.95f;
		var meshFilter = cell.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = hexMesh;
		var meshRenderer = cell.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = defaultMaterial;
		var meshCollider = cell.gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = hexMesh;
		cell.gameObject.layer = LayerMask.NameToLayer("Battlefield");
	}

	public List<Cell> Search(Cell start, int movement)
	{
		var visited = new List<Cell>();
		var fringes = new List<List<Cell>>();
		fringes.Add(new List<Cell>() { start });

		for (int k = 1; k < movement; k++)
		{
			fringes.Add(new List<Cell>());
			foreach (var cell in fringes[k - 1])
			{
				var neighbours = GetNeighbours(cell);
				for (var n = 0; n < neighbours.Count; n++)
				{
					var neighbour = neighbours[n];
					if (!neighbour.Occupied && !visited.Contains(neighbour))
						visited.Add(neighbour);

					fringes[k].Add(neighbour);
				}
			}
		}

		return visited;
	}

	public List<Cell> GetTilesInRange(Cell center, int range, Predicate<Cell> predicate)
	{
		var results = new List<Cell>();
		for (var x = -range; x <= range; x++)
			for (var y = -range; y <= range; y++)
				for (var z = -range; z <= range; z++)
				{
					if (x + y + z == 0)
					{
						var cubicIndex = center.cubePoint + new Vector3Int(x, y, z);
						var index = Cell.Point2Index(cubicIndex, width);
						if (index < 0 || index > cellData.Count - 1)
							continue;
						var cellAtDistance = cellData[index];

						if (predicate(cellAtDistance))
							results.Add(cellAtDistance);
					}
				}

		return results;
	}

	public List<Cell> GetTilesInRange(Cell center, int range)
	{
		return GetTilesInRange(center, range, (x) => { return true; });
	}
	public Stack<Cell> FindPath(Cell start, Cell end, int range)
	{
		if (Cell.Distance(start.cubePoint, end.cubePoint) > range)
			return new Stack<Cell>();

		var frontier = new Queue<Cell>();
		frontier.Enqueue(start);
		var walkedPath = new Dictionary<Cell, Cell>();
		walkedPath.Add(start, null);

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			var neighbours = GetNeighbours(current);
			//print("neighbors for " + current.ToString() + "(current): " + string.Concat(neighbours));
			if (Cell.Distance(current.cubePoint, start.cubePoint) >= range)
				continue;

			foreach (var neighbour in neighbours)
			{
				if (!walkedPath.ContainsKey(neighbour))
				{
					frontier.Enqueue(neighbour);
					walkedPath.Add(neighbour, current);
				}
			}
		}

		var currentCellToAdd = end;
		var path = new Stack<Cell>();
		while (currentCellToAdd != start)
		{
			path.Push(currentCellToAdd);
			print(currentCellToAdd.ToString() + " added to path");
			currentCellToAdd = walkedPath[currentCellToAdd];
		}

		//path.Enqueue(start);
		return path;
	}

	private List<Cell> GetNeighbours(Cell cell)
	{
		var neighbours = new List<Cell>();

		//print("Starting Search for neightbors of: " + cell.ToString());
		for (var dir = 0; dir < Cell.CubicDirections.Length; dir++)
		{
			var neightborCube = cell.cubePoint + Cell.CubicDirections[dir];
			var key = Cell.Point2Index(neightborCube, width);
			if (key < 0 || key > cellData.Count - 1)
				continue;

			neighbours.Add(cellData[key]);
		}
		return neighbours;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var cellsParent = transform.Find("Cells");
		for (int i = 0; i < cellsParent.childCount; i++)
		{
			var cubePoint = Cell.EvenCol2Cube(cellData[i].point);
			Handles.Label(cellsParent.GetChild(i).transform.position + new Vector3(-0.5f, 0, 0.5f), "X: " + cubePoint.x.ToString());
			Handles.Label(cellsParent.GetChild(i).transform.position + new Vector3(0, 0, -0.5f), "Y: " + cubePoint.y.ToString());
			Handles.Label(cellsParent.GetChild(i).transform.position + new Vector3(0.5f, 0, 0.5f), "Z: " + cubePoint.z.ToString());
		}

		//for (int i = 0; i < transform.childCount; i++)
		//{
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(-0.15f, 0, 0), "X: " + cellData[i].point.x.ToString());
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(0.15f, 0, 0), ", Y: " + cellData[i].point.y.ToString());
		//}
	}
#endif
}