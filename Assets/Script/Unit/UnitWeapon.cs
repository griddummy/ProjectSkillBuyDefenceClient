using UnityEngine;
using System.Collections;

public class UnitWeapon : MonoBehaviour
{
	[SerializeField] UnitProcess unit;
	[SerializeField] Collider weapon;
	[SerializeField] UnitProcess temp;
	[SerializeField] bool check;

	void Start()
	{
		weapon = GetComponent<Collider>();
		gameObject.layer = LayerMask.NameToLayer( "UnitWeapon" );
	}

	void Update()
	{	
		check = unit.AnimatorInfo.IsName( "Attack" );		
		weapon.enabled = unit.AnimatorInfo.IsName( "Attack" );		
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ))
		{
			temp = col.gameObject.GetComponent<UnitProcess>();
			temp.Damaged( unit.Info.Damage );
		}
	}
}
