using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyPanel : MonoBehaviour
{
    TextMeshProUGUI moneyText;

    private void Awake()
    {
        moneyText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// 플레이어의 돈이 변경될 때 실행되는 함수
    /// </summary>
    /// <param name="money">현재 플레이어가 가지고 있는 돈(표시할 돈)</param>
    public void Refresh(int money)
    {
        // 이 함수가 실행될 때마다.
        // MoneyPanel에서 표시하는 돈이 money로 설정된다.
        // 3자리마다 ,를 표시되게 한다. (10000 => 10,000)
        moneyText.text = $"{money:N0}";
    }
}
