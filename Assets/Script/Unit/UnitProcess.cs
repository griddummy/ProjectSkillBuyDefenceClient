using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//player unit process -> movable unit
public class UnitProcess : MonoBehaviour
{
	//simple data field
	[SerializeField] protected bool isMeleeAttack;
	[SerializeField] protected Rigidbody playerRig;
	[SerializeField] protected Vector3 destination;
	[SerializeField] protected State presentState;
	[SerializeField] protected UnitInformation info;
	[SerializeField] protected Vector3 velocity;
	[SerializeField] protected Collider[] enemy;

	//complex data field
	[SerializeField] protected GameObject targetEnemy;
	[SerializeField] protected GameObject throwObject;
	[SerializeField] protected Animator animator;
	[SerializeField] protected AnimatorStateInfo animatorInfo;
	[SerializeField] protected NavMeshAgent moveAgent;
	[SerializeField] protected GameManager manager;

	public enum State
	{
		Idle,
		Move,
		Attack,
		AttackMove,
		Hold,
		Skill,
		Die}
;

	public enum AnimatorState : int
	{
		Idle = 1,
		Run = 2,
		Attack = 3,
		ThrowAttack = 4,
		Casting = 5,
		CastingUltimate = 6,
		Die = 7}
;

	//property
	public Vector3 Destination
	{
		get { return destination; }
		set { destination = value; }
	}

	public UnitInformation Info
	{
		get { return info; }
	}

	public GameObject TargetEnemy
	{
		get { return targetEnemy; }
	}

	public AnimatorStateInfo AnimatorInfo
	{
		get { return animatorInfo; }
	}

	//initialize this script
	void Start()
	{
		playerRig = GetComponent<Rigidbody>();
		destination = transform.position;
		presentState = State.Hold;
		animator = GetComponent<Animator>();
		moveAgent = GetComponent<NavMeshAgent>();
		manager = GameObject.FindWithTag( "GameManager" ).GetComponent<GameManager>();
		SetLayer();
	}

	//Update is called once per frame
	void Update()
	{
		PreProcess();
		SkillProcess();

		switch (presentState)
		{
			case State.Idle:
				IdleProcess();
				break;
			case State.Move:
				MoveProcess();
				break;
			case State.Attack:
				AttackProcess();
				break;
			case State.AttackMove:
				AttackMoveProcess();
				break;
			case State.Hold:
				HoldProcess();
				break;
			case State.Die:
				DieProcess();
				break;
		}
	}

	//set layer -> use player information
	protected void SetLayer()
	{
		if (info.PlayerNumber == manager.PlayerNumber)
			gameObject.layer = LayerMask.NameToLayer( "Player" );
		else if (manager.CheckAlly[info.PlayerNumber])
			gameObject.layer = LayerMask.NameToLayer( "Ally" );
		else
			gameObject.layer = LayerMask.NameToLayer( "Enemy" );
	
	}

	//synchronization nav mesh data
	protected virtual void PreProcess()
	{
		moveAgent.speed = info.MoveSpeed;
		animator.speed = info.AttackSpeed;
		animatorInfo = this.animator.GetCurrentAnimatorStateInfo( 0 );

		if (info.PresentHealthPoint <= 0)
			presentState = State.Die;

		for (int i = 0; i < info.UnitBuffSet.Length; i++)
		{
			info.UnitBuffSet[i].ActiveBuff( this );
		}
	}

	//check skill and process coolTime
	protected void SkillProcess()
	{
		//check active skill cool Time
		for (int i = 0; i < info.UnitSkillSet.Length; i++)
		{
			if (( (int) info.UnitSkillSet[i].SkillType <= 3 ) && info.OnSkill[i])
			{
				info.CoolTime[i] += Time.deltaTime;
				if (info.CoolTime[i] > info.UnitSkillSet[i].CoolTime)
				{
					info.CoolTime[i] = 0.0f;
					info.OnSkill[i] = false;
				}
			}
		}

		//process buff
	}

	//idle state process
	//find target -> if find target state chanage by attack
	protected virtual void IdleProcess()
	{
		ActiveAnimator( AnimatorState.Idle );

		if (FindTarget())
			presentState = State.Attack;
		else
			return;
	}

	//move state process
	//only move to destination
	protected void MoveProcess()
	{
		ActiveAnimator( AnimatorState.Run );
		transform.LookAt( destination );
		//send unit data

		if (Vector3.Distance( transform.position, destination ) <= 0.1f)
		{
			presentState = State.Idle;
			ActiveAnimator( AnimatorState.Idle );
			//send unit data
		}
	}

