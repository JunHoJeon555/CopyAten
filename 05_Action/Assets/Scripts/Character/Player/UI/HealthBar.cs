using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : BarBase
{
    private void Start()
    {
        Player player = GameManager.Inst.Player;    // 플레이어 찾기
        maxValue = player.MaxHP;                    // 최대 값 설정
        max.text = $"/ {maxValue}";                 // 최대 값 표시
        current.text = player.HP.ToString();        // 현재 HP 설정

        slider.value = player.HP / maxValue;        // 슬라이더 값 설정
        player.onHealthChange += OnValueChange;     // HP 변경될 때 실행될 함수 등록
    }
}
