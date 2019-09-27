using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Instance = null;

	//初始化单例
	private void Awake()
	{
		//初始化
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public enum BulletType
	{
		None,
		Strong,
		Bounce
	};

	public enum SkillType
	{
		None,
		Satellite,
		Laser
	}

	//外层碰撞器
	public Collider2D EdgeCollider;
	//内层碰撞器
	public Collider2D HeartCollider;
	//外层贴图
	public SpriteRenderer EdgeSprite;
	//内层贴图
	public SpriteRenderer HeartSprite;
	//切削产生的角
	public HashSet<GameObject> Angles = new HashSet<GameObject>();

	private float maxHealth = 100;
	private float maxEnergy = 100;
	private float maxBullet = 100;
	private float health = 100;
	private float energy = 100;
	private float bullet = 100;
	private BulletType bulletType;
	private SkillType skillType;

	public void Fire()
	{

	}

	public void BeingSmall()
	{

	}

	public void Bomb()
	{

	}

	public void Move()
	{

	}

	public void BeingAttack(Collider2D other)
	{

	}

	public void BeingCut(Collider2D other)
	{

	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
