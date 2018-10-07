using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdMobScript : MonoBehaviour {
    public static AdMobScript Instance;
    // Initialize an InterstitialAd.
    InterstitialAd interstitial;
    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
#if UNITY_ANDROID
            string appId = "ca-app-pub-8506736924404686~5710940889";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#endif
        MobileAds.Initialize(appId);
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-8506736924404686/4051209008";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
       /* // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        */
    }
   /* public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        interstitial.Destroy();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
        interstitial.Destroy();
    }
    */


}
