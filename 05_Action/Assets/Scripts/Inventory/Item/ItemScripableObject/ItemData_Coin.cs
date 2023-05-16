using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 코인은 먹는 즉시 소비되는 아이템
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data - Coin", order = 2)]
public class ItemData_Coin : ItemData, IConsumable
{
    public void Consume(GameObject target)
    {
        Player player = target.GetComponent<Player>();
        if(player != null)              // 동전은 플레이어 대상으로만 사용됨
        {
            player.Money += (int)price; // 동전의 가격만큼 플레이어의 돈이 증가
        }
    }
}
