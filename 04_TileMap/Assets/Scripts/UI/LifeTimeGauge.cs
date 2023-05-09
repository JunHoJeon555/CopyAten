using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeGauge : MonoBehaviour
{
    /// <summary>
    /// 증가 속도
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// 목표치
    /// </summary>
    float targetValue = 1.0f;

    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;

        slider.value = 1;   // 초기화
        targetValue = 1.0f;
    }

    private void Update()
    {
        // slider.value가 targetValue쪽으로 변경되도록 실행

        if( slider.value > targetValue)    // 슬라이더 위치가 목표치보다 클때
        {
            // slider.value가 줄어야 한다.
            slider.value -= Time.deltaTime * speed;
            if(slider.value < targetValue)  // 줄였다가 목표치를 넘어섰을 때
            {
                slider.value = Mathf.Max(0, targetValue);   // targetValue가 되거나 0으로 설정
            }
        }
        else //슬라이더 위치가 목표치보다 작을때
        {
            // slider.value가 늘어야 한다.
            slider.value += Time.deltaTime * speed;
            if (slider.value > targetValue) // 늘렸다가 목표치를 넘어섰을 때
            {
                slider.value = Mathf.Min(1, targetValue);   // targetValue가 되거나 1로 설정
            }
        }
    }

    /// <summary>
    /// 플레이어의 수명이 변경되면 실행되는 함수
    /// </summary>
    /// <param name="ratio">비율(0~1)</param>
    private void OnLifeTimeChange(float ratio)
    {
        targetValue = ratio;    // 목표치만 변경
    }
}

