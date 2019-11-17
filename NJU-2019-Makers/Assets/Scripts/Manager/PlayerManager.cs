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
	};

	//left right 为 以圆心到切削点为正方向 切削点->左边一个碰撞器上的点 和 切削点->右边一个碰撞器上的点 的角度
	public struct CutPointInfo
	{
		public Vector2 vector;
		public float left;
		public float right;
	}

	//外层碰撞器(改了类型)
	public PolygonCollider2D EdgeCollider;
	//内层碰撞器
	public CircleCollider2D HeartCollider;
	//外层贴图
	public SpriteRenderer EdgeSprite;
	//内层贴图
	public SpriteRenderer HeartSprite;
	//切削遮罩
	public GameObject CutMask;
	//切削产生的角
	public HashSet<GameObject> Angles = new HashSet<GameObject>();
	//边界点 (改成了动态数组和类型)
	public Vector2[] Points = null;
	//切削产生的点的编号(改成了记录点的vector,是相对位置！)
	public HashSet<CutPointInfo> KeyPoints = new HashSet<CutPointInfo>();

	//血量
	public float maxHealth = 100;
	public float health;

	//能量（缩放），0->初始不放缩情况
	private float maxEnergy;
	public float energy;

	//子弹条
	public float maxBullet = 100;
	public float bullet;


	//弹开之后的保护状态
	public bool protect { get; private set; }
	public const float protect_time = 0.6f;

	private BulletType bulletType;
	private SkillType skillType;

	//按键配置
	private bool LEFT => Input.GetKey(KeyCode.A);
	private bool RIGHT => Input.GetKey(KeyCode.D);
	private bool UP => Input.GetKey(KeyCode.W);
	private bool DOWN => Input.GetKey(KeyCode.S);
	private bool FIRE => Input.GetMouseButton(0) && !FireLock;
	private bool SMALL => Input.GetKey(KeyCode.Space) && !BombLock;
	private bool BOMB => Input.GetKeyUp(KeyCode.Space) && !BombLock;
	//鼠标位置
	private Vector2 MOUSE => Camera.main.ScreenToWorldPoint(Input.mousePosition);
	//获取子物体
	private GameObject GOHeart => HeartCollider.gameObject;
	private GameObject GOEdge => EdgeCollider.gameObject;
	//自身刚体
	public Rigidbody2D m_rb { get; private set; }
	//动画
	private Animator EdgeAnimator;
	private Animator HeartAnimator;
	//子弹Prefeb
	public GameObject BulletNormal;
	public GameObject BulletStrong;
	public GameObject BulletTan;
	public GameObject BulletCut;

	//子弹发射位置
	public Transform FirePosEdge;
	public Transform FirePosHeart;

	//图形界面加预设物体 TODO
	//public GameObject BulletPrefab;

	//子弹类型伤害 和 速度
	public const float NoneDamage = 10;
	public const float StrongDamage = 10;
	public const float BounceDamage = 5;
	public const float NoneSpeed = 15;
	public const float StrongSpeed = 15;
	public const float BounceSpeed = 20;


	//子弹误差默认值（角度值）：
	public const float deviation = 0;
	//发射帧间隔(s) 缩放所减少的子弹间隔：
	public const float shoot_interval = 0.4f;
	public const float shoot_change_interval = 0.3f;
	//最大的子弹增伤
	public const float damage_change = 3;
	//能够发射子弹（子弹间隔）
	private bool canFire;
	//装弹冷却
	private bool noBullet;
	//每发子弹消耗的百分比->每秒消耗的百分比
	private float fireCost = 50;
	//子弹回复速度
	private float reBullet = 30;

	// 放缩所需参数：
	//初始壳大小
	public const float initial_size = 3f;
	//初始心大小
	public const float initial_heart_size = 0.6f;
	//逐渐缩小过程中的时间间隔
	public const float small_interval = 0.02f;
	//具有攻击力的最小缩小程度（反弹阈值）
	public const float bounce_thresold = 0.5f;
	//内心的最大值
	public const float heart_max = 0.2f;
	//缩小进度（最大为1），用于使用曲线渐变函数的
	private float step_percent;
	//缩小进度的单位增长值，用于使用曲线渐变函数的
	public const float step_interval = 0.05f;
	//放缩技能冷却时间
	public const float bounce_cd = 0.5f;
	//反弹后的无敌时间
	public const float invincible_time = 0.5f;
	private bool canBomb;
	private bool canSmall;

	// 缓慢自愈单次血量：
	public const float reBlood = 0.003f;

	public Animator EdgeBomb;
	public Animator HeartBomb;
	public Animator MainAnimator;


	//Combo计数
	public const float ComboResetTime = 1;
	private float ComboTime = 0;
	private int ComboCnt = 0;

	public Vector2 CameraOffset;

	[HideInInspector]
	public bool BombLock;
	[HideInInspector]
	public bool FireLock;
	[HideInInspector]
	public bool MoveLock;
	[HideInInspector]
	public int DieTime;
	[HideInInspector]
	public int Score;
	public Map1Manager Map1;


	public void ComboAdd()
	{
		ComboTime = ComboResetTime;
		ComboCnt++;
		UIManager.Instance.ComboShow(ComboCnt);
	}

	public void ComboUpdate()
	{
		ComboTime -= Time.deltaTime;
		if (ComboTime < 0)
		{
			if (ComboCnt != 0)
			{
				UIManager.Instance.ScoreBig();
				Score += (ComboCnt * ComboCnt * ComboCnt * 77) >> DieTime;
			}
			ComboTime = 0;
			ComboCnt = 0;
		}
	}


	//增加切削遮罩
	private void addCutMask(Vector2 vec)
	{
		var tmp = Instantiate(CutMask,GOEdge.transform);
		tmp.SetActive(true);
		tmp.transform.localPosition = vec;
		tmp.transform.localRotation = Quaternion.Euler(0, 0, -90+(Mathf.Atan2(vec.y, vec.x) / Mathf.PI * 180));// (0, 0,Mathf.Atan2(vec.y,vec.x), 1);
	}

	private void FaceToMouse()
	{
		var tmp = -(Vector2)transform.position + MOUSE;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(tmp.y, tmp.x) / Mathf.PI * 180f);
	}

	//当攻击键按下 TODO 不对啊还有子弹条没考虑进去-->有道理哦
	//那还要留一个UI的接口
	public void Fire()
	{
		if (FireLock) return;
		if (canFire)
		{
			float damage = 0;
			float speed = 0;
			EffectManager.EffectType effectType = EffectManager.EffectType.End;
			GameObject BulletPrefab = BulletNormal;
			switch (bulletType)
			{
				case BulletType.None:
					damage = NoneDamage;
					speed = NoneSpeed;
					effectType = EffectManager.EffectType.PlayerNormalOut;
					BulletPrefab = BulletNormal;
					break;
				case BulletType.Strong:
					damage = StrongDamage;
					speed = StrongSpeed;
					effectType = EffectManager.EffectType.PlayerStrongOut;
					BulletPrefab = BulletStrong;
					break;
				case BulletType.Bounce:
					damage = BounceDamage;
					speed = BounceSpeed;
					effectType = EffectManager.EffectType.PlayerTanOut;
					BulletPrefab = BulletTan;
					break;
				default:
					break;
			}
			canFire = false;
			float interval = shoot_interval - shoot_change_interval * energy / maxEnergy;
			damage += damage * energy / maxEnergy * damage_change;
			bullet -= fireCost * interval;
			StartCoroutine(Statics.WorkAfterSeconds(() => canFire = true, interval));
			var pos = Statics.V3toV2(transform.position);
			GameObject GObullet = GameObject.Instantiate(BulletPrefab, Statics.V2toV3(canBomb ? FirePosHeart.position : FirePosEdge.position), Quaternion.identity) as GameObject;
			GObullet.SetActive(true);
			//bullet.GetComponent<SpriteRenderer>().sprite = sprite;
			PlayerBullet playerBullet = GObullet.AddComponent<PlayerBullet>();
			playerBullet.Init(bulletType, damage, false, GObullet.AddComponent<Move>());
			float angle = Mathf.Atan2((MOUSE - pos).y, (MOUSE - pos).x) * Mathf.Rad2Deg + Random.Range(-deviation, deviation) * (1 - energy / maxEnergy);
			GObullet.transform.rotation = Quaternion.Euler(0, 0, angle);
			Vector2 direct = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			playerBullet.move.SetLineType(pos, direct, speed*2,0,0,GObullet.GetComponent<Rigidbody2D>());
			//playerBullet.StartCoroutine(Statics.WorkAfterSeconds(() => { playerBullet.move.acc = 0; }, 0.5f));
			EffectManager.Instance.PlayEffect(effectType, canBomb ? FirePosHeart.position : FirePosEdge.position, transform.rotation, 1f);
			AudioManager.Instance.PlaySound("Shoot2");
		}
	}

	//当缩小键按下 改变外壳大小 改变内核大小 改变enegy数值 音效 特效 TODO
	public void BeingSmall()
	{
		if(canSmall && step_percent <= 1){
			canSmall = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => {canSmall = true;energy = Statics.FixFun(Statics.FunType.SqrtX,0,maxEnergy,step_percent);},small_interval));
			step_percent += step_interval;
			float scale = energy / maxEnergy;
			if(scale >= bounce_thresold){
				canBomb = true;
			}
			//当形状足够小的时候把外壳设置为false 能够穿进敌人
			if (energy > 80)
			{
				EdgeCollider.enabled = false;
			}

		}
	}

	public void PlayBomb()
	{
		int num = Mathf.CeilToInt(energy / maxEnergy / (bounce_thresold/2));
		num = num >= 5 ? 4 : num;
		Statics.BombPlay(this, HeartBomb, num == 1 ? 2 : num);
		Statics.BombPlay(this, EdgeBomb, num);

	}

	//当放开缩小键 恢复外壳大小 恢复内核大小 设置短时间无敌 改变enegy数值（可能要设置缩放CD） 音效 特效
	//考虑技能释放 考虑角的攻击 TODO
	public void Bomb()
	{
		PlayBomb();
		EdgeCollider.enabled = true;
		if (canBomb){
			energy = 0;
			step_percent = 0;
			canBomb = false;
			StartCoroutine(Statics.WorkAfterSeconds(() => canSmall = false,small_interval));	//以免被BeingSmall的协程函数更改canSmall的值
			StartCoroutine(Statics.WorkAfterSeconds(() => canSmall = true,bounce_cd));
			protect = true;
			StartCoroutine(Statics.WorkAfterSeconds(() => protect = false,protect_time));
			ShootCut();
		}
		else{
			energy = 0;
			step_percent = 0;
		}
	}

	//切削子弹参数
	public float CutVelocity;
	public float CutAcc;
	public float CutAccTime;
	public float CutLastTime;

	public void ShootCut()
	{
		Debug.Log("cut run");
		foreach (var item in KeyPoints)
		{
			Vector2 tmpvec = transform.TransformPoint(item.vector) - transform.position;
			Debug.Log(tmpvec);
			GameObject GObullet = GameObject.Instantiate(BulletCut, transform.position, Quaternion.identity) as GameObject;
			GObullet.SetActive(true);
			GObullet.GetComponent<PlayerCut>().SetAngle(item.left, item.right);
			PlayerBullet playerBullet = GObullet.AddComponent<PlayerBullet>();
			playerBullet.Init(bulletType, 100000, false, GObullet.AddComponent<Move>());
			float angle = Mathf.Rad2Deg * Mathf.Atan2(tmpvec.y, tmpvec.x);
			GObullet.transform.rotation = Quaternion.Euler(0, 0, angle);
			Vector2 direct = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			playerBullet.move.rotate = 180;
			playerBullet.move.SetLineType(transform.position, direct, CutVelocity ,CutAcc , CutAccTime, GObullet.GetComponent<Rigidbody2D>());
			Destroy(GObullet, CutLastTime);
			//playerBullet.StartCoroutine(Statics.WorkAfterSeconds(() => { playerBullet.move.acc = 0; }, 0.5f));

		}

	}

	//表示移动的速度
	const float speed = 12;
    //移动 TODO
    public void Move()
	{
        
        float inputX = Input.GetAxis("Horizontal");//横向上的移动
        float inputY = Input.GetAxis("Vertical");//竖直线上的移动
        Vector3 v = new Vector3(inputX, inputY, 0); //新建移动向量
		v = v.normalized;                              //如果是斜线方向，需要对其进行标准化，统一长度为1
		v = v * speed;                //乘以速度调整移动速度，乘以deltaTime防止卡顿现象
		//transform.Translate(v);                       //移动
		m_rb.velocity = v;
        if (LEFT)
		{

		}

		if (RIGHT)
		{

		}

		else if (UP)
		{

		}

		else if (DOWN)
		{

		}
	}

	//内核被攻击 死亡 或者 读取存档点等 TODO 先只考虑死亡
	public void AttackHeart(GameObject other)
	{
		if (protect) return;
		Hurt();
		m_rb.velocity = Vector3.zero;
		GameManager.Instance.GameVideo();
		UIManager.Instance.Flash(Color.black, Color.red, 0.5f);
		//StartCoroutine(Statics.WorkAfterSeconds(() => { UIManager.Instance.Flash(Color.black, Color.red, 0.5f); }, 0.5f));
		StartCoroutine(Statics.WorkAfterSeconds(() => { UIManager.Instance.Flash(Color.red, Color.clear, 1f); }, 2f));
		StartCoroutine(Statics.WorkAfterSeconds(() => { GameManager.Instance.GameRestart(); }, 2f));
		

		Map1.ReStart(Map1.save);
		
		//health = 0;
		//Statics.AnimatorPlay(this, EdgeAnimator, Statics.AnimatorType.Die);
		//Statics.AnimatorPlay(this, HeartAnimator, Statics.AnimatorType.Die);
		//Destroy(gameObject,5f);
	}

	public void Hurt()
	{
		Statics.AnimatorPlay(this, EdgeAnimator, Statics.AnimatorType.Attack);
		Statics.AnimatorPlay(this, HeartAnimator, Statics.AnimatorType.Attack);
		EffectManager.Instance.CameraShake(0.5f, 0.5f);
		UIManager.Instance.BloodFlash();
	}

	//受到攻击 减少生命 减少外壳大小 音效 特效 TODO
	public void BeingAttack(float damage)
	{
		if (protect) return;
        health -= damage;
		maxEnergy = health;
		Hurt();
	}

	//受到切削 改变外壳形状 新增角的攻击点 维护Mask （需要判断是否切到核心） 音效 特效 TODO
	public void BeingCut(GameObject other)
	{
	}

	public void BeingCut(GameObject other, Vector2 point, Vector2 dir)
	{
		if (protect) return;
		Hurt();
		//求交点
		Vector2[] intersectPs = new Vector2[2];
		int[] pos = new int[2];
		int pointer = 0; //工具人，后续会重新使用这个变量，不用管
		for(int i = 0; i < Points.Length; i++){
			Vector2 intersect = Statics.IntersectPoint(point,dir,EdgeCollider.transform.TransformPoint(Points[i]),EdgeCollider.transform.TransformPoint(Points[(i+1)%Points.Length]));
			if(!float.IsNaN(intersect.x)){
				intersectPs[pointer] = intersect;
				pos[pointer] = i;
				pointer++;
			}
		}
		//判断是否切到核心
		Vector2 center = Statics.V3toV2(EdgeCollider.transform.position);
		float distance = Mathf.Abs((dir.y*center.x - dir.x*center.y + dir.x*point.y-dir.y*point.x) / (Mathf.Pow(dir.y*dir.y+dir.x*dir.x,0.5f)));
		if(distance <= HeartCollider.radius)
		{
			AttackHeart(other);
			return;
		}
		//维护边界点集和碰撞器
		Vector2[] tmp = new Vector2[Points.Length+2];
		pointer = 0;
		for(int i = 0; i <= pos[0]; i++){
			if(!Statics.WhetherDelete(point,dir,center,EdgeCollider.transform.TransformPoint(Points[i]))){
				tmp[pointer] = Points[i];
				pointer++;
			}
		}
		tmp[pointer] = EdgeCollider.transform.InverseTransformPoint(intersectPs[0]);
		pointer++;
		for(int i = pos[0]+1; i <= pos[1]; i++){
			if(!Statics.WhetherDelete(point,dir,center,EdgeCollider.transform.TransformPoint(Points[i]))){
				tmp[pointer] = Points[i];
				pointer++;
			}
		}
		tmp[pointer] = EdgeCollider.transform.InverseTransformPoint(intersectPs[1]);
		pointer++;
		for(int i = pos[1]+1; i < Points.Length; i++){
			if(!Statics.WhetherDelete(point,dir,center,EdgeCollider.transform.TransformPoint(Points[i]))){
				tmp[pointer] = Points[i];
				pointer++;
			}
		}
		Points = new Vector2[pointer];
		for(int i = 0; i < pointer; i++){
			Points[i] = tmp[i];
		}
		EdgeCollider.points = Points;
		//添加切削点，维护切削点集
		for(int i = 0; i < intersectPs.Length; i++){
			CutPointInfo tmpinfo = new CutPointInfo();
			//TODO : cutpointinfo 的角度 删除被切削的切削点
			tmpinfo.left = -30; tmpinfo.right = 30;
			//TODO : cutpointinfo 的角度 删除被切削的切削点
			tmpinfo.vector = EdgeCollider.transform.InverseTransformPoint(intersectPs[i]);

			KeyPoints.Add(tmpinfo);
		}
		//添加遮罩
        Vector2 vec = new Vector2(dir.y, dir.x);
        vec.Normalize();
        vec = vec * distance;
        float y = (dir.y / dir.x) * (center.x - point.x) + point.y;
        if (y > center.y)
        {
            if (vec.y > 0)
            {
                vec.x = -vec.x;
            }
            else
            {
                vec.y = -vec.y;
            }
        }
        else
        {
            if (vec.y > 0)
            {
                vec.y = -vec.y;
            }
            else
            {
                vec.x = -vec.x;
            }
        }
        addCutMask(EdgeCollider.transform.InverseTransformVector(vec));
	}

	//回血 TODO: done.
	public void ReHealth(float x)
	{
		if((health + x) >= maxHealth){
			health = maxHealth;
			maxEnergy = health;
		}else{
			health += x;
			maxEnergy = health;
		}
	}

    //随帧检测参数设置圆形大小
    public void ReShape()
    {
        if(energy == 0)
        {
            float scale = health / maxHealth;
            GOEdge.transform.localScale = new Vector3(initial_size*scale, initial_size*scale, 1);
            GOHeart.transform.localScale = new Vector3(initial_heart_size, initial_heart_size, 1);
        }
        else
        {
            float energyScale = energy / maxEnergy;
			float bloodScale = health / maxHealth;
            GOEdge.transform.localScale = new Vector3(initial_size*bloodScale*(1 - energyScale), initial_size*bloodScale*(1 - energyScale), 1);
            if(canBomb)
            {
                float heartScale = ((energyScale - bounce_thresold) / (1 - bounce_thresold))*heart_max;
                GOHeart.transform.localScale = new Vector3(initial_heart_size + heartScale, initial_heart_size + heartScale, 1);
            }
        }
		//Debug.Log(GOEdge.transform.localScale.ToString() +" " +energy + " "+health);
    }

	public void SetBullet(BulletType bullet)
	{
		bulletType = bullet;
	}

	public void SetSkill(SkillType skill)
	{
		skillType = skill;
	}

	// 初始化 TODO
	void Start()
    {
		health = maxHealth;
		energy = 0;
		bullet = maxBullet;
		protect = false;
		bulletType = BulletType.None;
		skillType = SkillType.None;
        GOEdge.transform.localScale = new Vector3(initial_size, initial_size, 1);
        GOHeart.transform.localScale = new Vector3(1, 1, 1);
		canFire = true;
		step_percent = 0;
		canBomb = false;
		canSmall = true;
		maxEnergy = health;
		m_rb = GetComponent<Rigidbody2D>();
		EdgeAnimator = GOEdge.GetComponent<Animator>();
		HeartAnimator = GOHeart.GetComponent<Animator>();
		Points = EdgeCollider.points;
		CameraOffset = Vector2.zero;
		DieTime = 0;

		EffectManager.Instance.SetCameraContinueFocus(() => { return PlayerManager.Instance.transform.position; }, true, 0.2f);


		//BulletNormal = Resources.Load("Sprites/normal.png") as Sprite;
		//BulletStrong = Resources.Load("Sprites/strong.png") as Sprite;
		//BulletTan = Resources.Load("Sprites/tantan.png") as Sprite;
		//Debug.Log(BulletNormal);

	}

	void StartReBullet()
	{
		UIManager.Instance.SetBulletState(false);
		noBullet = true;
	}

	void ReBullet(float x)
	{
		bullet += x * reBullet * Time.deltaTime;
		if (bullet >= maxBullet)
		{
			bullet = maxBullet;
			noBullet = false;
			UIManager.Instance.SetBulletState(true);
		}
	}

	public void GoSmall()
	{
		MainAnimator.SetBool("Small",true);
	}

	public void GoBig()
	{
		MainAnimator.SetBool("Small", false);
	}

	// Update is called once per frame
	// 添加了随每帧缓慢恢复血量、改变大小
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
		FaceToMouse();

		if (FIRE && bullet > 0 && !noBullet)
		{
			Fire();
		}
		else if (bullet <= 0 && !noBullet)
		{
			StartReBullet();
		}else if (noBullet)
		{
			ReBullet(2);
		}else if (!FIRE)
		{
			ReBullet(1);
		}

		if (SMALL)
		{
			BeingSmall();
		}

		if (BOMB)
		{
			Bomb();
		}
        
        if (!MoveLock) Move();
		ReHealth(reBlood);
        ReShape();
		ComboUpdate();
	}
}
