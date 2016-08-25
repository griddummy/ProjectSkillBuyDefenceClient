
using UnityEngine;
using System.Collections;

//enemy & ally unit process -> movable unit
public class UnitPlayer : UnitProcess
{
	//simple field
	[SerializeField] bool isMove;
	[SerializeField] AnimatorState presentAnimatorState;

	// initialize this script
	void Start()
	{
		animator = GetComponent<Animator>();
		moveAgent = GetComponent<NavMeshAgent>();
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
		DataInitialize();
	}
	
	// Update is called once per frame
	void Update()
	{
		PreProcess();	

		if (unitTarget != null)
		{
			destination = unitTarget.transform.position;
		}

		if (isMove && ( Vector3.Distance( transform.position, destination ) >= 0.1f ))
		{
			transform.LookAt( destination );		
		}
		else
		{
			destination = transform.position;
			moveAgent.ResetPath();
			presentAnimatorState = AnimatorState.Idle;
			isMove = false;
		}

		ActiveAnimator( presentAnimatorState );
	}

	protected override void PreProcess()
	{
		moveAgent.speed = info.MoveSpeed;
		animator.speed = info.AttackSpeed;
		animatorInfo = this.animator.GetCurrentAnimatorStateInfo( 0 );

		//aura move
		for (int i = 0; i < auraEffect.Length; i++)
			if (auraEffect[i] != null)
				auraEffect[i].transform.position = transform.position + ( Vector3.up * 0.3f );

		if (animatorInfo.IsName( "Attack" ))
			moveAgent.updatePosition = false;
		else
			moveAgent.updatePosition = true;


	}

	//set player animation - use present state
	protected override void ActiveAnimator( AnimatorState present )
	{
		switch (present)
		{
			case AnimatorState.Idle:
				animator.SetInteger( "State", (int) AnimatorState.Idle );
				if (!animatorInfo.IsName( "Idle" ))
					animator.Play( "Idle" );
				break;
			case AnimatorState.Run:
				animator.SetInteger( "State", (int) AnimatorState.Run );
				if (!animatorInfo.IsName( "Run" ))
					animator.Play( "Run" );
				break;
			case AnimatorState.Attack:
				animator.SetInteger( "State", (int) AnimatorState.Attack );
				if (!animatorInfo.IsName( "Attack" ))
					animator.Play( "Attack" );
				break;
			case AnimatorState.ThrowAttack:
				animator.SetInteger( "State", (int) AnimatorState.ThrowAttack );
				if (!animatorInfo.IsName( "ThrowAttack" ))
					animator.Play( "ThrowAttack" );
				break;
			case AnimatorState.Casting:
				animator.SetInteger( "State", (int) AnimatorState.Casting );
				if (!animatorInfo.IsName( "Casting" ))
					animator.Play( "Casting" );
				break;
			case AnimatorState.CastingUltimate:
				animator.SetInteger( "State", (int) AnimatorState.CastingUltimate );
				if (!animatorInfo.IsName( "CastingUltimate" ))
					animator.Play( "CastingUltimate" );
				break;
			case AnimatorState.Die:
				if (!animatorInfo.IsName( "Die" ))
					animator.Play( "Die" );
				break;
		}
	}

	//public method

	//unit position & animation state data receive
	public void ReceiveData( Vector3 positionData, AnimatorState stateData )
	{
		destination = positionData;
		presentAnimatorState = stateData;
		moveAgent.SetDestination( destination );
		isMove = true;

		if (stateData == AnimatorState.Die)
			Destroy( this.gameObject, 3f );
	}

	//receive data -> interpolate position
	public void ReceiveData( Vector3 interpolatePoint )
	{
		if (Vector3.Distance( transform.position, interpolatePoint ) >= 1f)
			transform.position = interpolatePoint;
	}

	//unit damage calculate
	//send unit information to another client
	public override void Damaged( float damage )
	{
		info.PresentHealthPoint -= damage;
		//send unit data
		manager.UnitDamaged( this, damage );
	}

	public override void SetAttackTarget( GameObject target )
	{
		unitTarget = target;
	}
}
