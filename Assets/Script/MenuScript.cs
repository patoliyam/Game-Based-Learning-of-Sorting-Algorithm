/*
 * Name of module       : MenuScript 
 * functions            :
 *                       1. Start()
 *                          Input  : Starting of scene
 *                          Output : Variables are initiated
 *                       2. Awake()
 *                           Input : Starting of the load game
 *                           Outut : Music will remain off in the starting of the scene
 *                       3. MusicToggle()
 *                           Input :  music toggle button is pressed
 *                           Output: Music is toggled
 *                       4. ExitPress()   
 *                           Input : Quit button is pressed
 *                           Output: Pop up is shown for confirmation      
 *                       5. NoPress()
 *                           Input : No button is pressed in Quit confirmation dialoag 
 *                           Output: Quit pop up disappears        
 *                       6. LoadHelp()
 *                           Input : Help button is pressed
 *                           Output: Help scene is loaded
 *                       7. StartLevel()      
 *                           Input : Play button is pressed
 *                           Output:
 *                       8. ExitGame()
 *                           Input : Yes Button is pressed 
 *                           Output: Game is exited          
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public Canvas quitMenu;
    public Button startText;            //For play button
    public Button exitText;
    public static bool music;           //Indicates Music on or off
    public AudioSource musicClip;
    public GameObject musicSource ;
    public GameObject musicButton;
    public Text musicText;
    public GameObject musicStatusObject;
    //public Button musicText; //Music Text button

    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start ()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //we got our components
        quitMenu = quitMenu.GetComponent<Canvas>();
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
        musicButton = GameObject.FindGameObjectWithTag("AudioButton");
        quitMenu.enabled = false;
        musicSource = GameObject.FindGameObjectWithTag("AudioClip1");
        musicClip = musicSource.GetComponent<AudioSource>();
        musicText = musicButton.GetComponent<Text>();//Text Written on button
        musicStatusObject = GameObject.Find("MusicStatus");
        music = musicStatusObject.GetComponent<MusicStatusScript>().GetStatus();

        if (music)
        {
            musicText.text = "Turn Off Music";
        }
        else
        {
            musicText.text = "Turn On Music";
        }

    }

    //Called when the script instance is being loaded
    void Awake()
    {
        musicSource = GameObject.FindGameObjectWithTag("AudioClip1");
        musicClip = musicSource.GetComponent<AudioSource>();
        musicStatusObject = GameObject.Find("MusicStatus");
        if (PlayerPrefs.GetInt("music")==1)
        {
            if (!musicClip.isPlaying)
            {
                musicClip.Play();
            }
            DontDestroyOnLoad(musicSource);
            musicStatusObject.GetComponent<MusicStatusScript>().SetStatus(true);
     
        }
        else
        {
            musicClip.Pause();
            DontDestroyOnLoad(musicSource);
            musicStatusObject.GetComponent<MusicStatusScript>().SetStatus(false);
        }

    }

    //this function will be executed to toggle music status on pressing button
    public void MusicToggle()
    {
        if (musicStatusObject.GetComponent<MusicStatusScript>().GetStatus())
        {
            musicClip.Pause();
            musicStatusObject.GetComponent<MusicStatusScript>().SetStatus(false);
            musicText.text = "Turn On Music";
        }
        else
        {
            musicClip.Play();
            musicStatusObject.GetComponent<MusicStatusScript>().SetStatus(true);
            musicText.text = "Turn Off Music";
        }
    }

    //Now we will write functions which will be called by buttons
    //this function will be executed when quit button will be pressed in main menu
    public void ExitPress()
    {
        quitMenu.enabled = true;
        startText.enabled = false;
        exitText.enabled = false;
    }

    //Opposite of exit press, will be called by no button of the quit menu
    public void NoPress()
    {
        quitMenu.enabled = false;
        startText.enabled = true;
        exitText.enabled = true;
    }

    //This function will be executed when play button will be pressed
    public void StartLevel()
    {
        SceneManager.LoadScene(1);
    }

    //This function will load the eneral purpose help
    public void LoadHelp()
    {
        SceneManager.LoadScene(5);
    }

    //This function will be executed when we will press 'yes' in quit confirmation display
    public void ExitGame()
    {
        Application.Quit();
    }

}