using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class Statics : MonoBehaviour
{
	//委托（函数指针类型）
	public delegate void vFunv();
	public delegate bool bFunv();
	public delegate Vector2 V2Funv();
	public delegate Vector3 V3Funv();

	//表示的是类型：X2表示X^2速度增长，表示先慢后快递增，先快后慢递减
	//X表示匀速递增或递减
	//LogX表示LogX速度，表示先快后慢递增，先慢后快递减。
	//SqrtX表示SqrtX速度，表示先快后慢递增，先慢后快递减。
	public enum FunType
	{
		X2,
		X,
		LogX,
		SqrtX
	}

	public class RefInt
	{
		public int data;
	}
	public class RefFloat
	{
		public float data;
	}
	//将size从startsize到endsize的平滑移动，这里的step是百分比
	public static float FixFun(FunType type, float startsize, float endsize, float step)
	{
		//if (startsize > endsize)//若是下降形式，那么将其反向处理，转成上升
		//{
		//	float tmp = startsize;
		//	startsize = endsize;
		//	endsize = tmp;
		//	step = 1 - step;
		//}
		//保证startsize < endsize且step在（0，1）
		switch (type)
		{
			case FunType.X: { return startsize + (endsize - startsize) * step; }; break;
			case FunType.X2: { return startsize + (endsize - startsize) * step * step; } break;
			case FunType.LogX: { return startsize + (endsize - startsize) * Mathf.Log(step); }; break;
			case FunType.SqrtX: { return startsize + (endsize - startsize) * Mathf.Sqrt(step); } break;
			default: Debug.LogAssertion("Wrong FunType!"); break;
		}
		return 0;
	}

	//x限制在[mn,mx]
	public static float InRange(float x, float mn, float mx)
	{
		return Mathf.Max(Mathf.Min(mx, x), mn);
	}

	//取三维向量的x，y到二维向量
	public static Vector2 V3toV2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 V2toV3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0);
	}

	public static Quaternion FaceTo(Vector2 v,int offset = 0)
	{
		return Quaternion.Euler(0, 0, Mathf.Atan2(v.y, v.x) * 180 / Mathf.PI + offset);
	}

	public static Vector2 FaceVec(Quaternion q,int offset = 0)
	{
		var tmp = (q.eulerAngles.z + offset) / 180 * Mathf.PI;
		return new Vector2(Mathf.Cos(tmp), Mathf.Sin(tmp));
	}

	public static IEnumerator WorkAfterSeconds(vFunv fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun();
	}

	public static IEnumerator WorkAfterFrame(vFunv fun, int frame)
	{
		while (frame-- != 0)
			yield return new WaitForEndOfFrame();
		fun();
	}

	public static IEnumerator DestroyAfterSeconds(GameObject go, float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(go);
	}


	public static IEnumerator Flash(Image img,Color st, Color ed, float time,FunType t = FunType.X)
	{
		float delta = Mathf.Min(1,Time.fixedDeltaTime / time), p = 0;
		while (p<1)
		{
			p += delta;
			img.color = new Color(FixFun(t, st.r, ed.r, p), FixFun(t, st.g, ed.g, p), FixFun(t, st.b, ed.b, p), FixFun(t, st.a, ed.a, p));
			yield return new WaitForFixedUpdate();
		}
	}

	public static IEnumerator Move(Transform transform, Vector3 st, Vector3 ed, float time, FunType t = FunType.X)
	{
		float delta = Mathf.Min(1, Time.fixedDeltaTime / time), p = 0;
		while (p < 1)
		{
			p += delta;
			transform.localPosition = new Vector3(FixFun(t, st.x, ed.x, p), FixFun(t, st.y, ed.y, p), FixFun(t, st.z, ed.z, p));
			yield return new WaitForFixedUpdate();
		}
	}

	public static IEnumerator MoveWorld(Transform transform, Vector3 st, Vector3 ed, float time, FunType t = FunType.X)
	{
		float delta = Mathf.Min(1, Time.fixedDeltaTime / time), p = 0;
		while (p < 1)
		{
			p += delta;
			transform.position = new Vector3(FixFun(t, st.x, ed.x, p), FixFun(t, st.y, ed.y, p), FixFun(t, st.z, ed.z, p));
			yield return new WaitForFixedUpdate();
		}
	}

	public static IEnumerator MoveWorld(Transform transform, Vector3 st,Transform ted, float time, FunType t = FunType.X,float offset = 0)
	{
		float delta = Mathf.Min(1, Time.fixedDeltaTime / time), p = 0;
		while (p < 1)
		{
			p += delta;
			var ed = ted.position;
			ed.z = offset;
			//Debug.Log(ed);
			transform.position = new Vector3(FixFun(t, st.x, ed.x, p), FixFun(t, st.y, ed.y, p), FixFun(t, st.z, ed.z, p));
			yield return new WaitForFixedUpdate();
		}
	}

	public enum AnimatorType
	{
		Attack,
		Die,
		Null
	}

	public static void BombPlay(MonoBehaviour mono, Animator animator, int num)
	{
		if (animator.GetInteger("Bomb") == 0)
			mono.StartCoroutine(WorkAfterFrame(() => { animator.SetInteger("Bomb", 0); }, 2));
		animator.SetInteger("Bomb", num);
	}

	public static void AnimatorPlay(MonoBehaviour mono,Animator animator,AnimatorType type)
	{
		switch (type)
		{
			case AnimatorType.Attack:
				if (animator.GetInteger("State") == 0)
					mono.StartCoroutine(WorkAfterFrame(() => { if (animator.GetInteger("State") == 1) animator.SetInteger("State", 0); }, 2));
				animator.SetInteger("State", 1);
				break;
			case AnimatorType.Die:
				animator.SetInteger("State", 2);
				break;
			case AnimatorType.Null:
				animator.SetInteger("State", 0);
				break;
			default:
				break;
		}
	}

	/*public static bool IsPointCut(Vector2 point, Vector2 point1, Vector2 point2){
		float x1 = (point1.x < point2.x)?(point1.x):(point2.x);
		float x2 = (point1.x >= point2.x)?(point1.x):(point2.x);
		float y1 = (point1.y < point2.y)?(point1.y):(point2.y);
		float y2 = (point1.y >= point2.y)?(point1.y):(point2.y);
		float x = point.x;
		float y = point.y;
		if((x1 <= x)&(x <= x2)&(y1 <= y)&(y <= y2)){
			return true;
		}
		return false;
	}*/

	public static Vector2 IntersectPoint(Vector2 point, Vector2 dir, Vector2 point1, Vector2 point2){
		float k1 = dir.y / dir.x;
		float k2 = (point2.y-point1.y) / (point2.x-point1.x);
		float x = (point2.y - point.y - k2*point2.x + k1*point.x) / (k1-k2);
		float y = k2*(x-point2.x) + point2.y;
		float x1 = (point1.x < point2.x)?(point1.x):(point2.x);
		float x2 = (point1.x >= point2.x)?(point1.x):(point2.x);
		if(x >= x1 & x <= x2){
			Vector2 result = new Vector2(x,y);
			return result;
		}else{
			Vector2 result = new Vector2(float.NaN,float.NaN);
			return result;
		}
	}

	public static bool WhetherDelete(Vector2 cutP, Vector2 dir, Vector2 center, Vector2 point){
		float k1 = dir.y / dir.x;
		float y1 = k1*(center.x-cutP.x) + cutP.y;
		float y2 = k1*(point.x-cutP.x) + cutP.y;
		if((center.y-y1)*(point.y-y2) < 0){
			return true;
		}else{
			return false;
		}
	}

	public static void SwapPos(Transform a,Transform b)
	{
		var tmp = a.position;
		a.position = b.position;
		b.position = tmp;
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	public static void SetEnable(GameObject obj,bool t)
	{
		obj.GetComponent<Enemy>().enabled = t;
		obj.GetComponent<Move>().enabled = t;
	}
}
