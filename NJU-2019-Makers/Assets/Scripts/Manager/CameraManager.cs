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
	public GameObject BackGroundOutSide;
	public GameObject BackGroundRoad;
	//背景的父物体（控制中心点）
	public GameObject PBackGround;
	//重复背景
	private GameObject[,] m_backGrounds = new GameObject[3, 3];
	//当前相机坐标(以背景为一格)
	private Vector2Int m_cameraPos;
	//背景图片的大小
	private Vector2 BGsize;
	//地图编号
	public int MapNumber;
	//花纹路径
	public const string Path = "Prefabs/MapFlower/";
	//花纹信息
	private int outSize = 0, roadSize = 0;
	private List<GameObject> outside = new List<GameObject>();
	private List<GameObject> road = new List<GameObject>();
	//花纹间隔
	public int StepX;
	public int StepY;
	public int RandomX;
	public int RandomY;
	public int RandomR;

	public void AddFlower(GameObject obj,Vector2 size,List<GameObject> list)
	{
		for (int x = 0; x < size.x; x += StepX )
		{
			for (int y = 0; y < size.y; y += StepY)
			{
				int Rx = Random.Range(-RandomX, RandomX);
				int Ry = Random.Range(-RandomY, RandomY);
				int Rr = Random.Range(-RandomR, RandomR);
				var tmp = Instantiate(list[Random.Range(0, list.Count)], obj.transform);
				tmp.transform.localPosition = new Vector2(x+Rx,y+Ry) / Scale;
				tmp.transform.localRotation = Quaternion.Euler(0,0,Rr);
			}
		}
		Debug.Log("OK");
	}

	//加载花纹
	private void LoadFlower()
	{
		for (int i = 1; true; i++)
		{
			var tmpo = Resources.Load(Path + "M" + MapNumber.ToString() + "O" + i.ToString()) as GameObject;
			var tmpr = Resources.Load(Path + "M" + MapNumber.ToString() + "R" + i.ToString()) as GameObject;
			if (tmpo) { outside.Add(tmpo);}
			if (tmpr) { road.Add(tmpr);}
			if (!tmpr && !tmpo) break;
		}
	}

	//初始化数据
	public void Init()
	{
		LoadFlower();
		m_camera = Camera.main.gameObject;
		var background = BackGroundRoad.GetComponent<SpriteRenderer>().sprite.rect;
		BGsize = new Vector2(background.width, background.height) / Scale;
		BackGroundRoad.transform.localPosition = BGsize / 2;
		BackGroundOutSide.transform.localPosition = BGsize / 2;
		m_cameraPos = new Vector2Int(Mathf.FloorToInt(m_camera.transform.position.x / BGsize[0]), Mathf.FloorToInt(m_camera.transform.position.y / BGsize[1]));
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				m_backGrounds[i + 1, j + 1] = Instantiate(PBackGround, (m_cameraPos + new Vector2Int(i, j)) * BGsize, new Quaternion());
				m_backGrounds[i + 1, j + 1].SetActive(true);
				//增加花纹
				AddFlower(m_backGrounds[i + 1, j + 1], new Vector2(background.width, background.height),outside);
				AddFlower(m_backGrounds[i + 1, j + 1], new Vector2(background.width, background.height),road);
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
		if (delta.x != 0) {
			for (int i = 0; i < 3; i++) Statics.SwapPos(m_backGrounds[0, i].transform,m_backGrounds[2, i].transform);
			for (int i = 0; i < 3; i++) Statics.SwapPos(m_backGrounds[1, i].transform,m_backGrounds[1+delta.x, i].transform);
		}
		if (delta.y != 0) {
			for (int i = 0; i < 3; i++) Statics.SwapPos(m_backGrounds[i, 0].transform,m_backGrounds[i, 2].transform);
			for (int i = 0; i < 3; i++) Statics.SwapPos(m_backGrounds[i, 1].transform,m_backGrounds[i, 1+delta.y].transform);
		}
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
