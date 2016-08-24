using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MinimapMove : MonoBehaviour
{
	[SerializeField] CameraMove mainCamera;
	[SerializeField] Camera minimap;

	//initialize this script
	void Start()
	{
		mainCamera = GameObject.FindWithTag( "MainCamera" ).GetComponent<CameraMove>();
		minimap = GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{			
			if (Input.GetButton( "LeftClick" ))
				ProcessMinimapMove();
		}
	}

	public void ProcessMinimapMove()
	{
		Ray ray = minimap.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "MinimapBackGround" ) ))
		{
			mainCamera.Position = new Vector3 ( hitInfo.point.x, Camera.main.transform.position.y, hitInfo.point.z - 10f );
		}
	}
}
