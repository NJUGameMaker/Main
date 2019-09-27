using System.Collections;
using System.Collections.Generic;
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

		//Main 物体 跨场景
		DontDestroyOnLoad(gameObject);
	}

	//开始播放剧情 UI TODO
	public void StartPlayVideo()
	{

	}

	//显示对话框 TODO
	public void ShowDialog()
	{

	}

	//显示文字 TODO 
	public void ShowText(string text)
	{

	}

	//隐藏文字和对话框 TODO
	public void HideDialogAndText()
	{

	}

	//更新UI条 TODO
	public void updateUI()
	{

	}

	//设置临时UI条
	//..........

	private void Update()
	{
		updateUI();
	}
}
