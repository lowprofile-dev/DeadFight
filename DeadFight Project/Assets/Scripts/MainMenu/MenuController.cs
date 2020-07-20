using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public void PlayButton(){
		Debug.Log ("PlayButton clicked");
		SceneManager.LoadScene (1);
	}

	public void QuitButton(){
		Debug.Log ("QuitButton clicked");
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}

}
