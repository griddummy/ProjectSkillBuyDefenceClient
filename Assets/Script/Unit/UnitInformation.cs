using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitInformation
{
	//simple data field
	[SerializeField] int defaultID;
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
	[SerializeField] bool isMeleeAttack;
	[SerializeField] float attackRange;
	[SerializeField] float searchRange;

	//complex data field
	[SerializeField] Skill[] activeSkillSet;
	[SerializeField] Skill[] passiveSkillSet;
	[SerializeField] bool[] onSkill;
	[SerializeField] float[] coolTime;
	[SerializeField] Buff[] unitBuffSet;

	//property
	public int DefaultID { get { return defaultID; } }

	public int UnitID { get { return unitID; } }

	public int PlayerNumber { get { return playerNum; } }

	public string Name { get { return unitName; } }

	public int Level { get { return level; } }

	public float PresentExp { get { return PresentExp; } }

	public float HealthPoint { get { return healthPoint; } }

	public float PresentHealthPoint { get { return presentHealthPoint; } set { presentHealthPoint = Mathf.Clamp( value, 0, healthPoint ); } }

	public float ManaPoint { get { return manaPoint; } }

	public float PresentManaPoint { get { return presentManaPoint; } set { presentManaPoint = Mathf.Clamp( value, 0, manaPoint ); } }

	public float Damage { get { return damage; } }

	public float MoveSpeed { get { return moveSpeed; } }

	public float AttackSpeed { get { return attackSpeed; } }

	public bool IsMelee { get { return isMeleeAttack; } }

	public float AttackRange { get { return attackRange; } }

	public float SearchRange { get { return searchRange; } }

	public Skill[] ActiveSkillSet { get { return activeSkillSet; } }

	public Skill[] PassiveSkillSet { get { return passiveSkillSet; } }

	public bool[] OnSkill { get { return onSkill; } }

	public float[] CoolTime { get { return coolTime; } }

	public Buff[] UnitBuffSet { get { return unitBuffSet; } }

	public UnitInformation ()
	{
		unitName = "default";
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
        unitID = info.unitID;
        playerNum = info.playerNum;
		unitName = info.unitName;
		level = info.level;
		presentExp = info.presentExp;
		requireExp = info.requireExp;
		healthPoint = info.healthPoint;
		presentHealthPoint = healthPoint;
		manaPoint = info.manaPoint;
		presentManaPoint = manaPoint;
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
		unitName = info.unitName;
		level = info.level;
		presentExp = info.presentExp;
		requireExp = info.requireExp;
		healthPoint = info.healthPoint;
		presentHealthPoint = healthPoint;
		manaPoint = info.manaPoint;
		presentManaPoint = manaPoint;
		moveSpeed = info.moveSpeed;
		attackSpeed = info.attackSpeed;
		attackRange = info.attackRange;
		searchRange = info.searchRange;

		SkillInitalize();
	}

    public UnitInformation(InGameCreateUnitData createData, UnitData unitData, UnitLevelData unitLevelData)
    {
        this.unitID = createData.identity.unitId;
        this.playerNum = createData.identity.unitOwner;
        this.unitName = unitData.unitName;
        this.level = unitLevelData.level;
        this.presentExp = 0;
        this.requireExp = unitLevelData.exp;
        this.healthPoint = unitLevelData.hp;
        this.presentHealthPoint = unitLevelData.hp;
        this.manaPoint = unitLevelData.mp;
        this.presentManaPoint = unitLevelData.mp;
        this.moveSpeed = unitData.mvSpeed;
        this.attackSpeed = unitData.atkSpeed;
        this.attackRange = unitData.atkRange;
        this.searchRange = unitData.searchRange;

        SkillInitalize();
    }

	//create default use database
	public UnitInformation (
		int _defaultID,
		string _unitName,
		int _level,
		float _requireExp,
		float _healthPoint,
		float _manaPoint,
		float _moveSpeed,
		float _attackSpeed,
		bool _isMeleeAttack,
		float _attackRange,
		float _searchRange)
	{
		defaultID = _defaultID;
		unitName = _unitName;
		level = _level;
		presentExp = 0f;
		requireExp = _requireExp;
		healthPoint = _healthPoint;
		presentHealthPoint = healthPoint;
		manaPoint = _manaPoint;
		presentManaPoint = manaPoint;
		moveSpeed = _moveSpeed;
		attackSpeed = _attackSpeed;
		isMeleeAttack = _isMeleeAttack;
		attackRange = _attackRange;
		searchRange = _searchRange;

		SkillInitalize();
	}


	//pubilc method

	//clear skill
	public void SkillInitalize()
	{
		activeSkillSet = new Skill[6];
		for (int i = 0; i < activeSkillSet.Length; i++)
			activeSkillSet[i] = new Skill ();

		passiveSkillSet = new Skill[6];
		for (int i = 0; i < activeSkillSet.Length; i++)
			passiveSkillSet[i] = new Skill ();

		onSkill = new bool[12];
		coolTime = new float[12];

		unitBuffSet = new Buff[12];

		for (int i = 0; i < unitBuffSet.Length; i++)
			unitBuffSet[i] = new Buff ();
	}

	//data initialize
	public void DataInitialize()
	{

	}

	//apply buff effect
	public void ApplyBuff( float _damage, float _attackSpeed, float _moveSpeed, float _healthPoint, float _manaPoint )
	{
		damage += _damage;
		attackSpeed += _attackSpeed;
		moveSpeed += _moveSpeed;
		healthPoint += _healthPoint;
		manaPoint += _manaPoint;
	}

	//add skill -> passive skill -> add buff
	public bool AddSkill( Skill data )
	{
		//active type
		if ((int) data.SkillType < 4)
		{					
			for (int i = 0; i < activeSkillSet.Length; i++)
			{
				if (activeSkillSet[i].ID == 0)
				{
					activeSkillSet[i] = data;
					return true;
				}
			}
		}
		else
		{
			for (int i = 0; i < passiveSkillSet.Length; i++)
			{		
				if (passiveSkillSet[i].ID == 0)
				{
					passiveSkillSet[i] = data;
					unitBuffSet[i] = new Buff ( passiveSkillSet[i].SkillBuff );
					return true;
				}
			}
		}
		//add false
		return false;	
	}

	public void SetUnitId( int value )
	{
		unitID = value;
	}
}