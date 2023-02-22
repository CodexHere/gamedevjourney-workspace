using UnityEngine;

public class SpherePush : MonoBehaviour {
    private Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (Input.GetButton("Jump")) {
            m_Rigidbody.AddForce(transform.forward * m_Thrust);
        }
    }
}
