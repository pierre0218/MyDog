using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DogControl : MonoBehaviour
{
	public Animator anim;
	public string AnimatorName;
	public int Move;

	public int Pose = 0;
	public int CurrentPose = 0;
	public bool ChangePose = false;

	public float CrossfadeVal = 0.25f;

	private FSMSystem fsm;

	public void SetTransition(Transition t) { fsm.PerformTransition(t); }

	public void Start()
	{
		anim = GetComponent<Animator> ();
		AnimatorName = anim.name;

		MakeFSM();

		SetTransition (Transition.ToRun);
	}

	public void Update()
	{
		if (ChangePose) 
		{
			print ("Change Pose");
			ChangePose = false;
			//if stands
			if (CurrentPose == 0) {
                if (Pose == 2) {
					anim.CrossFade (AnimatorName +"IdleToSit", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "IdleToLay", CrossfadeVal);
				} 

				CurrentPose = Pose;
			}
			//Jump
			else if (CurrentPose == 1) {
				if (Pose == 2) {
					anim.CrossFade (AnimatorName +"IdleToSit", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "IdleToLay", CrossfadeVal);
				} 

				CurrentPose = Pose;
			}
			//Sit
			else if (CurrentPose == 2) {
				if (Pose == 0) {
					anim.CrossFade (AnimatorName + "SitToIdle", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "SitToLay", CrossfadeVal);
				}

				CurrentPose = Pose;
			}
			//Lay
			else if (CurrentPose == 3) {
				if (Pose == 0) {
					anim.CrossFade (AnimatorName + "LayToIdle", CrossfadeVal);
				}else if (Pose == 2) {
					anim.CrossFade (AnimatorName + "LayToSit", CrossfadeVal);
				}
				CurrentPose = Pose;
			}
			//walk
			else if (CurrentPose == 4) {
				if (Pose == 0) {
					anim.CrossFade (AnimatorName + "Idle", CrossfadeVal);
				} else if (Pose == 2) {
					anim.CrossFade (AnimatorName + "IdleToSit", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "IdleToLay", CrossfadeVal);
				}

				CurrentPose = Pose;
			} 
			//Run
			else if (CurrentPose == 5) {
				if (Pose == 0) {
					anim.CrossFade (AnimatorName + "Idle", CrossfadeVal);
				} else if (Pose == 2) {
					anim.CrossFade (AnimatorName + "IdleToSit", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "IdleToLay", CrossfadeVal);
				}

				CurrentPose = Pose;
			}
			//Dead
			else if (CurrentPose == 6) {
				if (Pose == 2) {
					anim.CrossFade (AnimatorName +"IdleToSit", CrossfadeVal);
				} else if (Pose == 3) {
					anim.CrossFade (AnimatorName + "IdleToLay", CrossfadeVal);
				} 

				CurrentPose = Pose;
			}
		}
	}

	public void FixedUpdate()
	{
		fsm.CurrentState.Reason();
		fsm.CurrentState.Act();
	}

	// The NPC has two states: FollowPath and ChasePlayer
	// If it's on the first state and SawPlayer transition is fired, it changes to ChasePlayer
	// If it's on ChasePlayerState and LostPlayer transition is fired, it returns to FollowPath
	private void MakeFSM()
	{
		StandState stand = new StandState(this);
		stand.AddTransition(Transition.ToDead, StateID.Dead);
		stand.AddTransition(Transition.ToWalk, StateID.Walk);
		stand.AddTransition(Transition.ToRun, StateID.Run);
		stand.AddTransition(Transition.ToSit, StateID.Sit);
		stand.AddTransition(Transition.ToJump, StateID.Jump);
		stand.AddTransition(Transition.ToLay, StateID.Lay);


		WalkState walk = new WalkState(this);
		walk.AddTransition(Transition.ToDead, StateID.Dead);
		walk.AddTransition(Transition.ToStand, StateID.Stand);
		walk.AddTransition(Transition.ToRun, StateID.Run);
		walk.AddTransition(Transition.ToSit, StateID.Sit);
		walk.AddTransition(Transition.ToJump, StateID.Jump);
		walk.AddTransition(Transition.ToLay, StateID.Lay);

		RunState run = new RunState(this);
		run.AddTransition(Transition.ToDead, StateID.Dead);
		run.AddTransition(Transition.ToStand, StateID.Stand);
		run.AddTransition(Transition.ToWalk, StateID.Walk);
		run.AddTransition(Transition.ToSit, StateID.Sit);
		run.AddTransition(Transition.ToJump, StateID.Jump);
		run.AddTransition(Transition.ToLay, StateID.Lay);

		SitState sit = new SitState(this);
		sit.AddTransition(Transition.ToDead, StateID.Dead);
		sit.AddTransition(Transition.ToStand, StateID.Stand);
		sit.AddTransition(Transition.ToRun, StateID.Run);
		sit.AddTransition(Transition.ToWalk, StateID.Walk);
		sit.AddTransition(Transition.ToJump, StateID.Jump);
		sit.AddTransition(Transition.ToLay, StateID.Lay);

		LayState lay = new LayState(this);
		lay.AddTransition(Transition.ToDead, StateID.Dead);
		lay.AddTransition(Transition.ToStand, StateID.Stand);
		lay.AddTransition(Transition.ToRun, StateID.Run);
		lay.AddTransition(Transition.ToSit, StateID.Sit);
		lay.AddTransition(Transition.ToJump, StateID.Jump);
		lay.AddTransition(Transition.ToWalk, StateID.Walk);

		JumpState jump = new JumpState(this);
		jump.AddTransition(Transition.ToDead, StateID.Dead);
		jump.AddTransition(Transition.ToStand, StateID.Stand);
		jump.AddTransition(Transition.ToRun, StateID.Run);
		jump.AddTransition(Transition.ToSit, StateID.Sit);
		jump.AddTransition(Transition.ToWalk, StateID.Walk);
		jump.AddTransition(Transition.ToLay, StateID.Lay);

		DeadState dead = new DeadState(this);
		dead.AddTransition(Transition.ToWalk, StateID.Walk);
		dead.AddTransition(Transition.ToStand, StateID.Stand);
		dead.AddTransition(Transition.ToRun, StateID.Run);
		dead.AddTransition(Transition.ToSit, StateID.Sit);
		dead.AddTransition(Transition.ToJump, StateID.Jump);
		dead.AddTransition(Transition.ToLay, StateID.Lay);

		fsm = new FSMSystem();
		fsm.AddState(stand);
		fsm.AddState(walk);
		fsm.AddState(run);
		fsm.AddState(sit);
		fsm.AddState(lay);
		fsm.AddState(jump);
		fsm.AddState(dead);
	}
}






