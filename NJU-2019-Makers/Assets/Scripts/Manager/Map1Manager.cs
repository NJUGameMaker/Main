using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Manager : MonoBehaviour
{
	public enum SavePoint
	{
		Map1Level1,
		Map1Level2,
		Map1Level3,
		Map1Story2,
		Map2Level1,
		Map2Level2,
		Map2Level3,
		Map2Level4,
		Map4Boss,
	}

	//玩家出生点
	public Transform PlayerStart;
	public GameObject Player;
	//剧情关键点
	public Transform[] StoryEnemy1St;
	public Transform[] StoryEnemy1Ed;
	public GameObject Story1Enemy1;
	public GameObject StoryShotBullet;

	public GameObject Story2Enemys;
	public Transform Story2Effect;

	public GameObject[] Levels;
	public Transform[] SavePos;

	private GameObject CurrentLevel;
	private List<GameObject> objs = new List<GameObject>();

	private bool MouseDown;
	private GameObject Story2Temp;
	private bool StoryEnd;

	public SavePoint save;

	public void ReLoad(SavePoint s)
	{
		UIManager.Instance.UseBossHealth(0, false);
		Destroy(CurrentLevel);
		CurrentLevel = Instantiate(Levels[(int)s], Levels[(int)s].transform.parent);
		CurrentLevel.SetActive(true);
		//temp
		if (s == SavePoint.Map4Boss)
		{
			EffectManager.Instance.CameraZoom(1f, 15f);
		}
		else
		{
			EffectManager.Instance.CameraZoom(1f, 8.5f);
		}
	}

	public void ReStart(SavePoint s)
	{
		if (s == SavePoint.Map1Level1) PlayerManager.Instance.BombLock = true;
		PlayerManager.Instance.FireLock = false;
		PlayerManager.Instance.MoveLock = false;
		PlayerManager.Instance.transform.position = SavePos[(int)s].position;
		PlayerManager.Instance.health = PlayerManager.Instance.maxHealth;
		Destroy(Story2Temp);
		ReLoad(s);
	}

	//初始化
	public void Init()
	{
		UIManager.Instance.Mask.color = Color.black;
		UIManager.Instance.UseBossHealth(0, false);
		UIManager.Instance.HideDialogAndText();
		CameraManager.Instance.ReloadMap(1);
		MouseDown = true;
	}

	public IEnumerator Story1_Start()
	{
		PlayerManager.Instance.BombLock = true;
		PlayerManager.Instance.FireLock = true;
		PlayerManager.Instance.MoveLock = false;
		GameManager.Instance.GameVideo();
		const float MoveTime = 12f;
		const float WaitTime = 2f;
		const float SlowTime = 6f;
		for (int i = 0; i < StoryEnemy1St.Length; i++)
		{
			Transform st = StoryEnemy1St[i], ed = StoryEnemy1Ed[i];
			var mid = st.position * 0.2f + ed.position * 0.8f;

			var tmp = Instantiate(Story1Enemy1, st);
			objs.Add(tmp);
			tmp.SetActive(true);
			tmp.transform.rotation = Statics.FaceTo(ed.position - st.position, -90);
			objs[i].GetComponent<Move>().direction = ed.position - st.position;
			objs[i].GetComponent<Enemy>().Active = true;
			StartCoroutine(Statics.MoveWorld(tmp.transform, st.position, mid, MoveTime, Statics.FunType.X));
			StartCoroutine(Statics.WorkAfterSeconds(() => { StartCoroutine(Statics.MoveWorld(tmp.transform, mid, ed.position, SlowTime, Statics.FunType.SqrtX)); }, MoveTime+WaitTime));
		}
		yield return new WaitForSeconds(0);
	}

	public IEnumerator Story1_Dialog1()
	{
		UIManager.Instance.Flash(Color.black, Color.clear, 1.5f);
		//yield return new WaitForSeconds(1f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return objs[2].transform.position; }, true);
		EffectManager.Instance.CameraZoom(0.8f, 3f);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowDialog();
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("快跟上！杀了它！", 1f);
		yield return new WaitForSeconds(1.2f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return objs[0].transform.position; }, true);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("包围！包围！别让他跑了！", 1f);
		yield return new WaitForSeconds(1.2f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return objs[1].transform.position; }, true);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("冲鸭！！！", 1f);
		yield return new WaitForSeconds(1.2f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.008f);
		EffectManager.Instance.CameraZoom(2f, 6.5f);
		UIManager.Instance.HideDialogAndText();
		yield return new WaitForSeconds(2f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.04f);
		EffectManager.Instance.CameraZoom(2f, 2f);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowDialog();
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("“戈罗，你在犹豫什么？”", 1.5f);
		yield return new WaitForSeconds(1.5f);
		UIManager.Instance.ShowText("“再不反击就死定啦！”", 1.5f);
		yield return new WaitForSeconds(0.5f);
		EffectManager.Instance.CameraZoom(1f, 8.5f);
		yield return new WaitForSeconds(1f);
		StoryShotBullet.SetActive(true);
		StartCoroutine(Statics.WorkAfterSeconds(() => { StoryShotBullet.SetActive(false); }, 1));
		yield return new WaitForSeconds(0.5f);
		UIManager.Instance.ShowText("“小心！！”", 1f);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("被三角族撞到或者射击到会变虚弱！ [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("你的壳会缩小，等到缩小太过的时候，心也会变大。 [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("要记得不论什么时候，心被击中就完蛋啦！ [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("先冲出去，逃出森林再说吧！ [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【WASD上下左右，鼠标左键发射子弹】 [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【注意左下角子弹条，子弹耗尽会进入冷却】 [左键开始游戏...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();

		for (int i = 0; i < 3; i++)
		{
			Statics.SetEnable(objs[i], true);
		}

		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.4f);
		UIManager.Instance.HideDialogAndText();
		GameManager.Instance.GameRestart();

		ReLoad(SavePoint.Map1Level1);
		save = SavePoint.Map1Level1;

		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		PlayerManager.Instance.FireLock = false;

	}

	public IEnumerator PreStory()
	{
		const float showTime = 5f;
		const float waitTime = 6.5f;
		Player.transform.position = PlayerStart.position;
		GameManager.Instance.GameVideo();
		StoryEnd = false;
		UIManager.Instance.Mask.color = Color.black;
		UIManager.Instance.ShowDialog();
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowStoryTextMid("十五年前的一个清晨", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextMid("阿婆推开门", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextMid("发现了她——戈罗·奈特", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextMid("一个与边角国格格不入的圆族", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextMid("她的身边只有一封信", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowPicture("Story0_1", waitTime*2+showTime);
		UIManager.Instance.ShowStoryTextBottom("这也是阿婆十五年来唯一瞒着她的事情", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextBottom("在阿婆的抚养之下", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextBottom("戈罗度过了一个无忧无虑的童年", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.ShowStoryTextMid("但平静的生活被突如其来的三角军团打破", showTime);
		yield return new WaitForSeconds(waitTime/2);
		AudioManager.Instance.StopBGM();
		yield return new WaitForSeconds(waitTime/2);
		AudioManager.Instance.PlayBGM("Map1BGM",0.5f);
		UIManager.Instance.ShowPicture("Story0_2", waitTime);
		UIManager.Instance.ShowStoryTextBottom("而戈罗的身世之谜，也将由她亲手解开", showTime);
		yield return new WaitForSeconds(waitTime);
		UIManager.Instance.HideDialogAndText();
		yield return new WaitForSeconds(1);
		StoryEnd = true;
	}

	public IEnumerator Story1()
	{
		yield return new WaitForEndOfFrame();
		StartCoroutine(PreStory());
		while (!StoryEnd) yield return new WaitForEndOfFrame();
		StartCoroutine(Story1_Start());
		yield return new WaitForEndOfFrame();
		StartCoroutine(Story1_Dialog1());
		yield return new WaitForEndOfFrame();
	}

	void Story2EnemyShow()
	{
		for (int i = 0; i < Story2Temp.transform.childCount; i++)
		{
			StartCoroutine(Statics.Flash(Story2Temp.transform.GetChild(i).GetComponentInChildren<SpriteRenderer>(), Color.clear, Color.white,1));
		}
	}

	void Story2EnemyActive()
	{
		for (int i = 0; i < Story2Temp.transform.childCount; i++)
		{
			StartCoroutine(Story2Temp.transform.GetChild(i).GetComponent<EnemyAI>().StartActive());
		}
	}

	public IEnumerator Story2()
	{
		PlayerManager.Instance.health = PlayerManager.Instance.maxHealth;
		save = SavePoint.Map1Story2;
		Story2Temp = Instantiate(Story2Enemys, Story2Enemys.transform.parent);
		Story2Temp.SetActive(true);
		PlayerManager.Instance.BombLock = false;
		PlayerManager.Instance.FireLock = true;
		PlayerManager.Instance.MoveLock = true;
		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.4f);
		GameManager.Instance.GameVideo();
		//yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(0.5f);
		GameManager.Instance.GameVideo();
		yield return new WaitForSeconds(0.5f);
		UIManager.Instance.ShowDialog();
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("哈哈哈！！逃出来啦！！", 1.5f);
		yield return new WaitForSeconds(1.5f);
		UIManager.Instance.ShowText("Emmmm。。。", 1.5f);
		yield return new WaitForSeconds(0.8f);
		Story2EnemyShow();
		EffectManager.Instance.PlayEffect(EffectManager.EffectType.Active, Story2Effect.position, Quaternion.identity);
		yield return new WaitForSeconds(0.7f);
		UIManager.Instance.ShowText("糟了！！中计了！！！完了完了完了。。", 1.5f);
		yield return new WaitForSeconds(1.5f);
		UIManager.Instance.ShowText("“别慌！你可是圆形啊！弹开它们的子弹反击吧！”[左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【长按 鼠标右键/空格键 蓄力】[左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【松开时可以反弹附近的敌人和子弹】[左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【蓄力时会获得更高的移动速度，子弹射速和伤害】[左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【当蓄力达到临界值时可以穿越某些特定障碍】[左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		UIManager.Instance.ShowText("【但是！注意！蓄力会让脆弱的内心暴露在外！】[左键开始游戏...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();
		Story2EnemyActive();
		UIManager.Instance.HideDialogAndText();
		GameManager.Instance.GameRestart();
		while (Story2Temp && Story2Temp.transform.childCount != 0) yield return new WaitForEndOfFrame();
		if (Story2Temp)
		{
			PlayerManager.Instance.FireLock = false;
			PlayerManager.Instance.MoveLock = false;
			save = SavePoint.Map1Level2;
			ReLoad(SavePoint.Map1Level2);
		}
		yield return new WaitForEndOfFrame();
	}

	//更新鼠标信息
	private void MouseUpdate()
	{
		if (Input.GetMouseButtonDown(0)) MouseDown = true;
	}

	// Start is called before the first frame update
	void Start()
    {
		//EffectManager.Instance.CameraZoom(1f, 8.5f);
		//EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.4f);

		Init();
		//UIManager.Instance.Mask.color = Color.clear;
		StartCoroutine(Story1());
	}

	// Update is called once per frame
	void Update()
    {
		MouseUpdate();
    }
}
