using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

	[System.Serializable]
	public class SpriteListWrapper{
		public States state;
		public Directions direction;
		public bool flip = false;
		public float TimeBetweenSprites = 0.5f;
		public List<Sprite> sprites;
	}

	public List<SpriteListWrapper> sprites;

	private float currentTime;
	private int currentSprite;
	private int currentIndex;

	private SpriteRenderer spriteRenderer;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		currentTime = 0;
		currentSprite = 0;
		currentIndex = 0;

		UpdateSprite ();
	}

	int FindIndex(States state, Directions dir){
		for (int i = 0; i < sprites.Count; i++){
			if (sprites [i].state == state && sprites[i].direction == dir)
				return i;
		}

		//Se for igual entao activa a animaçao de Atack
		if (state == States.Attack) {
			for (int i = 0; i < sprites.Count; i++) {
				if (sprites [i].state == States.Move && sprites [i].direction == dir)
					return i;
			}
		}

		//find idle
		for (int i = 0; i < sprites.Count; i++){
			if (sprites [i].state == States.Idle)
				return i;
		}

		//find first appearance of direction if no idle
		for (int i = 0; i < sprites.Count; i++) {
			if (sprites [i].direction == dir)
				return i;
		}
		Debug.Log ("State: " + state.ToString() + " Dir: " + dir.ToString ());
		Debug.Log ("No Index Found");
		return 0;
	}

	void Update () {
		currentTime += Time.deltaTime;

		if (currentTime > sprites[currentIndex].TimeBetweenSprites) {
			currentTime = 0;
			currentSprite++;
			if (currentSprite >= sprites [currentIndex].sprites.Count) {
				currentSprite = 0;
			}
			UpdateSprite ();
		}
	}

	void UpdateSprite(){
//		Debug.Log ("Index: " + currentIndex + "/" + sprites.Count);
//		Debug.Log ("Sprite: " + currentSprite + "/" + sprites [currentIndex].sprites.Count);
//		Debug.Log (" ");
		spriteRenderer.sprite = sprites[currentIndex].sprites[currentSprite];
	}

	public void ChangeState(States state, Directions dir){
		int newIndex = FindIndex (state, dir);
		if (currentIndex != newIndex) {
			currentIndex = FindIndex (state, dir);
			if (sprites [currentIndex].flip)
				spriteRenderer.flipX = true;
			else
				spriteRenderer.flipX = false;
			currentSprite = 0;
			currentTime = 0;
			UpdateSprite ();
		}

		//Debug.Log ("STATE CHANGE: " + state.ToString ());
	}
}
