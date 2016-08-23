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
	[SerializeField] protected float damage;
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

	public int SkillBuyCost { get { return skillBuyCost; } }

	public int Level { get { return level; } }

	public float Damage { get { return damage; } }

	public float CoolTime { get { return coolTime; } }

	public float SkillCost { get { return skillCost; } }

	public float SkillRange { get { return skillRange; } }

	public bool OnTarget { get { return onTarget; } }

	public Type SkillType { get { return skillType; } }

	public Sprite Icon { get { return icon; } }

	public Buff SkillBuff { get { return skillBuff; } }

	//constructor - default
	public Skill ()
	{
		id = 0;
		name = "Default";
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

	public static bool Compare( Skill data1, Skill data2 )
	{
		if (data1.name == data2.name)
			return true;
		else
			return false;
	}
}
