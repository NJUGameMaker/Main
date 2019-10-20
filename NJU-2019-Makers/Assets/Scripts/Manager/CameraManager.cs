using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance = null;

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

	//像素点与坐标的比例
	const int Scale = 100;

	//相机的引用
	private GameObject m_camera;
	//背景
	public GameObject BackGround;
	//背景的父物体（控制中心点）
	public GameObject PBackGround;
	//重复背景
	private GameObject[,] m_backGrounds = new GameObject[3, 3];
	//当前相机坐标(以背景为一格)
	private Vector2Int m_cameraPos;
	//背景图片的大小
	private Vector2 BGsize;

	//初始化数据
	public void Init()
	{
		m_camera = Camera.main.gameObject;
		var background = BackGround.GetComponent<SpriteRenderer>().sprite.rect;
		BGsize = new Vector2(background.width, background.height) / Scale;
		BackGround.transform.localPosition = BGsize / 2;
		m_cameraPos = new Vector2Int(Mathf.FloorToInt(m_camera.transform.position.x / BGsize[0]), Mathf.FloorToInt(m_camera.transform.position.y / BGsize[1]));
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				m_backGrounds[i + 1, j + 1] = Instantiate(PBackGround, (m_cameraPos + new Vector2Int(i, j)) * BGsize, new Quaternion());
				m_backGrounds[i + 1, j + 1].SetActive(true);
			}
		}
		PBackGround.SetActive(false);
		EffectManager.Instance.SetCameraContinueFocus(() => { return PlayerManager.Instance.transform.position; }, true);
	}

	void Start()
    {
		Init();
		//test
		//StartCoroutine(Statics.Move(m_camera.transform, m_camera.transform.position, new Vector3(1000, 0, -10), 100));
	}

	//根据相机位置调整背景
	void FixBackGround()
	{
		if (!m_camera) Init();
		Vector2Int nowpos = new Vector2Int(Mathf.FloorToInt(m_camera.transform.position.x / BGsize[0]), Mathf.FloorToInt(m_camera.transform.position.y / BGsize[1]));
		var delta = nowpos - m_cameraPos;
		if (delta != Vector2Int.zero)
		{
			foreach (var item in m_backGrounds)
			{
				item.transform.position += (Vector3)(BGsize * delta);
			}
			m_cameraPos = nowpos;
		}
	}

	// Update is called once per frame
	void Update()
	{
		FixBackGround();
	}
}
