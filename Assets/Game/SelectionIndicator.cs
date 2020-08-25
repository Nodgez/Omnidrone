using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SelectionIndicator : Indicator
{
	private MeshRenderer meshRenderer;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		var selectionShader = Shader.Find("Outlined/UltimateOutline");
		if (meshRenderer.material.shader != selectionShader)
			meshRenderer.material.shader = selectionShader;
	}
	public override void Indicate()
	{
		var materialSelectionProperty = new MaterialPropertyBlock();
		materialSelectionProperty.SetFloat("_FirstOutlineWidth", 0.25f);
		meshRenderer.SetPropertyBlock(materialSelectionProperty);
	}

	public override void Conceal()
	{
		meshRenderer.SetPropertyBlock(null);
	}
}
