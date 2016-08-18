using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RestoureAura : Skill
{
	[SerializeField] int healAmount;

	public RestoureAura ()
	{
		id = 0002;
		name = "RestoreAura";
		level = 1;
		skillRange = 5f;
		coolTime = 0f;
		onTarget = false;
		skillType = Type.PassiveAura;
		healAmount = 10;
	}
}


