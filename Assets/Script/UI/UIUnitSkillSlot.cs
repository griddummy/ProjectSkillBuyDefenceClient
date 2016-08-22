using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIUnitSkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] UISkillElement[] activeSkills;
	[SerializeField] UISkillElement[] passiveSkills;

	//initialize this script
	public void LinkComponent()
	{
		activeSkills = new UISkillElement[6];
		passiveSkills = new UISkillElement[6];

		for (int i = 0; i < activeSkills.Length; i++)
		{
			string name = "ActiveSkill" + ( i + 1 ).ToString();
			activeSkills[i] = transform.Find( name ).GetComponent<UISkillElement>();
		}

		for (int i = 0; i < passiveSkills.Length; i++)
		{
			string name = "PassiveSkill" + ( i + 1 ).ToString();
			passiveSkills[i] = transform.Find( name ).GetComponent<UISkillElement>();
		}	
	}

	//pointer enter ui area
	public void OnPointerEnter( PointerEventData eventData )
	{

	}

	//pointer exit ui area
	public void OnPointerExit( PointerEventData eveneData )
	{

	}

	//skill slot data update
	public void UpdateSkillSlot( UnitProcess data )
	{
		for (int i = 0; i < data.Info.ActiveSkillSet.Length; i++)
		{
			activeSkills[i].SkillInfo = data.Info.ActiveSkillSet[i];
			//unitSkills[i].UpdateSkillICon();
		}

		for (int i = 0; i < data.Info.PassiveSkillSet.Length; i++)
		{
			passiveSkills[i].SkillInfo = data.Info.PassiveSkillSet[i];
			//unitSkills[i].UpdateSkillICon();
		}
	}
}
