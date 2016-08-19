
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISkillElement :MonoBehaviour
{
	//complex data field
	[SerializeField] Image skillIcon;
	[SerializeField] Skill skillInfo;

	//property
	public Skill SkillInfo { get { return skillInfo; } set { skillInfo = value; } }

	public Image SkillIcon { get { return skillIcon; } }

	public virtual void LinkElement()
	{
		skillIcon = GetComponent<Image>();
	}

	public void UpdateSkillIcon( UnitInformation data )
	{
		skillInfo.SetSkillIcon();
		skillIcon.sprite = skillInfo.Icon;
	}

}