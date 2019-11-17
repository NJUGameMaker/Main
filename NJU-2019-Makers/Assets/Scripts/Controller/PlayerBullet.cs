using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//作为玩家子弹和炸开时候角的攻击出现
//不具有移动功能 移动功能另加Move组件
public class PlayerBullet : MonoBehaviour
{

	//子弹类型
	private PlayerManager.BulletType bulletType;
	//伤害
	public float damage;
	//是否始终跟随
	private bool isStatic;
	//移动类型
	public Move move { get; private set; }
	//动画
	private Animator animator;

	//初始化 应该写完了 TODO
	public void Init(PlayerManager.BulletType type, float d, bool s, Move m)
	{
		bulletType = type; damage = d; isStatic = s; move = m;
	}


	//攻击到敌人 根据子弹类型判断 弹开（修改自己的move）穿透还是删除自身 
	//敌人扣血和特效写在enemy 不用考虑 TODO
	public void Attack(GameObject enemy)
	{
		if (tag == "PlayerCut" || enemy.GetComponent<Enemy>().isGoast) return;
		switch (bulletType)
		{
			case PlayerManager.BulletType.None:
				EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerNormalOn, transform.position, transform.rotation, 1f);
				Destroy(gameObject);
				break;
			case PlayerManager.BulletType.Strong:
				EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerStrongOn, transform.position, transform.rotation, 1f);
				
				break;
			case PlayerManager.BulletType.Bounce:
				EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerTanOn, transform.position, transform.rotation, 1f);
				//TODO 反弹
				break;
			default:
				break;
		}
	}

	//碰到墙 应该写完了 TODO
	public void Wall()
	{
		//Debug.Log(isStatic);
		if (tag != "PlayerCut")
		{
			switch (bulletType)
			{
				case PlayerManager.BulletType.None:
					EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerNormalOn, transform.position, transform.rotation, 1f);
					AudioManager.Instance.PlaySound("HitWall");
					break;
				case PlayerManager.BulletType.Strong:
					EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerStrongOn, transform.position, transform.rotation, 1f);
					AudioManager.Instance.PlaySound("HitWall");
					break;
				case PlayerManager.BulletType.Bounce:
					EffectManager.Instance.PlayEffect(EffectManager.EffectType.PlayerTanOn, transform.position, transform.rotation, 1f);
					break;
				default:
					break;
			}
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			Attack(collision.gameObject);
		}
		else if (collision.gameObject.tag == "Wall")
		{
			Wall();
		}
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
	}
}
