using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject;

	private bool buttonSelected;

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetAxisRaw("ShootVertical") != 0 && buttonSelected == false) 
		{
			eventSystem.SetSelectedGameObject(selectedObject);
			buttonSelected = true;
		}
	}

	private void OnDisable()
	{
		buttonSelected = false;
	}
}
