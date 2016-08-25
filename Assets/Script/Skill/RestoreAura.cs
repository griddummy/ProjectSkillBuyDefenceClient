using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RestoreAura : Skill
{
	[SerializeField] float healAmount;

	public RestoreAura ()
	{
		id = 0002;
		name = "RestoreAura";
		skillBuyCost = 100;
		level = 1;
		skillRange = 5f;
		coolTime = 0f;
		onTarget = false;
		skillType = Type.PassiveAura;
		healAmount = 10;
		skillBuff = new Buff ( id, name, Mathf.Infinity, skillRange, 0f, 0f, 0f, 0f, 0f, healAmount, 0f );
	}
}