	//attack state process
	//chase and attack target
	protected virtual void AttackProcess()
	{
		if (( targetEnemy != null ) && ( Vector3.Distance( targetEnemy.transform.position, transform.position ) > info.AttackRange + targetEnemy.transform.lossyScale.x ))
		{
			if (animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );
			moveAgent.SetDestination( targetEnemy.transform.position );
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Run );
		}
		else if (( targetEnemy != null ) && ( ( Vector3.Distance( targetEnemy.transform.position, transform.position ) <= info.AttackRange + targetEnemy.transform.lossyScale.x ) ))
		{
			if (!animatorInfo.IsName( "Attack" ))
				animator.Play( "Idle" );
			moveAgent.ResetPath();
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
			//send unit data
		}
		else if (targetEnemy == null)
		{
			//find target -> no target state idle
			// -> reset target
			if (FindTarget())
				return;
			else if (presentState == State.AttackMove)
				return;
			else
				presentState = State.Idle;				
		}
	}

	//attack move state process
	//move to destination
	//if find target -> battle by target
	//battle over find target
	//no target -> move destination
	protected void AttackMoveProcess()
	{
		if (( targetEnemy == null ) && !FindTarget() && !animatorInfo.IsName( "Attack" ))
		{
			moveAgent.SetDestination( destination );
			MoveProcess();
		}
		else
			AttackProcess();
	}

	//hold state process
	//hold on transfrom position
	//if find target -> battle by target
	protected void HoldProcess()
	{
		if (!animatorInfo.IsName( "Attack" ))
			animator.Play( "Idle" );

		if (( targetEnemy == null ) && !FindTarget())
			ActiveAnimator( AnimatorState.Idle );
		else if (Vector3.Distance( targetEnemy.transform.position, transform.position ) > info.AttackRange)
		{
			animator.Play( "Idle" );
			targetEnemy = null;
		}
		else if (!animatorInfo.IsName( "Attack" ))
		{
			transform.LookAt( targetEnemy.transform );
			ActiveAnimator( AnimatorState.Idle );
			ActiveAnimator( AnimatorState.Attack );
			//send unit data
		}
	}

	protected void DieProcess()
	{
		if (!animatorInfo.IsName( "Die" ))
		{
			ActiveAnimator( AnimatorState.Die );
			//send unit data
			Destroy( gameObject, 3f );
		}
	}

	//use process

	//find enemy target
	protected bool FindTarget()
	{
		//make collider array -> enemy target in range
		if (presentState != State.Hold)
			enemy = Physics.OverlapSphere( transform.position, info.SearchRange, 1 << LayerMask.NameToLayer( "Enemy" ) );
		else
			enemy = Physics.OverlapSphere( transform.position, info.AttackRange, 1 << LayerMask.NameToLayer( "Enemy" ) );

		//if no enemy -> return false
		if (enemy.Length == 0)
			return false;
		else
		{
			//find shortest distance target
			for (int i = 0; i < enemy.Length; i++)
			{
				if (( ( i == 0 ) || ( Vector3.Distance( enemy[i].transform.position, transform.position ) <= Vector3.Distance( TargetEnemy.transform.position, transform.position ) ) )
				    && ( enemy[i].gameObject.layer == LayerMask.NameToLayer( "Enemy" ) ))
					targetEnemy = enemy[i].gameObject;
			}
			//send unit data
			return true;		
		}
	}

	//set player animation - use present state
	protected virtual void ActiveAnimator( AnimatorState present )
	{
		switch (present)
		{
			case AnimatorState.Idle:
				animator.SetInteger( "State", (int) AnimatorState.Idle );
				break;
			case AnimatorState.Run:
				animator.SetInteger( "State", (int) AnimatorState.Run );
				break;
			case AnimatorState.Attack:
				if (isMeleeAttack)
					animator.SetInteger( "State", (int) AnimatorState.Attack );
				else
					animator.SetInteger( "State", (int) AnimatorState.ThrowAttack );
				break;
			case AnimatorState.Casting:
				animator.SetInteger( "State", (int) AnimatorState.Casting );
				break;
			case AnimatorState.CastingUltimate:
				animator.SetInteger( "State", (int) AnimatorState.Casting );
				break;
			case AnimatorState.Die:
				animator.SetTrigger( "Die" );
				break;
		}
	}

	//public method

	//character stop
	public void SetStop()
	{
		if (presentState != State.Die)
		{
			presentState = State.Idle;
			targetEnemy = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
			//send unit data
		}
	}

	//set hold state
	public void SetHold()
	{
		if (presentState != State.Die)
		{
			presentState = State.Hold;
			targetEnemy = null;
			moveAgent.ResetPath();
			ActiveAnimator( AnimatorState.Idle );
		}
	}

	//set attack state & set attack target
	public void SetAttackTarget( GameObject target )
	{
		if (presentState != State.Die)
		{
			destination = transform.position;
			targetEnemy = target;
			presentState = State.Attack;
		}
	}

	//set destination for nav mesh agent
	public void SetDestination( Vector3 point )
	{
		if (presentState != State.Die)
		{
			presentState = State.Move;
			destination = point;
			moveAgent.destination = destination;
		}
	}

	//set destination for nav mesh agent
	//search range -> find enemy & attack
	public void SetAttackDestination( Vector3 point )
	{
		if (presentState != State.Die)
		{
			presentState = State.AttackMove;
			destination = point;
			moveAgent.destination = destination;
		}
	}

	//unit use active skill
	public void ActiveSkill( int index )
	{		
		if (( presentState != State.Die ) && ( (int) info.UnitSkillSet[index].SkillType <= 3 ))
		{
			info.OnSkill[index] = true;
			info.UnitSkillSet[index].UseSkill( transform.position );
			moveAgent.ResetPath();
			animator.Play( "Casting" );
		}
	}

	//use throw attack
	public void ThrowAttackObject()
	{		
		GameObject temp = (GameObject) Instantiate( throwObject, transform.position + new Vector3 ( -0.2f, 1.5f, 0.2f ), transform.rotation );
		temp.GetComponent<ThrowObject>().SetTargetAndSpeed( this, targetEnemy );
	}

	//reset animator information
	public void ReloadAnimatorInfo()
	{
		animatorInfo = animator.GetCurrentAnimatorStateInfo( 0 );
	}

	//unit damage calculate
	public virtual void Damaged( float damage )
	{
		info.PresentHealthPoint -= damage;
		//send unit data
	}

	//use attack animation -> animation event
	public void EndAttackAnimation()
	{
		
	}

	//use casting animation -> animation event
	public void EndCastingAnimation()
	{

	}

}


