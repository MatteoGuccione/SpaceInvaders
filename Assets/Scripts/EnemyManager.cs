using UnityEngine;

public class EnemyManager : MonoBehaviour {

	[SerializeField]
	PrimitiveType voxel = PrimitiveType.Cube;

    bool animate = false;

	GameObject goShape1;
	GameObject goShape2;

	float deltaX = 1;
	float deltaY = 1;

	float fireDelay = 0;

	GameManager gameManager;

	int hitPoints;

	public void Configure(Sprite frame1Shape, Sprite frame2Shape, Material newMat, int points, GameManager manager) {

		hitPoints = points;

		gameManager = manager;

		fireDelay = Random.Range(5, 10);

		goShape1 = new GameObject();
		goShape1.AddComponent<DestroyOnHit>();
		goShape1.name = "Frame1";
		goShape1.transform.parent = transform;

		goShape2 = new GameObject();
		goShape2.AddComponent<DestroyOnHit>();
		goShape2.name = "Frame2";
		goShape2.transform.parent = transform;

		int startX = (int)frame1Shape.textureRect.xMin;
		int startY = (int)frame1Shape.textureRect.yMin;

		int w = (int)frame1Shape.textureRect.width;
        int h = (int)frame1Shape.textureRect.height;

		//Debug.Log($"Image 1 size: {w}x{h} from:{startX}x{startY} to {startX+w}x{startY+h}");

        //Prepare to read pixels colors for frame1
		//Color[] pixels = frame1Shape.texture.GetPixels();

		float currentX = 0;
		float currentY = 0;

		//iterate image 1

		for (int i = startX; i < startX + w; i++) {
			for (int j = startY; j < startY + h; j++) {

				//Debug.LogFormat("EM step {0}:{1}-> {2} value: {3}",i,j, (i * j) + j, frame1Shape.texture.GetPixel(i,j));

				Color c = frame1Shape.texture.GetPixel(i,j);
				if (c.r != 0 && c.g != 0 && c.b != 0) {
					//not black
					GameObject go = GameObject.CreatePrimitive(voxel);
					go.GetComponent<MeshRenderer>().material = newMat;
					go.name = $"{i}:{j}:{frame1Shape.texture.GetPixel(i,j)}%";
					go.transform.parent = goShape1.transform; //make object child of goShape1
					go.transform.position = new Vector3(currentX, currentY, 0);
					//Debug.LogFormat("Img1 block at: {0} {1}", i, j);
				}

				currentY += deltaY;
			}

			currentY = 0;
			currentX += deltaX;

		}

		currentX = 0;
		currentY = 0;

		startX = (int)frame2Shape.textureRect.xMin;
		startY = (int)frame2Shape.textureRect.yMin;
		w = (int)frame2Shape.textureRect.width;
		h = (int)frame2Shape.textureRect.height;

		//Debug.Log($"Image 2 size: {w}x{h} from:{startX}x{startY} to {startX + w}x{startY + h}");

		//Prepare to read pixels colors for frame2
		//pixels = frame2Shape.texture.GetPixels();

		//iterate image 2
		for (int i = startX; i < startX + w; i++) {
			for (int j = startY; j < startY + h; j++) {
				
				Color c = frame1Shape.texture.GetPixel(i,j);
				
				if (c.r != 0 && c.g != 0 && c.b != 0) {
					//other color
					GameObject go = GameObject.CreatePrimitive(voxel);
					go.GetComponent<MeshRenderer>().material = newMat;
					go.name = $"{i}:{j}:{frame1Shape.texture.GetPixel(i,j)}%";
					go.transform.parent = goShape2.transform; //make object child of goShape2
					go.transform.position = new Vector3(currentX, currentY, 0);
					//Debug.LogFormat("Img2 block at: {0} {1}",i,j);
				}
				currentY += deltaY;
			}

			currentY = 0;
			currentX += deltaX;
		}

		MeshMerger mr1 = goShape1.AddComponent<MeshMerger>();
		mr1.Configure(newMat,false);

		MeshMerger mr2 = goShape2.AddComponent<MeshMerger>();
		mr2.Configure(newMat,false);

		//hide second frame
		goShape2.SetActive(false);

		InvokeRepeating("Fire", fireDelay, fireDelay);

		animate = true;

	}

	// Update is called once per frame
	void Update()
    {
        if (!animate) return;

		//Debug.Log(Mathf.FloorToInt(Time.time));

		goShape1.SetActive(Mathf.FloorToInt(Time.time) % 2 == 0);

		goShape2.SetActive(!goShape1.activeSelf);
	}

	private void Fire()
	{

		if (Random.Range(1, 100) > 80) {
			GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
			bullet.tag = "EnemyBullet";
			bullet.transform.localScale = new Vector3(2, 4, 2);
			bullet.transform.position = transform.position;
			bullet.AddComponent<EnemyBulletManager>();
			Destroy(bullet, 4);
		}
	}

	public void WasHit() {
		gameManager.DidHitEnemy(hitPoints);
	}
}
