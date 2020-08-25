using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public CameraController raycastCamera;
	public LayerMask detectionMask;

	public Vector3 MouseDelta { get; private set; }
	private Vector3 mousePositionLastFrame;
	private Vector3 mouseInputBegin;
	private void Update()
	{
		MouseDelta = mousePositionLastFrame - Input.mousePosition;

		if (Input.GetMouseButtonDown(0))
			mouseInputBegin = Input.mousePosition;

		if (Input.GetMouseButton(0))
			raycastCamera.UpdateTarget(MouseDelta);

		RaycastHit hitinfo;
		var ray = raycastCamera.GetRay;
		if (Input.GetMouseButtonUp(0))
		{
			raycastCamera.UpdateTarget(Vector3.zero);
			if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity, detectionMask))
			{
				var distanceFromInputBegin = (Input.mousePosition - mouseInputBegin).magnitude;
				if (distanceFromInputBegin < 20)
				{
					var interactable = hitinfo.collider.GetComponent<IInteractable>();
					interactable.Interact();
				}
			}
		}

		mousePositionLastFrame = Input.mousePosition;
	}
}
