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
	public void AddUnitHPBar(CellUnit unit)
	{
		unitHealthMap.Add(unit, Instantiate(healthBarPrefab, sliderParent));
		unit.onHealthAltered.AddListener(UpdateHealthBar);
	}

	private void Update()
	{
		foreach (var key in unitHealthMap.Keys)
		{
			var screenUnitTransform = gameCamera.WorldToScreenPoint(key.transform.position);
			(unitHealthMap[key].transform as RectTransform).anchoredPosition3D = screenUnitTransform;
		}
	}

	private void UpdateHealthBar(CellUnit unit)
	{
		unitHealthMap[unit].value = unit.CurrentHealth;
	}
		
}
