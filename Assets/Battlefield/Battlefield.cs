using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class Battlefield : MonoBehaviour
{
	[HideInInspector]
	public List<Cell> cellData = new List<Cell>();
	public Material cellMaterial;
	private Mesh hexMesh;

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
		meshRenderer.material = defaultMaterial;
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
				for (var dir = 0; dir < Cell.CubicDirections.Length; dir++)
				{
					var queryPoint = cell.cubePoint + Cell.CubicDirections[dir];
					var neightbour = cellData.Find(c => c.cubePoint == queryPoint);
					if (neightbour == null)
						continue;
					if (!neightbour.Occupied && !visited.Contains(neightbour))
					{
						visited.Add(neightbour);
					}

					fringes[k].Add(neightbour);
				}
			}
		}

		return visited;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			var cubePoint = Cell.EvenCol2Cube(cellData[i].point);
			Handles.Label(transform.GetChild(i).transform.position + new Vector3(-0.5f, 0, 0.5f), "X: " + cubePoint.x.ToString());
			Handles.Label(transform.GetChild(i).transform.position + new Vector3(0, 0, -0.5f), "Y: " + cubePoint.y.ToString());
			Handles.Label(transform.GetChild(i).transform.position + new Vector3(0.5f, 0, 0.5f), "Z: " + cubePoint.z.ToString());
		}

		for (int i = 0; i < transform.childCount; i++)
		{
			Handles.Label(transform.GetChild(i).transform.position + new Vector3(-0.15f, 0, 0), "X: " + cellData[i].point.x.ToString());
			Handles.Label(transform.GetChild(i).transform.position + new Vector3(0.15f, 0, 0), ", Y: " + cellData[i].point.y.ToString());
		}
	}
#endif
}
