using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//simple data field
	[SerializeField] bool modeAttack;
	[SerializeField] bool modeSkill;

	//complex data field
	[SerializeField] Skill presentSkill;
	[SerializeField] UnitProcess player;

	//initialize this script
	void Start()
	{
		player = GetComponent<UnitProcess>();
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
		else if (Input.GetButtonDown( "Skill1" ))
		{
			if (player.Info.ActiveSkillSet[0].OnTarget)
			{
				modeSkill = true;
				presentSkill = player.Info.ActiveSkillSet[0];
			}
			else
				player.ActiveSkill( 0 );
		}
	}

	void InitializeData()
	{
		modeAttack = false;
		modeSkill = false;
		presentSkill = new Skill ();
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
		if (modeAttack || modeSkill)
		{
			modeSkill = false;
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
