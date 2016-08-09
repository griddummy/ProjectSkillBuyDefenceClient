using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//complex data field
	[SerializeField] PlayerProcess playerChar;

	//initialize this script
	void Start()
	{
		playerChar = GetComponent<PlayerProcess>();
	}


	void Update()
	{
		if (Input.GetButtonDown( "Move" ))
			SetDestination();
	}

	void SetDestination()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			playerChar.Destination = hitInfo.point;
		}

		Debug.Log( playerChar.Destination );
	}
}
