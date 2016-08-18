using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Buff
{
	//simple field
	[SerializeField] string buffName;
	[SerializeField] bool buff;
	[SerializeField] float buffTime;
	[SerializeField] int damage;
	[SerializeField] float attackSpeed;
	[SerializeField] float moveSpeed;
	[SerializeField] int healAmount;

	public Buff ()
	{
		
	}

	public Buff (string _buffName)
	{
	}
}


