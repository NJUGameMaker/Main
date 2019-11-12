using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int MaxNum = 10;
    public int IntervalTime = 100;
    public GameObject[] EnemyList;
    public GameObject player;
    public float EnterDistance;
    public int EnemyKinds;
    public float DistanceWithPlayer;
    public float Radius = 2;
    public int EnemyNum { get; private set; }
    public bool IsActive { get; set; }
    private float timer;
    private List<GameObject> LiveEnmey;

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
        LiveEnmey = new List<GameObject>();
        timer = 0;
        IsActive = true;
        EnemyKinds = EnemyList.Length;
        DistanceWithPlayer = 2;
        EnterDistance = 6;
        //StartCoroutine(Active);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            LiveEnmey.Clear();
        
        timer += Time.deltaTime;
        if ((transform.position - player.transform.position).magnitude > EnterDistance)
            return;
        if (LiveEnmey.Count <= 0 || timer > IntervalTime)
        {
            CreateEnemy();
            timer = 0;
        }
        timer += Random.value;
    }

    public void CreateEnemy()
    {
        if (LiveEnmey.Count > MaxNum)
            return;
        int index = Random.Range(0, EnemyKinds);
        Vector3 pos = transform.position + new Vector3(Random.Range(-Radius, Radius), Random.Range(-Radius, Radius), 0); //新建位置
        if ((player.transform.position - pos).magnitude < DistanceWithPlayer)
            return;
        GameObject obj = Object.Instantiate(EnemyList[index], pos, Quaternion.identity);
        LiveEnmey.Add(obj);
    }
}
