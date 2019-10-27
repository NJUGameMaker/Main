using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAI : MonoBehaviour
{
	//自身Enemy引用
	private Enemy enemy;

    //子弹的Preb By BJY
    public GameObject bulletPrefab;

	//速度
	const float speed = 1f;

    // 初始化自身属性 TODO
    void Start()
    {
		enemy = gameObject.AddComponent<Enemy>();    
		enemy.Init(100, gameObject.AddComponent<Move>());
		enemy.move.Init();
        enemy.move.SetStopType(transform.position);
    }
    //发射帧间隔(s)：
    public const float interval = 3f;
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
			bullet.SetActive(true);
			//子弹
			//EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
			//bullet.tag = "EnemyBullet";
			//enemyBullet.Init(damage, false, bullet.AddComponent<Move>());
			//enemyBullet.move.SetAIFollowType(PlayerManager.Instance.transform.gameObject, speed, bullet.GetComponent<Rigidbody2D>());

			//切削子弹
			var cut = bullet.AddComponent<EnemyCut>();
			bullet.tag = "EnemyCut";
			cut.Init(false, bullet.AddComponent<Move>());
			cut.move.SetLineType(transform.position, PlayerManager.Instance.transform.position - transform.position, speed, bullet.GetComponent<Rigidbody2D>());
			//enemyBullet.tag = "EnemyCut";
			//enemyBullet.move.SetAIFollowType(PlayerManager.Instance.transform.gameObject,speed,bullet.GetComponent<Rigidbody2D>());

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
