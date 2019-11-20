using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackToBoss : CallBack
{
	public Map1Manager manager;
	public override void Fun()
	{
		PlayerManager.Instance.BombLock = false;
		PlayerManager.Instance.FireLock = false;
		PlayerManager.Instance.MoveLock = false;
		manager.save = Map1Manager.SavePoint.Map4Boss;
		manager.ReLoad(Map1Manager.SavePoint.Map4Boss);
		CameraManager.Instance.ReloadMap(4);
	}
}