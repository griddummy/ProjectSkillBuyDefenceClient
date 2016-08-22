using UnityEngine;
using System.Collections;

public class UnitWeapon : MonoBehaviour
{
	[SerializeField] UnitProcess unit;
	[SerializeField] Collider weapon;
	[SerializeField] UnitProcess temp;

	void Start()
	{
		weapon = GetComponent<Collider>();
		gameObject.layer = LayerMask.NameToLayer( "UnitWeapon" );
	}

	void Update()
	{	
		unit.ReloadAnimatorInfo();
		weapon.enabled = unit.AnimatorInfo.IsName( "Attack" );		
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			temp = col.gameObject.GetComponent<UnitProcess>();
			temp.Damaged( unit.Info.Damage );
		}
		else if (( unit.gameObject.layer == LayerMask.NameToLayer( "Enemy" ) )
		         && ( col.gameObject.layer == LayerMask.NameToLayer( "Player" ) ))
		{
			temp = col.gameObject.GetComponent<UnitProcess>();
			temp.Damaged( unit.Info.Damage );
		}
	}
}
