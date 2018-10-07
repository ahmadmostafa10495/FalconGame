using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance;

    [SerializeField] private PCamera myCam;
    [HideInInspector] public CheckPoint CurrentCP;
    [HideInInspector] public int Score = 0;
    [SerializeField] private List<Coin> CoinsTaken;
    [SerializeField] private List<RectTransform> whiteDots;
    [SerializeField] private RectTransform blackDot;
    [SerializeField] private List<RectTransform> lives;
    [SerializeField] private GameObject wantToContinue;
    [SerializeField] private GameObject levelFinished;
    [SerializeField] private float timeToPopAd = 180;

    [Header("Testing")]
    [SerializeField] private Level testingLevel;

    private Level curLevel;
    private int curLevelIndex;

    private float lastAdTime;
    private int livesCount = 3;
    //private bool addOneLife = false;
    private bool alreadyAdded = false;
    

    private int curCPIndex = 0;
    public Ball ball;

    private void Awake()
    {
        Instance = this;
        lastAdTime = Time.time;
    }

    void Start ()
    {
        //LoadLevel(0);
        LoadLevelFromScene(testingLevel);
    }

    void LoadLevel(int levelIndex)
    {
        if (curLevel != null)
            UnloadCurrentLevel();

        curLevelIndex = levelIndex;

        GameObject lvl = Resources.Load<GameObject>("Levels/Level_" + levelIndex);
        curLevel = Instantiate(lvl).GetComponent<Level>();

        ResetLevel();
    }
    void LoadLevelFromScene(Level lvl)
    {
        if (curLevel != null)
            UnloadCurrentLevel();

        curLevelIndex = -1;

        curLevel = lvl;

        ResetLevel();
    }
    void UnloadCurrentLevel()
    {
        Destroy(curLevel.gameObject);
        Resources.UnloadUnusedAssets();
    }

    void Update () {
		
	}

    public void CheckPointReached(CheckPoint cp)
    {
        curCPIndex = curLevel.AllCPs.IndexOf(cp);
        if (cp != CurrentCP)
        {
            CheckPointUIHighlighter(curCPIndex);
            IncreaseScore();
        }
        else
            ResetCoins();
        CurrentCP = cp;
        DeactivateCP(CurrentCP);

        if (curLevel.AllCPs.Count > curCPIndex + 1)
        {
            myCam.PlaceCameraOnTarget(CurrentCP.transform);
            DoorControl(curCPIndex);
        }
        else
        {
            Win();
        }
    }
    public void GoToCurrentCP()
    {
        ball.PlaceOnCP(CurrentCP, true);
        DeactivateCP(CurrentCP);
    }

    public void ActivateCP(CheckPoint cp)
    {
        cp.Activate();
    }
    public void DeactivateCP(CheckPoint cp)
    {
        cp.Deactivate();
    }
    public void DoorControl(int cpIndex)
    {
        if (curLevel.AllCPs[cpIndex].DoorAbove != null)
            curLevel.AllCPs[cpIndex].DoorAbove.Open();
        if (curLevel.AllCPs[cpIndex].DoorBelow != null)
            curLevel.AllCPs[cpIndex].DoorBelow.Close();

        int nextCP = cpIndex + 1;
        if (curLevel.AllCPs[nextCP].DoorAbove != null)
            curLevel.AllCPs[nextCP].DoorAbove.Close();
        if (curLevel.AllCPs[nextCP].DoorBelow != null)
            curLevel.AllCPs[nextCP].DoorBelow.Open();
    }

    public void KillBall()
    {
        LivesControl();

        ball.Die();
        PCamera.Instance.Shake(0.1f, 0.1f);
        ResetCoins();
        
        if(livesCount == 0)
        {
                if (!alreadyAdded)
                    wantToContinue.SetActive(true);
                else
                    CancelVideo();
        }
        else
            GoToCurrentCP();
    }

    void Win()
    {
        Debug.Log("Won");
        if (Time.time - lastAdTime >= timeToPopAd)
        {
            lastAdTime = Time.time;
            AdsScript.ShowInterstitialAds();
        }
        levelFinished.SetActive(true);
       
    }
    private void IncreaseScore()
    {
        for (int i = 0; i < CoinsTaken.Count; i++)
        {
            Score += CoinsTaken[i].coinValue;
        }
        CoinsTaken.Clear();
    }
    public void CoinTaken(Coin coin)
    {
        CoinsTaken.Add(coin);
        coin.Hide();
    }
    private void ResetCoins()
    {
        for (int i = 0; i < CoinsTaken.Count; i++)
        {
          CoinsTaken[i].Show();
        }
        CoinsTaken.Clear();
    }
    private void CheckPointUIHighlighter(int index)
    {
        blackDot.position = whiteDots[index].position;

    }
    private void LivesControl()
    {
        livesCount--;
        lives[livesCount].gameObject.SetActive(false);
    }
    private void ResetLevel()
    {
        curCPIndex = 0;
        CurrentCP = curLevel.AllCPs[curCPIndex];
        GoToCurrentCP();
        myCam.PlaceCameraOnTarget(CurrentCP.transform);
        DoorControl(curCPIndex);
        ResetLives();
        alreadyAdded = false;
        CheckPointUIHighlighter(curCPIndex);

    }
    void ResetLives()
    {
        livesCount = 3;
        for (int i = 0; i < lives.Count; i++)
        {
            lives[i].gameObject.SetActive(true);
        }
    }
    /*public void AddOneLife()
    {
        addOneLife = true;
    }*/
    public void ShowVideo()
    {
        wantToContinue.SetActive(false);
        AdsScript.Instance.ShowRewardedVideo();
        /*if (addOneLife)
        {
            IncreaseLifeCount();
        }*/
    }
    public void CancelVideo()
    {
        wantToContinue.SetActive(false);
        ResetLevel();
        if (Time.time - lastAdTime >= timeToPopAd)
        {
            lastAdTime = Time.time;
            AdsScript.ShowInterstitialAds();
        }
    }
    public void IncreaseLifeCount()
    {
        livesCount++;
        lives[0].gameObject.SetActive(true);
        //addOneLife = false;
        alreadyAdded = true;
        GoToCurrentCP();
    }
    public void Replay()
    {
        LoadLevel(curLevelIndex);
        levelFinished.SetActive(false);
    }
    public void NextLevel()
    {
        curLevelIndex++;
        LoadLevel(curLevelIndex);
        levelFinished.SetActive(false);
    }
}
