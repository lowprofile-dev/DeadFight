using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsList : MonoBehaviour {

	public static WeaponsList Instance = null; 
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public List<GameObject> Weapons = new List<GameObject>();

	public GameObject GetWeaponOfType(WeaponTypes type){
		foreach (GameObject weapon in Weapons) {
			if (weapon.GetComponent<Weapon> ().Type == type)
				return weapon;
		}

		//Isto nunca deve acontecer.
		return Weapons [0];
	}
}
