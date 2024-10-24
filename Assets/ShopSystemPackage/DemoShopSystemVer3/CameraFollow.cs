using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemPackage
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;

        Transform cam;
        Vector3 offset;

        private void Start()
        {
            cam = Camera.main.transform;
            offset = cam.position - target.position;
        }

        private void LateUpdate()
        {
            cam.position = target.position + offset;
        }

    }

}
