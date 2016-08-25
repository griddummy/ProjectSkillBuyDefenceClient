using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class KaBoom : Skill
{
	[SerializeField] float activeRange;

	public KaBoom ()
	{
		id = 0005;
		name = "KaBoom";
		damage = 1000;
		skillCost = 50f;
		skillBuyCost = 100;
		level = 1;
		skillRange = 12f;
		coolTime = 40f;
		onTarget = true;
		skillType = Type.ActiveTargetArea;
		skillBuff = null;
		activeRange = 10f;
	}

	public override void UseSkill( Vector3 position )
	{
		Collider[] target = Physics.OverlapSphere( position, activeRange, 1 << LayerMask.NameToLayer( "Enemy" ) );

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

