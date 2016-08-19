using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Buff
{
	//simple field
	[SerializeField] int id;
	[SerializeField] string buffName;
	[SerializeField] bool onActivate;
	[SerializeField] float buffTime;
	[SerializeField] float buffRange;
	[SerializeField] float damage;
	[SerializeField] float attackSpeed;
	[SerializeField] float moveSpeed;
	[SerializeField] float healthPoint;
	[SerializeField] float manaPoint;
	[SerializeField] float healAmount;

	public Buff ()
	{
		buffName = null;
	}

	public Buff (
		int _id,
		string _buffName, 
		float _buffTime,
		float _buffRange,
		float _damage, 
		float _attackSpeed, 
		float _moveSpeed,
		float _healthPoint,
		float _manaPoint,
		float _healAmount)
	{
		id = _id;
		buffName = _buffName;
		onActivate = false;
		buffTime = _buffTime;
		buffRange = _buffRange;
		damage = _damage;
		attackSpeed = _attackSpeed;
		moveSpeed = _moveSpeed;
		healthPoint = _healthPoint;
		manaPoint = _manaPoint;
		healAmount = _healAmount;
	}

	public Buff(Buff data)
	{
		id = data.id;
		buffName = data.buffName;
		onActivate = false;
		buffTime = data.buffTime;
		buffRange = data.buffRange;
		damage = data.damage;
		attackSpeed = data.attackSpeed;
		moveSpeed = data.moveSpeed;
		healthPoint = data.healthPoint;
		manaPoint = data.manaPoint;
		healAmount = data.healAmount;
	}

	//apply buff
	public void ActiveBuff( UnitProcess data )
	{
		if (!onActivate)
		{
			data.Info.ApplyBuff( damage, attackSpeed, moveSpeed, healthPoint, manaPoint );
			onActivate = true;
		}

		if (buffRange != 0)
		{
			Collider[] target = Physics.OverlapSphere( data.transform.position, buffRange, 1 << LayerMask.NameToLayer( "Player" ) );

			for (int i = 0; i < target.Length; i++)
			{
				target[i].gameObject.GetComponent<UnitProcess>().Info.PresentHealthPoint += healAmount * Time.deltaTime;
			}
		}

	}
}


