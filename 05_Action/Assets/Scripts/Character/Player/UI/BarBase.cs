using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 게이지 표시용 바의 공통 부모 클래스
public class BarBase : MonoBehaviour
{
    /// <summary>
    /// 표시될 색상
    /// </summary>
    public Color color = Color.white;

    protected Slider slider;
    protected TextMeshProUGUI current;
    protected TextMeshProUGUI max;

    /// <summary>
    /// 표시할 값의 최대 값
    /// </summary>
    protected float maxValue;

    private void Awake()
    {
        // 컴포넌트 찾아 놓기
        slider = GetComponent<Slider>();
        Transform child = transform.GetChild(2);
        current = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(3);
        max = child.GetComponent<TextMeshProUGUI>();

        // 색상 지정하기. 배경 색은 fill 영역 색에서 알파만 절반
        child = transform.GetChild(1);
        Image fillImage = child.GetComponentInChildren<Image>();
        fillImage.color = color;
        child = transform.GetChild(0);
        Image bgImage = child.GetComponentInChildren<Image>();
        Color bgColor = new Color(color.r, color.g, color.b, color.a * 0.5f);
        bgImage.color = bgColor;
    }

    /// <summary>
    /// 값이 변경되면 실행되는 함수
    /// </summary>
    /// <param name="ratio">슬라이더에서 표시될 비율</param>
    protected void OnValueChange(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);               // ratio의 범위를 0~1 사이로
        slider.value = ratio;                       // 슬라이더 설정
        current.text = $"{(ratio * maxValue):f0}";  // 텍스트 변경
    }
}
