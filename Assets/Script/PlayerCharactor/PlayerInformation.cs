using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerInformation
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

	//property
	public float MoveSpeed
	{
		get { return moveSpeed; }
	}

	public float AttackRange
	{
		get { return attackRange; }
	}

	public float SearchRange
	{
		get { return searchRange; }
	}

	public PlayerInformation ()
	{
		level = 1;
		presentExp = 0f;
		requireExp = 1000f;
		healthPoint = 1000;
		manaPoint = 100;
		moveSpeed = 5f;
		attackSpeed = 1f;
		attackRange = 2f;
		searchRange = 20f;
	}

	public PlayerInformation (PlayerInformation info)
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

}
