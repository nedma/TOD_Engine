using UnityEngine;
using WNEngine;
using WNGameBase;

[ExecuteInEditMode]
public partial class TOD_Sky : MonoBehaviour
{
    public Transform target;
    private Camera m_TargetCamera;

    private int _CloudScale;
    private int _UvPow;
    private int _CloudSpeed;
    private int _CloudUV;
    private int _IV_MVP;

    private int _CloudVLitOffset;

    private int _CloudCover;

    private int _Contrast;
    private int _Haziness;
    private int _Fogginess;
    private int _Horizon;
    private int _OpticalDepth;
    private int _OneOverBeta;
    private int _BetaRayleigh;
    private int _BetaRayleighTheta;
    private int _BetaMie;
    private int _BetaMieTheta;
    private int _BetaMiePhase;
    private int _SunnyDegree;
    private int _CloudIntensity;

    private int TOD_LightColor;
    private int TOD_CloudColor;
    private int TOD_SunColor;
    private int TOD_AdditiveColor;

    private int TOD_LocalSunDirection;

    public bool IsUpdating = false;

    protected Transform GetTargetTransform()
    {
        if (target == null)
        {
            if (Camera.main != null)
            {
                target = Camera.main.transform;
            }
        }
        return target;
    }

    protected Camera GetTargetCamera()
    {
        if (m_TargetCamera == null)
        {
            var targettransform = GetTargetTransform();
            if (targettransform)
            {
                m_TargetCamera = targettransform.GetComponent<Camera>();
            }
        }
        return m_TargetCamera;
    }

    protected void OnEnable()
    {
        Components = GetComponent<TOD_Components>();

        if (!Components)
        {
            Debug.LogError("TOD_Components not found. Disabling script.");
            this.enabled = false;
            return;
        }

        //RenderSettings.fogColor = SampleAtmosphere(Vector3.forward);

        _CloudScale = Shader.PropertyToID("_CloudScale");
        _UvPow = Shader.PropertyToID("_UvPow");
        _CloudSpeed = Shader.PropertyToID("_CloudSpeed");

        _CloudUV = Shader.PropertyToID("_CloudUV");
        _IV_MVP = Shader.PropertyToID("_IV_MVP");

        _CloudVLitOffset = Shader.PropertyToID("_CloudVLitOffset");

        _CloudCover = Shader.PropertyToID("_CloudCover");
        _CloudIntensity = Shader.PropertyToID("_CloudIntensity");

        _Contrast = Shader.PropertyToID("_Contrast");
        _Haziness = Shader.PropertyToID("_Haziness");
        _Fogginess = Shader.PropertyToID("_Fogginess");
        _Horizon = Shader.PropertyToID("_Horizon");
        _OpticalDepth = Shader.PropertyToID("_OpticalDepth");
        _OneOverBeta = Shader.PropertyToID("_OneOverBeta");
        _BetaRayleigh = Shader.PropertyToID("_BetaRayleigh");
        _BetaRayleighTheta = Shader.PropertyToID("_BetaRayleighTheta");
        _BetaMie = Shader.PropertyToID("_BetaMie");
        _BetaMieTheta = Shader.PropertyToID("_BetaMieTheta");
        _BetaMiePhase = Shader.PropertyToID("_BetaMiePhase");
        _SunnyDegree = Shader.PropertyToID("_SunnyDegree");

        TOD_LightColor = Shader.PropertyToID("TOD_LightColor");
        TOD_CloudColor = Shader.PropertyToID("TOD_CloudColor");
        TOD_SunColor = Shader.PropertyToID("TOD_SunColor");
        TOD_AdditiveColor = Shader.PropertyToID("TOD_AdditiveColor");

        TOD_LocalSunDirection = Shader.PropertyToID("TOD_LocalSunDirection");
    }

    public bool bUpdateCloud = true;

