/*
 * Name of module       : MusicStatusScript 
 * functions            :
 *                       1. Start()
 *                          Input  : Starting of scene
 *                          Output : Variables are initiated
 *                       2. LoadMainMenu()
 *                          Input  : Press on help button
 *                          Output : Loading help scene
 *                       3. GetStatus()
 *                          Input  : None
 *                          Output : Status of music i.e. on/off
 *                       4. SetStatus()
 *                          Input  : Status
 *                          Output : Status is set
 *                       
 */

using UnityEngine;

public class MusicStatusScript : MonoBehaviour {
    
    public static bool musicStatus;
   
    // Use this for initialization
    void Awake () {
        DontDestroyOnLoad(this);
        musicStatus = (PlayerPrefs.GetInt("music") == 1);
           
    }

    //Get method
    public bool GetStatus()
    {
        return musicStatus;
    }

    //Set method
    public void SetStatus(bool status)
    {
        musicStatus=status;
        PlayerPrefs.SetInt("music", status ? 1 : 0);
    }

}
