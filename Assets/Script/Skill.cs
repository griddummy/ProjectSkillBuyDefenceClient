using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Skill
{
	//skill data
	[SerializeField] string name;
	[SerializeField] float damage;
	[SerializeField] float coolTime;
	[SerializeField] float useTime;
	[SerializeField] bool onTarget;
	[SerializeField] Type skillType;

	public enum Type
	{
		Active,
		Passive,
		Buff}
;
	//property
	public string Name { get { return name; } }

	public float CoolTime { get { return coolTime; } }

	public bool OnTarget { get { return onTarget; } }

	public Type SkillType { get { return skillType; } }

	//constructor - default
	public Skill ()
	{
		name = null;
		damage = 0.0f;
		useTime = 0.0f;
		coolTime = 5.0f;
		onTarget = false;
	}

	//constructor - direct data
	public Skill (string _name, float _damage, float _useTime, Type _skillType)
	{
		name = _name;
		damage = _damage;
		useTime = _useTime;
		skillType = _skillType;
	}

	//public method
	public virtual void UseSkill()
	{

	}
}
