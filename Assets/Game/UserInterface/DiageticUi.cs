using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiageticUi : MonoBehaviour
{
	private static DiageticUi instance;
	public static DiageticUi Instance
	{
		get { return instance; }
	}

	public Camera gameCamera;
	public Slider healthBarPrefab;
	public RectTransform sliderParent;
	private Dictionary<CellUnit, Slider> unitHealthMap = new Dictionary<CellUnit, Slider>();

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);
	}
	public void AddUnitHPBar(CellUnit cellUnit)
	{
		var newhealthbarInstance = Instantiate(healthBarPrefab, sliderParent);
		newhealthbarInstance.maxValue = cellUnit.MaxHealth;
		newhealthbarInstance.value = cellUnit.MaxHealth;
		unitHealthMap.Add(cellUnit, newhealthbarInstance);

		cellUnit.onHealthAltered.AddListener((unit) => unitHealthMap[unit].value = unit.CurrentHealth);
		cellUnit.onDeath.AddListener(ClearHealthBar);
	}

	private void Update()
	{
		foreach (var key in unitHealthMap.Keys)
		{
			var screenUnitTransform = gameCamera.WorldToScreenPoint(key.transform.position);
			(unitHealthMap[key].transform as RectTransform).anchoredPosition3D = screenUnitTransform;
		}
	}

	private void ClearHealthBar(CellUnit unit)
	{
		Destroy(unitHealthMap[unit].gameObject);
		unitHealthMap.Remove(unit);
	}
		
}
