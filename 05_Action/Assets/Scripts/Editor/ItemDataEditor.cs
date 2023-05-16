using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// ItemData용 커스텀 에디터라는 의미, 두번째 파라메터가 true면 자식 클래스도 같이 적용 받는다.
[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    ItemData itemData;

    private void OnEnable()
    {
        itemData = target as ItemData;
    }

    public override void OnInspectorGUI()
    {
        if(itemData != null)
        {
            if(itemData.itemIcon != null)
            {
                Texture2D texture;
                EditorGUILayout.LabelField("Item Icon Preview");
                texture = AssetPreview.GetAssetPreview(itemData.itemIcon);
                if(texture != null)
                {
                    GUILayout.Label("", GUILayout.Height(64), GUILayout.Width(64));
                    GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
                }
            }

        }
        base.OnInspectorGUI();
    }
}

#endif