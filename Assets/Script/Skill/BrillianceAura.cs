using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BrillianceAura : Skill
{
	[SerializeField] float manaRegenAmount;

	public BrillianceAura ()
	{
		id = 0006;
		name = "BrillianceAura";
		skillBuyCost = 100;
		level = 1;
		skillRange = 5f;
		coolTime = 0f;
		onTarget = false;
		skillType = Type.PassiveAura;
		manaRegenAmount = 10;
		skillBuff = new Buff ( id, name, Mathf.Infinity, skillRange, 0f, 0f, 0f, 0f, 0f, 0f, manaRegenAmount );
	}
}

