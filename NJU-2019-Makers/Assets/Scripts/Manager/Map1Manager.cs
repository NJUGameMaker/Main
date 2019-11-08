using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map1Manager : MonoBehaviour
{
	//玩家出生点
	public Transform PlayerStart;
	public GameObject Player;
	//剧情关键点
	public Transform[] StoryEnemy1St;
	public Transform[] StoryEnemy1Ed;
	public GameObject StoryEnemy1;
	public GameObject StoryShotBullet;
	public GameObject Level1;

	private List<GameObject> objs = new List<GameObject>();

	private bool MouseDown;

	//初始化
	public void Init()
	{
		MouseDown = true;
		Player.transform.position = PlayerStart.position;
		GameManager.Instance.GameVideo();
		//EffectManager.Instance.CameraFocus(1, PlayerStart.position, Statics.FunType.SqrtX);
		//UIManager.Instance.HideDialogAndText();
	}

	public IEnumerator Story1_Start()
	{
		const float MoveTime = 12f;
		const float WaitTime = 2f;
		const float SlowTime = 6f;
		for (int i = 0; i < StoryEnemy1St.Length; i++)
		{
			Transform st = StoryEnemy1St[i], ed = StoryEnemy1Ed[i];
			var mid = st.position * 0.2f + ed.position * 0.8f;

			var tmp = Instantiate(StoryEnemy1, st);
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
		EffectManager.Instance.SetCameraContinueFocus(() => { return objs[2].transform.position; }, true);
		EffectManager.Instance.CameraZoom(0.8f, 2.5f);
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
		EffectManager.Instance.CameraZoom(2f, 6f);
		UIManager.Instance.HideDialogAndText();
		yield return new WaitForSeconds(2f);
		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.04f);
		EffectManager.Instance.CameraZoom(2f, 1.5f);
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowDialog();
		yield return new WaitForSeconds(1f);
		UIManager.Instance.ShowText("“戈罗，你在犹豫什么？”", 1.5f);
		yield return new WaitForSeconds(1.5f);
		UIManager.Instance.ShowText("“现在不掏枪就死定啦！”", 1.5f);
		yield return new WaitForSeconds(0.5f);
		EffectManager.Instance.CameraZoom(1f, 6.5f);
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
		UIManager.Instance.ShowText("【WASD上下左右，鼠标左键发射子弹】 [左键继续...]");
		MouseDown = false; while (!MouseDown) yield return new WaitForEndOfFrame();

		for (int i = 0; i < 3; i++)
		{
			Statics.SetEnable(objs[i], true);
		}

		EffectManager.Instance.SetCameraContinueFocus(() => { return Player.transform.position; }, true, 0.4f);
		UIManager.Instance.HideDialogAndText();
		GameManager.Instance.GameRestart();
		Level1.SetActive(true);
	}

	public IEnumerator Story1()
	{
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(Story1_Start());
		StartCoroutine(Story1_Dialog1());
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
		Init();
		StartCoroutine(Story1());
    }

    // Update is called once per frame
    void Update()
    {
		MouseUpdate();
    }
}
