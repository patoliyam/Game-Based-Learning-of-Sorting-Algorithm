
/*
 * Name of module       : SwapCubesBubbleSort
 *Functions             :               
 *                        1. Swap()
 *                           Input  : Simultaneous touch on two cubes
 *                           Output : Drive 3 coroutines Movet1_t2, Movel1_l2, Movet3_t4 
 *                        2. ValidateSwap()
 *                           Input  : Index of 2 cubes touched
 *                           Output : Tells cubes touched are correct or not
 *                        3. UpdateScore()        
 *                           Input  : Validity of Move happened
 *                           Output : Updates Score 
 *                        4. ReverseSwapping()  
 *                           Input  : Validity of Move happened
 *                           Output : Triggers reverse swapping in case of incorrect move. Drives it by 3 coroutines : Movet1_t2_r, Movel1_l2_r, Movet3_t4_r 
 *                        5. GetIndices() 
 *                           Input  : Touch on cubes
 *                           Output : Returns list of indices touched     
 *                        6. EnableTouch()
 *                           Input  : None
 *                           Output : Enables detections of touch on cubes
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwapCubesSelectionSort : MonoBehaviour
{

    LayerMask TouchInput;                               //Only objects of layer TouchInput can be detected by raycaster
    RaycastHit hit;
    GameObject[] cubeList = new GameObject[8];          //Stores all the cube gameobjects in array
    List<GameObject> touchList = new List<GameObject>();//stores indices of cubes touched at current frame
    GameObject[] touchOld;                              //stores indices of cubes touched at previous frame
    List<Int32> index = new List<Int32>();
    GameObject GameController;                          //It will store game controller object
    bool valid;                                         //indicates validity of move made
    bool transferringCubes;                             //Indicates currently cubes are transfering or not



    //It is predefines function which runs on starting of the scene
    // Use this for initialization of variables
    void Start()
    {
        Debug.Log("GameController Initiatiation Started");
        GameController = GameObject.FindGameObjectWithTag("GameController");
        Debug.Log("GameController Initiatiation Ended");
        Debug.Log("Gameobject array formation started");
        cubeList = GameObject.FindGameObjectsWithTag("Cube");
        Debug.Log("Gameobject array formation Ended");

        TouchInput = 1 << LayerMask.NameToLayer("TouchInput");
    }


    // Update is predefined function in unity which is called once per frame
    void Update()
    {
        int count = 0;
        foreach (GameObject cube in cubeList)
        {
            if(cube.layer == LayerMask.NameToLayer("TouchInput"))
            {count++;}
        }
        if(count == 8){
            transferringCubes = false;}
        else{
            transferringCubes = true;}

        //If number of simultaneous touches = 2 and cubes are not transferring
        if (Input.touchCount ==2 && transferringCubes == false)
        {
            //Copying the touchList to touchOld
            touchOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchOld);
            touchList.Clear();
            index.Clear();

            //Making list of cubes being touched and their indices
            foreach (Touch touch in Input.touches)
            {
               Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
               if (Physics.Raycast(ray, out hit,Mathf.Infinity, TouchInput))
                {
                    //reciepient is the gameobject being touched
                    GameObject reciepient = hit.transform.gameObject;
                    touchList.Add(reciepient);

                    //index will store the indices of cubes being touched
                    index.Add(Convert.ToInt32(reciepient.name));
                }
            }
            if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended)
            {
                //Swapping of cubes
                StartCoroutine(Swap(10, touchList[0], touchList[1]));
            }
        }
    }

    //Swap Coroutine swaps cubes go1 and go2 with given speed in following fashion
    /*
     *              l1
     *    ^ ------------------>
     *  t1|                   |t3
     *  go1===================go2
     *    ^                   |t2
     * t4 |<------------------
     *             l2    
     *             
     *  Where, l1 and l2 are lateral movement go1 and go2 respectively
     *         t1, t3 are transverse movement of go1
     *         t2, t4 are transverse movement of go2          
     */
    IEnumerator Swap(float speed, GameObject go1, GameObject go2)
    {
        //Disabling touch during movement of cubes
        foreach (GameObject cube in cubeList)
        {
            cube.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        //index Of Cubes Being Swapped are interchanged
        string temp = touchList[0].name;
        touchList[0].name = touchList[1].name;
        touchList[1].name = temp;

        yield return StartCoroutine(Movet1_t2(10, touchList[0], touchList[1])); //making moves t1 and t2
        yield return StartCoroutine(Movel1_l2(10, touchList[0], touchList[1])); //making moves l1 and l2
        yield return StartCoroutine(Movet3_t4(10, touchList[0], touchList[1])); //making moves t3 and t4

        Debug.Log("ValidSwap Started");
        yield return StartCoroutine(ValidateSwap()); //Validating swap performed
        Debug.Log("ValidateSwap Ended");
        Debug.Log("Valid : " + valid);
        Debug.Log("ScoreUpdate Started");
        yield return StartCoroutine(UpdateScore());  //Updating Score on the basis of validation and perform reverse swapping accordingly
        Debug.Log("ScoreUpdate Ended");
        //Enabling touch after movement of cubes
        yield return StartCoroutine(EnableTouch());  //Enables touch after whole process is complete

    }

    //Making transverse moves t1 and t2
    IEnumerator Movet1_t2(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f ||
                Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z - 3f)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z - 3f), speed * Time.deltaTime);
            yield return 0;
        }
    }

    //Making lateral moves l1 and l2
    IEnumerator Movel1_l2(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos2.x, tempPos1.y, tempPos1.z)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos2.x, tempPos1.y, tempPos1.z), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z), speed * Time.deltaTime);
            yield return 0;
        }
    }

    //Making transverse moves t3 and t4
    IEnumerator Movet3_t4(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z - 3f)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z - 3f), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f), speed * Time.deltaTime);
            yield return 0;
        }
    }

    //Validates swap just happened and update valid flag accordingly it also checks for game over and doesnt perform reverse swap in case of losing game
    IEnumerator ValidateSwap()
    {
        Debug.Log("Validation Started");
        valid = GameController.GetComponent<GameBrainSelectionSort>().ValidateMove(index);
       
        GameController.GetComponent<GameBrainSelectionSort>().GameOver();
        Debug.Log("Validation Ended");
        yield return 0;
    }

    //To update the score text and calls reverse swapping function in case of invalid swap
    IEnumerator UpdateScore()
    {
        Debug.Log("UpdateScore Started");
        GameController.GetComponent<GameBrainSelectionSort>().SetScoreText(valid); 
        if (!GameController.GetComponent<GameBrainSelectionSort>().lose)
        {
            if (!valid)
            {
                yield return StartCoroutine(ReverseSwapping(10, touchList[0], touchList[1]));
            }
            yield return 0;
        }
        Debug.Log("UpdateScore Ended");
    }

    //Coroutine reverseswaps cubes go1 and go2 with given speed  if swap happened is invalid in following fashion
    /*
     *              l1
     *     <------------------^
     *  t3|                   |t1
     *  go2===================go1
     *  t2|                   ^t4
     *     ------------------>|
     *             l2    
     *             
     *  Where, l1 and l2 are lateral movement go1 and go2 respectively
     *         t1, t3 are transverse movement of go1
     *         t2, t4 are transverse movement of go2          
     */
    IEnumerator ReverseSwapping(float speed, GameObject go1, GameObject go2)
    {
        if (valid == false)
        {
            //finding initial positions of cubes
            string temp = touchList[0].name;
            touchList[0].name = touchList[1].name;
            touchList[1].name = temp;
            yield return StartCoroutine(Movet1_t2_r(10, touchList[0], touchList[1])); //making moves t1 and t2
            yield return StartCoroutine(Movel1_l2_r(10, touchList[0], touchList[1])); //making moves l1 and l2
            yield return StartCoroutine(Movet3_t4_r(10, touchList[0], touchList[1])); //making moves t3 and t4
            yield return 0;
        }
    }

    //Making transverse moves t1 and t2
    IEnumerator Movet1_t2_r(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f ||
                Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z - 3f)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z - 3f), speed * Time.deltaTime);
            yield return 0;
        }
    }

    //Making lateral moves l1 and l2
    IEnumerator Movel1_l2_r(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos2.x, tempPos1.y, tempPos1.z)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos2.x, tempPos1.y, tempPos1.z), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z), speed * Time.deltaTime);
            yield return 0;
        }
    }

    //Making transverse moves t3 and t4
    IEnumerator Movet3_t4_r(float speed, GameObject go1, GameObject go2)
    {
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z - 3f)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
        {
            go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z - 3f), speed * Time.deltaTime);
            go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f), speed * Time.deltaTime);
            yield return 0;
        }
        yield return 0;
    }

    //The function returns the list of indices
    public List<int> GetIndices()
    {
        return index;
    }

    //The function makes touch enable when: game is neither won or lost, and currently cubes are not moving
    IEnumerator EnableTouch()
    {
        if (!(GameController.GetComponent<GameBrainSelectionSort>().won || GameController.GetComponent<GameBrainSelectionSort>().lose))
        {
            foreach (GameObject cube in cubeList)
            {
                cube.layer = LayerMask.NameToLayer("TouchInput");
            }
        }
        yield return 0;
    }
}


