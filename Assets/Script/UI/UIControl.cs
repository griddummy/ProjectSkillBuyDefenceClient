using UnityEngine;
using System.Collections;

public class UIControl : MonoBehaviour
{
	//simple data field
	[SerializeField] int gold;

	//complex data field -> UI Component
	[SerializeField] UnitProcess presentUnit;
	[SerializeField] UIUnitStatus unitStatus;
	[SerializeField] UICommandButton commandButton;
	[SerializeField] UIUnitSkillSlot unitSkills;
	[SerializeField] UIBuySkillSlot buySkills;

	//complex data field -> temp data
	[SerializeField] UISkillElement presentSelectSkill;

	//property
	public int Gold { get { return gold; } set { gold = value; } }

	//initialize this script
	void Start()
	{
		gold = 2000;
		LinkComponent();
	}

	//link low rank element
	void LinkComponent()
	{
		unitStatus = GetComponentInChildren<UIUnitStatus>();

		unitSkills = GetComponentInChildren<UIUnitSkillSlot>();
		unitSkills.LinkComponent();

		buySkills = GetComponentInChildren<UIBuySkillSlot>();
		buySkills.LinkComponent();
	}


	//public method

	//update ui Information
	public void UpdateUIInformation( UnitProcess data )
	{
		if (data != null)
		{
			unitStatus.ActiveComponent();
			unitStatus.UpdateStatus( data );
		}
		else
		{
			unitStatus.RestComponent();
		}


		//update data


	}

	//skill buy process
	public bool SkillBuyProcess( Skill data )
	{
		//input skilldata by unit skill
		if (presentUnit.Info.AddSkill( data ))
		{
			//synchroize skill info unit & UIUnitSkill
			unitSkills.UpdateSkillSlot( presentUnit );
			return true;
		}
		else
			return false;
	}

	public void UnitSkillSynchroize()
	{
		Debug.Log( "Synchroize Success" );
	}
}
