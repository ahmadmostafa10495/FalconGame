using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    public int coinValue;
    [SerializeField]
    private float coinSpeed = 220;
    [SerializeField]
    private ParticleSystem coinPS;
    [SerializeField]
    private Color particleColor;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, coinSpeed * Time.deltaTime, 0));
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //LevelManager.Instance.IncreaseScore(coinValue);
            LevelManager.Instance.CoinTaken(this);
            CoinPSControl(transform.position);
            //Destroy(gameObject);
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void CoinPSControl(Vector3 position)
    {
        coinPS.transform.position = position;
        ParticleSystem.MainModule m = coinPS.main;
        m.startColor = particleColor;
        Instantiate(coinPS);
    }
}
