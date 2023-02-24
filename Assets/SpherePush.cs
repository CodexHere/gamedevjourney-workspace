using UnityEngine;

public class SpherePush : MonoBehaviour {
    private Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 force = Vector3.zero;

        if (Input.GetButton("Jump")) {
            force += transform.up;
        }

        if (Input.GetButton("Vertical")) {
            force += Input.GetAxis("Vertical") > 0 ? transform.forward : -transform.forward;
        }

        if (Input.GetButton("Horizontal")) {
            force += Input.GetAxis("Horizontal") > 0 ? transform.right : -transform.right;
        }

        if (force != Vector3.zero) {
            m_Rigidbody.AddForce(force * m_Thrust);
        }
    }
}
