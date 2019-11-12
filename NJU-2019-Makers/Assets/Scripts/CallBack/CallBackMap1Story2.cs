using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBackMap1Story2 : CallBack
{
	public Map1Manager manager;
	public override void Fun()
	{
		manager.StartCoroutine(manager.Story2());
	}
}
