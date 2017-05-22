/*
 * Name of module       : HelpMenuScript
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

public class HelpMenuScript : MonoBehaviour {

    public Text helpGame;               //Help to play game

    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start()
    {
        helpGame.text = "Sort Numbers in increasing order\nThe cubes on the screen are swappable. \nSwap the cubes correctly by simultaneously \ntouching them according to the algorithm. \nEvery correct swap adds a score of plus 10 \nand every wrong swap minus 10. \nFor three consecutive incorrect swaps, \nthe game will be lost. \nGame will be completed when array is sorted.";
    }

    //This function will load Main menu
    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
