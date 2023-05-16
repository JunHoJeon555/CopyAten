using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    Transform[] children;

    int index = 0;

    public Transform Current => children[index];

    private void Awake()
    {
        // children에 모든 자식 트랜스폼 넣기
        children = new Transform[transform.childCount];
        for(int i=0; i<children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    /// <summary>
    /// 다음 웨이포인트를 돌려주는 함수
    /// </summary>
    /// <returns>다음으로 이동할 웨이포인트</returns>
    public Transform Next()
    {
        index++;
        index %= children.Length;
        return children[index];
    }
}
