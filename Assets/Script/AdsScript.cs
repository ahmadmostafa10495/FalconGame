using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsScript : MonoBehaviour {
    public static AdsScript Instance;
    private string androidGameID = "2813318";
    private string iosGameID = "2813320";
    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start ()
    {
#if UNITY_ANDROID
        Advertisement.Initialize(androidGameID);
#endif
#if UNITY_IOS
        Advertisement.Initialize(iosGameID);
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}
    public static void ShowInterstitialAds()
    {
        Advertisement.Show();
    }
    public void ShowRewardedVideo()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show("rewardedVideo", options);
    }

    private void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Video completed - Offer a reward to the player");
            // Reward your player here.
            LevelManager.Instance.IncreaseLifeCount();

        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");
            LevelManager.Instance.CancelVideo();
        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
            LevelManager.Instance.CancelVideo();
        }
    }
}
