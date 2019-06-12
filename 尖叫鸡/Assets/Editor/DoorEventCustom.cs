using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 自定义的Inspector布局,用于配置门的事件和属性

[CustomPropertyDrawer(typeof(DoorEvent))]
public class DoorEventDrawer : PropertyDrawer
{
    private float cutOff = 20.0f;//缩进值
    private float rowHeight = 18.0f;//行高

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, rowHeight);
        var Rect1 = new Rect(position.x + cutOff, position.y + rowHeight * 1, position.width - cutOff, rowHeight);
        var Rect2 = new Rect(position.x + cutOff, position.y + rowHeight * 2, position.width - cutOff, rowHeight);
        var Rect3 = new Rect(position.x + cutOff, position.y + rowHeight * 3, position.width - cutOff, rowHeight);
        var Rect4 = new Rect(position.x + cutOff, position.y + rowHeight * 4, position.width - cutOff, rowHeight);
        var Rect5 = new Rect(position.x + cutOff, position.y + rowHeight * 5, position.width - cutOff, rowHeight);

        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(Rect1, property.FindPropertyRelative("eventType"), new GUIContent("事件类型"));

        switch (property.FindPropertyRelative("eventType").enumValueIndex)
        {
            case 0://IDLE类型
                EditorGUI.PropertyField(Rect2, property.FindPropertyRelative("idleTime"), new GUIContent("闲置时间(秒)"));
                break;
            case 1://BUSY类型
                EditorGUI.PropertyField(Rect2, property.FindPropertyRelative("propType"), new GUIContent("需求材料"));
                EditorGUI.PropertyField(Rect3, property.FindPropertyRelative("busyTime"), new GUIContent("需求持续时间(秒)"));
                EditorGUI.PropertyField(Rect4, property.FindPropertyRelative("completedAction"), new GUIContent("需求达成"));
                EditorGUI.PropertyField(Rect5, property.FindPropertyRelative("failedAction"), new GUIContent("需求失败"));
                break;
        }
    }

    //修改属性占用的高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        switch (property.FindPropertyRelative("eventType").enumValueIndex)
        {
            case 0://IDLE类型
                return rowHeight * 3;
            case 1://BUSY类型
                return rowHeight * 6;
            default:
                return rowHeight * 6;
        }
    }
}
