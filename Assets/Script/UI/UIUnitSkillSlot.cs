using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIUnitSkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] UISkillElement[] unitSkills;

	//initialize this script
	public void LinkComponent()
	{
		unitSkills = GetComponentsInChildren<UISkillElement>();

		//initialize UISkillElement
		foreach (UISkillElement elements in unitSkills)
			elements.LinkElement();
	}

	//pointer enter ui area
	public void OnPointerEnter( PointerEventData eventData )
	{

	}

	//pointer exit ui area
	public void OnPointerExit( PointerEventData eveneData )
	{

	}

	public bool AddSkill( Skill data )
	{
		for (int i = 0; i < unitSkills.Length; i++)
		{
			if (unitSkills[i].SkillInfo.Name == null)
			{
				unitSkills[i].SkillInfo = data;
				return true;
			}
		}
		return false;
	}
}
