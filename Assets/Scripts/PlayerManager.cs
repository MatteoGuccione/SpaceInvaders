using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerManager : MonoBehaviour
{
	[SerializeField]
	Vector3 startPotision = new Vector3(70,-125,0);

	[SerializeField]
	PrimitiveType voxel = PrimitiveType.Cube;

	[SerializeField]
	float horizontalDelta = 90;

	[SerializeField]
	Sprite player;

	[SerializeField]
	Color color = Color.white;

	[SerializeField]
	float speed = 1000;

	[SerializeField]
	Material playerMaterial;

	[SerializeField]
	Material playerEmissiveMaterial;

	[SerializeField]
	GameManager manager;

	GameObject playerShape;

	[SerializeField]
	AudioSource fireAS;

	[SerializeField]
	AudioSource dieAS;

    [SerializeField]
    int Lives;

    float deltaX = 1;
	float deltaY = 1;
	private float _deceleration = 0f;
	[SerializeField] private float decelerationTime = 2;

	private void Start()
	{
		playerShape = new GameObject();
		playerShape.name = "Player";
		playerShape.transform.parent = transform;

		PlayerDestroyOnHit pdh = playerShape.AddComponent<PlayerDestroyOnHit>();
		pdh.Manager = manager;
		pdh.Lives = Lives;

		int startX = (int)player.textureRect.xMin;
		int startY = (int)player.textureRect.yMin;

		int w = (int)player.textureRect.width;
		int h = (int)player.textureRect.height;

		//Debug.Log($"Player size: {w}x{h} from:{startX}x{startY} to {startX+w}x{startY+h}");

		//Prepare to read pixels colors for frame1
		//Color[] pixels = player.texture.GetPixels();

		float currentX = 0;
		float currentY = 0;

		//iterate image 1
		for (int i = startX; i < startX + w; i++) {
			for (int j = startY; j < startY + h; j++) {

				//Debug.LogFormat("EM step {0}:{1}-> {2} value: {3}",i,j, (i * j) + j, frame1Shape.texture.GetPixel(i,j));

				Color c = player.texture.GetPixel(i,j);
				if (c.r != 0 && c.g != 0 && c.b != 0) {
					//not black
					GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
					go.GetComponent<MeshRenderer>().material = playerEmissiveMaterial;
					go.name = $"{i}:{j}:{player.texture.GetPixel(i,j)}%";
					go.transform.position = new Vector3(currentX, currentY, 0);
					go.transform.parent = playerShape.transform; //make object child of playerShape
					//Debug.LogFormat("Img1 block at: {0} {1}", i, j);
				}

				currentY += deltaY;
			}

			currentY = 0;
			currentX += deltaX;

		}

		MeshMerger mm = playerShape.AddComponent<MeshMerger>();
		mm.Configure(playerMaterial,false);

		//The translation should be done after merging the geometry
		transform.position = startPotision;
		transform.localScale = new Vector3(2, 2, 2);
	}

	private void OnEnable()
	{
		PlayerDestroyOnHit.OnPlayerDestroyed += PlayerWasHit;
	}

	private void PlayerWasHit()
	{
		enabled = false;
		GetComponentInChildren<MeshRenderer>().enabled = false;
		
	}

	// Update is called once per frame
	void Update() {
		
		//Not the best solution isn't? Multiplatform ....?
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			//left movement
			transform.Translate(transform.right * (Time.deltaTime * speed * -1));
		}
		else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			//right movement
			transform.Translate(Time.deltaTime * speed * transform.right);
		}
		else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			_deceleration = -decelerationTime;
		}
		else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
		{
			_deceleration = decelerationTime;
		}
		
		transform.Translate(Time.deltaTime * _deceleration * transform.right);

		if (_deceleration > 0)
		{
			_deceleration -= Time.deltaTime;
			
		}
		else if (_deceleration < 0)
		{
			_deceleration += Time.deltaTime;
		}
		
		if (Mathf.Approximately(_deceleration, 0f))
		{
			_deceleration = 0f;
		}

		//Avoid moving outside the right / left delta
		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(transform.position.x, startPotision.x-horizontalDelta, startPotision.x + horizontalDelta);
		transform.position = pos;

		if (Input.GetKeyDown(KeyCode.Space)) {

			fireAS.PlayOneShot(fireAS.clip);

			//fire
			GameObject bullet = GameObject.CreatePrimitive(voxel); // GameObject.CreatePrimitive(PrimitiveType.Cube);
			bullet.tag = "PlayerBullet";
			bullet.transform.localScale = new Vector3(3, 5, 3);
			bullet.transform.position = transform.position + new Vector3(10,20,0);
			BulletManager bm = bullet.AddComponent<BulletManager>();
			Destroy(bullet, 4);
		}
	}

	private void OnDisable()
	{
		PlayerDestroyOnHit.OnPlayerDestroyed -= PlayerWasHit;
		dieAS.Play();
	}
}
