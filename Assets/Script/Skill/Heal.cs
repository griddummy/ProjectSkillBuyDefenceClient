using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Heal : Skill
{
	[SerializeField] float healAmount;
	public Heal()
	{
		id = 0004;
		name = "Heal";
		damage = 0;
		skillBuyCost = 100;
		level = 1;
		skillRange = 8f;
		coolTime = 10f;
		onTarget = true;
		skillType = Type.ActiveTarget;
		skillBuff = null;
		healAmount = 400f;
	}

	public override void UseSkill(UnitProcess data)
	{
		Debug.Log( "Heal" );
		data.Info.PresentHealthPoint += healAmount;
	}
}