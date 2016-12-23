using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SmartCamMain : MonoBehaviour
{

    public Camera smartCam;

    private string initialId = "CU-F1";
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
    public bool isMoveCamera;
    public bool isFollowCamera;
    public Vector3 hasilPosGenerated;
    public Vector3 hasilRotGenerated;
    public float CameraDistance = 3f; // if datacamera.cs changed, replace 5f with larger than 5f..
    string temp = "";
    bool isZoom;
    

    void Start()
    {
        
        counterId = initialId;
        tempduration = duration;
        duration = tempduration;
        GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().text = "Press 'L' to hide camera information"+"\n"+
        "Press 'I' to display camera information" + "\n" +
        "Press 'M' to enable camera movement" + "\n" +
        "Press 'N' to disable camera movement";
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.forward);
        float[] values;
        float fitness;
        string id;

        KeyControl();

        duration -= Time.deltaTime;
        GameObject.FindGameObjectWithTag("text duration").GetComponent<Text>().text = "Elapsed Duration : " + duration.ToString() + "\n" +
        "Camera Movement Status : " + isMoveCamera.ToString() + "\n" +
        "Camera Follow Status : " + isFollowCamera.ToString() + "\n" +
        "Press 'L' to hide camera information" + "\n" +
        "Press 'I' to display camera information" + "\n";

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

            isMoveCamera = (Random.value > 0.5f) ? true : false;
            isFollowCamera = (Random.value > 0.5f) ? true : false;

            if (!isFollowCamera)
                isMoveCamera = false;

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
                                                                                     + "Fitness : " + fitness.ToString();
            counterId = initialId;

            if (ts.Milliseconds >= 50)
            {
                GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(255, 0, 0);
            }
            else
                GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(0, 253, 243);

            smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
            smartCam.transform.position = transform.position + (transform.rotation * hasilPosGenerated);
            if (isFollowCamera)
                smartCam.transform.parent = transform;
            else
                smartCam.transform.parent = null;
            temp = Regex.Replace(counterId, @"[\d-]", string.Empty);
            if (temp == "MSF")
                isZoom = Random.value > 0.5f ? true : false;
        }

        if (isMoveCamera)
        {
            CameraMovementPose();
        }

        var screenPoint = Vector3.Distance(smartCam.transform.position, transform.position);
        if (screenPoint > CameraDistance)
        {
            if (!isFollowCamera)
            {
                if (duration > 1f)
                {
                    smartCam.transform.LookAt(transform);
                    duration = duration - Time.deltaTime * 1f;
                }
            }
        }
    }
    void CameraMovementPose()
    {

        if (temp == "LSHAF")
        {
            //zoom in
            if (Vector3.Distance(smartCam.transform.position, transform.position) > 1.5f)
            {
                smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
                smartCam.transform.position = Vector3.MoveTowards(smartCam.transform.position, transform.position + new Vector3(0, 1.38f, 0), 0.3f * Time.deltaTime);
            }

        }
        else if (temp == "CUF")
        {
            //zoom out
            if (Vector3.Distance(smartCam.transform.position, transform.position) > 0.5f)
            {
                smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
                smartCam.transform.position = Vector3.MoveTowards(smartCam.transform.position, transform.position + new Vector3(0, 1.38f, 0), -0.3f * Time.deltaTime);
            }

        }
        else if (temp == "MSF")
        {
            if (isZoom)
            {
                //zoom in
                if (Vector3.Distance(smartCam.transform.position, transform.position) > 1.4f)
                {
                    smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
                    smartCam.transform.position = Vector3.MoveTowards(smartCam.transform.position, transform.position + new Vector3(0, 1.38f, 0), 0.3f * Time.deltaTime);
                }
            }
            else
            {
                //zoom out
                if (Vector3.Distance(smartCam.transform.position, transform.position) > 0.5f)
                {
                    smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
                    smartCam.transform.position = Vector3.MoveTowards(smartCam.transform.position, transform.position + new Vector3(0, 1.38f, 0), -0.3f * Time.deltaTime);
                }
            }

        }
        else if (temp == "MSL")
        {
            //left-to-right
            if (smartCam.transform.position.x <= 1f)
            {
                smartCam.transform.LookAt(transform.position + new Vector3(0, 1.31f, 0));
                smartCam.transform.RotateAround(transform.position, Vector3.up, 15f * Time.deltaTime);
            }

        }
        else if (temp == "MSR")
        {
            //right-to-left
            if (smartCam.transform.position.x >= -1f)
            {
                smartCam.transform.LookAt(transform.position + new Vector3(0, 1.31f, 0));
                smartCam.transform.RotateAround(transform.position, Vector3.down, 15f * Time.deltaTime);
            }

        }
    }

    void KeyControl()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("text duration").GetComponent<Text>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().enabled = false;
            GameObject.FindGameObjectWithTag("text duration").GetComponent<Text>().enabled = false;
        }
    }

}