public class StandState : FSMState
{
	DogControl m_dog;
	public StandState(DogControl npc)
	{
		stateID = StateID.Stand;
		m_dog = npc;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Stand state");
		Vector3 cameraForward = Camera.main.transform.forward;
		cameraForward.y = 0;
		m_dog.transform.forward = -cameraForward;

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Idle", 0.5f);

		if (m_dog.Pose != 0) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 0;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{

	}

	public override void DoBeforeLeaving() 
	{ 

	}

}

public class JumpState : FSMState
{
	DogControl m_dog;
	public JumpState(DogControl npc)
	{
		stateID = StateID.Jump;
		m_dog = npc;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Jump state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Jump", 0.5f);

		if (m_dog.Pose != 1) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 1;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{

	}

	public override void DoBeforeLeaving() 
	{ 
		Debug.Log ("Leave Jump state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Idle", 0.5f);

	}

}

public class SitState : FSMState
{
	DogControl m_dog;
	public SitState(DogControl npc)
	{
		stateID = StateID.Sit;
		m_dog = npc;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Sit state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		//m_dog.anim.CrossFade (m_dog.AnimatorName + "Sit", 0.5f);

		if (m_dog.Pose != 2) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 2;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{

	}

	public override void DoBeforeLeaving() 
	{ 
		Debug.Log ("Leave Sit state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Idle", 0.5f);

	}
}


public class LayState : FSMState
{
	DogControl m_dog;
	public LayState(DogControl npc)
	{
		stateID = StateID.Lay;
		m_dog = npc;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Lay state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
	//	m_dog.anim.CrossFade (m_dog.AnimatorName + "Lay", 0.5f);

		if (m_dog.Pose != 3) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 3;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{

	}

	public override void DoBeforeLeaving() 
	{ 
		Debug.Log ("Leave Lay state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Idle", 0.5f);

	}
}

public class WalkState : FSMState
{
	DogControl m_dog;
	Vector3 targetPos = Vector3.zero;
	Vector3 movingDirection = Vector3.zero;

