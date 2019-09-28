using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Statics : MonoBehaviour
{
	//委托（函数指针类型）
	public delegate void vFunv();
	public delegate bool bFunv();
	public delegate Vector2 V2Funv();

    public enum FunType
	{
		X2,
        X
	}
    //将size从startsize到endsize的平滑移动
	public static float FixFun(FunType type,float startsize, float endsize,float step)
	{
        if(startsize > endsize)//若是下降形式，那么将其反向处理，转成上升
        {
            float tmp = startsize;
            startsize = endsize;
            endsize = tmp;
            step = 1 - step;
        }
        //保证startsize < endsize且step在（0，1）
        switch (type)
        {
            case FunType.X: { return startsize + (endsize - startsize) * step; };break;
            case FunType.X2: { return startsize + (endsize - startsize) * step * step; } break;
            default: Debug.LogAssertion("Wrong FunType!");break;
        }
		return 0;
	}

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

	public static Vector3 V2toV3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0);
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