    [HideInInspector]
    public Vector4 vLitOffset;
    public void UpdateCloudPbrParamemters()
    {
        if (!bUpdateCloud)
        {
            return;
        }

        Camera targetcamera = GetTargetCamera();
        if (Components.CloudPbrShader && targetcamera)
        {
#if !SHPPING_EXTERNAL
            Profiler.BeginSample("Set cloud uv");
#endif
            Vector4 cloudUVpbr = Components.Animation.CloudUV + Components.Animation.OffsetUV;

            // set up uv animation uniform variables
            float cloudscale = CloudsPbr.Scale;
            Components.CloudPbrShader.SetFloat(_CloudScale, cloudscale);
            float cloudUvPow = CloudsPbr.UvPow;
            Components.CloudPbrShader.SetFloat(_UvPow, cloudUvPow);
            Components.CloudPbrShader.SetFloat(_CloudSpeed, CloudsPbr.Speed);
            Components.CloudPbrShader.SetVector(_CloudUV, cloudUVpbr);
#if !SHPPING_EXTERNAL
            Profiler.EndSample();
#endif


            // set up PBR uniform variables
#if !SHPPING_EXTERNAL
            Profiler.BeginSample("compute camera params");
#endif
            Matrix4x4 immtx = Components.CloudsPbr.transform.worldToLocalMatrix;
            Matrix4x4 ivmtx = targetcamera.cameraToWorldMatrix;
            Matrix4x4 pmtx = targetcamera.projectionMatrix.inverse;
            Matrix4x4 invmtx = immtx * ivmtx * pmtx;
#if !SHPPING_EXTERNAL
            Profiler.EndSample();
#endif

            Components.CloudPbrShader.SetMatrix(_IV_MVP, invmtx); // this matrix is used to calculate the model space position of a each cloud's pixel

#if !SHPPING_EXTERNAL
            Profiler.BeginSample("compute vlit");
#endif
            Vector4 vlitW = new Vector4(LightDirection.x, LightDirection.y, LightDirection.z);
            vlitW.Normalize();
            if (vlitW.y > 0.0f)
            {
                vlitW.y = 0.0f;
                vlitW.Normalize();
            }
            Vector4 vlitP = (targetcamera.projectionMatrix * targetcamera.worldToCameraMatrix) * vlitW;
            vlitP.x /= vlitP.w;
            vlitP.y /= vlitP.w;
            vlitP.x = vlitP.x * 0.5f + 0.5f;
            vlitP.y = vlitP.y * 0.5f + 0.5f; // yeah... unity trick

            if (vlitP.w > 0.0f) // the sun is at the back side of camera
            {
                vLitOffset = new Vector4(1.0f, 1.0f, -vlitP.x, -vlitP.y);
            }
            else // the sun is at the front side of camera
            {
                vLitOffset = new Vector4(-1.0f, -1.0f, vlitP.x, vlitP.y);
            }
#if !SHPPING_EXTERNAL
            Profiler.EndSample();

            Profiler.BeginSample("other set");
#endif
            Components.CloudPbrShader.SetVector(_CloudVLitOffset, vLitOffset);

            float cover = CloudsPbr.CloudCover;
            Components.CloudPbrShader.SetFloat(_CloudCover, Mathf.Lerp(cover, 0.95f, RainyDegree));  // higher of this value will cause denser cloud
            Components.CloudPbrShader.SetFloat(_CloudIntensity, CloudsPbr.Brightness);

            // these redundant uniform sets are used for one-pass atmosphere color for cloud's back side
            Components.CloudPbrShader.SetFloat(_Contrast, Atmosphere.Contrast * OneOverGamma);
            Components.CloudPbrShader.SetFloat(_Haziness, Atmosphere.Haziness);
            Components.CloudPbrShader.SetFloat(_Fogginess, Atmosphere.Fogginess);
            Components.CloudPbrShader.SetFloat(_Horizon, World.HorizonOffset);
            Components.CloudPbrShader.SetVector(_OpticalDepth, opticalDepth);
            Components.CloudPbrShader.SetVector(_OneOverBeta, oneOverBeta);
            Components.CloudPbrShader.SetVector(_BetaRayleigh, betaRayleigh);
            Components.CloudPbrShader.SetVector(_BetaRayleighTheta, betaRayleighTheta);
            Components.CloudPbrShader.SetVector(_BetaMie, betaMie);
            Components.CloudPbrShader.SetVector(_BetaMieTheta, betaMieTheta);
            Components.CloudPbrShader.SetVector(_BetaMiePhase, betaMiePhase);
            Components.CloudPbrShader.SetFloat(_SunnyDegree, 1.0f - RainyDegree);
#if !SHPPING_EXTERNAL
            Profiler.EndSample();

            Profiler.BeginSample("switch keyword");
#endif
            if (targetcamera.targetTexture == null)
            {
                Components.CloudPbrShader.DisableKeyword("USING_RT");
            }
            else
            {
                Components.CloudPbrShader.EnableKeyword("USING_RT");
            }
#if !SHPPING_EXTERNAL
            Profiler.EndSample();
#endif
        }
    }

