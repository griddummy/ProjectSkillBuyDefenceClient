using UnityEngine;
using System.Collections;

public class PlayerTargetSearch : MonoBehaviour
{
	[SerializeField] PlayerProcess player;

	void OnTriggerStay( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer( "Enemy" ) && ( player.TargetEnemy == null ))
			player.SetAttackTarget( col.gameObject );
			
	}
}
