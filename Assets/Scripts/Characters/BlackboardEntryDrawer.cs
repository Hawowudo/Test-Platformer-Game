using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Blackboard.Entry))]
public class BlackboardEntryDrawer : PropertyDrawer
{
    private const float Spacing = 2f;

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("type");

        string valueField = GetValueFieldName(
            (Blackboard.ValueType)typeProp.enumValueIndex);

        float height =
            EditorGUIUtility.singleLineHeight + // key
            Spacing +
            EditorGUIUtility.singleLineHeight;  // type

        if (!string.IsNullOrEmpty(valueField))
        {
            var valueProp = property.FindPropertyRelative(valueField);

            height +=
                Spacing +
                EditorGUI.GetPropertyHeight(valueProp, true);
        }

        return height;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keyProp = property.FindPropertyRelative("key");
        var typeProp = property.FindPropertyRelative("type");

        float y = position.y;

        // Key
        Rect keyRect = new Rect(
            position.x,
            y,
            position.width,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(keyRect, keyProp);

        y += EditorGUIUtility.singleLineHeight + Spacing;

        // Type
        Rect typeRect = new Rect(
            position.x,
            y,
            position.width,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(typeRect, typeProp);

        y += EditorGUIUtility.singleLineHeight + Spacing;

        // Value
        string valueField = GetValueFieldName(
            (Blackboard.ValueType)typeProp.enumValueIndex);

        if (!string.IsNullOrEmpty(valueField))
        {
            var valueProp = property.FindPropertyRelative(valueField);

            float valueHeight =
                EditorGUI.GetPropertyHeight(valueProp, true);

            Rect valueRect = new Rect(
                position.x,
                y,
                position.width,
                valueHeight);

            EditorGUI.PropertyField(
                valueRect,
                valueProp,
                true);
        }

        EditorGUI.EndProperty();
    }

    private string GetValueFieldName(
        Blackboard.ValueType type)
    {
        return type switch
        {
            Blackboard.ValueType.Int => "intValue",
            Blackboard.ValueType.Float => "floatValue",
            Blackboard.ValueType.Bool => "boolValue",
            Blackboard.ValueType.String => "stringValue",
            Blackboard.ValueType.Vector2 => "vector2Value",
            Blackboard.ValueType.Vector3 => "vector3Value",
            Blackboard.ValueType.GameObject => "gameObjectValue",
            Blackboard.ValueType.Transform => "transformValue",
            _ => null
        };
    }
}