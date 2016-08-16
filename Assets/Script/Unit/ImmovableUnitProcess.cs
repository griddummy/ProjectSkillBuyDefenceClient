using UnityEngine;
using System.Collections;

public class ImmovableUnitProcess : UnitProcess
{
	void Start()
	{
		isMeleeAttack = false;
		playerRig = GetComponent<Rigidbody>();
		presentState = State.Idle;
		moveAgent = GetComponent<NavMeshAgent>();
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
		moveAgent.speed = info.MoveSpeed;

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