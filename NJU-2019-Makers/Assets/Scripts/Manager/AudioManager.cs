using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance = null;


	//设置音频路径
	private const string path = "Audios/";
	//设置音频名
	private readonly string[] AudioNames = { "test","test2" };
	//音频映射
	private Dictionary<string, AudioClip> AudioDic = new Dictionary<string, AudioClip>();
	//BGM播放
	private AudioSource BGMPlayer;
	//音效播放
	private HashSet<AudioSource> SoundPlayer = new HashSet<AudioSource>();
	private Dictionary<Statics.bFunv, AudioSource> SoundPlayerUntil = new Dictionary<Statics.bFunv, AudioSource>();
	//音效 BGM 音量
	private float _BGMVolume, _SoundVolume;
	public float BGMVolume { get => _BGMVolume; set => _BGMVolume = BGMPlayer.volume = Statics.InRange(value, 0, 1); }
	public float SoundVolume { get => _SoundVolume; set => _SoundVolume = Statics.InRange(value, 0, 1); }
	//临时删除表
	private List<Statics.bFunv> tmpdel = new List<Statics.bFunv>();

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

	//播放音效
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

	//持续播放
	public void PlayUntil(string name, Statics.bFunv fun)
	{
		if (AudioDic.ContainsKey(name))
		{
			var tmp = NewAudioSource(SoundVolume,true);
			SoundPlayerUntil[fun] = tmp;
			tmp.clip = AudioDic[name];
			tmp.Play();
		}
	}

	//播放背景音乐
	public void PlayBGM(string name)
	{
		if (AudioDic.ContainsKey(name))
		{
			BGMPlayer.clip = AudioDic[name];
			BGMPlayer.volume = _BGMVolume;
			BGMPlayer.loop = true;
			BGMPlayer.Play();
		}
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
		BGMVolume = SoundVolume = 1;
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
		Debug.Log(SoundPlayerUntil.Keys.Count);
		if (Input.GetKeyDown(KeyCode.A))
			PlaySound("test",()=>Debug.Log("Over"));
		if (Input.GetKeyDown(KeyCode.S))
			PlayUntil("test2",()=>Input.GetKey(KeyCode.S));
		if (Input.GetKeyDown(KeyCode.S))
			PlayUntil("test2", () => Input.GetKey(KeyCode.S));
		if (Input.GetKeyDown(KeyCode.D))
			PlayBGM("test");
		if (Input.GetKeyDown(KeyCode.F))
			PlayBGM("test2");
		if (Input.GetKeyDown(KeyCode.Q))
			SoundVolume += 0.1f;
		if (Input.GetKeyDown(KeyCode.W))
			SoundVolume -= 0.1f;
	}
}
