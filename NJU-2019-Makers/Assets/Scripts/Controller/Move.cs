using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自动移动脚本
public class Move : MonoBehaviour
{

	//移动类型 
	enum MoveType
	{
		Stop,
		Line,
		Cruve,
		Round,
		AIFollow
		//TODO ...
	};

	//设置各种类型移动的参数并且设置各种初始化函数 TODO
	public void Init()
	{

	}

	//根据类型和参数进行移动 TODO
	private void work()
	{

	}

    // Update is called once per frame
    void Update()
    {
		//暂停
		if (GameManager.Instance.pause)
		{
			return;
		}
		//播放剧情
		if (GameManager.Instance.playVideo)
		{
			return;
		}
		work();
	}
}
