using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickPosition : MonoBehaviour, IPointerDownHandler
{
	public bool left;
	public InnerJoystick inner;

	private float showAlpha = 50f / 255f;
	private float hideAlpha = 0;

	private Image img;

	void Start(){
		img = transform.GetChild (0).GetComponent<Image> ();
		Hide ();
	}

	public virtual void OnPointerDown(PointerEventData ped){
			Show ();
			Vector2 pos = Vector2.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
				img.rectTransform,
				ped.position,
				ped.pressEventCamera,
				out pos
			    )) {
				img.rectTransform.anchoredPosition = pos;
			}
		
	}

	public void Show(){
		img.color = new Color(img.color.r, img.color.g, img.color.b, showAlpha);
		img.rectTransform.anchoredPosition = Vector2.zero;
		inner.Show ();
	}

	public void Hide(){
		img.color = new Color(img.color.r, img.color.g, img.color.b, hideAlpha);
		int width = (left) ? Screen.width / 4 : 3 * Screen.width / 4;
		img.rectTransform.anchoredPosition = new Vector2 (width, Screen.height / 2);
	}


}
