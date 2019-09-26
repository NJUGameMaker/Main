using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Statics : MonoBehaviour
{
	public delegate void vFunv();
	public delegate bool bFunv();

	public static float InRange(float x,float mn,float mx)
	{
		return Mathf.Max(Mathf.Min(mx, x),mn);
	}

	public static IEnumerator WorkAfterSeconds(vFunv fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun();
	}
	public static IEnumerator DestroyAfterSeconds(GameObject go, float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(go);
	}
}
