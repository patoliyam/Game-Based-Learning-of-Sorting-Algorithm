/*
 * Name of module       : SwapCubesBubbleSort 
 *Functions             :               
 *                        1. Swap()
 *                           Input  : Simultaneous touch on two cubes
 *                           Output : Drive 3 coroutines TransverseMove1, LateralMove, TransverseMove2
 *                        2. ValidateSwap()
 *                           Input  : Index of 2 cubes touched
 *                           Output : Tells cubes touched are correct or not
 *                        3. UpdateScore()        
 *                           Input  : Validity of Move happened
 *                           Output : Updates Score 
 *                        4. ReverseSwapping()  
 *                           Input  : Validity of Move happened
 *                           Output : Triggers reverse swapping in case of incorrect move. Drives it by 3 coroutines : TransverseMove1_r, LateralMove_r, Transverse_r 
 *                        5. GetIndices() 
 *                           Input  : Touch on cubes
 *                           Output : Returns list of indices touched     
 *                        6. EnableTouch()
 *                           Input  : None
 *                           Output : Enables detections of touch on cubes
 *                           
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwapCubesInsertionSort : MonoBehaviour
{
    GameObject[] cubeList = new GameObject[8];          //Stores all the cube gameobjects in array
    LayerMask TouchInput;                               //Only objects of layer TouchInput can be detected by raycaster
    List<GameObject> touchList = new List<GameObject>();//stores indices of cubes touched at current frame
    GameObject[] touchOld;                              //stores indices of cubes touched at previous frame
    List<GameObject> shiftList = new List<GameObject>();//The cubes to be shifted in each correct moves
    RaycastHit hit;
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
            if (cube.layer == LayerMask.NameToLayer("TouchInput"))
            { count++; }
        }
        if (count == 8){
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
            shiftList.Clear();

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

            if (Input.touches[0].phase == TouchPhase.Ended|| Input.touches[1].phase==TouchPhase.Ended)
            {
                //Swapping of cubes
                StartCoroutine(Swap(10, touchList[0], touchList[1]));
            }
        }
    }

    //Coroutine to smoothly moves go2 to position of go1 and shift intermediate cubes by 1 unit
    /*
     *               l1
     *     <--------------------------^
     *  t2|                           |t1
     *  go1 =>=>=>=>=>=>=>=>=>=>=>=> go2
     *               l2    
     *             
     *  Where, l1 is lateral movement of go2 
     *         l2 is laterral movement of all objects between go1 and go2 including go1
     *         t1, t2 are transverse movement of go2
         
     */
    IEnumerator Swap(float speed, GameObject go1, GameObject go2)
    {
        //Disabling touch during movement of cubes
        Debug.Log("Disabling Touch started!");
        foreach (GameObject cube in cubeList)
        { 
            cube.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        Debug.Log("Disabling Touch ended!");

        //index Of Cubes Being Swapped are interchanged are sortd in touchList
        Debug.Log("go1 is the cube with lesser index ");
        if (Convert.ToInt32(go1.name)>Convert.ToInt32(go2.name))
        {
            GameObject go = go1;
            go1 = go2;
            go2 = go;
            //Making touchlist sorted by cube index
            touchList[0] = go1;
            touchList[1] = go2;
        }
        Debug.Log("go2 is the cube with higher index ");
        Vector3 tempPos1 = go1.transform.position;
        Vector3 tempPos2 = go2.transform.position;

        yield return StartCoroutine(TransverseMove1(speed, go1, go2, tempPos1, tempPos2)); //Perform t1 and 1/3rd of l2
        yield return StartCoroutine(LateralMove(speed, go1, go2, tempPos1, tempPos2)); //Preform l1 and 1/3rd of l2
        yield return StartCoroutine(TransverseMove2(speed, go1, go2, tempPos1, tempPos2)); //Perform t2 and 1/3rd of l2
        
        yield return StartCoroutine(ValidateSwap()); //Validate swap just performed
        yield return StartCoroutine(UpdateScoreInsertion()); //Update score based on validation step and triggers reverse movement in case of invalid move
        
        //Enabling touch after movement of cubes
        yield return StartCoroutine(EnableTouch());
    }

    //Perform transverse movement t1 and 1/3rd of lateral movement l2
    IEnumerator TransverseMove1(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 1f, tempPos1.y, tempPos1.z)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
        {
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
            {
                go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos2.x, tempPos2.y, tempPos2.z + 3f), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 1f, tempPos1.y, tempPos1.z)) > 0.01f)
            {
                int first = Convert.ToInt32(go1.name);
                int second = Convert.ToInt32(go2.name);
                for (int u = second - 1; u >= first; u--)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x + 1f - (3 * (second - u)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / 3);
                }
            }

            yield return 0;
        }
    }

    //Perform lateral movement l1 and 1/3rd of lateral movement l2
    IEnumerator LateralMove(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 2f, tempPos1.y, tempPos1.z)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
        {
            int first = Convert.ToInt32(go1.name);
            int second = Convert.ToInt32(go2.name);
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z + 3f)) > 0.01f)
            {
                go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos1.x, tempPos2.y, tempPos2.z + 3f), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 2f, tempPos1.y, tempPos1.z)) > 0.01f)
            {
                for (int u = second - 1; u >= first; u--)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x + 2f - (3 * (second - u)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / ((second - first) * 3));
                }
            }

            yield return 0;
        }
    }

    //Perform transverse movement t2 and 1/3rd of lateral movement l2
    IEnumerator TransverseMove2(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 3f, tempPos1.y, tempPos1.z)) > 0.01f ||
               Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z)) > 0.01f)
        {
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z)) > 0.01f)
            {
                go2.transform.position = Vector3.MoveTowards(go2.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x + 3f, tempPos1.y, tempPos1.z)) > 0.01f)
            {
                int first = Convert.ToInt32(go1.name);
                int second = Convert.ToInt32(go2.name);
                for (int u = second - 1; u >= first; u--)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x + 3f - (3 * (second - u)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / 3);
                }
            }

            yield return 0;
        }
    }

    //Perform validation of swap performed and determines is game is over or not
    IEnumerator ValidateSwap()
    {
        Debug.Log("Validation Started");
        valid = GameController.GetComponent<GameBrainInsertionSort>().ValidateMove(index);
        //Changing the names of the cubes, suitable after shifting of the cubes
        if (valid)
        {
           
            int i1 = Convert.ToInt32(touchList[0].name);
            int i2 = Convert.ToInt32(touchList[1].name);
            for (int u = i2 - 1; u >= i1; u--)
            {
                GameObject.Find(Convert.ToString(u)).name = Convert.ToString(u + 1);
            }
            touchList[1].name = Convert.ToString(i1);
        }
       
        GameController.GetComponent<GameBrainInsertionSort>().GameOver();
        Debug.Log("Validation Ended");
        yield return 0;
    }

    //To update the score text and calls reverse swapping function in case of invalid swap
    IEnumerator UpdateScoreInsertion()
    {
        Debug.Log("UpdateScore Started");
        GameController.GetComponent<GameBrainInsertionSort>().SetScoreText(valid);   //GameBrain.SetScoreText(this.index);

        if (!GameController.GetComponent<GameBrainInsertionSort>().lose)
        {
            if (!valid)
            {
               yield return StartCoroutine(ReverseSwapping(10, touchList[1], touchList[0]));
            }
            yield return 0;
        }
        Debug.Log("UpdateScore Ended");
    }

    //Performs reverse movement in case of invalid move
    //Coroutine to smoothly moves go2 to position of go1 and shift intermediate cubes by 1 unit
    /*
     *               l1
     *    ^--------------------------->
     *  t1|                           |t2
     *  go1 <=<=<=<=<=<=<=<=<=<=<=<= go2
     *               l2    
     *             
     *  Where, l1 is lateral movement of go1 
     *         l2 is laterral movement of all objects between go1 and go2 including go2
     *         t1, t2 are transverse movement of go2
         
     */
    IEnumerator ReverseSwapping(float speed, GameObject go1, GameObject go2)
    {
        if (valid == false)
        {
            Vector3 tempPos1 = go1.transform.position; //initial position of the cube go1
            Vector3 tempPos2 = go2.transform.position; // initial position of the cube go2
            Vector3 tempgo1destination = GameObject.Find(Convert.ToString(Convert.ToInt32(go1.name)-1)).transform.position; //destination of the cube go1
            int start = Convert.ToInt32(go2.name);      //index of the first cube to be shifted laterally
            int end = Convert.ToInt32(go1.name) - 1;    //index of last cube to be shifted
                                                        //Move go1 to position to go1-1 and move go2 to go1 -1 

            yield return StartCoroutine(TransverseMove1_r(speed, go1, go2, tempPos1, tempPos2, start, end, tempgo1destination));
            yield return StartCoroutine(LateralMove_r(speed, go1, go2, tempPos1, tempPos2, start, end, tempgo1destination));
            yield return StartCoroutine(TransverseMove2_r(speed, go1, go2, tempPos1, tempPos2, start, end, tempgo1destination));
        }
        yield return 0;
    }

    //Perform transverse movement t1 and 1/3rd of lateral movement l2
    IEnumerator TransverseMove1_r(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2, int start, int end, Vector3 tempgo1destination)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f || Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 1f, tempPos2.y, tempPos2.z)) > 0.01f)
        {
            if (Vector3.Distance(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f)
            {
                go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempPos1.x, tempPos1.y, tempPos1.z + 3f), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 1f, tempPos2.y, tempPos2.z)) > 0.01f)
            {
                for (int u = start; u <= end; u++)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x - 1f + (3 * (u - start)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / 3);
                }
            }
            yield return 0;
        }
    }

    //Perform lateral movement l1 and 1/3rd of lateral movement l2
    IEnumerator LateralMove_r(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2, int start, int end, Vector3 tempgo1destination)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempgo1destination.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f || Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 2f, tempPos2.y, tempPos2.z)) > 0.01f)
        {
            if (Vector3.Distance(go1.transform.position, new Vector3(tempgo1destination.x, tempPos1.y, tempPos1.z + 3f)) > 0.01f)
            {
                go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempgo1destination.x, tempPos1.y, tempPos1.z + 3f), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 2f, tempPos2.y, tempPos2.z)) > 0.01f)
            {
                for (int u = start; u <= end; u++)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x - 2f + (3 * (u - start)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / 3);
                }
            }
            yield return 0;
        }

    }

    //Perform transverse movement t2 and 1/3rd of lateral movement l2
    IEnumerator TransverseMove2_r(float speed, GameObject go1, GameObject go2, Vector3 tempPos1, Vector3 tempPos2, int start, int end, Vector3 tempgo1destination)
    {
        while (Vector3.Distance(go1.transform.position, new Vector3(tempgo1destination.x, tempgo1destination.y, tempgo1destination.z)) > 0.01f || Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 3f, tempPos2.y, tempPos2.z)) > 0.01f)
        {
            if (Vector3.Distance(go1.transform.position, new Vector3(tempgo1destination.x, tempgo1destination.y, tempgo1destination.z)) > 0.01f)
            {
                go1.transform.position = Vector3.MoveTowards(go1.transform.position, new Vector3(tempgo1destination.x, tempgo1destination.y, tempgo1destination.z), speed * Time.deltaTime);
            }
            if (Vector3.Distance(go2.transform.position, new Vector3(tempPos2.x - 3f, tempPos2.y, tempPos2.z)) > 0.01f)
            {
                for (int u = start; u <= end; u++)
                {
                    GameObject.Find(Convert.ToString(u)).transform.position = Vector3.MoveTowards(GameObject.Find(Convert.ToString(u)).transform.position, new Vector3(tempPos2.x - 3f + (3 * (u - start)), tempPos2.y, tempPos2.z), speed * Time.deltaTime / 3);
                }
            }
            yield return 0;

        }

    }

    //The function returns the list of indices
    public List<int> GetIndices()
    {
        return index;
    }

    //The function makes touch enable when: game is neither won or lost, and currently cubes are not moving
    IEnumerator EnableTouch()
    {
        if (!(GameController.GetComponent<GameBrainInsertionSort>().won || GameController.GetComponent<GameBrainInsertionSort>().lose))
        {
            foreach (GameObject cube in cubeList)
            {
                cube.layer = LayerMask.NameToLayer("TouchInput");
            }
        }
        yield return 0;
    }
}


