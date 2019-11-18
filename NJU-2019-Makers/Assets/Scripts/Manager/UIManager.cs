using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance = null;

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

	//UI画布及子物体
	public Canvas canvas;
	public Image Mask;
	public Image TopBlack;
	public Image BottomBlack;
	public Image BloodMask;
	public Text Dialog;
	public Text Combo;
	public Text ComboBlack;
	public Text Score;
	public Text ScoreBlack;
	public Animator ComboAnimator;
	public Animator BulletUIAnimator;
	public Animator ScoreAnimator;
	public Image BossHealth;


	private float health;
	private float maxhealth;

	//上下对话信息
	private const float ShowHideTime = 0.6f;
	private const float BlackProportion = 0.15f;
	private Vector2 BlackShow;
	private Vector2 BlackHide;

	private int scoreNum;
	private float scoreChangeRate = 0.05f;

	//画布大小
	private Vector2 m_Canvasize;

	//初始化
	public void Init()
	{
		canvas.gameObject.SetActive(true);

		var rect = canvas.GetComponent<RectTransform>();
		Mask.rectTransform.sizeDelta = rect.sizeDelta;
		TopBlack.rectTransform.sizeDelta = rect.sizeDelta;
		BottomBlack.rectTransform.sizeDelta = rect.sizeDelta;
		BloodMask.rectTransform.sizeDelta = rect.sizeDelta;
		Dialog.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x * 1.1f, rect.sizeDelta.y * BlackProportion * 0.9f) ;

		Mask.rectTransform.localPosition = Vector3.zero;
		BlackHide = new Vector2(0, rect.sizeDelta.y);
		BlackShow = BlackHide - BlackProportion * BlackHide;
		TopBlack.rectTransform.localPosition = BlackHide;
		BottomBlack.rectTransform.localPosition = -BlackHide;
		BloodMask.rectTransform.localPosition = Vector3.zero;
		Dialog.rectTransform.localPosition = new Vector2(0, rect.sizeDelta.y * (-1 + BlackProportion) / 2);

		Dialog.fontSize = (int)rect.sizeDelta.x / 30;
		Dialog.enabled = false;
		Mask.color = Color.clear;
		BloodMask.color = Color.clear;
	}

	private void Start()
	{
		Init();
		//test
		//Flash(Color.black, Color.clear, 0, Statics.FunType.X);
	}

	//开始播放剧情 UI TODO
	public void StartPlayVideo()
	{

	}

	//显示对话框 TODO
	public void ShowDialog()
	{
		StartCoroutine(Statics.Move(TopBlack.transform, BlackHide, BlackShow, ShowHideTime));
		StartCoroutine(Statics.Move(BottomBlack.transform, -BlackHide, -BlackShow, ShowHideTime));
		Dialog.enabled = true;
		Dialog.text = "";
	}

	//显示文字 TODO 
	public void ShowText(string text,float time = 0)
	{
		Dialog.text = text;
		if (time != 0) StartCoroutine(Statics.WorkAfterSeconds(() => { Dialog.text = ""; }, time));
	}

	//隐藏文字和对话框 TODO
	public void HideDialogAndText()
	{
		StartCoroutine(Statics.Move(TopBlack.transform, BlackShow, BlackHide, ShowHideTime));
		StartCoroutine(Statics.Move(BottomBlack.transform, -BlackShow, -BlackHide, ShowHideTime));
		Dialog.enabled = false;
	}

	//更新UI条 TODO
	public void updateUI()
	{
		scoreNum += Mathf.CeilToInt((PlayerManager.Instance.Score - scoreNum) * scoreChangeRate);
		string tmp = scoreNum.ToString();
		while (tmp.Length <= 8) tmp = "0" + tmp;
		Score.text = ScoreBlack.text = tmp;
		if (maxhealth > 0)
		{
			float tar = health / maxhealth;
			BossHealth.fillAmount += (tar - BossHealth.fillAmount) * 0.05f;
		}
	}

	public void Flash(Color st,Color ed,float time,Statics.FunType t = Statics.FunType.X)
	{
		//Mask.color = st;
		StartCoroutine(Statics.Flash(Mask, st, ed, time, t));
	}

	public void Flash(Color st, Color ed, float time,float last, Statics.FunType t = Statics.FunType.X)
	{
		//Mask.color = st;
		StartCoroutine(Statics.Flash(Mask, st, ed, time, t));
		StartCoroutine(Statics.WorkAfterSeconds(() => { StartCoroutine(Statics.Flash(Mask, ed, st, time)); }, time+last));
	}

	const float bloodtime = 0.2f;
	public void BloodFlash(float time = bloodtime)
	{
		var st = new Color(1, 0, 0, 0f);
		var ed = new Color(1, 0, 0, 0.5f);
		StartCoroutine(Statics.Flash(BloodMask, st,ed, bloodtime));
		StartCoroutine(Statics.WorkAfterSeconds(() => { StartCoroutine(Statics.Flash(BloodMask, ed, st, bloodtime)); }, bloodtime));
	}

	public void ComboShow(int x)
	{
		ComboBlack.text = Combo.text = x + "Combo";
		ComboAnimator.SetBool("Combo", true);
		StartCoroutine(Statics.WorkAfterFrame(() => { ComboAnimator.SetBool("Combo", false); }, 2));
	}

	public void ScoreBig()
	{
		ScoreAnimator.SetBool("Score", true);
		StartCoroutine(Statics.WorkAfterFrame(() => { ScoreAnimator.SetBool("Score", false); }, 2));
	}

	public void SetBulletState(bool f)
	{
		BulletUIAnimator.SetBool("HaveBullet", f);
	}

	public void UseBossHealth(float health , bool use)
	{
		maxhealth = health;
		BossHealth.gameObject.SetActive(use);
	}

	public void SetCurHealth(float h)
	{
		health = h;
	}
	//设置临时UI条
	//..........

	private void FixedUpdate()
	{
		updateUI();
	}

	private void Update()
	{
		//test
		//if (Input.GetKey(KeyCode.A))
		//	ShowDialog();
		//if (Input.GetKey(KeyCode.S))
		//	HideDialogAndText();
		//if (Input.GetKey(KeyCode.D))
		//	ShowText("dddddddddddd");
  //      if (Input.GetKey(KeyCode.F))
  //      {
  //          BloodFlash();
  //          EffectManager.Instance.CameraShake(0.2f, 0.5f);
  //      }

	}
}
