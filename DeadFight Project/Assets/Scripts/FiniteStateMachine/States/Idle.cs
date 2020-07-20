using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State {

	public Idle(){
		state = States.Idle;
	}

	public override void Enter(Directions moveDir, Directions weaponDir){
		if (player == null)
			player = PlayerController.Instance;
		
		animator = player.GetComponentInChildren<SpriteAnimator> ();
	}

	public override void Exit(){

	}

	public override void Execute(Directions moveDir, Directions weaponDir){
		animator.ChangeState (state, moveDir);

		if (player.CurrentWeaponScript != null)
			player.CurrentWeaponScript.ChangeState (state, moveDir);
	}

}
