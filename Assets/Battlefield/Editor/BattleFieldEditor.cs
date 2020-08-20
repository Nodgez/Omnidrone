using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Battlefield))]
public class BattleFieldEditor : EditorWindow
{
	private int width = 10, bredth = 10, cellSize = 1;
	private Material defaultMaterial;
	private Battlefield battlefield;
	[MenuItem("RPG Arena/Battlefield Generator")]
	static void Init()
	{
		BattleFieldEditor window = (BattleFieldEditor)EditorWindow.GetWindow(typeof(BattleFieldEditor));
		window.Show();
	}

	public void OnHierarchyChange()
	{
		battlefield = GameObject.FindGameObjectWithTag("Battlefield").GetComponent<Battlefield>();
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		Undo.RecordObject(battlefield.gameObject, "battlefield");
	}

	public void OnGUI()
	{
		if (battlefield == null)
		{
			if (GUILayout.Button("Initialize Battlefield"))
			{
				var go = new GameObject("Battlefield");
				Undo.RecordObject(go, "battlefield");
				battlefield = go.AddComponent<Battlefield>();
			}
			return;
		}

		width = EditorGUILayout.IntField("Width", width);
		bredth = EditorGUILayout.IntField("Breath", bredth);
		cellSize = EditorGUILayout.IntField("Size", cellSize);
		defaultMaterial = EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), false) as Material;
		if (GUILayout.Button("Create Cells"))
		{
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
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < bredth; z++)
				{
					battlefield.cellData.Clear();
				}
			}
			battlefield.transform.ClearChildrenImmediate();
		}
		
		if (GUILayout.Button("Test Path"))
		{
			var startIndex = Cell.Point2Index(new Vector2Int(0, 1), width);
			var endIndex = Cell.Point2Index(new Vector2Int(3, 6), width);
			var materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", Color.green);
			battlefield.FindPath(battlefield.cellData[startIndex], battlefield.cellData[endIndex]).ForEach(
				pathCell => 
				pathCell.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock)
			);
		}
	}
}