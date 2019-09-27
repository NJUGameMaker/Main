using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAI : MonoBehaviour
{
	//自身Enemy引用
	private Enemy enemy;

    // 初始化自身属性 TODO
    void Start()
    {
		enemy = gameObject.AddComponent<Enemy>();
		enemy.Init(100, gameObject.AddComponent<Move>());
		enemy.move.Init();
    }

	//自身AI行为 攻击 等 TODO
	private void myAI()
	{

	}

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
		myAI();
	}
}
