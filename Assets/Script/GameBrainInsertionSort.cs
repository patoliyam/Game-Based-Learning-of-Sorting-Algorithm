/*
 * Name of module       : GameBrainInsertionSort 
 * Functions            :
 *                      1. Generate_Cubes()
 *                          Input : None
 *                          Output : Generates 8 prefabs(cubes) with random numbers on them 
 *                      2. FindValidMoveList()
 *                          Input : Random numbers generated on cubes
 *                          Output : Correct sequence of the pair of cubes to be touched
 *                      3. SetScoreText()
 *                          Input : score at given point of time
 *                          Output : Shows score as GUI text element 
 *                      4. ValidateMove()
 *                          Input : Indices of pair of cubes touched
 *                          Output : Tells that move is correct or not
 *                      5. GameOver()
 *                          Input : Two boolean parameters won and lose
 *                          Output : Sets gameover text depanding in values of those boolean parameters
 *                      6.HelpToggle()
 *                          Input : Touch on help button
 *                          Output : Toggles the status of help shown
 *                      7. Start()
 *                          Input : 
 *                          Output : It initiate the variables in scripts
 */

using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameBrainInsertionSort : MonoBehaviour
{
    GameObject prefab;                              //Prefab of tiled_cube 
    GameObject go;                                  //Dummy game object variable 
    int score;                                      //Score till now
    List<GameObject> cubes = new List<GameObject>();//Stores cube gameobjects
    List<Move> moveList = new List<Move>();         //Stores each move as move structure
    List<int> Number = new List<int>();             //Stores numbers on the cubes
    int iterator;                                   //Iterator of movelist
    int wrongCount;                                 //Indicates consecutive wronge moves
    public bool won;                                //Indicates game is won or not
    public bool lose;                               //Indicates game is lost or not
    public Text scoreText;                          //Score Text display element  
    public Text gameOverText;                       //Text diplayed if game is finished
    public Text helpText;                           //Help (Algorithm) displayed will be written in this text


    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start()
    {
        //To set highscore to 0, if it was not defined already 
        if (!PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.SetInt("highscore", 0);
        }
        
        //Text displayed on pressing help button
        helpText.text = "InsertionSort(Number[ ], N){\n\tfor i = 2 to N\n\t\tfor j = i-1 to 1\n\t\t\tif Number[j] < Number[i]\n\t\t\t\tShift Numbers j+1 to i-1 one unit forward and Shift\n\t\t\t\t original Number i to j+1\n\t\t\t\tTouch the cubes i and j+1 simultaneously for a valid swap\n\t\t\t\tBreak the loop\n}\n";
        //Initially hiding help text
        helpText.enabled = false; 

        //To set Screen landscap by default
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        //score will ba updated dynamically
        score = 0;

        //Initializing iterator to zero
        iterator = 0;

        //Initializing won, lose and restartGame variables
        won = false;
        lose = false;
        PlayerPrefs.SetInt("won", 0);
        PlayerPrefs.SetInt("lose", 0);

        //Initializing GameOverText
        gameOverText.text = "";

        //It will accumulate consecutive wrong moves
        wrongCount = 0;

        //Initializing text element of Scoreboard
        scoreText.text = "Score : " + 0;

        //Generate 8 cubes
        Generate_cubes();

        //Find valid move list for generated random numberrs on cubes
        FindValidMoveList();

    }

    //Generating 8 prefabs(cubes) with random numbers on them
    public void Generate_cubes()
    {
        Number.Add(0); //Making 0th element = 0
        System.Random rnd = new System.Random();
        prefab = Resources.Load("Tiled_Cube") as GameObject;
        for (int i = 1; i <= 8; i++)
        {
            go = Instantiate(prefab);
            Debug.Log("Cube No. " + i + " initiated");
            go.name = Convert.ToString(i);
            go.tag = "Cube";
            go.transform.position = new Vector3(3f * i - 14f, 1, 0);
            go.layer = LayerMask.NameToLayer("TouchInput");
            int temp = rnd.Next(10, 100);
            string Num = Convert.ToString(temp);
            Number.Add(temp);
            go.GetComponentInChildren<TextMesh>().text = Num;
            cubes.Add(go);
        }
    }

    //Defining valid moves list as correct sequence of cubes to be touches determined by actually running the algorithm
    public void FindValidMoveList()
    {
        for (int i = 2; i <= 8; i++)
        {
            int key = Number[i];
            int j = i - 1;
            while (j >= 1 && Number[j] > key)
            {
                Number[j + 1] = Number[j];
                j--;
            }
            Number[j + 1] = key;
            if (i != j + 1)
            {
                moveList.Add(new Move() { first = j + 1, second = i });
            }
        }
    }



    //It takes validity of move as input and updates score in the game, valid move => +10, not valid => -10
    public void SetScoreText(bool valid)
    {
        if (valid)
        {
            score += 10;
        }
        else
        {
            score -= 10;
        }
        scoreText.text = "Score : " + score.ToString();
    }

    //Function takes indices of cubes touched and validates the move made by player 
    //Checks game is over or not
    //Updates highscore in case of winning of game 
    public bool ValidateMove(List<Int32> index)
    {
        if ((index[0] == moveList[iterator].first && index[1] == moveList[iterator].second) || (index[1] == moveList[iterator].first && index[0] == moveList[iterator].second)) //Move is valid
        {
            iterator++;
            wrongCount = 0;

            if (iterator >= moveList.Count) //Game is won
            {
                won = true;
                PlayerPrefs.SetInt("won", 1);
                if (PlayerPrefs.GetInt("highscore") < score + 10) //Record is broken
                {
                    PlayerPrefs.SetInt("highscore", score + 10); //Highscore is updated
                }
            }
            return true;
        }
        else //Move is not valid
        {
            wrongCount++;
            if (wrongCount >= 3) //Game is lost
            {
                lose = true;
                PlayerPrefs.SetInt("lose", 1);
            }
            return false;
        }
    }

    //Displays the "win" or "lose" text on ending the game
    public void GameOver()
    {
        if (won)
        {
            gameOverText.text = "You Won!";
        }
        if (lose)
        {
            gameOverText.text = "You Lost!";
        }
    }

    //Displays and hides the help
    public void HelpToggle()
    {
        helpText.enabled = !helpText.enabled;
    }
}
