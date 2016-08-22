﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShockStun : Skill
{
	public ShockStun ()
	{
		id = 0003;
		name = "ShockStun";
		damage = 500f;
		skillBuyCost = 100;
		level = 1;
		skillRange = 0f;
		coolTime = 15f;
		onTarget = false;
		skillType = Type.ActiveNonTarget;
		skillBuff = null;
	}

	public override void UseSkill(Vector3 position)
	{
		Debug.Log( "Active ShockStun" );
	}

}
