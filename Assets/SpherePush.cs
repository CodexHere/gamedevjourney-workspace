using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePush : MonoBehaviour {
    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (Input.GetButton("Jump")) {
            m_Rigidbody.AddForce(transform.forward * m_Thrust);
        }
    }
}
