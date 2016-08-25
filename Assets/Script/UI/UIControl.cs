using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIControl : MonoBehaviour
{
	//simple data field
	[SerializeField] int gold;
	[SerializeField] int playerNum;

	//complex data field -> UI Component
	[SerializeField] UnitProcess presentUnit;
	[SerializeField] UIUnitStatus unitStatus;
	[SerializeField] UICommandButton commandButton;
	[SerializeField] UIUnitSkillSlot unitSkills;
	[SerializeField] UIBuySkillSlot buySkills;
	[SerializeField] Text moneyText;

	//complex data field -> temp data
	[SerializeField] UISkillElement presentSelectSkill;

	//property
	public int Gold { get { return gold; } set { gold = value; } }

	public int PlayerNumber { get { return playerNum; } set { playerNum = value; } }

	public UIBuySkillSlot BuySkills { get { return buySkills; } }

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
		//update data
		presentUnit = data;

		if (data != null)
		{
			unitStatus.ActiveComponent();
			unitStatus.UpdateStatus( presentUnit );
			unitSkills.UpdateSkillSlot( presentUnit, playerNum );
		}
		else
		{
			unitStatus.RestComponent();
			unitSkills.UpdateDefault();
		}

		moneyText.text = " $ " + gold.ToString();
	}

	//skill buy process
	public bool SkillBuyProcess( Skill data )
	{
		if (( presentUnit == null ) || ( presentUnit.Info.PlayerNumber != playerNum ))
		{
			Debug.Log( "NoBuy!" );
			return false;
		}
		//input skilldata by unit skill
		else if (presentUnit.AddSkill( data ))
		{
			//synchroize skill info unit & UIUnitSkill
			unitSkills.UpdateSkillSlot( presentUnit, playerNum );
			return true;
		}
		else
			return false;
	}
}
