using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager Instance = null;

	//初始化单例
	private void Awake()
	{
		//初始化
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		//Main 物体 跨场景
		DontDestroyOnLoad(gameObject);
	}

	//屏幕特效 TODO
	public void ScreenEffect()
	{

	}

	//镜头震动 TODO
	public void CameraShake(float time, float force)
	{

	}

	//镜头放大 TODO
	public void CameraZoom(float time, float size)
	{

	}

	//镜头跟踪 TODO
	public void CameraFocus(float time, Vector2 focus)
	{

	}
}
