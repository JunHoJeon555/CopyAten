using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어를 따라다니는 미니맵 카메라
/// </summary>
public class MinimapCamera : MonoBehaviour
{
    /// <summary>
    /// 정지 상황일 때 카메라 크기
    /// </summary>
    public float normalSize = 20.0f;

    /// <summary>
    /// 플레이어가 최고 속도일 때 카메라 크기
    /// </summary>
    public float maxSize = 30.0f;
    
    /// <summary>
    /// 미니맵의 크기가 변동되는 속도
    /// </summary>
    public float sizeSpeed = 5.0f;

    /// <summary>
    /// 카메라가 플레이어를 따라다니는 속도
    /// </summary>
    public float followSpeed = 3.0f;

    /// <summary>
    /// 이 스크립트가 달려있는 카메라
    /// </summary>
    Camera minimap;

    /// <summary>
    /// 플레이어
    /// </summary>
    Player player;

    /// <summary>
    /// 카메라의 목표 크기
    /// </summary>
    float targetSize = 20.0f;

    private void Awake()
    {
        minimap = GetComponent<Camera>();
    }

    private void Start()
    {
        player = GameManager.Inst.Player;
        player.onDie += () => StartCoroutine(DieProcess()); // 플레이어가 죽었을 때 노멀 상태로 되돌리기
    }

    private void LateUpdate()
    {
        // 플레이어가 살아있을 때만 동작시키기
        if (player.IsAlive)  
        {
            // 현재 속도 비율에 따라 목표로 하는 카메라 크기 계산
            targetSize = normalSize + (maxSize - normalSize) * player.SpeedRatio;
            // lerp이용해서 부드럽게 변경
            minimap.orthographicSize = Mathf.Lerp(minimap.orthographicSize, targetSize, Time.deltaTime * sizeSpeed);

            // 카메라가 플레이어 추적하기(회전 없이 xz평면상에서만 움직임)
            Vector3 targetPos = player.transform.position;
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        }
    }

    /// <summary>
    /// 플레이어 사망시 원상목구 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator DieProcess()
    {
        targetSize = normalSize;    // 노멀 상태로 돌아가는 것이 목표
        while(minimap.orthographicSize > (targetSize + 0.01f))   // 현재 카메라 크기가 목표 크기보다 크면 계속 실행
        {
            // 사이즈 줄이기
            minimap.orthographicSize = Mathf.Lerp(minimap.orthographicSize, targetSize, Time.deltaTime * sizeSpeed);
            yield return null;
        }
        minimap.orthographicSize = targetSize;  // 목표에 근접하면 목표 크기로 설정
    }
}
