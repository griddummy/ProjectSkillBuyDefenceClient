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
	[SerializeField] float manaRegenAmount;
	[SerializeField] Collider[] target;

	//property
	public string Name { get { return buffName; } }

	public int ID { get { return id; } }

	public Buff ()
	{
		buffName = null;
		int id = 0;
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
		float _healAmount,
		float _manaRegenAmount)
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
		manaRegenAmount = _manaRegenAmount;
	}

	public Buff (Buff data)
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
		manaRegenAmount = data.manaRegenAmount;
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
			data.Info.PresentHealthPoint += healAmount * Time.deltaTime;
			data.Info.PresentManaPoint += manaRegenAmount * Time.deltaTime;

			int layer = 1 << LayerMask.NameToLayer( "player" );
			layer |= 1 << LayerMask.NameToLayer( "Ally" );
			target = Physics.OverlapSphere( data.transform.position, buffRange, layer );
			for (int i = 0; i < target.Length; i++)
			{			
				target[i].gameObject.GetComponent<UnitProcess>().Info.PresentHealthPoint += healAmount * Time.deltaTime;
				target[i].gameObject.GetComponent<UnitProcess>().Info.PresentManaPoint += manaRegenAmount * Time.deltaTime;
			}
		}

	}
}


