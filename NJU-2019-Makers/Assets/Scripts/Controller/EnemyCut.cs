using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//作为子弹和切削敌人的头部出现
//不具有移动功能 移动功能另加Move组件
public class EnemyCut : MonoBehaviour
{
	//是否始终跟随
	private bool isStatic;
	//移动类型
	public Move move { get; private set; }



	//初始化 应该写完了 TODO
	public void Init(bool s,Move m) { isStatic = s; move = m; }

	//攻击到玩家 应该写完了 还有特效 音效 TODO
	public void Attack()
	{
		PlayerManager.Instance.BeingCut(gameObject);
		Destroy(gameObject);
	}

	//攻击到玩家核心 应该写完了 还有特效 音效 TODO
	public void AttackHeart()
	{
		PlayerManager.Instance.AttackHeart(gameObject);
		Destroy(gameObject);
	}

	//碰到墙 应该写完了 还有特效 音效 TODO
	public void Wall()
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "PlayerHeart")
		{
			AttackHeart();
		}
		if (collision.gameObject.tag == "PlayerEdge")
		{
			Attack();
		}
		else if (collision.gameObject.tag == "Wall")
		{
			Wall();
		}
	}

	private void Start()
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
	}
}
