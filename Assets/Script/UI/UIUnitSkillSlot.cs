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

}
