using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    /// <summary>
    /// 생성할 아이템의 아이디 역할. (이름 구분용)
    /// </summary>
    static int itemCount = 0;

    /// <summary>
    /// ItemCode로 아이템을 생성하는 함수
    /// </summary>
    /// <param name="itemCode">생성할 아이템 종류</param>
    /// <returns>생성 완료된 아이템</returns>
    public static GameObject MakeItem(ItemCode itemCode)
    {
        ItemData itemData = GameManager.Inst.ItemData[itemCode];
        GameObject obj = GameObject.Instantiate(itemData.modelPrefab);  // 아이템 데이터에 들어있는 프리팹 생성
        Item item = obj.GetComponent<Item>();
        item.ItemData = itemData;                       // 아이템 데이터 넣기

        string[] itemName = itemData.name.Split('_');   // 아이템 데이터의 이름을 _기준으로 분리하기( 00_Ruby를 00과 Ruby로 분리)
        obj.name = $"{itemName[1]}_{itemCount++}";      // 구분을 위해 유니크한 이름 설정

        return obj;
    }

    /// <summary>
    /// ItemCode로 아이템을 생성하는 함수
    /// </summary>
    /// <param name="itemCode">생성할 아이템 종류</param>
    /// <param name="pos">생성될 위치</param>
    /// <param name="randomNoise">생성될 위치에 랜덤성 추가 여부. true면 약간의 랜덤성을 더한다.</param>
    /// <returns></returns>
    public static GameObject MakeItem(ItemCode itemCode, Vector3 pos, bool randomNoise = false)
    {
        GameObject obj = MakeItem(itemCode);
        if(randomNoise)
        {
            // 랜덤성 정도는 x와 -0.5~0.5, z가 -0.5~0.5 정도씩 오차가 생김
            Vector2 noise = Random.insideUnitCircle * 0.5f;
            pos.x += noise.x;
            pos.z += noise.y;

            //obj.transform.Rotate(0, Random.Range(0.0f, 360.0f), 0); // 시작 각도 다르게 만들기
        }
        obj.transform.position = pos;

        return obj;
    }

    /// <summary>
    /// ItemCode로 아이템을 생성하는 함수
    /// </summary>
    /// <param name="itemCode">생성할 아이템 종류</param>
    /// <param name="count">생성할 아이템의 갯수</param>
    /// <returns>생성된 아이템들</returns>
    public static GameObject[] MakeItem(ItemCode itemCode, uint count)
    {
        GameObject[] objs = new GameObject[count];
        for(int i=0; i < count;i++)
        {
            objs[i] = MakeItem(itemCode);
        }

        return objs;
    }

    /// <summary>
    /// ItemCode로 아이템을 생성하는 함수
    /// </summary>
    /// <param name="itemCode">생성할 아이템 종류</param>
    /// <param name="count">생성할 아이템의 갯수</param>
    /// <param name="pos">생성될 위치</param>
    /// <param name="randomNoise">생성될 위치에 랜덤성 추가 여부. true면 약간의 랜덤성을 더한다.</param>
    /// <returns>생성된 아이템들</returns>
    public static GameObject[] MakeItem(ItemCode itemCode, uint count, Vector3 pos, bool randomNoise = false)
    {
        GameObject[] objs = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            objs[i] = MakeItem(itemCode, pos, randomNoise);
        }

        return objs;
    }
}
