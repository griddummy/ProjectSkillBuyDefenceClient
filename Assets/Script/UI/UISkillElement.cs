using UnityEngine;
using UnityEngine.UI;
using System;
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
		skillInfo = new Skill ();
	}

	public void UpdateSkillIcon()
	{
		skillInfo.SetSkillIcon();
		try
		{
			skillIcon.sprite = skillInfo.Icon;
		}
		catch (NullReferenceException e)
		{
			Debug.Log( e.InnerException );
		}
	}
}