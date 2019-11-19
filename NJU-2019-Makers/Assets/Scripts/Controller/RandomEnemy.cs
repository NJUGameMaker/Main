using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int MaxNum;
    public int IntervalTime;
    public GameObject[] EnemyList;
    public GameObject player;
    public float EnterDistance;
    public int EnemyKinds;
    public float DistanceWithPlayer;
    public float Radius;
    public int EnemyNum { get; private set; }
    public bool IsActive { get; set; }
    private float timer;
    private GameObject LiveEnemy;

    /*IEnumerator Go()
    {
        while (IsActive)
        {
            ActiveEnemy();
            yield return new WaitForSeconds(IntervalTime);
                
        }
    }*/
    void Start()
    {
        LiveEnemy = new GameObject("ContainerForLiveEnemy");
        //timer = 0;
        IsActive = true;
        EnemyKinds = EnemyList.Length;
        //DistanceWithPlayer = 6;
        //EnterDistance = 10;
        //StartCoroutine(Active);
        player = PlayerManager.Instance.gameObject;
    }
    void ClearLiveEnemy()
    {
        for (int i = LiveEnemy.transform.childCount - 1; i >= 0; i--)
            Destroy(LiveEnemy.transform.GetChild(i).gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            ClearLiveEnemy();
        
        timer += Time.deltaTime;
        if ((transform.position - player.transform.position).magnitude > EnterDistance)
            return;
        if (LiveEnemy.transform.childCount >= MaxNum && timer < IntervalTime)
        {
            Debug.Log("Too Much Enmey so don't create" + timer + LiveEnemy.transform.childCount);
            return;
        }
        else if (LiveEnemy.transform.childCount >= MaxNum)
            ClearLiveEnemy();
        if (LiveEnemy.transform.childCount <= 0 || timer > IntervalTime)
        {
            Debug.Log("To create" + timer + LiveEnemy.transform.childCount);
            CreateEnemy();
            timer = 0;
        }
        
        //timer += Random.value;
    }

    public void CreateEnemy()
    {
        if (LiveEnemy.transform.childCount > MaxNum)
            return;
        int index = Random.Range(0, EnemyKinds);
        Vector3 pos = transform.position + new Vector3(Random.Range(-Radius, Radius), Random.Range(-Radius, Radius), 0); //新建位置
        if ((player.transform.position - pos).magnitude < DistanceWithPlayer)
            return;
        GameObject obj = Object.Instantiate(EnemyList[index], pos, Quaternion.identity);
        obj.transform.parent = LiveEnemy.transform;
        var ai = obj.GetComponent<EnemyAI>();
        ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
            ai.BeActive();
        }, 1.5f));
        
    }
}
