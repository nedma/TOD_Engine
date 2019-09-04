using UnityEngine;

/// Cloud animation class.
///
/// Component of the sky dome parent game object.

[ExecuteInEditMode]
public class TOD_Animation : MonoBehaviour
{
    /// Wind direction in degrees.
    /// = 0 for wind blowing in northern direction.
    /// = 90 for wind blowing in eastern direction.
    /// = 180 for wind blowing in southern direction.
    /// = 270 for wind blowing in western direction.
    public float WindDegrees = 0.0f;

    /// Speed of the wind that is acting upon the clouds.
    public float WindSpeed = 3.0f;

    /// Current cloud UV coordinates.
    /// Can be synchronized between multiple game clients to guarantee identical cloud coverage.
    internal Vector4 CloudUV
    {
        get; set;
    }

    /// Current offset UV coordinates.
    /// Is being calculated from the sky dome world position.
    internal Vector4 OffsetUV
    {
        get
        {
            Vector3 pos = transform.position;
            Vector3 scale = transform.lossyScale;
            Vector3 offset = new Vector3(pos.x / scale.x, 0, pos.z / scale.z);
            offset = -transform.TransformDirection(offset);
            return new Vector4(offset.x, offset.z, offset.x, offset.z);
        }
    }

    protected void Update()
    {
        // Update cloud UV coordinates
        float wDegreeForCloud = WindDegrees; //
        float wSpeed = WindSpeed*Mathf.Max(1.0f, 30.0f*0.3f);
        Vector2 v1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad*(wDegreeForCloud)),
            Mathf.Sin(Mathf.Deg2Rad*(wDegreeForCloud)));
        Vector4 windForCloud = wSpeed/100f*new Vector4(-v1.x, -v1.y, 0.0f, 0.0f);
        CloudUV += Time.deltaTime*windForCloud;
    }
}
