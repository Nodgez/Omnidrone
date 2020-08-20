using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class Battlefield : MonoBehaviour
{
	public List<Cell> cellData;
	private Mesh hexMesh;
	private int width, bredth;

	public void Initialize(int width, int bredth)
	{
		this.width = width;
		this.bredth = bredth;
		tag = "Battlefield";

		cellData = new List<Cell>(width * bredth);
	}

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
		cell.transform.SetParent(this.transform);

		var x = cell.point.x * CellConfig.Instance.width * 0.75f;
		var z = cell.point.y * CellConfig.Instance.height + (cell.point.x % 2 == 0 ? CellConfig.Instance.height * 0.5f : 0);
		cell.transform.position = new Vector3(x, 0, z);

		var meshFilter = cell.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = hexMesh;
		var meshRenderer = cell.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = defaultMaterial;
	}

	private void UpdateCell(Cell cell)
	{ 
		
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

	public List<Cell> FindPath(Cell start, Cell end)
	{
		var graph = Search(start, 5);

		var frontier = new Queue<Cell>();
		frontier.Enqueue(start);
		var walkedPath = new Dictionary<Cell, Cell>();
		walkedPath.Add(start, null);

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			var neighbours = GetNeighbours(current);
			foreach (var neighbour in neighbours)
			{
				if (neighbour.Occupied)
					continue;
				if (!walkedPath.ContainsKey(neighbour))
				{
					frontier.Enqueue(neighbour);
					walkedPath.Add(neighbour, current);
				}
			}
		}

		var currentCellToAdd = end;
		var path = new List<Cell>();
		while (currentCellToAdd != start)
		{
			path.Add(currentCellToAdd);
			currentCellToAdd = walkedPath[currentCellToAdd];
		}

		path.Add(start);
		path.Reverse();
		return path;
	}

	private List<Cell> GetNeighbours(Cell cell)
	{
		var neighbours = new List<Cell>();
		for (var dir = 0; dir < Cell.CubicDirections.Length; dir++)
		{
			var key = Cell.Point2Index(cell.cubePoint + Cell.CubicDirections[dir], width);
			if (!(key < cellData.Count && key > -1))
				continue;

			neighbours.Add(cellData[key]);
		}
		return neighbours;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		//for (int i = 0; i < transform.childCount; i++)
		//{
		//	var cubePoint = Cell.EvenCol2Cube(cellData[i].point);
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(-0.5f, 0, 0.5f), "X: " + cubePoint.x.ToString());
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(0, 0, -0.5f), "Y: " + cubePoint.y.ToString());
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(0.5f, 0, 0.5f), "Z: " + cubePoint.z.ToString());
		//}

		//for (int i = 0; i < transform.childCount; i++)
		//{
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(-0.15f, 0, 0), "X: " + cellData[i].point.x.ToString());
		//	Handles.Label(transform.GetChild(i).transform.position + new Vector3(0.15f, 0, 0), ", Y: " + cellData[i].point.y.ToString());
		//}
	}
#endif
}