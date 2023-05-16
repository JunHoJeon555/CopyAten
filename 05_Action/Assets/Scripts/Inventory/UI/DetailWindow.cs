using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DetailWindow : MonoBehaviour
{
    public float maxAlpha = 0.7f;
    public float alphaChangeSpeed = 5.0f;
    float alphaTarget = 0.0f;

    bool isPause = false;
    public bool IsPause
    {
        get => isPause;
        set
        {
            isPause = value;
            if(isPause)
            {
                Close();
            }
        }
    }

    Image itemIcon;
    TextMeshProUGUI itemName;
    TextMeshProUGUI itemPrice;
    TextMeshProUGUI itemDescription;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        Transform child = transform.GetChild(0);
        itemIcon = child.GetComponent<Image>();
        child = transform.GetChild(1);
        itemName = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        itemPrice = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(3);
        itemDescription = child.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if( alphaTarget > 0.0f )
        {
            canvasGroup.alpha += Time.deltaTime * alphaChangeSpeed;
        }
        else
        {
            canvasGroup.alpha -= Time.deltaTime * alphaChangeSpeed;
        }
        canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha, 0.0f, maxAlpha);
    }

    /// <summary>
    /// 상세창을 여는 함수
    /// </summary>
    /// <param name="data">열면서 표시할 데이터</param>
    public void Open(ItemData data)
    {
        if(!isPause && data != null)        // 일시 정지 상태가 아니고 데이터가 있을때만 열기
        {
            itemIcon.sprite = data.itemIcon;                    // 아이콘 이미지 설정
            itemName.text = data.itemName;                      // 이름 설정
            itemPrice.text = data.price.ToString();             // 가격 설정
            itemDescription.text = data.itemDescription;        // 상세 설명 설정

            alphaTarget = 1;                // 보이게끔 alpha 목표치 설정

            MovePosition(Mouse.current.position.ReadValue());   // 열릴 때 마우스 위치 기준으로 열기
        }
    }

    /// <summary>
    /// 상세창을 닫는 함수
    /// </summary>
    public void Close()
    {
        alphaTarget = 0;    // 안보이게끔 alpha 목표치 설정
    }

    /// <summary>
    /// 상세 정보창의 위치를 옮기는 함수
    /// </summary>
    /// <param name="screenPos">새 위치(스크린 좌표)</param>
    public void MovePosition(Vector2 screenPos)
    {
        if (alphaTarget > 0)                // 보일때만 움직이게 하기
        {
            RectTransform rect = (RectTransform)transform;

            int diffX = (int)(screenPos.x + rect.sizeDelta.x) - Screen.width;
            diffX = Mathf.Max(0, diffX);    // 넘친부분만큼만 왼쪽으로 보내기
            screenPos.x -= diffX;           // 화면을 안벗어나게 만들기

            //if (screenPos.x + rect.sizeDelta.x > Screen.width)   // 가로로 벗어난 경우
            //{
            //    screenPos.x -= rect.sizeDelta.x;                // 디테일창 가로 크기만큼 왼쪽으로 보내기
            //}
            //if (screenPos.y - rect.sizeDelta.y < 0)             // 세로로 벗어난 경우
            //{
            //    screenPos.y += rect.sizeDelta.y;                // 디테일창 세로 크기만큼 위로 보내기
            //}

            transform.position = screenPos;     // 위치 변경
        }
    }
}
