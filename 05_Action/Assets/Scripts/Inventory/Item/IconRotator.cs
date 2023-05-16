using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotator : MonoBehaviour
{
    // 1. y축 기준으로 계속 회전한다.
    // 2. y높이가 0.5 ~ 1.5사이로 계속 변경된다.
    // 20분(12시까지)

    public float rotateSpeed = 360.0f;
    public float moveSpeed = 2.0f;

    public float minHeight = 0.5f;
    public float maxHeight = 2.5f;
    float timeElapsed = 0.0f;

    private void Update()
    {
        timeElapsed += Time.deltaTime * moveSpeed;
        Vector3 pos;
        pos.x = transform.parent.position.x;
        pos.z = transform.parent.position.z;
        pos.y = minHeight + (1-Mathf.Cos(timeElapsed)) * 0.5f * (maxHeight - minHeight);

        // Mathf.Cos() * 0.5f + 0.5f; => 1 ~ 0
        // 1 - (Mathf.Cos() * 0.5f + 0.5f); => 0 ~ 1
        // 1 - (Mathf.Cos() * 0.5f + 0.5f) * (maxHeight - minHeight); => 0 ~ (maxHeight - minHeight)
        // minHeight + (1 - (Mathf.Cos() * 0.5f + 0.5f) * (maxHeight - minHeight)); => minHeight ~ maxHeight
        // minHeight + ( 1 - Mathf.Cos() ) * 0.5f * (maxHeight - minHeight); => 마지막 정리

        transform.position = pos;
        transform.Rotate(0, Time.deltaTime * rotateSpeed, 0);
    }

}
