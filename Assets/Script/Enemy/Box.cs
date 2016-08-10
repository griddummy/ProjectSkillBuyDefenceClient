using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour
{

	public float hp = 100f;

	public void Hit( float damage )
	{
		hp -= damage;

		if (hp <= 0)
			Destroy( gameObject );
	}
}
