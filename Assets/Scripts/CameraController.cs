using UnityEngine;

namespace GBGame
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController CurrentCam;
        [field: SerializeField] public Camera Camera { get; private set; }

        private Transform RigTrans;
        private Transform CamTrans;

        private void Awake()
        {
            CamTrans = Camera.transform;
            RigTrans = this.transform;
            CurrentCam = this;
        }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Distance { get; set; }

        private void LateUpdate()
        {
            RigTrans.SetPositionAndRotation(Position, Rotation);
            CamTrans.localPosition = Vector3.back * Distance;
        }
    } 
}
