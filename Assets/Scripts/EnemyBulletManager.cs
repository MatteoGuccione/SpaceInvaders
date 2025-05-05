using UnityEngine;

public class EnemyBulletManager : MonoBehaviour
{
	Rigidbody rb;

	float physicSpeed = 50;

	Vector3 movementDirection = Vector3.down;

	private void Start() {
		rb = GetComponent<Rigidbody>();

		if (!rb) {
			rb = gameObject.AddComponent<Rigidbody>();
		}

		rb.useGravity = false;
		rb.isKinematic = true;
		rb.interpolation = RigidbodyInterpolation.Extrapolate;

	}

	private void FixedUpdate() {
		rb.MovePosition(transform.position + movementDirection * physicSpeed * Time.fixedDeltaTime);
	}

	private void OnCollisionEnter(Collision collision) {

		if (!enabled) return;

		if (collision.gameObject.CompareTag("Player")) {
			//Self destruct bullet
			Destroy(gameObject);
			enabled = false;
		}
		else if (collision.gameObject.CompareTag("Barrier")){
			//Self destruct bullet
			Destroy(gameObject);
			enabled = false;
		}
		else if (collision.gameObject.CompareTag("PlayerBullet")) {

			//If collision with player bullet destroy both
			Destroy(collision.gameObject);

			//Self destruct bullet
			Destroy(gameObject);
			enabled = false;
		}
	}
}
