using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCut : MonoBehaviour
{
	public GameObject Left;
	public GameObject Right;

	public void SetAngle(Vector2 left,Vector2 mid,Vector2 right)
	{
		var l = left - mid;
		var r = right - mid;
		Left.transform.rotation = Quaternion.Euler(0, 0,Mathf.Rad2Deg*Mathf.Atan2(l.y, l.x));
		Right.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(r.y, r.x));
	}

	public void SetAngle(float left, float right)
	{
		Left.transform.rotation = Quaternion.Euler(0, 0, left);
		Right.transform.rotation = Quaternion.Euler(0, 0, right);
	}

}
