using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class EnemiesMover : MonoBehaviour {
    float deltaX = 5;
    float step = 1;
    int direction = 1;

    [SerializeField]
    float gameOverLowerY = -18;

    // Start is called before the first frame update
    void Start() {
        InvokeRepeating("Move",step,step);
    }

    void Move() {
        transform.position += Vector3.right * deltaX * direction;

        if (transform.position.x > 20 && direction == 1) {
			CancelInvoke();

			direction *= -1;
            transform.position -= Vector3.up;

            step -= 0.1f;

			InvokeRepeating("Move", step, step);
		}
        else if (transform.position.x < -20 && direction == -1) {

			CancelInvoke();

			direction *= -1;
			transform.position -= Vector3.up;

			step -= 0.1f;

            if (step >= 0.1) {
                InvokeRepeating("Move", step, step);
            }
            else {
				CancelInvoke();

				GetComponent<GameManager>().GameOver();

				enabled = false;
			}
		}

        if (transform.position.y < gameOverLowerY) {
            //GameOver

            CancelInvoke();

            GetComponent<GameManager>().GameOver();

            enabled = false;
        }
    }
}
