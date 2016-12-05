using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SmartCamMain : MonoBehaviour
{

    private Animator anim;
    public Camera smartCam;

    public string initialId = "CU-F";
    private string counterId;
    public int poseTolerance = 2;
    public float duration = 2f;
    public string executionTime = "Initialize..";
    private float tempduration;

    public string fitnessFile = "";
    public int generations = 20;
    public int populationSize = 20; 
    public float mutationRate = 0.05f;
    public float crossoverRate = 0.8f;
    public bool elitism = false;

    private Vector3 hasilPosGenerated = Vector3.zero;
    private Vector3 hasilRotGenerated = Vector3.zero;
             
    void Start()
    {
        anim = GetComponent<Animator>();
        counterId = initialId;
        tempduration = duration;
        duration = tempduration;
        GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().text = "Id Pose : " + initialId + "\n"
                                                                                         + "Duration : " + duration + "\n"
                                                                                         + "Execution Time : -" + "\n"
                                                                                         + "Fitnes : -";
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.forward);
        float[] values;
        float fitness;
        string id;
        
        
        if (IsStandBy(anim))
        {
            duration -= Time.deltaTime;
            GameObject.FindGameObjectWithTag("text duration").GetComponent<Text>().text = "Current Duration : " + duration.ToString();
            
            if (duration <= 0)
            {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                GeneticAlgo GA = new GeneticAlgo(counterId);
                GA.Generations = generations;
                GA.MutationRate = mutationRate;
                GA.CrossoverRate = crossoverRate;
                GA.FitnessFile = @fitnessFile;
                GA.Elitism = elitism;
                GA.PopulationSize = populationSize; 
                GA.Compute();

                GA.GetBest(out values, out fitness, out id, out duration);

                while (fitness != 1.0f)
                {
                    GA.Compute();
                    GA.GetBest(out values, out fitness, out id, out duration);
                }
               
                hasilPosGenerated = new Vector3(values[0], values[1], values[2]);
                hasilRotGenerated = new Vector3(values[3], values[4], values[5]);
               
                initialId = id;

                stopWatch.Stop();
                System.TimeSpan ts = stopWatch.Elapsed;
                executionTime = System.String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

                GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().text = "Id Pose : " + initialId + "\n"
                                                                                         + "Duration : " + duration + "\n"
                                                                                         + "Execution Time : " + executionTime + "\n"
                                                                                         + "Fitness : "+fitness.ToString();
                counterId = initialId;
                
                if (ts.Milliseconds >= 50)
                //if (fitness < 1f)
                {
                    GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(255, 0, 0);
                    initialId = BreakRule(initialId);
                }
                else
                    GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(0, 253, 243);

                smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
              //  smartCam.transform.position = new Vector3(hasilPosGenerated.x + (hasilPosGenerated.z * transform.forward.x), hasilPosGenerated.y, hasilPosGenerated.z * transform.forward.z) + transform.position;
                smartCam.transform.position = transform.position + (transform.rotation * hasilPosGenerated); 
                

            }
           
        }
        else
        {
            duration = -1f;
            //smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
            //smartCam.transform.position = transform.position + (transform.rotation * hasilPosGenerated); 
        }

       // Debug.Log(transform.forward);

    }

    bool IsStandBy(Animator an)
    {
        bool res = false;
        if (an)
        {
            float fr = an.GetFloat("Forward");
            bool isGround = an.GetBool("OnGround");
            float tr = an.GetFloat("Turn");
            //Debug.Log(fr);

            if (fr > -0.01f && fr < 0.01f && isGround && tr > -0.01f && tr < 0.01f)
            {
                res = true;
                //Debug.Log("Idle");
            }
            else
            {
                res = false;
                //Debug.Log("Move");
            }
        }
        return res;
    }

    string BreakRule(string currId)
    {
        //currId = currId.Remove(currId.Length - 1);
        currId = Regex.Replace(currId, @"[\d-]", string.Empty);
        switch (currId)
        {
            case "CUF": return DataCamera.poseRangeFromCUF[0].identity;
            case "MSF": return DataCamera.poseRangeFromMSF[0].identity;
            case "MSHAL": return DataCamera.poseRangeFromMSHAL[0].identity;
            case "LSHAF": return DataCamera.poseRangeFromLSHAF[0].identity;
            case "MSHAF": return DataCamera.poseRangeFromMSHAF[0].identity;
            case "MSLAF": return DataCamera.poseRangeFromMSLAF[0].identity;
            default: return DataCamera.poseRangeFromCUF[0].identity;
        }
    }

}
