using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public Camera raycastCamera;
	public LayerMask detectionMask;
	private void Update()
	{
		RaycastHit hitinfo;
		var ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, detectionMask))
		{
			if (Input.GetMouseButtonDown(0))
			{
				var interactable = hitinfo.collider.GetComponent<IInteractable>();
				interactable.Interact();
			}
		}

	}
}
