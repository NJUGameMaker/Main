using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackMap1toMap2 : CallBack
{
	public Map1Manager manager;
	public override void Fun()
	{
		CameraManager.Instance.ReloadMap(3);
		manager.ReLoad(manager.save = Map1Manager.SavePoint.Map2Level1);
	}
}
