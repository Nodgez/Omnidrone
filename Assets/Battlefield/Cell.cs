using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour, IInteractable
{
	public CellUnit cellUnit;
	public int cellIndex;
	public Vector2Int point;
	public Vector3Int cubePoint;
	public bool unWalkable;

	public MeshRenderer meshRenderer { get; private set; }
	public bool Occupied
	{
		get { return cellUnit != null; }
	}

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}
	public void SetUp(int columnCount, int rowCount, Vector2Int center)
	{
		this.point = center;
		this.cubePoint = EvenCol2Cube(point);
		this.cellIndex = Point2Index(point, columnCount, rowCount);
	}

	public void Interact()
	{
		if (Occupied)
		{
			GameController.Instance.ActiveArmy.SelectUnit(cellUnit);
		}
		else
			GameController.Instance.ActiveArmy.MoveSelectedUnit(this);
	}


	public bool IsAllyCell(string unitTag)
	{
		return cellUnit.CompareTag(unitTag);
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
		return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
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

	public static int Point2Index(Vector2Int point, int columnCount, int rowCount)
	{
		var isColumnOutOfBounds = point.x >= columnCount || point.x < 0;
		var isRowOutOfBounds = point.y >= rowCount || point.y < 0;

		if (isColumnOutOfBounds || isColumnOutOfBounds)
			return -1;

		return point.x + (point.y * columnCount);
	}
	
	public static int Point2Index(Vector3Int point, int columnCount, int rowCount)
	{
		var offsetPoint = Cube2EvenCol(point);
		return Point2Index(offsetPoint, columnCount, rowCount);
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

			Vertices[i] = new Vector3(size * Mathf.Cos(angleRadian), -0.5f, size * Mathf.Sin(angleRadian));
		}
	}
}
