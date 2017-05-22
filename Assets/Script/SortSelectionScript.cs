/*
 * Name of module       : SortSelectionScript 
 * functions            :
 *                       1. Start()
 *                          Input  : Starting of scene
 *                          Output : Variables are initiated
 *                       2. LoadMenu()
 *                          Input  : Press on back button
 *                          Output : Loading main menu scene
 *                       3. LoadBubble()
 *                          Input  : Press on 'bubble sort' button
 *                          Output : Looading Bubble Sort scene
 *                       4. LoadSelection()
 *                          Input  : Press on 'selection sort' button
 *                          Output : Looading Selection Sort scene
 *                       5. LoadInsertion()   
 *                          Input  : Press on 'insertion sort' button   
 *                          Output : Looading Insertion Sort scene   
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class SortSelectionScript : MonoBehaviour
{
    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start()
    {
        //It will make screen orientation horizontal
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    //Now we will write functions which will be called by buttons
    //This function will be executed when bubble sort button will be pressed and it will lead to bubble sort
    public void LoadBubble()
    {
        SceneManager.LoadScene(2);
    }

    //This function will be executed when selection sort button will be pressed and it will lead to selection sort
    public void LoadSelection()
    {
        SceneManager.LoadScene(3);
    }

    //This function will be executed when insertion sort button will be pressed and it will lead to insertion sort
    public void LoadInsertion()
    {
        SceneManager.LoadScene(4);
    }

    //This function will be executes when back button will be pressed and it will lead to main menu
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }


}