    public void UpdateAll()
    {
        Transform tgt = GetTargetTransform();
        Camera tgtCamera = GetTargetCamera();

        if (tgt && tgtCamera)
        {
            transform.position = new Vector3(tgt.position.x, tgt.position.y, tgt.position.z);

            if (GlobalEngine.Engine != null && GlobalEngine.Engine.GameWorld != null &&
                GlobalEngine.Engine.GameWorld.Game != null)
            {
                if (GameTool.GameInfo != null && GameTool.LocalPlayer != null)
                {
                    if (LogicalMapStore.IsSurvivalTeamGame(GameTool.GameInfo.MatchType) || GameTool.LocalPlayer.Camp == ECamp.Observer)
                    {
                        if (_firstUpdate)
                        {
                            Components.CloudPbrShader.shader.maximumLOD = 100;
                            UpdateTod();
                        }
                    }
                    else
                    {
                        if (_firstUpdate)
                        {
                            Components.CloudPbrShader.shader.maximumLOD = -1;
                            UpdateTod();
                        }
                    }

                    _firstUpdate = false;
                }
            }

            int mylod = Components.CloudPbrShader.shader.maximumLOD == -1 ?
                Shader.globalMaximumLOD : Components.CloudPbrShader.shader.maximumLOD;

            if (mylod >= 200)
            {
                UpdateTod();
                IsUpdating = true;
            }
            else
            {
                IsUpdating = false;
            }
        }
    }

    private void UpdateTod()
    {
        SetupSunAndMoon();
        SetupScattering();

        UpdateGlobalVariables();

        UpdateCloudPbrParamemters();
    }

    private bool _firstUpdate = true;
    protected void LateUpdate()
    {
        UpdateAll();
    }



    protected void UpdateGlobalVariables()
    {

        if (SceneEnvironment.Instance == null || SceneEnvironment.Instance.currentEnvRegion == SceneEnvironment.SceneRegion.Terrain)
        {
            RenderSettings.fogColor = 0.25f * SampleAtmosphere((Vector3.forward + 0.1f * Vector3.up).normalized) +
                                      0.25f * SampleAtmosphere((Vector3.back + 0.1f * Vector3.up).normalized) +
                                      0.25f * SampleAtmosphere((Vector3.left + 0.1f * Vector3.up).normalized) +
                                      0.25f * SampleAtmosphere((Vector3.right + 0.1f * Vector3.up).normalized);
        }

        Shader.SetGlobalColor(TOD_LightColor, LightColor);
        Shader.SetGlobalColor(TOD_CloudColor, CloudColor);
        Shader.SetGlobalColor(TOD_SunColor, SunColor);
        Shader.SetGlobalColor(TOD_AdditiveColor, AdditiveColor);
        Shader.SetGlobalVector(TOD_LocalSunDirection, Components.DomeTransform.InverseTransformDirection(SunDirection));
    }
}
