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
}
