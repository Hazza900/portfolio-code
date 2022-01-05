using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : MonoBehaviour {

	public Text errorMessage;

	public static AccountManager instance;

	public bool loggedIn;
	public string userid;
	public string username;

	string LoginURL = "http://127.0.0.1/GT3Login.php";
	string RegisterURL = "http://127.0.0.1/GT3Register.php";
	string ScoresURL = "http://127.0.0.1/GT3Times.php";
	string AddURL = "http://127.0.0.1/GT3Add.php";

	public PlayerScores playerScores;

	// Use this for initialization
	void Awake() 
	{
		if (instance != null)
        {
			Destroy(gameObject);
			return;
        }
		else
        {
			instance = this;
			DontDestroyOnLoad(gameObject);
        }
	}
	
	public IEnumerator SignIn(string u, string p)
    {
		string URL = LoginURL + "?Username=" + u + "&Password=" + p;
		WWW loginPage = new WWW(URL);
		yield return loginPage;

		if (loginPage.error != null)
		{
			Debug.Log("Error connecting to Database server");
			errorMessage.text = "Error connecting to Database server";
			errorMessage.gameObject.SetActive(true);
		}
		else
        {
			Debug.Log(loginPage.text);

			if (loginPage.text != "Failed")
            {
				loggedIn = true;
				userid = loginPage.text;				
				username = u;

				errorMessage.gameObject.SetActive(false);
            }
			else
            {
				//Display error message
				errorMessage.text = "User details not recognised, please try again";
				errorMessage.gameObject.SetActive(true);
			}
        }
    }

	public IEnumerator Register(string u, string p)
    {
		string URL = RegisterURL + "?Username=" + u + "&Password=" + p;
		WWW registerPage = new WWW(URL);
		yield return registerPage;

		if (registerPage.error != null)
		{
			Debug.Log("Error connecting to Database server");
			errorMessage.text = "Error connecting to Database server";
			errorMessage.gameObject.SetActive(true);
		}
		else
        {
			if (registerPage.text != "Failed")
            {
				loggedIn = true;
				userid = registerPage.text;
				username = u;

				errorMessage.gameObject.SetActive(false);
			}
			else
            {
				errorMessage.text = "Username already in use, please use an alternative";
				errorMessage.gameObject.SetActive(true);
			}
        }
	}

	public IEnumerator GetScores()
    {
		string url = ScoresURL + "?id=" + userid;
		WWW scorePage = new WWW(url);
		yield return scorePage;

		Debug.Log(scorePage.text);

		if (scorePage.error != null)
		{
			Debug.Log("Error connecting to Database server");
			errorMessage.text = "Error connecting to Database server";
			errorMessage.gameObject.SetActive(true);
		}
		else
        {
			string[] scores = scorePage.text.Split(' ');
			List<float> times = new List<float>();

			foreach (string score in scores)
            {
				if (score != "")
                {
					Debug.Log(score);
					times.Add(float.Parse(score));
				}
            }

			//order array
			times.Sort();

			playerScores = GameObject.FindGameObjectWithTag("Scores").GetComponent<PlayerScores>();

			foreach (float time in times)
				playerScores.CreateScore(time);
        }
	}

	public void AddTime(float time)
	{
		string URL = AddURL + "?id=" + userid + "&time=" + time;
		WWW loginPage = new WWW(URL);
	}

	public void SignOut()
    {
		loggedIn = false;
		userid = null;
		username = null;
    }
}
