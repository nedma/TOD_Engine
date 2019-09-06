using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HTOD_MinAttribute))]
public class HTOD_MaxDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as HTOD_MaxAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Min(newValue, attr.max);
			
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Min(newValue, (int)attr.max);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use HTOD_Max with float or int.");
		}

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(HTOD_MinAttribute))]
public class HTOD_MinDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as HTOD_MinAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Max(newValue, attr.min);
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Max(newValue, (int)attr.min);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use HTOD_Min with float or int.");
		}

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(HTOD_RangeAttribute))]
public class HTOD_RangeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		var attr = attribute as HTOD_RangeAttribute;

		if (property.propertyType == SerializedPropertyType.Float)
		{
			EditorGUI.BeginChangeCheck();
			float newValue = EditorGUI.FloatField(position, label, property.floatValue);
			if (EditorGUI.EndChangeCheck()) property.floatValue = Mathf.Clamp(newValue, attr.min, attr.max);
		}
		else if (property.propertyType == SerializedPropertyType.Integer)
		{
			EditorGUI.BeginChangeCheck();
			int newValue = EditorGUI.IntField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck()) property.intValue = Mathf.Clamp(newValue, (int)attr.min, (int)attr.max);
		}
		else
		{
			EditorGUI.LabelField (position, label.text, "Use HTOD_Range with float or int.");
		}

		EditorGUI.EndProperty();
	}
}
