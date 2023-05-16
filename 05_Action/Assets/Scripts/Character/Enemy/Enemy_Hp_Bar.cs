using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hp_Bar : MonoBehaviour
{
    Transform fill;

    private void Awake()
    {
        fill = transform.GetChild(1);

        IHealth target = GetComponentInParent<IHealth>();
        target.onHealthChange += Refresh;
    }

    private void Refresh(float ratio)
    {
        fill.localScale = new Vector3(ratio, 1, 1);
    }
}
