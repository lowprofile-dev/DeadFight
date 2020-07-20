using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM {

	private List<State> states = new List<State>();
	public State CurrentState;
	public States StateString;
	private Directions moveDirection;
	private Directions weaponDirection;

	public void Init(State initState, Directions moveDir, Directions weaponDir){
		CurrentState = states.Find (s => s.state == initState.state);
		CurrentState.Enter (moveDir, weaponDir);
	}

	public void Update(){
		if (CurrentState == null)
			return;

		StateString = CurrentState.state;

		foreach (Transition t in CurrentState.Transitions) {
			if (t.Condition ()) {
				CurrentState.Exit ();
				CurrentState = t.NextState;
				CurrentState.Enter (moveDirection, weaponDirection);
				//Debug.Log ("FSM State: " + CurrentState.state.ToString ());
			}
		}

		CurrentState.Execute (moveDirection, weaponDirection);
	}

	public void AddState(State state){
		states.Add (state);
	}

	public void UpdateDirection(Directions moveDir, Directions shootDir){
		moveDirection = moveDir;
		weaponDirection = shootDir;
	}

}
