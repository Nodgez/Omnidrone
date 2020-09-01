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
	private int columns = 10, rows = 10, cellSize = 1;
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
		if (EditorApplication.isPlayingOrWillChangePlaymode)
			return;
		DrawGenerationFields();
		DrawCellUnitSelection();
	}

	private void DrawCellUnitSelection()
	{
		GUILayout.Label("Cell Units");
		assetScroll = GUILayout.BeginScrollView(assetScroll);
		RenderUnitSelectionForArmy("Player/");
		RenderUnitSelectionForArmy("AI/");

		GUILayout.EndScrollView();
	}

	private void RenderUnitSelectionForArmy(string armyFolder)
	{
		GUILayout.Label(armyFolder);
		var dirInfo = new DirectoryInfo(Application.dataPath + CELL_UNIT_PATH + armyFolder);
		var playerUnits = dirInfo.GetFiles("*.prefab");
		//var cellUnitsPaths = AssetDatabase.();
		foreach (var unit in playerUnits)
		{
			var buttonText = unit.Name.Substring(0, unit.Name.Length - 7);
			if (GUILayout.Button(buttonText))
			{
				var prefab = AssetDatabase.LoadAssetAtPath("Assets" + CELL_UNIT_PATH + armyFolder + unit.Name, typeof(GameObject));
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

		columns = EditorGUILayout.IntField("Width", columns);
		rows = EditorGUILayout.IntField("Breath", rows);
		defaultMaterial = EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), false) as Material;
		if (GUILayout.Button("Create Cells"))
		{
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			battlefield.Initialize(columns, rows);
			for (var x = 0; x < columns; x++)
			{
				for (var z = 0; z < rows; z++)
				{
					var newCell = new GameObject(string.Format("Cell [{0}, {1}]", x, z)).AddComponent<Cell>();
					newCell.SetUp(columns, rows, new Vector2Int(x, z));
					battlefield.cellData.Add(newCell);
				}
			}
			new CellConfig(cellSize);
			battlefield.RenderCells(defaultMaterial);
		}
		if (GUILayout.Button("Clear Cells"))
		{
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			battlefield.cellData.Clear();
			battlefield.transform.Find("Cells").ClearChildrenImmediate();
			battlefield.transform.Find("Units").ClearChildrenImmediate();
		}

		if (GUILayout.Button("Mark Unwalkable"))
		{
			var selectedCellObjects = Selection.objects.Where(x => IsSelectedObjectCell(x));
			foreach (var cell in selectedCellObjects)
			{
				var cellComponent = (cell as GameObject).GetComponent<Cell>();
				cellComponent.unWalkable = true;
				(cell as GameObject).GetComponent<MeshRenderer>().enabled = false;
			}
		}

		GUILayout.EndVertical();
	}

	private bool IsSelectedObjectCell(UnityEngine.Object obj)
	{
		var gameObj = obj as GameObject;
		return gameObj.GetComponent<Cell>() != null;
	}
}