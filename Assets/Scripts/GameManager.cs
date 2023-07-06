using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public GameObject PowerupButtons;
    [SerializeField] private int _difficulty;
    [SerializeField] private int _ballsEase;
    public bool IsAllDirectionActive;
    public GameObject AllDirectionsBlock;
    public GameObject HowToPlayPopup;
    public GameObject FX;
    public AudioClip levelComplete;
    public Color mainColor;
    public Color[] _Colors;
    public static GameManager instance;
    public GameObject wall_left;
    public GameObject confettiFx;
    public GameObject BackButtonConfirmationWin;
    public GameObject AddTimeButton;
    public GameObject wall_right;
    public GameObject wall_top;
    public GameObject wall_bottom;
    public GameObject Ball;
    public GameObject f1_prefab;
    public GameObject[] f2_prefab;
    public GameObject BallParent;
    public GameObject Block;
    public GameObject Corner;
    public GameObject top_Bottom;
    public GameObject BlockParent;
    public GameObject SoundParent;
    public GameObject ballsDownButton;
    public GameObject speedUpButton;
    public GameObject MessageBox;
    public GameObject endGameScreen;
    public GameObject animationMessageScreen;
    public GameObject f1_gameobjectParent;
    public GameObject f2_gameobjectParent;
    public GameObject f3_gameobjectParent;
    public GameObject targetIndicator;
    public GameObject timerIndicator;
    public GameObject gameWonBgPanel;
    public GameObject notice;
    public int score = 0;
    public Camera camera_;
    public int maxSpeedVal = 20;
    public List<GameObject> players = new List<GameObject>();
    Vector2[] touches = new Vector2[3];
    public bool projectiling;
    public bool clampSpeedNow = false;
    public bool MainBallDown = false;
    public GameObject mainBall;
    public bool addNewLevel = false;
    public bool progressSaved = false;
    bool f1_spawned = false;
    bool f2_spawned = false;
    public Text ballCount;
    public Text HighscoreText;
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text MovesText;
    public bool isGameEnded = false;
    bool isGameover = false;
    bool isLevelClear = false;
    public float deadLine = 0;
    public Vector2 base_;
    public Vector2 forceVal;
    public LineRenderer GameOverLine;
    [SerializeField] bool IsTesting;
    public int LevelMovesCache = 0;
    public int TotalMoves = 0;
    public bool Initialised;
    public List<string> Patterns;
	private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Time.timeScale = 1;
    }
    void Start()
    {
        StartAsync();
    }
    async Task StartAsync()
    {
        Debug.Log("Started");
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
        {
            int _curLevel = PlayerPrefs.GetInt("mission", 1);

			if(!IsTesting)
            {
				if (_curLevel < 2)
				{
					_ballsEase = 10;
					_difficulty = 1;
                    TotalMoves = 10;
				}
				else
				{
                    TotalMoves = _curLevel * _curLevel * (int)FirebaseManager.instance.remoteConfig.GetValue("TargetFactor").LongValue;
					_ballsEase = (int)FirebaseManager.instance.remoteConfig.GetValue("BallsEase").LongValue;
					_difficulty = (int)FirebaseManager.instance.remoteConfig.GetValue("Difficulty").LongValue;
				}
			}
            else
            {
                TotalMoves = 100;
                _ballsEase = 10;
                _difficulty = 1;
            }
            int MovesFromServer = await Server.Instance.CheckMovesValueExists(PlayerPrefs.GetInt("mission",1));
            
            if (MovesFromServer != -1)
            {
                TotalMoves = MovesFromServer;
                MovesText.text = "Moves: " + TotalMoves.ToString();
                ShowMessage($"Our best player finished this level in {MovesFromServer} moves only. \ncan you ?");
            }
            else
            {
				ShowMessage($"Seems like you're the first player on this level.");
			}
            LevelMovesCache = TotalMoves;
            MovesText.text = "Moves: " + TotalMoves.ToString();
            showGameobjectForTime(targetIndicator, 10);
            if (!PlayerPrefs.HasKey("mission"))
            {
                PlayerPrefs.SetInt("mission", 1);
            }
            levelText.text = "LEVEL: "+ (PlayerPrefs.GetInt("mission")).ToString();
			spawnWalls();
			isGameover = false;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float ratio = (float)screenHeight / (float)screenWidth;
            float newBlockSize = (1.777777778f * 0.5f) / ratio;
            Block.transform.localScale = new Vector3(newBlockSize, newBlockSize, newBlockSize);
            projectiling = false;

            if (PlayerPrefs.HasKey("highScore"))
            {
                HighscoreText.text = "    HIGHSCORE: " + fromIntToString(PlayerPrefs.GetInt("highScore"));
            }
            else
            {
                HighscoreText.text = "    HIGHSCORE: 0";
            }
            CreateLevel(Random.Range(0,Patterns.Count), PlayerPrefs.GetInt("myLevel", 5));
        }
    }
	List<int> ExtractNumbers(string input)
	{
		List<int> numbersList = new List<int>();

		string[] numberStrings = input.Split('-');

		foreach (string numberString in numberStrings)
		{
			if (int.TryParse(numberString, out int number))
			{
				numbersList.Add(number);
			}
		}

		return numbersList;
	}
	void CreateLevel(int patternIndex, int Level)
    {
        Debug.Log("Here");
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		float ratio = (float)screenHeight / (float)screenWidth;
		float downUnit = (1.777777778f * 0.55f) / ratio;
        List<Vector3> positionList = new List<Vector3>();
        for(int i = 0; i < 10; i++)
        {
            for(float j = 0; j < 10; j++)
            {
                Vector3 Pos = camera_.ViewportToWorldPoint(new Vector3((j * 0.1f) + 0.05f, 0.8f, camera_.nearClipPlane));
                Pos = new Vector3(Pos.x, Pos.y - (i * downUnit), Pos.z);
                positionList.Add(Pos);
			}
		}
        List<int> indexListForPattern = ExtractNumbers(Patterns[patternIndex]);
		for (int i = 0; i < indexListForPattern.Count; i++)
		{
			GameObject block = Instantiate(Block, new Vector3(positionList[indexListForPattern[i]].x, positionList[indexListForPattern[i]].y + 5, 0), Quaternion.identity);
			block.GetComponent<SpriteRenderer>().color = mainColor;
			int ranNumChoose = Random.Range(0, 5);
			int val = (PlayerPrefs.GetInt("mission",1) * _difficulty);
			val = ConvertToNearestTen(val);
			if (val < 10)
			{
				val = 10;
			}
			block.GetComponent<ValueHolder>().BlockValue = val;
			block.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = ConvertNumber(val);
			block.transform.parent = BlockParent.transform;
			block.GetComponent<BlockMovAnim>().Initialise(mainColor, new Vector3(positionList[indexListForPattern[i]].x, positionList[indexListForPattern[i]].y, 0), true);
		}
		Initialised = true;
	}
    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && Initialised)
        {
            if (!isLevelClear)
            {
                CheckLevelClear();
            }
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                scoreText.text = "SCORE: " + fromIntToString(score);
            }

            if (score >= PlayerPrefs.GetInt("highScore"))
            {
                if(!PlayerPrefs.HasKey("prevHighscore"))
                {
                    PlayerPrefs.SetInt("prevHighscore", score);
                }
                PlayerPrefs.SetInt("highScore", score);
                HighscoreText.text = "    HIGHSCORE: " + fromIntToString(PlayerPrefs.GetInt("highScore"));
                if(score > PlayerPrefs.GetInt("prevHighscore") + 500)
                {
                    PlayerPrefs.SetInt("prevHighscore", score);
                }
            }
            if (clampSpeedNow)
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(player.GetComponent<Rigidbody2D>().velocity, maxSpeedVal);
                }
            }
            if (allBallStationary() == 1 && !progressSaved)
            {
                progressSaved = true;
            }
            if (allBallStationary() == 1)
            {
                checkEndGame();
                if(IsAllDirectionActive)
                {
                    IsAllDirectionActive = false;
                    if(AllDirectionsBlock != null)
                    {
                        Destroy(AllDirectionsBlock);
                    }
                }
                projectiling = false;
                clampSpeedNow = false;
                if (ballsDownButton.activeInHierarchy)
                {
                    ballsDownButton.SetActive(false);
                }
                if (maxSpeedVal > 7)
                {
                    maxSpeedVal = 7;
                }
                speedUpButton.SetActive(false);
                if(players.Count > 0)
                {
					if (players[0].GetComponent<SpriteRenderer>().color != Color.green)
					{
						players[0].GetComponent<SpriteRenderer>().color = Color.green;
					}
					if (players[0].GetComponent<SpriteRenderer>().sortingOrder != 2)
					{
						players[0].GetComponent<SpriteRenderer>().sortingOrder = 2;
					}
				}
                foreach (GameObject player in players)
                {
                    player.GetComponent<gamePlay>().collidedToF1 = false;
                }
                if (f1_gameobjectParent.transform.childCount > 0 && f1_gameobjectParent.transform.GetChild(0).CompareTag("f1done"))
                {
                    Destroy(f1_gameobjectParent.transform.GetChild(0).gameObject);
                }
                if (f2_gameobjectParent.transform.childCount > 0 && f2_gameobjectParent.transform.GetChild(0).CompareTag("f2done"))
                {
                    Destroy(f2_gameobjectParent.transform.GetChild(0).gameObject);
                }
                if (f1_spawned == true)
                {
                    f1_spawned = false;
                }
                if (f2_spawned == true)
                {
                    f2_spawned = false;
                }
            }
            if (allBallStationary() == 0)
            {
                progressSaved = false;
                clampSpeedNow = true;
                projectiling = true;
                speedUpButton.SetActive(true);
            }
            if (allBallMoving() == 0)
            {
                ballCount.gameObject.SetActive(true);
                ballCount.gameObject.transform.position = new Vector2(base_.x, base_.y + 0.22f);
                ballCount.text = "x" + numOfStationaryBalls().ToString();
            }
            if (allBallMoving() == 1)
            {
                ballCount.gameObject.SetActive(false);
            }
        }

    }
    [SerializeField] GameObject PowerupPopup;
    [SerializeField] GameObject BaseButtons;
    private void Update()
    {
        if(Initialised)
        {
			if (isGameEnded)
			{
				PlayerPrefs.DeleteKey("myLevel");
				isGameover = true;
				endGameScreen.SetActive(true);
				isGameEnded = false;
				stopAllBalls();
			}

			if (Input.touchCount > 0 && !projectiling && !isGameover && !PowerupPopup.activeInHierarchy && !HowToPlayPopup.activeInHierarchy)
			{
				PlayerPrefs.SetInt("ballCount", 0);
				RaycastHit2D ray;
				RaycastHit2D ray2;
				if ((Input.GetTouch(0).position.y < AddTimeButton.transform.GetChild(2).position.y && Input.GetTouch(0).position.y > BaseButtons.transform.position.y + 100))
				{
					if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
					{
						Touch touchInfo = Input.GetTouch(0);
						touches[0] = Camera.main.ScreenToWorldPoint(new Vector3(touchInfo.position.x, touchInfo.position.y, -10));
						int layerMask = ~(LayerMask.GetMask("ball"));
						ray = Physics2D.Raycast(mainBall.transform.position, new Vector2(touches[0].x, touches[0].y + 4), Mathf.Infinity, layerMask);
						Vector2 reflectedray = new Vector2();
						if (ray.collider.tag == "leftWall" || ray.collider.tag == "rightWall" || ray.collider.tag == "block")
						{
							reflectedray = new Vector2(mainBall.transform.position.x, 2 * (ray.point.y) - mainBall.transform.position.y);
						}
						else if (ray.collider.tag == "topWall" || ray.collider.tag == "blockBase")
						{
							reflectedray = new Vector2(2 * (ray.point.x) - mainBall.transform.position.x, mainBall.transform.position.y);
						}
						string prevTag = ray.collider.gameObject.tag;
						int layerMask2 = ~(LayerMask.GetMask(prevTag));
						ray2 = Physics2D.Raycast(ray.point, reflectedray, Mathf.Infinity, layerMask2);
						touches[1] = ray.point;
						touches[2] = reflectedray;
						Vector2 semifinalPoint = new Vector2(2 * reflectedray.x - touches[1].x, 2 * reflectedray.y - touches[1].y);
						Vector2 finalPoint = new Vector2(2 * semifinalPoint.x - touches[1].x, 2 * semifinalPoint.y - touches[1].y);
						mainBall.GetComponent<LineRenderer>().enabled = true;
						mainBall.GetComponent<LineRenderer>().SetPosition(0, mainBall.transform.position);
						mainBall.GetComponent<LineRenderer>().SetPosition(1, touches[1]);
						mainBall.GetComponent<LineRenderer>().SetPosition(2, finalPoint);
					}
					if (Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						MainBallDown = false;
						int tmp = PlayerPrefs.GetInt("spawn");
						PlayerPrefs.SetInt("spawn", tmp + 1);
						changeForceVal();
						clampSpeedNow = true;
						foreach (GameObject player in players)
						{
							player.GetComponent<LineRenderer>().enabled = false;
						}
						projectiling = true;
						moveAllBalls1();
					}
				}

			}
		}
    }
    int allBallStationary()
    {
        int Val = 1;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val = 1 * Val;
            }
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude != 0)
            {
                Val = 0 * Val;
            }
        }
        return Val;
    }
    int allBallMoving()
    {
        int Val = 1;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude != 0)
            {
                Val = 1 * Val;
            }
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val = 0 * Val;
            }
        }
        return Val;
    }
    void checkEndGame()
    {
        if (!isGameover)
        {
			if (TotalMoves <= 0 && BlockParent.transform.childCount > 0)
			{
                PowerupPopup.SetActive(true);
                PowerupPopup.GetComponent<PowerupUseController>().init(3);
			}
        }
    }
    IEnumerator changeColorOfBlock(GameObject block)
    {
        yield return new WaitForSeconds(1f);
        if (block)
        {
            if (block.GetComponent<SpriteRenderer>().color != _Colors[3])
            {
                block.GetComponent<SpriteRenderer>().color = _Colors[3];
            }
        }
    }
    IEnumerator changeColorOfBlock2(GameObject block)
    {
        yield return new WaitForSeconds(1f);
        if (block)
        {
            if (block.GetComponent<SpriteRenderer>().color != _Colors[5])
            {
                block.GetComponent<SpriteRenderer>().color = _Colors[5];
            }
        }
    } 
    void changeForceVal()
    {
        forceVal = (-new Vector2(mainBall.transform.position.x, mainBall.transform.position.y) + touches[1]) * 1000 * Time.deltaTime;
    }
    void closeAllClearScreen()
    {
        MessageBox.SetActive(false);
    }
	public int ConvertToNearestTen(int number)
	{
		int remainder = number % 10;
		int result;

		if (remainder < 5)
			result = number - remainder;
		else
			result = number + (10 - remainder);

		return result;
	}
	void CheckLevelClear()
    {
        if (BlockParent.transform.childCount == 0)
        {
            PlayerPrefs.DeleteKey("myLevel");
            showMessageInAnimation("congratulations!!!\n mission passed\n level++", 1);
            gameWonBgPanel.SetActive(true);
            GameObject cnfettiFx = Instantiate(confettiFx);
            Destroy(cnfettiFx, 7);
            PlayerPrefs.SetInt("mission", PlayerPrefs.GetInt("mission") + 1);
            isLevelClear = true;
            Invoke("playWinSound", 1);
            Invoke("loadNewLevel", 7);
            PlayerPrefs.Save();
            stopAllBalls();
		}
    }
    public GameObject ParentBalls,ParentBlocks;
    void playWinSound()
    {
		SoundManager.instance.playThisSound(levelComplete);
    }
    public void exitApplication()
    {
        Application.Quit();
    }
    string fromIntToString(int value1)
    {
        float value = value1;
        string returnValue = "0";
        if (value < 10000)
        {
            returnValue = value.ToString();
        }
        else if (value >= 10000 && value < 1000000)
        {
            returnValue = (value / 1000).ToString("F2") + "K";
        }
        else if (value >= 1000000 && value < 1000000000)
        {
            returnValue = (value / 1000000).ToString("F2") + "B";
        }
        return returnValue;
    }
    public void GetAllBallsDown()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.transform.position = base_;
        }
        ballsDownButton.SetActive(false);
    }
    public void GetMainBall(GameObject ball)
    {
        MainBallDown = true;
        mainBall = ball;
        base_ = mainBall.transform.position;
    }
    public void gameToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    IEnumerator hideMessageInAnimation(int num, int time)
    {
        yield return new WaitForSeconds(time);
        animationMessageScreen.SetActive(false);
    }
    void hideMessageAnimated()
    {
        animationMessageScreen.SetActive(false);
    }
    IEnumerator hideObjectAfterTime(GameObject gameObject, int time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    void moveAllBalls1()
    {
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(forceVal, maxSpeedVal);
        if (PlayerPrefs.GetInt("ballCount") < players.Count - 1)
        {
            PlayerPrefs.SetInt("ballCount", PlayerPrefs.GetInt("ballCount") + 1);
            Invoke("moveAllBalls2", 0.08f);
        }
        else if (PlayerPrefs.GetInt("ballCount") == players.Count - 1)
        {
            ballsDownButton.SetActive(true);
            PlayerPrefs.SetInt("ballCount", 0);

			TotalMoves -= 1;
			MovesText.text = "Moves: " + TotalMoves.ToString();
		}
    }
    void moveAllBalls2()
    {
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        players[PlayerPrefs.GetInt("ballCount")].GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(forceVal, maxSpeedVal);
        if (PlayerPrefs.GetInt("ballCount") < players.Count - 1)
        {
            PlayerPrefs.SetInt("ballCount", PlayerPrefs.GetInt("ballCount") + 1);
            Invoke("moveAllBalls1", 0.08f);
        }
        else if (PlayerPrefs.GetInt("ballCount") == players.Count - 1)
        {
            ballsDownButton.SetActive(true);
            PlayerPrefs.SetInt("ballCount", 0);

			TotalMoves -= 1;
			MovesText.text = "Moves: " + TotalMoves.ToString();
		}
    }
    void loadNewLevel()
    {
		endGameScreen.SetActive(true);
        endGameScreen.GetComponent<GameOverPopupController>().IsWin = true;
	}
    int numOfStationaryBalls()
    {
        int Val = 0;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            {
                Val++;
            }
        }
        return Val;
    }
    public void OnClickNo()
    {
        BackButtonConfirmationWin.SetActive(false);
        Time.timeScale = 1;
    }
    public void OpenScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    void PopUpNotice(string message)
    {
        notice.SetActive(true);
        notice.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
    }
    void showGameobjectForTime(GameObject gameObject, int time)
    {
        gameObject.SetActive(true);
        StartCoroutine(hideObjectAfterTime(gameObject, time));
    }
    void showMessageInAnimation(string message, int num)
    {
        animationMessageScreen.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
        if(num == 2)
        {
            animationMessageScreen.SetActive(true); StartCoroutine(hideMessageInAnimation(num, 3));
        }
        else
        {
            animationMessageScreen.SetActive(true); StartCoroutine(hideMessageInAnimation(num, 7));
        }
        
    }
    
    void spawnWalls()
    {
        Vector3 top = camera_.ViewportToWorldPoint(new Vector3(0.5f, 1, camera_.nearClipPlane));
        Vector3 left = camera_.ViewportToWorldPoint(new Vector3(0, 0.5f, camera_.nearClipPlane));
        Vector3 right = camera_.ViewportToWorldPoint(new Vector3(1, 0.5f, camera_.nearClipPlane));
        Vector3 bottom = camera_.ViewportToWorldPoint(new Vector3(0.5f, 0, camera_.nearClipPlane));
        Vector3 ball = camera_.ViewportToWorldPoint(new Vector3(0.5f, 0.08f, camera_.nearClipPlane));

        GameObject TopWall = Instantiate(wall_top, top, Quaternion.identity);
        Instantiate(wall_left, left, Quaternion.identity);
        Instantiate(wall_right, right, Quaternion.identity);
        GameObject bottomWall = Instantiate(wall_bottom, bottom, Quaternion.identity);

        TopWall.transform.parent = top_Bottom.transform;
        bottomWall.transform.parent = top_Bottom.transform;
        Vector2 corner_2 = camera_.ViewportToWorldPoint(new Vector3(0, 1, camera_.nearClipPlane));
        Vector2 corner_4 = camera_.ViewportToWorldPoint(new Vector3(1, 1, camera_.nearClipPlane));
        GameObject c2 = Instantiate(Corner, corner_2, Quaternion.identity);
        GameObject c4 = Instantiate(Corner, corner_4, Quaternion.identity);
        c2.transform.position = new Vector2(c2.transform.position.x, TopWall.transform.GetChild(0).transform.position.y);
        c4.transform.position = new Vector2(c4.transform.position.x, TopWall.transform.GetChild(0).transform.position.y);

        for (int i = 0; i < PlayerPrefs.GetInt("mission",1) * _ballsEase; i++)
        {
            GameObject player = Instantiate(Ball, ball, Quaternion.identity);
            players.Add(player);
            players[i].transform.parent = BallParent.transform;
        }
        GetMainBall(players[0]);
        base_ = mainBall.transform.position;
    }
    public void Give10ExtraBalls()
    {
		for (int i = 0; i < 10; i++)
		{
			GameObject player = Instantiate(Ball, mainBall.transform.position, Quaternion.identity);
			players.Add(player);
			players[i].transform.parent = BallParent.transform;
		}
	}
    public string ConvertNumber(float number)
    {
        string[] suffixes = { "", "K", "M", "B", "T" };
        int suffixIndex = 0;

        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;
        }

        string formattedNumber = number.ToString("0.##") + suffixes[suffixIndex];
        return formattedNumber;
    }
    public void ShowMessage(string message, bool Errormessage = false)
    {
        if(PlayerPrefs.HasKey("HowToPlay"))
        {
			MessageBox.SetActive(true);
			if (Errormessage)
			{
				MessageBox.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
			}
			else
			{
				MessageBox.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
			}
			MessageBox.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
		}
    }
    void stopAllBalls()
    {
        foreach(GameObject player in players)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
    public void speedUpAllBalls()
    {
        foreach (GameObject player in players)
        {
            maxSpeedVal = 10;
            player.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity * 2;
        }
        speedUpButton.SetActive(false);
    }
}
