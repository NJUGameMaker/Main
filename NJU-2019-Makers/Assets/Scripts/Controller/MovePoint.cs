using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
	public Transform target;
	public CallBack callBack;
	private Animator animator;


	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.pause || GameManager.Instance.playVideo) return;
		if (collision.tag == "PlayerHeart")
		{
			animator.SetBool("Active", true);
			UIManager.Instance.Flash(Color.clear, Color.black, 1, 0.5f);
			PlayerManager.Instance.m_rb.velocity = Vector2.zero;
			GameManager.Instance.GamePause();
			PlayerManager.Instance.GoSmall();
			StartCoroutine(Statics.WorkAfterSeconds(() => { PlayerManager.Instance.transform.position = target.position; animator.SetBool("Active", false); }, 1));
			StartCoroutine(Statics.WorkAfterSeconds(PlayerManager.Instance.GoBig, 1)); 
			if (callBack) StartCoroutine(Statics.WorkAfterSeconds(callBack.Fun, 1));
			StartCoroutine(Statics.WorkAfterSeconds(GameManager.Instance.GameRestart, 1.5f));
		}
	}
}
