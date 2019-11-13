using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int MaxNum = 10;
    public int IntervalTime = 10;
    public GameObject[] EnemyList;
    public GameObject player;
    public float EnterDistance = 10;
    public int EnemyKinds;
    public float DistanceWithPlayer = 8;
    public float Radius = 6;
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
        //DistanceWithPlayer = 6;
        EnterDistance = 10;
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
        if (LiveEnmey.Count >= MaxNum && timer < IntervalTime)
        {
            Debug.Log("Too Much Enmey so don't create");
            return;
        }
        else if (LiveEnmey.Count >= MaxNum)
            LiveEnmey.Clear();
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
        var ai = obj.GetComponent<EnemyAI>();
        ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
            ai.BeActive();
        }, 2f));
        LiveEnmey.Add(obj);
    }
}
