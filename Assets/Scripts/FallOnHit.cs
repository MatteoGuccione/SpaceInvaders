using UnityEngine;

public class FallOnHit : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(gameObject);

		enabled = false;
	}
}
