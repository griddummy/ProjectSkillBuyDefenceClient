using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//simple data field
	[SerializeField] bool modeAttack;
	[SerializeField] bool modeSkill;

	//complex data field
	[SerializeField] int presentSkillIndex;
	[SerializeField] UnitProcess player;

	//initialize this script
	void Start()
	{
		InitializeData();
	}

	//initialize this script
	void Update()
	{			
		if (!EventSystem.current.IsPointerOverGameObject())
		{			
			if (Input.GetButtonDown( "LeftClick" ))
				ProcessLeftClick();
			else if (Input.GetButtonDown( "RightClick" ))
				ProcessRightClick();			
		}

		if (Input.GetButtonDown( "CommandStop" ))
			player.SetStop();
		else if (Input.GetButtonDown( "CommandHold" ))
			player.SetHold();
		else if (Input.GetButtonDown( "CommandAttack" ))
			modeAttack = true;
		else if (Input.GetButtonDown( "Skill1" ))
			SkillCasting( 0 );
		else if (Input.GetButtonDown( "Skill2" ))
			SkillCasting( 1 );
		else if (Input.GetButtonDown( "Skill3" ))
			SkillCasting( 2 );
		else if (Input.GetButtonDown( "Skill4" ))
			SkillCasting( 3 );
		else if (Input.GetButtonDown( "Skill5" ))
			SkillCasting( 4 );
		else if (Input.GetButtonDown( "Skill6" ))
			SkillCasting( 5 );	
	}

	void InitializeData()
	{
		modeAttack = false;
		modeSkill = false;
		presentSkillIndex = -1;
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
		//clickmode - skill target set
		else if (modeSkill && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			player.ActiveSkill( presentSkillIndex, hitInfo.collider.gameObject );
			modeSkill = false;
			return;
		}
		else if (modeSkill && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			player.ActiveSkill( presentSkillIndex, hitInfo.point );
			modeSkill = false;
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

	void SkillCasting( int index )
	{
		if (player.Info.ActiveSkillSet[index].OnTarget && !player.Info.OnSkill[index])
		{
			modeSkill = true;
			presentSkillIndex = index;
		}
		else
			player.ActiveSkill( index );		
	}
}
