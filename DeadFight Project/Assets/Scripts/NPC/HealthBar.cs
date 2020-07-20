using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {


	public Transform currentValTransform;

	private float MaxHealth;
	public float health;

	void Start(){
		MaxHealth = health;
	}

	public void ChangeHealth(float val){
		health = Mathf.Clamp (health + val, 0, MaxHealth);
	}

	public void UpdateHealthBar(){
		float percentage = (float)health / (float)MaxHealth;
		Vector3 currentScale = currentValTransform.localScale;
		currentValTransform.localScale = new Vector3(percentage, currentScale.y, currentScale.z);

	}

	public bool isDead(){
		if (health <= 0)
			return true;
		else
			return false;
	}

}
