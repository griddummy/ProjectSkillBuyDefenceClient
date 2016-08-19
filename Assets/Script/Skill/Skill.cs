using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Skill
{
	//skill data
	[SerializeField] protected int id;
	[SerializeField] protected string name;
	[SerializeField] protected int skillBuyCost;
	[SerializeField] protected int level;
	[SerializeField] protected float skillCost;
	[SerializeField] protected float skillRange;
	[SerializeField] protected float coolTime;
	[SerializeField] protected bool onTarget;
	[SerializeField] protected Type skillType;
	[SerializeField] protected Sprite icon;
	[SerializeField] protected Buff skillBuff;

	public enum Type : int
	{
		ActiveTarget = 1,
		ActiveTargetArea = 2,
		ActiveNonTarget = 3,
		Passive = 4,
		PassiveAura = 5,
	};

	//property
	public int ID { get { return id; } }

	public string Name { get { return name; } }

	public int SkillBuyCost{ get { return skillBuyCost; } }

	public float CoolTime { get { return coolTime; } }

	public bool OnTarget { get { return onTarget; } }

	public Type SkillType { get { return skillType; } }

	public Sprite Icon { get { return icon; } }

	//constructor - default
	public Skill ()
	{
		id = 0;
		name = null;
		level = 0;
		skillRange = 0f;
		coolTime = 0.0f;
		onTarget = false;
	}

	//public method
	//skill use
	public virtual void UseSkill( UnitProcess data )
	{
	}

	//skill use active target only
	public virtual void UseSkill( Vector3 position )
	{
	}

	//set skill icon
	public void SetSkillIcon()
	{
		string path = "Skill/Skill" + name;
		icon = Resources.Load<Sprite>( path ); 
	}
}
