using UnityEngine;
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
		skillCost = 30f;
		skillBuyCost = 100;
		level = 1;
		skillRange = 6f;
		coolTime = 15f;
		onTarget = false;
		skillType = Type.ActiveNonTarget;
		skillBuff = null;
	}

	public override void UseSkill(Vector3 position)
	{
		Collider[] target = Physics.OverlapSphere( position, skillRange, 1 << LayerMask.NameToLayer( "Enemy" ) );

		for (int i = 0; i < target.Length; i++)
		{
			try
			{
				target[i].GetComponent<UnitProcess>().Info.PresentHealthPoint -= damage;
			}
			catch(MissingReferenceException e)
			{
				Debug.Log( e.InnerException );
			}
		}	
	}

}

