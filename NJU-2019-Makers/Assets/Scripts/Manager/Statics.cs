using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Statics : MonoBehaviour
{
	//委托（函数指针类型）
	public delegate void vFunv();
	public delegate bool bFunv();

	//x限制在[mn,mx]
	public static float InRange(float x,float mn,float mx)
	{
		return Mathf.Max(Mathf.Min(mx, x),mn);
	}

	//取三维向量的x，y到二维向量
	public static Vector2 V3toV2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
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
