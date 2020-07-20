using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {

	public abstract void Enter (Directions moveDir, Directions weaponDir);
	public abstract void Exit();
	public abstract void Execute(Directions moveDir, Directions weaponDir);

	public States state;
	protected PlayerController player;
	protected SpriteAnimator animator;

	public List<Transition> Transitions = new List<Transition>();

	public void AddTransition(Transition transition){
		Transitions.Add (transition);
	}
}

public class Transition{
	public readonly State NextState;
	public readonly Func<bool> Condition;

	public Transition(State NextState, Func<bool> Condition){
		this.NextState = NextState;
		this.Condition = Condition;
	}
}