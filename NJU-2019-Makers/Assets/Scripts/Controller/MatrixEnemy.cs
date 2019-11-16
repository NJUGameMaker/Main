using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixEnemy : MonoBehaviour
{
    public float ActiveTime;
    public float ActiveMaxDistance;
    public float ActiveMinDistance;
    public int EnemyNum;
    public Collider2D ActiveCollider;
    public GameObject Container;
    public GameObject EnemyPrefab;
    //没有设置敌人的状态，给到的敌人都是
    public enum MatrixType
    {
        Stop,
        RectAngle,
        Cruve,
        Round,
        AIFollow,
        InOrder
    };
    public MatrixType MType;
    // Start is called before the first frame update
    void Start()
    {
        ActiveMaxDistance = 3;
        ActiveMinDistance = 0;
        Container = new GameObject("Container");
        Container.transform.position = transform.position;
        MType = MatrixType.Stop;
        EnemyNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = (transform.position - PlayerManager.Instance.transform.position).magnitude;
        if(Container.transform.childCount < 1)//已经没有敌人
        {
            if(dis < ActiveMaxDistance && dis > ActiveMinDistance)//准备生成
            {
                CreateEnemyMatrix();
            }
        }
        
    }
    private void CreateEnemyMatrix()
    {
        CleanAllEnemy();
        switch(MType)
        {
            case MatrixType.Stop: { }break;
            case MatrixType.RectAngle: { }break;
            default: { }break;
        }
    }
    private void CleanAllEnemy()
    {
        for (int i = Container.transform.childCount - 1; i >= 0; i--)
            Destroy(Container.transform.GetChild(i).gameObject);
    }
    //设置matrix的形状
    void SetMatrixType(MatrixType s)
    {
        MType = s;
    }

    private void StopMatrix()
    {

    }
    private void RectangleMatrix()
    {
        for(int i = 0; i < EnemyNum; i++)
        {
            GameObject obj = Object.Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
            var ai = obj.GetComponent<EnemyAI>();
            ai.StartCoroutine(Statics.WorkAfterSeconds(() => {
                ai.BeActive();
            }, 2f));
            obj.transform.parent = transform;
        }
    }
}
