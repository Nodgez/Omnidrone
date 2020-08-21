using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour, IInteractable
{
	public CellUnit cellUnit;
	public Vector2Int point;
	public Vector3Int cubePoint;
	public bool Occupied
	{
		get { return cellUnit != null; }
	}
	public void SetUp(float size, Vector2Int center)
	{
		this.point = center;
		this.cubePoint = EvenCol2Cube(point);
	}

	public void Interact()
	{
		print("Cell interacted with");
		if (Occupied)
			TurnManager.Instance.ActiveArmy.SelectUnit(cellUnit);
		else
			TurnManager.Instance.ActiveArmy.MoveSelectedUnit(this);
	}

	public static Vector3Int[] CubicDirections = new Vector3Int[]
	{
		new Vector3Int(1, -1, 0),
		new Vector3Int(1, 0, -1),
		new Vector3Int(0, -1, -1),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(-1, 0, 1),
		new Vector3Int(0, -1, 1)
	};

	public static int Distance(Vector3Int a, Vector3Int b)
	{
		return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
	}

	public static Vector2Int Cube2EvenCol(Vector3Int cubeCoordinate)
	{
		var col = cubeCoordinate.x;
		var row = cubeCoordinate.z + (cubeCoordinate.x + (cubeCoordinate.x & 1)) / 2;
		return new Vector2Int(col, row);
	}

	public static Vector3Int EvenCol2Cube(Vector2Int cellPoint)
	{
		var x = cellPoint.x;
		var z = cellPoint.y - (cellPoint.x + (cellPoint.x & 1)) / 2;
		var y = -x - z;

		return new Vector3Int(x, y, z);
	}

	public static int Point2Index(Vector2Int point, int mapWidth)
	{
		return point.y + (point.x * mapWidth);
	}
	
	public static int Point2Index(Vector3Int point, int mapWidth)
	{
		var offsetPoint = Cube2EvenCol(point);
		return Point2Index(offsetPoint, mapWidth);
	}

	public override string ToString()
	{
		return point.ToString() + " ";
	}
}
public class CellConfig
{
	private static CellConfig instance;
	public static CellConfig Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CellConfig(1f);
			}
			return instance;
		}
	}

	public Vector3[] Vertices { get; private set; }
	public float width { get; private set; }
	public float height { get; private set; }
	public CellConfig(float size)
	{
		instance = this;
		width = 2 * size;
		height = Mathf.Sqrt(3) * size;

		Vertices = new Vector3[6];

		for (var i = 0; i < Vertices.Length; i++)
		{
			var angleDegree = 60 * i;
			var angleRadian = Mathf.Deg2Rad * angleDegree;

			Vertices[i] = new Vector3(size * Mathf.Cos(angleRadian), 0, size * Mathf.Sin(angleRadian));
		}
	}
}
