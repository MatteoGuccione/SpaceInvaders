using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDestroyOnHit : MonoBehaviour
{
	GameManager manager;
	
	public static Action OnPlayerDestroyed;

	int lives;

	public GameManager Manager { get => manager; set => manager = value; }
	public int Lives { get => lives; set => lives = value; }
    //public GameManager Manager => manager;
    private void OnCollisionEnter(Collision collision)
	{
		if (!enabled) return;

		if (collision.gameObject.CompareTag("EnemyBullet")) {

			Debug.Log("Hit by Enemy bullet", this);

			//GetComponentInParent<PlayerManager>().enabled = false;
			//GetComponentInParent<MeshRenderer>().enabled = false;
			
            lives--;
            if (lives > 0)
            {
                Destroy(collision.gameObject);
            }
            else
			{
                Destroy(gameObject, 2);

                OnPlayerDestroyed?.Invoke();


				bool first = true;

				foreach (Transform t in transform)
				{

					Debug.Log(t.name);

					if (first)
					{
						first = false;
						continue;
					}
					t.gameObject.SetActive(true);

					Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
					rb.AddExplosionForce(20, t.transform.position, 100, 0, ForceMode.Impulse);
				}

				Manager.GameOver();

				enabled = false;
			}
		}
	}
}
