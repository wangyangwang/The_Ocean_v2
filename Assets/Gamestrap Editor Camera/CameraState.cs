using UnityEngine;

namespace Gamestrap
{
    [System.Serializable]
    public class CameraState
    {
        public static Color defaultColor = new Color(0.9f,0.4f,0.4f);

        public string name = "";
        public Color color = defaultColor;
        public bool active;
        public Vector3 position;
        public Quaternion rotation;
        public float fieldOfView;
        public bool orthographic;
        public bool in2dMode;
        public Vector3 pivot;
        public float size;
#if UNITY_EDITOR
        public UnityEditor.DrawCameraMode renderMode;
#endif
    }

}