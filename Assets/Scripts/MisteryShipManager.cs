using UnityEngine;

public class MisteryShipManager : MonoBehaviour
{
	[SerializeField]
	Vector3 startPosition;

	[SerializeField]
	Vector3 endPosition;

	[SerializeField]
	Sprite ship;

	[SerializeField]
	Color color = Color.magenta;

	[SerializeField]
	float speed = 1000;

	[SerializeField]
	Material shipMaterial;

	[SerializeField]
	Material shipEmissiveMaterial;

	[SerializeField]
	GameManager manager;

	[SerializeField]
	int[] hitPoints = { 50, 100, 150, 200, 300 };

	GameObject shipShape;

	float deltaX = 1;
	float deltaY = 1;

	bool animate = false;

	MeshRenderer[] renderers;

	AudioSource aso;

	private void Start() {

		aso = GetComponent<AudioSource>();

		shipShape = new GameObject();
		shipShape.name = "MisteryShip";
		shipShape.transform.parent = transform;

		PlayerDestroyOnHit pdh = shipShape.AddComponent<PlayerDestroyOnHit>();
		pdh.Manager = manager;

		int startX = (int)ship.textureRect.xMin;
		int startY = (int)ship.textureRect.yMin;

		int w = (int)ship.textureRect.width;
		int h = (int)ship.textureRect.height;

		//Debug.Log($"Ship size: {w}x{h} from:{startX}x{startY} to {startX+w}x{startY+h}");

		//Prepare to read pixels colors for frame1
		//Color[] pixels = ship.texture.GetPixels();

		float currentX = 0;
		float currentY = 0;

		//iterate image 1
		for (int i = startX; i < startX + w; i++) {
			for (int j = startY; j < startY + h; j++) {

				//Debug.LogFormat("EM step {0}:{1}-> {2} value: {3}",i,j, (i * j) + j, frame1Shape.texture.GetPixel(i,j));

				Color c = ship.texture.GetPixel(i,j);
				if (c.r != 0 && c.g != 0 && c.b != 0) {
					//not black
					GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
					go.GetComponent<MeshRenderer>().material = shipEmissiveMaterial;
					go.name = $"{i}:{j}:{ship.texture.GetPixel(i,j)}%";
					go.transform.position = new Vector3(currentX, currentY, 0);
					go.transform.parent = shipShape.transform; //make object child of shipShape
															   //Debug.LogFormat("Img1 block at: {0} {1}", i, j);
				}

				currentY += deltaY;
			}

			currentY = 0;
			currentX += deltaX;

		}

		MeshMerger mm = shipShape.AddComponent<MeshMerger>();
		mm.Configure(shipMaterial, false);

		//The translation should be done after merging the geometry
		transform.position = startPosition;
		transform.localScale = new Vector3(2, 2, 2);

		//Get all renderers
		renderers = GetComponentsInChildren<MeshRenderer>();
		foreach (Renderer r in renderers) {
			r.enabled = false;
		}

		//Animate
		Invoke("Animate", Random.Range(5, 15));
		//Animate();
	}

	void Animate() {
		foreach (Renderer r in renderers) {
			r.enabled = true;
		}

		transform.position = startPosition;

		animate = true;

		aso.Play();
	}

	private void Update()
	{
		if (!animate) return;

		transform.Translate(speed * Time.deltaTime * Vector3.left);

		if (Vector3.Distance(endPosition, transform.position) <= 10f) {
			HideShip();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("PlayerBullet")) {

			HideShip();

			int points = hitPoints[Random.Range(0, hitPoints.Length)];

			manager.DidHitEnemy(points);
		}
	}

	private void HideShip() {

		aso.Stop();

		animate = false;

		foreach (Renderer r in renderers) {
			r.enabled = false;
		}

		Invoke("Animate", Random.Range(5, 15));
	}
}
