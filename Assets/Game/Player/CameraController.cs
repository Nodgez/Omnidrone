using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	public float MaxSpeed = 1f;

	private int xMoveRange;
	private int zMoveRange;
	private Camera playerCamera;
	private Vector2 currentMovement;
	private Vector2 desiredMovement;

	public Ray GetRay
	{
		get	{ return playerCamera.ScreenPointToRay(Input.mousePosition); }
	}

	private void Start()
	{
		playerCamera = GetComponent<Camera>();

		xMoveRange = Battlefield.Instance.ColumnCount;
		zMoveRange = Battlefield.Instance.RowCount / 2;

		transform.localRotation = Quaternion.Euler(45, 0, 0);
	}

	public void UpdateTarget(Vector3 delta)
	{
		desiredMovement = delta;
		//direction = delta.normalized;
		//targetSpeed = delta.magnitude;
	}

	private void Update()
	{
		currentMovement = currentMovement * 0.4f + desiredMovement * 0.6f;
		transform.Translate(currentMovement * Time.deltaTime);

		var x = Mathf.Clamp(transform.position.x, 0, xMoveRange);
		var z = Mathf.Clamp(transform.position.z, -zMoveRange, zMoveRange);

		transform.position = new Vector3(x, 10, z);
	}
}
