using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Enemies setup

    [SerializeField]
    Material enemyMaterial;

    [SerializeField]
    int enemyWaves;

    [SerializeField]
    int enemyRows = 1;

    int totalEnemyWaves;

    //Enemy 1st row top
    [SerializeField]
    Sprite enemy1;

    [SerializeField]
    Sprite enemy1b;

    [SerializeField]
    int enemy1Cols = 11;



    [SerializeField]
    int enemy1Points = 30;

    //Enemy 2nd and 3rd row middle
    [SerializeField]
    Sprite enemy2;

    [SerializeField]
    Sprite enemy2b;

    [SerializeField]
    int enemy2Cols = 11;

    [SerializeField]
    int enemy2Points = 20;

    //Enemy 4th and 5th row bottom

    [SerializeField]
    Sprite enemy3;

    [SerializeField]
    Sprite enemy3b;

    [SerializeField]
    int enemy3Cols = 11;

    [SerializeField]
    int enemy3Points = 10;

    [SerializeField]
    GameObject enemyGO;

    [SerializeField]
    float nextWaveTimer = 300;

    float timer;

    [SerializeField]
    float deltaX = 1;

    [SerializeField]
    float deltaY = 1;

    float currentY = 0;

    //Barriers setup

    [SerializeField]
    Material barrierMaterial;

    [SerializeField]
    GameObject barrierGO;

    [SerializeField]
    Sprite barrier;

    [SerializeField]
    int barriers = 4;

    [SerializeField]
    Vector3 barriesDelta = new Vector3(42, 0, 0);

    [SerializeField]
    Vector3 barriersStart = new Vector3(10, -90, 0);

    //Player
    [SerializeField]
    GameObject player;

    //Score
    int score = 0;

    //GUI
    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    TMP_Text scoreBestText;

    //GameOVer

    [SerializeField]
    GameObject gameOverCanvas;

    // Start is called before the first frame update
    void Start()
    {
        timer = nextWaveTimer;
        totalEnemyWaves = 0;

        scoreBestText.text = $"hi-Score:\n{PlayerPrefs.GetInt("score")}";
        scoreText.text = $"Score:\n{score}";

        InstantiateRow(enemyWaves);
        //Debug.Log($"DY: {deltaY}");



        //Debug.Log($"DY: {currentY}");



        //Barriers

        for (int i = 0; i < barriers; i++)
        {
            GameObject go = Instantiate(barrierGO);
            go.name = $"Barrier #{i}";
            go.SetActive(true);

            BarrierManager em = go.GetComponent<BarrierManager>();
            em.Configure(barrier, new Color(1, 1, 1), barrierMaterial);

            go.transform.position = new Vector3(barriersStart.x + i * barriesDelta.x,
                                                barriersStart.y,
                                                barriersStart.z);
        }
    }

    void Update()
    {
        Debug.Log(timer);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            InstantiateRow(1, true);
            timer = nextWaveTimer;
        }
    }

    public void DidHitEnemy(int newPoints)
    {
        score += newPoints;
        scoreText.text = $"Score:\n{score}";
    }

    public void GameOver()
    {
        player.GetComponent<PlayerManager>().enabled = false;
        gameOverCanvas.SetActive(true);

        int lastScore = PlayerPrefs.GetInt("score", 0);

        if (score > lastScore)
        {
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.Save();
        }
    }

    public void InstantiateRow(int enemyWaves, bool newWave = false)
    {
        if (newWave)
        {
            currentY = deltaY * totalEnemyWaves * enemyRows;
        }
        else
        {
            currentY = deltaY;
        }
        for (int k = 0; k < enemyWaves; k++)
        {
            int enemyID = Random.Range(1, 4);

            switch (enemyID)
            {
                case 1:
                    Material e1Material = new Material(enemyMaterial);

                    e1Material.color = new Color(0, 1, 0);

                    for (int i = 0; i < enemyRows; i++)
                    {
                        for (int j = 0; j < enemy1Cols; j++)
                        {
                            //Debug.Log("Loop GM: " + i + " j:" + j);

                            GameObject go = Instantiate(enemyGO);
                            go.SetActive(true);

                            EnemyManager em = go.GetComponent<EnemyManager>();
                            em.Configure(enemy1, enemy1b, e1Material, enemy1Points, this);

                            go.transform.position = new Vector3(j * deltaX, currentY, 0);

                            go.transform.parent = transform;
                        }

                        currentY -= deltaY;

                    }
                    break;
                case 2:
                    Material e2Material = new Material(enemyMaterial);

                    e2Material.color = new Color(1, 1, 0.5f);

                    for (int i = 0; i < enemyRows; i++)
                    {
                        for (int j = 0; j < enemy2Cols; j++)
                        {
                            GameObject go = Instantiate(enemyGO);
                            go.SetActive(true);

                            EnemyManager em = go.GetComponent<EnemyManager>();
                            em.Configure(enemy2, enemy2b, e2Material, enemy2Points, this);

                            go.transform.position = new Vector3(j * deltaX, currentY, 0);

                            go.transform.parent = transform;

                        }

                        currentY -= deltaY;
                    }
                    break;
                case 3:
                    Material e3Material = new Material(enemyMaterial);

                    e3Material.color = new Color(1, 0.5f, 0);

                    for (int i = 0; i < enemyRows; i++)
                    {
                        for (int j = 0; j < enemy3Cols; j++)
                        {
                            GameObject go = Instantiate(enemyGO);
                            go.SetActive(true);

                            EnemyManager em = go.GetComponent<EnemyManager>();
                            em.Configure(enemy3, enemy3b, e3Material, enemy3Points, this);

                            go.transform.position = new Vector3(j * deltaX, currentY, 0);

                            go.transform.parent = transform;

                        }

                        currentY -= deltaY;
                    }
                    break;
                default:
                    break;
            }
            totalEnemyWaves++;
        }
    }
}
