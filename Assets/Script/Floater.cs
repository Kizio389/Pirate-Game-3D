using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public class Floater : MonoBehaviour
    {
        public Rigidbody rigidbodyds;
        public float depthBeforeSubmerged = 1f;
        public float displacementAmount = 3f;
        public int floaterCount = 1;
        public float waterDrag = 0.99f;
        public float waterAgularDrag = .5f;

        private void FixedUpdate()
        {
            rigidbodyds.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
            float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.x);
            if (transform.position.y < waveHeight)
            {
                float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
                rigidbodyds.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
                rigidbodyds.AddForce(displacementMultiplier * -rigidbodyds.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rigidbodyds.AddTorque(displacementMultiplier * -rigidbodyds.angularVelocity * waterAgularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }
