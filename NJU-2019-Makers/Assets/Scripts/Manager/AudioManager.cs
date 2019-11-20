using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance = null;


	//设置音频路径
	private const string path = "Audios/";
	//设置音频名
	private readonly string[] AudioNames = {
		"StartBGM","Map1BGM","Map2BGM","Map3BGM","BossBGM",
		"Shoot1","Shoot2","HitEnemy",
		"EnemyDie","HitWall","EnemyActive",
		"MovePoint","EnemyShoot","BecomeGoast",
		"Bomb","Hurt","HaveNoBullet",
		"BeingSmall","Shoot3","ShootCut","Cut","Collect"
	};
	//音频映射
	private Dictionary<string, AudioClip> AudioDic = new Dictionary<string, AudioClip>();
	//BGM播放
	private AudioSource BGMPlayer;
	//音效播放
	private HashSet<AudioSource> SoundPlayer = new HashSet<AudioSource>();
	private Dictionary<Statics.bFunv, AudioSource> SoundPlayerUntil = new Dictionary<Statics.bFunv, AudioSource>();
	//音效 BGM 音量
	private float _BGMVolume, _SoundVolume;
	private float fixBGMVolume = 1;
	public float BGMVolume { get => _BGMVolume; set => _BGMVolume = BGMPlayer.volume = Statics.InRange(Statics.InRange(value, 0, 1) * fixBGMVolume, 0, 1); }
	public float SoundVolume { get => _SoundVolume; set => _SoundVolume = Statics.InRange(value, 0, 1); }
	//临时删除表
	private List<Statics.bFunv> tmpdel = new List<Statics.bFunv>();

	[HideInInspector]
	public string CurBGM = "";

	//从路径加载音频
	private AudioClip LoadAudioClip(string name)
	{
		return Resources.Load(path + name) as AudioClip;
	}

	//新建播放器
	private AudioSource NewAudioSource(float volume = 1, bool loop = false)
	{
		GameObject go = new GameObject("AudioPlayer");
		DontDestroyOnLoad(go);
		AudioSource tmp = go.AddComponent<AudioSource>();
		tmp.volume = volume;
		tmp.loop = loop;
		return tmp;
	}

	//播放音效 第二个参数是音效播放完后的回调函数
	public void PlaySound(string name,Statics.vFunv fun = null)
	{
		if (AudioDic.ContainsKey(name))
		{
			var tmp = NewAudioSource(SoundVolume);
			SoundPlayer.Add(tmp);
			tmp.clip = AudioDic[name];
			if (fun != null) { StartCoroutine(Statics.WorkAfterSeconds(fun, AudioDic[name].length)); }
			StartCoroutine(Statics.DestroyAfterSeconds(tmp.gameObject, AudioDic[name].length));
			tmp.Play();
		}
	}

	//持续播放 第二个参数是持续播放的条件 同一个持续播放不要重复调用
	public void PlayUntil(string name, Statics.bFunv fun,bool loop = false)
	{
		if (AudioDic.ContainsKey(name) && !SoundPlayerUntil.ContainsKey(fun))
		{
			var tmp = NewAudioSource(SoundVolume, loop);
			SoundPlayerUntil[fun] = tmp;
			tmp.clip = AudioDic[name];
			tmp.Play();
		}
	}

	//播放背景音乐
	public void PlayBGM(string name,float fix = 1)
	{
		if (AudioDic.ContainsKey(name) && CurBGM != name)
		{
			CurBGM = name;
			fixBGMVolume = fix;
			BGMPlayer.clip = AudioDic[name];
			BGMPlayer.volume = _BGMVolume * fixBGMVolume;
			BGMPlayer.loop = true;
			BGMPlayer.Play();
		}
	}

	public void StopBGM()
	{
		StartCoroutine(m_StopBGM());
	}

	private IEnumerator m_StopBGM()
	{
		for (float t = 0; t < 2; t += Time.deltaTime) {
			BGMPlayer.volume *= 0.98f;
			yield return new WaitForEndOfFrame();
		}
		BGMPlayer.Stop();
	}

	private void Start()
	{
		//加载所有音频
		foreach (var s in AudioNames)
		{
			AudioDic[s] = LoadAudioClip(s);
		}
		//创建BGMPLAYER
		BGMPlayer = NewAudioSource();
		//设置默认音量
		BGMVolume = 0.7f;
		SoundVolume = 0.3f;
		PlayBGM("StartBGM");
	}


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

		//Main 物体 跨场景
		DontDestroyOnLoad(gameObject);

	}

	private void Update()
	{
		//维护持续播放列表
		foreach (var x in SoundPlayerUntil)
		{
			if (!x.Key())
			{
				tmpdel.Add(x.Key);
				Destroy(x.Value.gameObject);
			}
		}
		foreach (var x in tmpdel)
		{
			SoundPlayerUntil.Remove(x);
		}
		tmpdel.Clear();


		//test
		//Debug.Log(SoundPlayerUntil.Keys.Count);
		//if (Input.GetKeyDown(KeyCode.A))
		//	PlaySound("test",()=>Debug.Log("Over"));
		//if (Input.GetKeyDown(KeyCode.S))
		//	PlayUntil("test2",()=>Input.GetKey(KeyCode.S));
		//if (Input.GetKeyDown(KeyCode.S))
		//	PlayUntil("test2", () => Input.GetKey(KeyCode.S));
		//if (Input.GetKeyDown(KeyCode.D))
		//	PlayBGM("test");
		//if (Input.GetKeyDown(KeyCode.F))
		//	PlayBGM("test2");
		//if (Input.GetKeyDown(KeyCode.Q))
		//	SoundVolume += 0.1f;
		//if (Input.GetKeyDown(KeyCode.W))
		//	SoundVolume -= 0.1f;
	}
}