	public WalkState(DogControl npc) 
	{ 
		stateID = StateID.Walk;
		m_dog = npc;

		float randX = UnityEngine.Random.Range (-1.0f, 1.0f);
		float randY = UnityEngine.Random.Range (-1.0f, 1.0f);
		targetPos = new Vector3(randX,0,randY);

		movingDirection = targetPos - m_dog.transform.position;
		movingDirection = movingDirection.normalized;
		m_dog.transform.forward = movingDirection;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Walk state");
		m_dog.Move = 1;
		m_dog.anim.SetFloat ("Move", m_dog.Move);

		if (m_dog.Pose != 4) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 4;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{
		if (Vector3.Distance (m_dog.transform.position, targetPos) > 0.2f) {
			m_dog.transform.position += 0.002f * movingDirection;
		} 
		else {
			m_dog.transform.position = targetPos;

			float randX = UnityEngine.Random.Range (-1.0f, 1.0f);
			float randY = UnityEngine.Random.Range (-1.0f, 1.0f);
			targetPos = new Vector3(randX,0,randY);

			movingDirection = targetPos - m_dog.transform.position;
			movingDirection = movingDirection.normalized;
			m_dog.transform.forward = movingDirection;
		}
	}

	public override void DoBeforeLeaving() 
	{ 

	}

} 

public class RunState : FSMState
{
	DogControl m_dog;
	Vector3 targetPos = Vector3.zero;
	Vector3 movingDirection = Vector3.zero;

	public RunState(DogControl npc) 
	{ 
		stateID = StateID.Run;
		m_dog = npc;

		float randX = UnityEngine.Random.Range (-1.0f, 1.0f);
		float randY = UnityEngine.Random.Range (-1.0f, 1.0f);
		targetPos = new Vector3(randX,0,randY);

		movingDirection = targetPos - m_dog.transform.position;
		movingDirection = movingDirection.normalized;
		m_dog.transform.forward = movingDirection;

		if (m_dog.Pose != 5) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 5;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Run state");
		m_dog.Move = 7;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
	}

	public override void Reason()
	{

	}

	public override void Act()
	{
		if (Vector3.Distance (m_dog.transform.position, targetPos) > 0.2f) {
			m_dog.transform.position += 0.004f * movingDirection;
		} 
		else {
			m_dog.transform.position = targetPos;

			float randX = UnityEngine.Random.Range (-1.0f, 1.0f);
			float randY = UnityEngine.Random.Range (-1.0f, 1.0f);
			targetPos = new Vector3(randX,0,randY);

			movingDirection = targetPos - m_dog.transform.position;
			movingDirection = movingDirection.normalized;
			m_dog.transform.forward = movingDirection;
		}
	}

} 

public class DeadState : FSMState
{
	DogControl m_dog;
	public DeadState(DogControl npc)
	{
		stateID = StateID.Dead;
		m_dog = npc;
	}

	public override void DoBeforeEntering() 
	{ 
		Debug.Log ("Enter Lay state");

		m_dog.Move = 0;
		m_dog.anim.SetFloat ("Move", m_dog.Move);
		m_dog.anim.CrossFade (m_dog.AnimatorName + "Death", 0.5f);

		if (m_dog.Pose != 6) {
			m_dog.ChangePose = true;
		}
		m_dog.Pose = 6;
	}

	public override void Reason()
	{

	}

	public override void Act()
	{

	}

}