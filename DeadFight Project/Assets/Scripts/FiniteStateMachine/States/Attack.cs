using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State {

	public Attack(){
		state = States.Attack;
	}

	public override void Enter(Directions moveDir, Directions weaponDir){
		//moveDir deve ser unspecified.
		if (player == null)
			player = PlayerController.Instance;

		animator = player.GetComponentInChildren<SpriteAnimator> ();
	}

	public override void Exit(){

	}

	public override void Execute(Directions moveDir, Directions weaponDir){
		animator.ChangeState (state, weaponDir);
		if (player.CurrentWeaponScript != null)
			player.CurrentWeaponScript.ChangeState (state, weaponDir);

	}
}
