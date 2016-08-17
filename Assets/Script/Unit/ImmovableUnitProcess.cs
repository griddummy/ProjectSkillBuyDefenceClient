using UnityEngine;
using System.Collections;

//player unit process -> Immovable unit
public class ImmovableUnitProcess : UnitProcess
{
	void Start()
	{
		isMeleeAttack = false;
		playerRig = GetComponent<Rigidbody>();
		presentState = State.Idle;
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
		SetLayer();
	}

	void Update()
	{
		PreProcess();

		switch (presentState)
		{
			case State.Idle:
				IdleProcess();
				break;
			case State.Attack:
				AttackProcess();
				break;
		}
	}

	protected override void PreProcess()
	{
		if (info.HealthPoint <= 0)
		{
			presentState = State.Die;
			Destroy( gameObject );
		}
	}

	protected override void IdleProcess()
	{

	}

	protected override void AttackProcess()
	{

	}
}