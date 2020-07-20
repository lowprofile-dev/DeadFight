using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollide : MonoBehaviour {

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag ("Projectile")) {
			//Disparo colide com a Parede
			Destroy(collider.gameObject);
		}
	}

}
