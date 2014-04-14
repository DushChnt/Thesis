using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

    public static bool DebugLog = false;

	/// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static void Log(string message)
    {
        if (DebugLog)
        {
            Debug.Log(message);
        }
    }

    public static float Clamp(float val)
    {
        return Clamp(val, 0, 1);
    }

    public static float Clamp(float val, float min, float max)
    {
        if (val < 0)
        {
            return 0;
        }
        if (val > 1)
        {
            return 1;
        }
        return val;
    }

    public static float GenerateNoise(float threshold)
    {
        return Random.Range(-threshold, threshold);
    }
}
