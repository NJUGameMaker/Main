using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager Instance = null;

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

	//设置预设特效路径
	private const string path = "Prefabs/Effect/";
	//设置特效名
	public enum EffectType
	{
		PlayerNormalOn,
		PlayerNormalOut0,
		PlayerNormalOut1,
		PlayerTanOn,
		PlayerTanOut,
		PlayerStrongOn,
		PlayerStrongOut,
		EnemyNormalOn,
		EnemyNormalOut,

		End
	}
	//特效哈希表
	private Dictionary<EffectType,GameObject> Effects = new Dictionary<EffectType, GameObject>();


	//屏幕特效 TODO
	public void ScreenEffect()
	{

	}

	//镜头震动 TODO
	public void CameraShake(float time, float force)
	{
        //注意：使用前提是将ShakeEffect脚本挂载到Camera上面
        if (Camera.main.GetComponent<ShakeEffect>() != null)
        {
            Camera.main.GetComponent<ShakeEffect>().SetShakePower(force);
            Camera.main.GetComponent<ShakeEffect>().BeginShake(time);
        }
        else
            Debug.LogWarning("Camera can't shake: may dont't have shakeEffect.cs ");
	}

	//协程：镜头放大
	private IEnumerator IECameraZoom(float st,float ed,float time)
	{
		for (float deltime = 0; deltime<time; deltime += Time.deltaTime)
		{
			//TODO 设置type
			Camera.main.orthographicSize = Statics.FixFun(Statics.FunType.X2, st, ed, deltime / time);
			yield return new WaitForEndOfFrame();
		}
	}

	//协程：镜头跟踪
	private IEnumerator IECameraFocus(Vector2 st, Vector2 ed, float time)
	{
		for (float deltime = 0; deltime < time; deltime += Time.deltaTime)
		{
			//TODO 设置type
			Camera.main.transform.position = new Vector3(Statics.FixFun(Statics.FunType.X2, st.x, ed.x, deltime / time), Statics.FixFun(Statics.FunType.X2, st.x, ed.x, deltime / time), -10);
			yield return new WaitForEndOfFrame();
		}
	}

	//镜头放大 TODO csk
	public void CameraZoom(float time, float size)
	{
		StartCoroutine(IECameraZoom(Camera.main.orthographicSize, size, time));
	}

	//镜头跟踪 TODO csk
	public void CameraFocus(float time, Vector2 focus)
	{
		StartCoroutine(IECameraFocus(Camera.main.transform.position, focus, time));
	}

	//是否持续跟踪
	private bool keepFocus;
	//持续跟踪的位置
	private Statics.V2Funv funFocus;
	//设置镜头是否持续跟踪 TODO csk
	public void SetCameraContinueFocus(Statics.V2Funv fun,bool f)
	{
		funFocus = fun;
		keepFocus = f;
	}
	//设置镜头持续跟踪 TODO csk
	public void CameraContinueFocus()
	{
		if (keepFocus)
		{
			const float con = 2;
			//调参数 TODO
			//Debug.Log((Vector3)funFocus());
			Vector3 tmp = Camera.main.transform.position + ((Vector3)funFocus() - Camera.main.transform.position) * Time.deltaTime * con;
			tmp.z = -10;
			Camera.main.transform.position = tmp;
		}
	}

	public void PlayEffect(EffectType type,Vector3 pos,Quaternion qua,float time)
	{
		Destroy(Instantiate(Effects[type],pos,qua),time);
	}

	public void PlayEffect(EffectType type, Vector3 pos, Quaternion qua)
	{
		Instantiate(Effects[type], pos, qua);
	}

	private void Start()
	{
		//初始化特效列表
		for (int i = 0; i < (int)EffectType.End; i++) {
			var type = (EffectType)i;
			Effects.Add(type,Resources.Load(path+type.ToString()) as GameObject);
		}
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
		//镜头持续跟踪
		CameraContinueFocus();
		//test
		if (Input.GetKeyDown(KeyCode.Z))
			SetCameraContinueFocus(() => Input.mousePosition, true);
		//gameObject.AddComponent<PlayerBullet>();
	}
}
