using UnityEngine;

namespace GBGame
{
    public class Billboard : MonoBehaviour
    {
        private Transform Target => CameraController.CurrentCam.Camera.transform;
        private Transform ThisTrans;

        [SerializeField] private bool Invert = false;

        private void Awake()
        {
            ThisTrans = transform;
        }

        Vector3 dir_cache;
        Quaternion FinalRotation;
        void Update()
        {
            dir_cache = Target.position - ThisTrans.position;
            FinalRotation = Quaternion.LookRotation(Invert ? -dir_cache : dir_cache, Target.up);
        }
        private void LateUpdate()
        {
            this.transform.rotation = FinalRotation;
        }
    } 
}