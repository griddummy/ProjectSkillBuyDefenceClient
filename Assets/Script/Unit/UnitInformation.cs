using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitInformation
{
	//simple data field
	[SerializeField] int unitID;
	[SerializeField] int playerNum;
	[SerializeField] string unitName;
	[SerializeField] int level;
	[SerializeField] float presentExp;
	[SerializeField] float requireExp;
	[SerializeField] float healthPoint;
	[SerializeField] float presentHealthPoint;
	[SerializeField] float manaPoint;
	[SerializeField] float presentManaPoint;
	[SerializeField] float damage;
	[SerializeField] float moveSpeed;
	[SerializeField] float attackSpeed;
	[SerializeField] float attackRange;
	[SerializeField] float searchRange;

	//complex data field
	[SerializeField] Skill[] unitSkillSet;
	[SerializeField] bool[] onSkill;
	[SerializeField] float[] coolTime;
	[SerializeField] Buff[] unitBuffSet;

	//property
	public int UnitID { get { return unitID; } }

	public int PlayerNumber { get { return playerNum; } }

	public string Name { get { return unitName; } }

	public int Level { get { return level; } }

	public float PresentExp { get { return PresentExp; } }

	public float HealthPoint { get { return healthPoint; } }

	public float PresentHealthPoint { get { return presentHealthPoint; } set { presentHealthPoint = Mathf.Clamp( presentHealthPoint + value, 0, healthPoint ); } }

	public float ManaPoint { get { return manaPoint; } }

	public float Damage { get { return damage; } }

	public float MoveSpeed { get { return moveSpeed; } }

	public float AttackSpeed { get { return attackSpeed; } }

	public float AttackRange { get { return attackRange; } }

	public float SearchRange { get { return searchRange; } }

	public Skill[] UnitSkillSet { get { return unitSkillSet; } }

	public bool[] OnSkill { get { return onSkill; } }

	public float[] CoolTime { get { return coolTime; } }

	public Buff[] UnitBuffSet { get { return unitBuffSet; } }

	public UnitInformation ()
	{
		level = 1;
		presentExp = 0f;
		requireExp = 1000f;
		healthPoint = 1000;
		presentHealthPoint = healthPoint;
		manaPoint = 100;
		presentManaPoint = manaPoint;
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

		SkillInitalize();
	}

	public UnitInformation (int _unitID, int _playerNum, UnitInformation info)
	{
		unitID = _unitID;
		playerNum = _playerNum;
		level = info.level;
		presentExp = info.presentExp;
		requireExp = info.requireExp;
		healthPoint = info.healthPoint;
		manaPoint = info.manaPoint;
		moveSpeed = info.moveSpeed;
		attackSpeed = info.attackSpeed;
		attackRange = info.attackRange;
		searchRange = info.searchRange;

		SkillInitalize();
	}

	//pubilc method

	//clear skill
	public void SkillInitalize()
	{
		unitSkillSet = new Skill[8];
		for (int i = 0; i < unitSkillSet.Length; i++)
			unitSkillSet[i] = new Skill ();

		onSkill = new bool[unitSkillSet.Length];
		coolTime = new float[unitSkillSet.Length];
	}

	//data initialize
	public void DataInitialize()
	{
		
	}

	//skill add skill slot
	public bool AddSkill( Skill data )
	{
		for (int i = 0; i < unitSkillSet.Length; i++)
		{
			if (unitSkillSet[i].Name == null)
				unitSkillSet[i] = data;
				
			return true;
		}
		return false;
	}

	//apply buff effect
	public void ApplyBuff(	float _damage, float _attackSpeed, float _moveSpeed, float _healthPoint, float _manaPoint )
	{
		damage += _damage;
		attackSpeed += _attackSpeed;
		moveSpeed += _moveSpeed;
		healthPoint += _healthPoint;
		manaPoint += _manaPoint;
	}
}