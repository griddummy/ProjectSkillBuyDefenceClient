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
	[SerializeField] UIUnitSkillSlot playerSkills;
	[SerializeField] UIBuySkillSlot buySkills;

	//complex data field -> temp data
	[SerializeField]  UISkillElement presentSelectSkill;

	//initialize this script
	void Start()
	{
		gold = 2000;
		LinkComponent();
	}

	//link low rank element
	void LinkComponent()
	{
		playerSkills = GetComponentInChildren<UIUnitSkillSlot>();
		playerSkills.LinkComponent();

		buySkills = GetComponentInChildren<UIBuySkillSlot>();
		buySkills.LinkComponent();
	}


	//public method

	//update ui Information
	public void UpdateUIInformation()
	{

	}
}
