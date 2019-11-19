using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public enum DoorLabel{
        Up,
        Down,
        Left,
        Right,
        GoIn
    };
    public DoorLabel Label; 
    public GameObject Manager;
    public CallBack callBack;
    public Transform target;
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
            if (Manager)
            {
                target = Manager.GetComponent<GateManager>().NextPosition((int)Label);
            }
            else
                Debug.LogWarning("No to Point");
            animator.SetBool("Active", true);
            AudioManager.Instance.PlaySound("MovePoint");
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

