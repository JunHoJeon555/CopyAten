using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 가능한 ItemData에 상속할 인터페이스
/// </summary>
interface IUsable
{
    bool Use(GameObject target);
}
