using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	//simple data field
	[SerializeField] bool modeAttack;
	[SerializeField] bool modeSkill;
	[SerializeField] int playerNum;
	[SerializeField] int presentSkillIndex;

	//complex data field
	[SerializeField] GameManager manager;
	[SerializeField] UnitProcess selectedUnit;
	[SerializeField] UIControl mainUI;

	//property
	public UnitProcess SelectedUnit { get { return selectedUnit; } }

	//initialize this script
	void Start()
	{
		manager = GetComponent<GameManager>();
		mainUI = GameObject.FindWithTag( "MainUI" ).GetComponent<UIControl>();
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

		if (CheckSelectedUnit())
		{
			if (Input.GetButtonDown( "CommandStop" ))
				selectedUnit.SetStop();
			else if (Input.GetButtonDown( "CommandHold" ))
				selectedUnit.SetHold();
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


	}

	void LateUpdate()
	{
		mainUI.UpdateUIInformation( selectedUnit );
	}

	void InitializeData()
	{
		modeAttack = false;
		modeSkill = false;
		presentSkillIndex = -1;
		playerNum = manager.PlayerNumber;
	}

	//click mouse left button
	void ProcessLeftClick()
	{		
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;

		// clickmode - attack , left click on enemy unit
		if (CheckSelectedUnit() && modeAttack && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			selectedUnit.SetAttackTarget( hitInfo.transform.gameObject );
			modeAttack = false;
			return;
		}
		// clickmode - attack , left click on terrain point
		else if (CheckSelectedUnit() && modeAttack && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			selectedUnit.SetAttackDestination( hitInfo.point );
			modeAttack = false;
			return;
		}
		//clickmode - skill target set
		else if (CheckSelectedUnit() && modeSkill && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			selectedUnit.ActiveSkill( presentSkillIndex, hitInfo.collider.gameObject );
			modeSkill = false;
			return;
		}
		else if (CheckSelectedUnit() && modeSkill && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			selectedUnit.ActiveSkill( presentSkillIndex, hitInfo.point );
			modeSkill = false;
			return;
		}
		else
		{
			int layer = 0;
			layer = 1 << LayerMask.NameToLayer( "Player" );
			layer |= 1 << LayerMask.NameToLayer( "Ally" );
			layer |= 1 << LayerMask.NameToLayer( "Enemy" );
			if (Physics.Raycast( ray, out hitInfo, Mathf.Infinity, layer ))
			{
				selectedUnit = hitInfo.collider.gameObject.GetComponent <UnitProcess>();
			}
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
		if (CheckSelectedUnit() && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Enemy" ) ))
		{
			selectedUnit.SetAttackTarget( hitInfo.transform.gameObject );
			return;
		}
		//right click on enemy unit
		else if (CheckSelectedUnit() && Physics.Raycast( ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer( "Terrain" ) ))
		{
			selectedUnit.SetDestination( hitInfo.point );		
			return;
		}
	}

	bool CheckSelectedUnit()
	{
		if (selectedUnit == null)
		{
			Debug.Log( "1" );
			return false;
		}
		else if (selectedUnit.Info.PlayerNumber == playerNum)
		{
			Debug.Log( "2" );
			return true;
		}
		else
		{
			Debug.Log( "3" );
			return false;
		}
	}

	void SkillCasting( int index )
	{
		if (selectedUnit.Info.ActiveSkillSet[index].OnTarget && !selectedUnit.Info.OnSkill[index])
		{
			modeSkill = true;
			presentSkillIndex = index;
		}
		else
			selectedUnit.ActiveSkill( index );		
	}
}
