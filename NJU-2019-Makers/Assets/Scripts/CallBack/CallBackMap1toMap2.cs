using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackMap1toMap2 : CallBack
{
	public override void Fun()
	{
		CameraManager.Instance.ReloadMap(3);
	}
}
