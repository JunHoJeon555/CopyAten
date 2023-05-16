using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData data = null;
    public ItemData ItemData
    {
        get => data;
        set
        {
            if( data == null)
            {
                data = value;
            }
        }
    }
}
