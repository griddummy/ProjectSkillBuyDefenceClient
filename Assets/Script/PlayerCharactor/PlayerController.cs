using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//simple data field
	[SerializeField] bool modeAttack;

	//complex data field
	[SerializeField] PlayerProcess player;

	//initialize this script
	void Start()
	{
		player = GetComponent<PlayerProcess>();
		InitializeData();
	}


	void Update()
	{
		if (Input.GetButtonDown( "CommandStop" ))
			player.SetStop();
		else if (Input.GetButtonDown( "CommandHold" ))
			player.SetHold();
		else if (Input.GetButtonDown( "CommandAttack" ))
			modeAttack = true;
		else if (Input.GetButtonDown( "LeftClick" ))
			ProcessLeftClick();
		else if (Input.GetButtonDown( "RightClick" ))
			ProcessRightClick();
	}

	void InitializeData()
	{
		modeAttack = false;
	}

	//click mouse left button
	void ProcessLeftClick()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		// clickmode - attack , left click on enemy unit
		if (modeAttack && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			player.SetAttackTarget( hitInfo.transform.gameObject );
			modeAttack = false;
			return;
		}
		// clickmode - attack , left click on terrain point
		else if (modeAttack && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			player.SetAttackDestination( hitInfo.point );
			modeAttack = false;
			return;
		}
		
	}

	//click mouse right button
	void ProcessRightClick()
	{
		//if clickmode - attack
		if (modeAttack)
		{
			modeAttack = false;
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		//right click on terrain point
		if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			player.SetAttackTarget( hitInfo.transform.gameObject );
			return;
		}
		//right click on enemy unit
		else if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			player.SetDestination( hitInfo.point );		
			return;
		}
	}
}
