/*
 * Name of module       : ExitRestartScript 
 * functions            :
 *                       1. Start()
 *                          Input  : Starting of scene
 *                          Output : Variables are initiated
 *                       2. LoadMainMenu()
 *                          Input  : Press on help button
 *                          Output : Loading help scene
 *                       
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitRestartScript : MonoBehaviour {

    public Button restartText;                  //Restart button
    public Button exitText;                     //Exit button
    public static float timer;                  //timer
    public static bool timeStarted = false;
    public GUIStyle timerStyle;

    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start () {
        timer = 0;
        timeStarted = true;
        PlayerPrefs.SetFloat("time", 0);
        //Restart button
        Debug.Log("Getting buttons");
        restartText = restartText.GetComponent<Button>();

        //Exit button
        exitText = exitText.GetComponent<Button>();
        Debug.Log("Buttons got");
    }

    // Update is predefined function in unity which is called once per frame
    void Update()
    {
        if (timeStarted == true & PlayerPrefs.GetInt("won")!=1 & PlayerPrefs.GetInt("lose")!=1) //Stop timer in case of winning or losing of the game
        {
            timer += Time.deltaTime;
            PlayerPrefs.SetFloat("time", timer);
        }
    }

    //Executes wheen 'restart' button is pressed, It reloads the active scene
    void RestartLevel()
    {
        Debug.Log("Restarting Level");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Debug.Log("Restarting Level completed");
    }

    //Executes when 'back' button is pressed and on pressing it, game goes into sort selection menu
   void ExitLevel()
    {
        Debug.Log("Exiting Level");
        SceneManager.LoadScene(1);
        Debug.Log("Exiting Level completed");
    }

    //This function defines the timer of form mm:ss and highscore as GUI element
    void OnGUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        //Debug.Log (PlayerPrefs.GetInt("highscore"));
        GUI.Label(new Rect(100, Screen.height - 50, 50, 50), niceTime, timerStyle);
        GUI.Label(new Rect(Screen.width - 225, Screen.height - 50, 50, 50), "Highscore : "+ PlayerPrefs.GetInt("highscore").ToString(), timerStyle);
    }

    //This function stops timer when called
    public void StopTimer()
    {
        timeStarted = false;
    }
}