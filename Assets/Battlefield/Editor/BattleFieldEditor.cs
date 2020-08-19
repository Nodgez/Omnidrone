using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Battlefield))]
public class BattleFieldEditor : EditorWindow
{
	private int width = 10, breath = 10, cellSize = 1;
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
				battlefield.tag = "Battlefield";
			}
			return;
		}

		width = EditorGUILayout.IntField("Width", width);
		breath = EditorGUILayout.IntField("Breath", breath);
		cellSize = EditorGUILayout.IntField("Size", cellSize);
		defaultMaterial = EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), false) as Material;
		if (GUILayout.Button("Create Cells"))
		{
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < breath; z++)
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
				for (var z = 0; z < breath; z++)
				{
					battlefield.cellData.Clear();
				}
			}
			battlefield.transform.ClearChildrenImmediate();
		}
		
		if (GUILayout.Button("Test Path"))
		{
			battlefield.Search(battlefield.cellData.Find(c => c.point == new Vector2Int(0, 1)), 5).ForEach(pathCell => pathCell.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green));
		}
	}
}