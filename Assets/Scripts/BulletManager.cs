using UnityEngine;

public class BulletManager : MonoBehaviour
{
	Rigidbody rb;

	float physicSpeed = 80;

	Vector3 movementDirection = Vector3.up;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		if (!rb) {
			rb = gameObject.AddComponent<Rigidbody>();
		}

		rb.useGravity = false;
		rb.isKinematic = true;
		rb.interpolation = RigidbodyInterpolation.Extrapolate;
		
	}

	private void FixedUpdate()
	{
			rb.MovePosition(transform.position + movementDirection * physicSpeed * Time.fixedDeltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		//Self destruct bullet
		Destroy(gameObject);
		enabled = false;
	}
}
