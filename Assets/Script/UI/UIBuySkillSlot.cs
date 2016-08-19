using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIBuySkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] UISkillElement[] buySkills;


	//initialize this script
	public void LinkComponent()
	{
		buySkills = GetComponentsInChildren<UISkillElement>();

		//initialize UISkillElement
		foreach (UISkillElement elements in buySkills)
			elements.LinkElement();

		buySkills[0].SkillInfo = Database.Instance.FindSkillByID( 0001 );
		buySkills[1].SkillInfo = Database.Instance.FindSkillByID( 0002 );
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
