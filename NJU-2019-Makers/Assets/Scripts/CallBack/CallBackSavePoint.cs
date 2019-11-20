using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackSavePoint : CallBack
{
	public Map1Manager manager;
	public Map1Manager.SavePoint save;
	public int MapNum;
	public bool End = false;
	public override void Fun()
	{
		if (End) GameManager.Instance.StartGame("TempEnd");
		CameraManager.Instance.ReloadMap(MapNum);
		manager.ReLoad(manager.save = save);
	}
}
