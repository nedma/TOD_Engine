using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HTOD_Sky)), CanEditMultipleObjects]
public class HTOD_SkyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		if (targets.Length == 1)
		{
			var sky = target as HTOD_Sky;
			var components = sky.gameObject.GetComponent<HTOD_Components>();
			var resources = sky.gameObject.GetComponent<HTOD_Resources>();

			int errors = 0;

			if (sky.Initialized && !components.Camera)
			{
				EditorGUILayout.LabelField("HTOD_Camera", "Missing component");
				errors++;
			}

			if (!components.Space)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Space");
				errors++;
			}
			if (!components.Stars)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Stars");
				errors++;
			}
			if (!components.Sun)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Sun");
				errors++;
			}
			if (!components.Moon)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Moon");
				errors++;
			}
			if (!components.Atmosphere)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Atmosphere");
				errors++;
			}
			if (!components.Clear)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Clear");
				errors++;
			}
			if (!components.Clouds)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Clouds");
				errors++;
			}
			if (!components.Billboards)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Billboards");
				errors++;
			}
			if (!components.Light)
			{
				EditorGUILayout.LabelField("HTOD_Components", "Missing reference: Light");
				errors++;
			}

			if (!resources.Skybox)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Skybox");
				errors++;
			}
			if (!resources.MoonLOD0)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Moon LOD0");
				errors++;
			}
			if (!resources.MoonLOD1)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Moon LOD1");
				errors++;
			}
			if (!resources.MoonLOD2)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Moon LOD2");
				errors++;
			}
			if (!resources.SkyLOD0)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Sky LOD0");
				errors++;
			}
			if (!resources.SkyLOD1)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Sky LOD1");
				errors++;
			}
			if (!resources.SkyLOD2)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Sky LOD2");
				errors++;
			}
			if (!resources.CloudsLOD0)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Clouds LOD0");
				errors++;
			}
			if (!resources.CloudsLOD1)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Clouds LOD1");
				errors++;
			}
			if (!resources.CloudsLOD2)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Clouds LOD2");
				errors++;
			}
			if (!resources.StarsLOD0)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Stars LOD0");
				errors++;
			}
			if (!resources.StarsLOD1)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Stars LOD1");
				errors++;
			}
			if (!resources.StarsLOD2)
			{
				EditorGUILayout.LabelField("HTOD_Resources", "Missing reference: Stars LOD2");
				errors++;
			}

			if (errors > 0)
			{
				if (errors > 1)
				{
					GUILayout.Label("Sky dome setup incomplete. (" + errors + " issues)");
				}
				else
				{
					GUILayout.Label("Sky dome setup incomplete. (" + errors + " issue)");
				}
				GUILayout.Label("Check the docs for more information.");
			}
		}

		DrawDefaultInspector();
	}
}
