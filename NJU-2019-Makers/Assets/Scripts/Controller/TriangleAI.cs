using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAI : MonoBehaviour
{
	//自身Enemy引用
	private Enemy enemy;

    //子弹的Preb By BJY
    public GameObject bulletPrefab;

    // 初始化自身属性 TODO
    void Start()
    {
		enemy = gameObject.AddComponent<Enemy>();    
		enemy.Init(100, gameObject.AddComponent<Move>());
		enemy.move.Init();
        enemy.move.SetStopType(Statics.V3toV2(transform.position));
    }
    //发射帧间隔(s)：
    public const float interval = 0.2f;
    //能够发射子弹
    private bool canFire = true;
    //自身AI行为 攻击 等 TODO:BJY
    private void myAI()
	{
        if (canFire)
        {
            float damage = 0;
            
            canFire = false;
            StartCoroutine(Statics.WorkAfterSeconds(() => canFire = true, interval));
            var pos = Statics.V3toV2(transform.position);
            GameObject bullet = GameObject.Instantiate(bulletPrefab, Statics.V2toV3(pos), Quaternion.identity) as GameObject;
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.Init(damage, false, bullet.AddComponent<Move>());
            enemyBullet.move.SetAIFollowType(PlayerManager.Instance.transform.gameObject, bullet.GetComponent<Rigidbody2D>());
        }
        
    }

	//血量低于0后行为
	private void AfterDie()
	{
        Destroy(this);
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
		if (enemy.health < 0)
		{
			AfterDie();
		}
	}
}
