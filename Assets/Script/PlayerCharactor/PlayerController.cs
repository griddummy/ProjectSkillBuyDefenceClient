using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//simple data field
	[SerializeField] bool modeAttack;

	//complex data field
	[SerializeField] PlayerProcess playerChar;

	//initialize this script
	void Start()
	{
		playerChar = GetComponent<PlayerProcess>();
	}


	void Update()
	{
		if (Input.GetButtonDown( "LeftClick" ))
			ProcessLeftClick();
		else if (Input.GetButtonDown( "RightClick" ))
			ProcessRightClick();


	}

	void InitializeData()
	{
		modeAttack = false;
	}

	void SetDestination()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			playerChar.Destination = hitInfo.point;
		}
	}

	void ProcessLeftClick()
	{

	}

	void ProcessRightClick()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
			playerChar.SetAttackTarget( hitInfo.transform.gameObject );
		else if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
			playerChar.SetDestination( hitInfo.point );
		
	}
}
