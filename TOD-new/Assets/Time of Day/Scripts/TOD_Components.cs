using UnityEngine;

/// Component manager class.
///
/// Component of the main camera of the scene.

[ExecuteInEditMode]
public class TOD_Components : MonoBehaviour
{
	static TOD_Components instance;

	public static TOD_Components GetInstance()
	{
		if (instance == null)
			instance = FindObjectOfType<TOD_Components>();
		return instance;
	}

    /// Sun child game object reference.
    public GameObject Sun = null;

    /// Clouds child game object reference, using PBR
    public GameObject CloudsPbr = null;

    /// Light child game object reference.
    public GameObject Light = null;

    /// Transform component of the sun game object.
    internal Transform SunTransform;

    /// Transform component of the main camera game object.
    internal Transform CameraTransform;

    /// Transform component of the light source game object.
    internal Transform LightTransform;

    internal Transform DomeTransform;

    /// Renderer component of the cloud pbr game object.
    internal Renderer CloudPbrRenderer;

    /// Renderer component of the sun game object.
    internal Renderer SunRenderer;

    /// Main material of the cloud pbr game object.
    internal Material CloudPbrShader;

    /// Main material of the sun game object.
    internal Material SunShader;

    /// Light component of the light source game object.
    internal TOD_FakeLight LightSource;

    /// Sky component of the sky dome game object.
    internal TOD_Sky Sky;

    /// Animation component of the sky dome game object.
    internal TOD_Animation Animation;

    /// Time component of the sky dome game object.
    internal TOD_Time Time;

	public TOD_Time GetTODTime()
	{
		if (Time == null)
			Time = GetComponent<TOD_Time>();
		return Time;
	}

	public TOD_Animation GetTODAnimation()
	{
		if (Animation == null)
			Animation = GetComponent<TOD_Animation>();
		return Animation;
	}

	public TOD_Sky GetTODSky()
	{
		if (Sky == null)
			Sky = GetComponent<TOD_Sky>();
		return Sky;
	}

    protected void OnEnable()
    {
        DomeTransform   = transform;
        CameraTransform = Camera.main != null ? Camera.main.transform : DomeTransform;

        Sky       = GetComponent<TOD_Sky>();
        Animation = GetComponent<TOD_Animation>();
        Time      = GetComponent<TOD_Time>();

        if (CloudsPbr)
        {
            CloudPbrRenderer = CloudsPbr.renderer;
            CloudPbrShader = CloudPbrRenderer.sharedMaterial;
        }

        if (Light)
        {
            LightTransform = Light.transform;
            LightSource = Light.GetComponent<TOD_FakeLight>();
        }
        else
        {
            Debug.LogError("Light reference not set. Disabling script.");
            this.enabled = false;
            return;
        }

        if (Sun)
        {
            SunTransform  = Sun.transform;
            SunRenderer   = Sun.renderer;
            SunShader     = SunRenderer.sharedMaterial;
        }
        else
        {
            Debug.LogError("Sun reference not set. Disabling script.");
            this.enabled = false;
            return;
        }
    }
}
