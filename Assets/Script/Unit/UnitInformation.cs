using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitInformation
{
	//field
	[SerializeField] int level;
	[SerializeField] float presentExp;
	[SerializeField] float requireExp;
	[SerializeField] int healthPoint;
	[SerializeField] int manaPoint;
	[SerializeField] float moveSpeed;
	[SerializeField] float attackSpeed;
	[SerializeField] float attackRange;
	[SerializeField] float searchRange;
	[SerializeField] Skill[] activeSkillSet;
	[SerializeField] Skill[] passiveSkillSet;

	//property
	public int Level { get { return level; } }

	public float PresentExp { get { return PresentExp; } }

	public int HealthPoint { get { return HealthPoint; } }

	public int ManaPoint{ get { return manaPoint; } }

	public float MoveSpeed { get { return moveSpeed; } }

	public float AttackSpeed{ get { return attackSpeed; } }

	public float AttackRange { get { return attackRange; } }

	public float SearchRange { get { return searchRange; } }

	public Skill[] ActiveSkillSet { get { return activeSkillSet; } }

	public Skill[] PassiveSkillSet { get { return passiveSkillSet; } }

	public UnitInformation ()
	{
		level = 1;
		presentExp = 0f;
		requireExp = 1000f;
		healthPoint = 1000;
		manaPoint = 100;
		moveSpeed = 5f;
		attackSpeed = 10f;
		attackRange = 10f;
		searchRange = 10f;

		SkillInitalize();
	}

	public UnitInformation (UnitInformation info)
	{
		level = info.level;
		presentExp = info.presentExp;
		requireExp = info.requireExp;
		healthPoint = info.healthPoint;
		manaPoint = info.manaPoint;
		moveSpeed = info.moveSpeed;
		attackSpeed = info.attackSpeed;
		attackRange = info.attackRange;
		searchRange = info.searchRange;
	}

	public void SkillInitalize()
	{
		activeSkillSet = new Skill[4];
		for (int i = 0; i < activeSkillSet.Length; i++)
			activeSkillSet[i] = new Skill ();

		passiveSkillSet = new Skill[4];
		for (int i = 0; i < passiveSkillSet.Length; i++)
			passiveSkillSet[i] = new Skill ();
	}

	public void AddSkill( Skill data )
	{
		if (data.SkillType == Skill.Type.Active || data.SkillType == Skill.Type.Buff)
			;
		else if (data.SkillType == Skill.Type.Passive)
			;
	}

}
