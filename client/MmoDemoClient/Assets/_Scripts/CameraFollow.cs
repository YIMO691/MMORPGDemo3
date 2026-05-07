using UnityEngine;

namespace MmoDemo.Client
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new(0, 10, -8);
        public float smoothSpeed = 8f;

        private void LateUpdate()
        {
            if (target == null) return;
            var desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
            transform.LookAt(target);
        }
    }
}
