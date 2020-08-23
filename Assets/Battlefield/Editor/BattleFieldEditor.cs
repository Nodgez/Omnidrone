using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Linq;

public class BattleFieldEditor : EditorWindow
{
	private int width = 10, bredth = 10, cellSize = 1;
	private Material defaultMaterial;
	private Battlefield battlefield;
	private Vector2 assetScroll;

	private Dictionary<string, ArmyController> armyControllers = new Dictionary<string, ArmyController>();

	private const string CELL_UNIT_PATH = "/CellUnits/Prefabs/";
	[MenuItem("RPG Arena/Battlefield Generator")]
	static void Init()
	{
		BattleFieldEditor window = (BattleFieldEditor)EditorWindow.GetWindow(typeof(BattleFieldEditor));
		window.Show();
	}

	public void OnHierarchyChange()
	{
		battlefield = GameObject.FindGameObjectWithTag("Battlefield").GetComponent<Battlefield>();
		armyControllers = FindObjectsOfType<ArmyController>().ToDictionary(x => x.tag);
		foreach (var key in armyControllers.Keys)
			armyControllers[key].ClearEmptyUnits();
		Undo.RecordObject(battlefield.gameObject, "battlefield");
	}

	public void OnGUI()
	{
		DrawGenerationFields();
		DrawCellUnitSelection();
	}

	private void DrawCellUnitSelection()
	{
		assetScroll = GUILayout.BeginScrollView(assetScroll);
		var dirInfo = new DirectoryInfo(Application.dataPath + CELL_UNIT_PATH);
		var cellUnitFiles = dirInfo.GetFiles("*.prefab");
		//var cellUnitsPaths = AssetDatabase.();
		foreach (var unit in cellUnitFiles)
		{
			if (GUILayout.Button(unit.Name))
			{

				var prefab = AssetDatabase.LoadAssetAtPath("Assets" + CELL_UNIT_PATH + unit.Name, typeof(GameObject));
				var selectedCellObjects = Selection.objects.Where(x => IsSelectedObjectCell(x));
				foreach (var cell in selectedCellObjects)
				{
					var cellComponent = (cell as GameObject).GetComponent<Cell>();
					var cellUnitInstance = (PrefabUtility.InstantiatePrefab(prefab, battlefield.transform.Find("Units")) as GameObject).GetComponent<CellUnit>(); ;

					cellUnitInstance.SetOnCell(cellComponent);
					cellComponent.cellUnit = cellUnitInstance;
					cellUnitInstance.transform.position = cellComponent.transform.position;

					armyControllers[cellUnitInstance.tag].AppendToArmy(cellUnitInstance);
				}

				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

		GUILayout.EndScrollView();
	}

	private void DrawGenerationFields()
	{
		GUILayout.BeginVertical(GUILayout.Width(400));
		if (battlefield == null)
		{
			if (GUILayout.Button("Initialize Battlefield"))
			{
				var field = new GameObject("Battlefield");
				new GameObject("Cells").transform.SetParent(field.transform);
				new GameObject("Units").transform.SetParent(field.transform);
				Undo.RecordObject(field, "battlefield");
				battlefield = field.AddComponent<Battlefield>();
			}
			return;
		}

		width = EditorGUILayout.IntField("Width", width);
		bredth = EditorGUILayout.IntField("Breath", bredth);
		cellSize = EditorGUILayout.IntField("Size", cellSize);
		defaultMaterial = EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), false) as Material;
		if (GUILayout.Button("Create Cells"))
		{
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			battlefield.Initialize(width, bredth);
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < bredth; z++)
				{
					var newCell = new GameObject(string.Format("Cell [{0}, {1}]", x, z)).AddComponent<Cell>();
					newCell.SetUp(cellSize, new Vector2Int(x, z));
					battlefield.cellData.Add(newCell);
				}
			}
			new CellConfig(cellSize);
			battlefield.RenderCells(defaultMaterial);
		}
		if (GUILayout.Button("Clear Cells"))
		{
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < bredth; z++)
				{
					battlefield.cellData.Clear();
				}
			}
			battlefield.transform.Find("Cells").ClearChildrenImmediate();
			battlefield.transform.Find("Units").ClearChildrenImmediate();
		}

		//if (GUILayout.Button("Test Path"))
		//{
		//	var materialPropertyBlock = new MaterialPropertyBlock();
		//	materialPropertyBlock.SetColor("_Color", Color.white);

		//	battlefield.cellData.ForEach(
		//	pathCell =>
		//		pathCell.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock)
		//	);
		//		var startIndex = Cell.Point2Index(new Vector2Int(1, 1), width);
		//	var endIndex = Cell.Point2Index(new Vector2Int(3, 6), width);
		//	materialPropertyBlock.SetColor("_Color", Color.green);
		//	battlefield.FindPath(battlefield.cellData[startIndex], battlefield.cellData[endIndex]).ForEach(
		//		pathCell =>
		//		pathCell.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock)
		//	);
		//}
		GUILayout.EndVertical();
	}

	private bool IsSelectedObjectCell(UnityEngine.Object obj)
	{
		var gameObj = obj as GameObject;
		return gameObj.GetComponent<Cell>() != null;
	}
}