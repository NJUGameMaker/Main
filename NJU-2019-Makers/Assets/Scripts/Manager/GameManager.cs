﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// GameManager 单例
	public static GameManager Instance = null;

	//是否暂停
	public bool pause { get; private set; }
	//是否播放剧情
	public bool playVideo { get; private set; }
	//分数统计
	private int score;
	public int Score { get => score; set => score = value; }
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

	//场景切换
	public void ChangeScene(string name) => SceneManager.LoadScene(name);

	//暂停
	public void GamePause() => pause = true;

	private void VideoUISet(bool b)
	{
		UIManager.Instance.BulletUI.SetActive(b);
		UIManager.Instance.MainScore.SetActive(b);
	}
	//播放剧情
	public void GameVideo() { playVideo = true; VideoUISet(false); }

	//开始播放剧情
	public void GameRestart() {  pause = playVideo = false; VideoUISet(true); PlayerManager.Instance.Bomb(); }

	public void StartGame(string name)
	{
		ChangeScene(name);
	}

	private void Start()
	{
		//StartGame("TestScene");
	}

	private void Update()
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
		//镜头跟随主角
		//Debug.Log(PlayerManager.Instance.transform.position);
		//EffectManager.Instance.CameraFocus(0, PlayerManager.Instance.transform.position);
	}

}
