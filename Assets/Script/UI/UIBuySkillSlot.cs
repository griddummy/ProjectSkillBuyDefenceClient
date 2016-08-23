using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIBuySkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] UIControl mainUI;
	[SerializeField] UISkillElement[] buySkills;
	[SerializeField] GameObject backGround;
	[SerializeField] GameObject openTabButton;
	[SerializeField] GameObject closeTabButton;

	//initialize this script
	public void LinkComponent()
	{
		mainUI = GetComponentInParent<UIControl>();

		buySkills = GetComponentsInChildren<UISkillElement>();

		//initialize UISkillElement
		foreach (UISkillElement elements in buySkills)
			elements.LinkElement();

		buySkills[0].SkillInfo = Database.Instance.FindSkillByID( 0001 );
		buySkills[1].SkillInfo = Database.Instance.FindSkillByID( 0002 );
		buySkills[2].SkillInfo = Database.Instance.FindSkillByID( 0003 );
		buySkills[3].SkillInfo = Database.Instance.FindSkillByID( 0004 );
		buySkills[4].SkillInfo = Database.Instance.FindSkillByID( 0005 );

		foreach (UISkillElement elements in buySkills)
			elements.UpdateSkillIcon();

		CloseBuySkillTab();
	}

	//pointer enter ui area
	public void OnPointerEnter( PointerEventData eventData )
	{

	}

	//pointer exit ui area
	public void OnPointerExit( PointerEventData eveneData )
	{

	}

	//onclick method
	public void OnClickBuySkill( int index )
	{
		index--;

		if (buySkills[index].SkillInfo.Name == null)
			return;
		else if (mainUI.Gold >= buySkills[index].SkillInfo.SkillBuyCost)
		{
			if (mainUI.SkillBuyProcess( buySkills[index].SkillInfo ))
				mainUI.Gold -= buySkills[index].SkillInfo.SkillBuyCost;
		}
	}

	//close this ui element
	public void OpenBuySkillTab()
	{
		backGround.SetActive( true );
		closeTabButton.SetActive( true );
		openTabButton.SetActive( false );
	}

	//open this ui element
	public void CloseBuySkillTab()
	{
		backGround.SetActive( false );
		closeTabButton.SetActive( false );
		openTabButton.SetActive( true );
	}
}
