using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InnerJoystick : MonoBehaviour
{
	public bool left;
	public JoystickPosition outerScript;

	private float showAlpha = 150f / 255f;
	private float hideAlpha = 0;

	private Image img;
	private Image joystickBg;

	private int touchIndex = -1;

	void Start(){
		img = transform.GetChild (0).GetComponent<Image> ();
		joystickBg = GetComponent<Image> ();
		Hide ();
	}

	void Update(){
			for (int i = 0; i < Input.touchCount; i++) {

				if ((left && Input.GetTouch(i).position.x < Screen.width / 2) || (!left && Input.GetTouch(i).position.x > Screen.width / 2)) {
					if (touchIndex == -1 || touchIndex > Input.touchCount)
						touchIndex = i;
					
					if (Input.GetTouch(touchIndex).phase == TouchPhase.Began)
							Show ();
					
					if (Input.GetTouch(touchIndex).phase == TouchPhase.Moved)
						MoveJoystick (Input.GetTouch(touchIndex).position);
				
					if (Input.GetTouch(touchIndex).phase == TouchPhase.Ended) {
						touchIndex = -1;
						Hide ();
						outerScript.Hide ();
					}
					return;
				}
			}


			if ((left && Input.mousePosition.x < Screen.width / 2) || (!left && Input.mousePosition.x > Screen.width / 2)){
				if (Input.GetMouseButtonDown(0))
					Show ();

				if (Input.GetMouseButton(0))
					MoveJoystick (Input.mousePosition);

				if (Input.GetMouseButtonUp (0)) {
					Hide ();
					outerScript.Hide ();
				}
			}
		
	}

	void MoveJoystick(Vector2 position){
		Vector2 input = position;

		//input == touch/mouse position
		//joystickBg.rectTransform.position == center pos of joystick background
		//joystickBg.rectTransform.sizeDelta == size of joystickBg
		Vector2 center = joystickBg.rectTransform.position;
		input.x = input.x - center.x;
		input.y = input.y - center.y;

		if (input.magnitude > 75)
			input = input.normalized;
		else
			input /= 75;
		
		img.rectTransform.anchoredPosition = new Vector2 (input.x * (img.rectTransform.sizeDelta.x / 1.5f), input.y * (img.rectTransform.sizeDelta.y / 1.5f));

		//if (left)
		//	PlayerController.Instance.leftStickVec = input;
		//else
		//	PlayerController.Instance.rightStickVec = input;
	}

	public void Show(){
		img.color = new Color(img.color.r, img.color.g, img.color.b, showAlpha);
		img.rectTransform.anchoredPosition = Vector2.zero;
		ResetStick ();
	}

	public void Hide(){
		img.color = new Color(img.color.r, img.color.g, img.color.b, hideAlpha);
		img.rectTransform.anchoredPosition = Vector2.zero;
		ResetStick ();
	}

	void ResetStick(){
		//if (left)
		//	PlayerController.Instance.leftStickVec = Vector3.zero;
		//else
		//	PlayerController.Instance.rightStickVec = Vector3.zero;
	}

	public Vector3 GetDiscrete(){//vector3 vec
		float x = 0;//vec.x;
		float y = 0;//vec.y;

		//Debug.Log ("Input: " + InputVec.x + " " + InputVec.y);

		if (x >= 0.3)
			x = 1;
		else if (x <= -0.3)
			x = -1;
		else
			x = 0;

		if (y >= 0.3)
			y = 1;
		else if (y <= -0.3)
			y = -1;
		else
			y = 0;

		Debug.Log ("Discrete: " + x + " " + y);
		return new Vector3 (x, 0, y);
	}
	
}
