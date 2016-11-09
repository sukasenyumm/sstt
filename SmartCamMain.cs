using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using System.Collections.Generic;
using UnityEngine.UI;

public class SmartCamMain : MonoBehaviour
{

    private Animator anim;
    public Camera smartCam;

    public string initialId = "CU-D-AE";
    private int counterPain = 0;
    private string counterId;
    public int poseTolerance = 2;
    public float duration = 2f;
    public string executionTime = "Initialize..";
    private float tempduration;
    //TruePoseCamera fixPose;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        counterId = initialId;
        //fixPose = FindTruePose(initialId);
        tempduration = duration;
        duration = tempduration;
        GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().text = "Id Pose : " + initialId + "\n"
                                                                                         + "Duration : " + duration + "\n"
                                                                                         + "Execution Time : -";
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStandBy(anim))
        {
            duration -= Time.deltaTime;
            GameObject.FindGameObjectWithTag("text duration").GetComponent<Text>().text = "Current Duration : " + duration.ToString();
            
            if (duration <= 0)
            {
                //Debug.Log(transform.rotation);
                //TEMPORARY Bugfix FORCE rotation to {0,1,0,1}
                //if (transform.rotation.x != 0.1f || transform.rotation.z != 0.1f)
                //{
                //    transform.rotation = Quaternion.Euler(0f, 1f, 0f);
                //}

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                PoseCamera[] poseRange = ChooseRule(initialId);


                Vector3 hasilPosGenerated = GeneratePosition(poseRange);
                Vector3 hasilRotGenerated = GenerateRotation(poseRange);

                initialId = GetNamePose(initialId, poseRange, hasilPosGenerated, hasilRotGenerated);
                //with find true pose
                //fixPose = FindTruePose(initialId);
                //initialId = fixPose.identity;

                //Debug.Log(initialId);
                //duration = fixPose.duration;
                //Debug.Log("Duration" + duration);
                duration = tempduration;

                stopWatch.Stop();
                System.TimeSpan ts = stopWatch.Elapsed;
                executionTime = System.String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

                GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().text = "Id Pose : " + initialId + "\n"
                                                                                         + "Duration : " + duration + "\n"
                                                                                         + "Execution Time : " + executionTime;
                // Indicator same pose more than 3 times.
                if (counterId == initialId)
                {
                    counterPain += 1;
                }
                else
                {
                    counterPain = 0;
                    counterId = initialId;
                }
                //Debug.Log(counterPain.ToString());
                if (counterPain >= poseTolerance)
                {
                    GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(255, 0, 0);
                    initialId = BreakRule(initialId);
                }
                else
                    GameObject.FindGameObjectWithTag("text pose").GetComponent<Text>().color = new Color(0, 253, 243);

                smartCam.transform.rotation = Quaternion.Euler(hasilRotGenerated + transform.rotation.eulerAngles);
                smartCam.transform.position = new Vector3(hasilPosGenerated.z * transform.forward.x, hasilPosGenerated.y, hasilPosGenerated.z * transform.forward.z) + transform.position;
            
            }
           
        }
        else
        {
            duration = -1f;
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

    PoseCamera[] ChooseRule(string currId)
    {
        switch (currId)
        {
            case "CU-D-AE": return DataCamera.poseRangeFromCUDAE;
            case "MS-D-HA": return DataCamera.poseRangeFromMSDHA;
            case "MS-D": return DataCamera.poseRangeFromMSD;
            case "LS-D-HA": return DataCamera.poseRangeFromLSDHA;
            default: return DataCamera.poseRangeFromCUDAE;
        }

    }
    string GetNamePose(string nameId, PoseCamera[] pose, Vector3 generatedPosValue, Vector3 generatedRotValue)
    {
        string result = nameId;
        bool trigger = false;
        for (int i = 0; i < pose.Length; i++)
        {
            if (trigger == false)
            {
                trigger = IsValidRule(nameId, pose[i], generatedPosValue, generatedRotValue);
                if (trigger == true)
                {
                    result = pose[i].identity;
                    tempduration = pose[i].duration;
                }
                trigger = true;
            }
        }

        return result;
    }
    bool IsValidRule(string currentId, PoseCamera target, Vector3 generatedPosValue, Vector3 generatedRotValue)
    {
        bool result = false;
        if (currentId == target.identity)
        {
            result = false;
        }
        else
        {
            if (((generatedPosValue.x >= target.minPosition.x) && (generatedPosValue.x <= target.maxPosition.x) &&
                (generatedPosValue.y >= target.minPosition.y) && (generatedPosValue.y <= target.maxPosition.y) &&
                (generatedPosValue.z >= target.minPosition.z) && (generatedPosValue.z <= target.maxPosition.z))
                &&
                ((generatedRotValue.x >= target.minRotation.x) && (generatedRotValue.x <= target.maxRotation.x) &&
                (generatedRotValue.y >= target.minRotation.y) && (generatedRotValue.y <= target.maxRotation.y) &&
                (generatedRotValue.z >= target.minRotation.z) && (generatedRotValue.z <= target.maxRotation.z)))
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }

        return result;
    }

    Vector3 GeneratePosition(PoseCamera[] poseRange)
    {
        Vector3 hasilMin = Vector3.zero;
        Vector3 hasilMax = Vector3.zero;
        if (poseRange.Length != 1)
        {
            for (int i = 0; i < poseRange.Length; i++)
            {
                if ((i + 1 != poseRange.Length))
                {
                    hasilMin = Vector3.Min(poseRange[i].minPosition, poseRange[i + 1].minPosition);
                    hasilMax = Vector3.Max(poseRange[i].maxPosition, poseRange[i + 1].maxPosition);
                }
            }
        }
        else
        {
            hasilMin = poseRange[0].minPosition;
            hasilMax = poseRange[0].maxPosition;
        }

        float posX = Random.Range(hasilMin.x, hasilMax.x);
        float posY = Random.Range(hasilMin.y, hasilMax.y);
        float posZ = Random.Range(hasilMin.z, hasilMax.z);

        Vector3 result = new Vector3(posX, posY, posZ);

        //Debug.Log("Hasil Minimal Posisi = " + hasilMin);
        //Debug.Log("Hasil Minimal Posisi = " + hasilMax);
        //Debug.Log("Hasil Posisi Generated = " + result);
        return result;
    }

    Vector3 GenerateRotation(PoseCamera[] poseRange)
    {
        Vector3 hasilMin = Vector3.zero;
        Vector3 hasilMax = Vector3.zero;
        if (poseRange.Length != 1)
        {
            for (int i = 0; i < poseRange.Length; i++)
            {
                if ((i + 1 != poseRange.Length))
                {
                    hasilMin = Vector3.Min(poseRange[i].minRotation, poseRange[i + 1].minRotation);
                    hasilMax = Vector3.Max(poseRange[i].maxRotation, poseRange[i + 1].maxRotation);
                }
            }
        }
        else
        {
            hasilMin = poseRange[0].minRotation;
            hasilMax = poseRange[0].maxRotation;
        }

        float posX = Random.Range(hasilMin.x, hasilMax.x);
        float posY = Random.Range(hasilMin.y, hasilMax.y);
        float posZ = Random.Range(hasilMin.z, hasilMax.z);

        Vector3 result = new Vector3(posX, posY, posZ);

        //Debug.Log("Hasil Minimal Rotasi = " + hasilMin);
        //Debug.Log("Hasil Minimal Rotasi = " + hasilMax);
        //Debug.Log("Hasil Rotasi Generated = " + result);
        return result;
    }

    string BreakRule(string currId)
    {
        switch (currId)
        {
            case "CU-D-AE": return DataCamera.poseRangeFromCUDAE[0].identity;
            case "MS-D-HA": return DataCamera.poseRangeFromMSDHA[0].identity;
            case "MS-D": return DataCamera.poseRangeFromMSD[0].identity;
            case "LS-D-HA": return DataCamera.poseRangeFromLSDHA[0].identity;
            default: return DataCamera.poseRangeFromCUDAE[0].identity;
        }
    }

    //TruePoseCamera FindTruePose(string currId)
    //{
    //    TruePoseCamera result = DataCamera.poseFix[0];
    //    for (int i = 0; i < DataCamera.poseFix.Length; i++)
    //    {
    //        if (currId == DataCamera.poseFix[i].identity)
    //        {
    //            result = DataCamera.poseFix[i];
    //            break;
    //        }
    //    }
    //    return result;
    //}

    //float FindDuration(string currId)
    //{
    //    float result = 0f;
    //    for (int i = 0; i < DataCamera.poseFix.Length; i++)
    //    {
    //        if (currId == DataCamera.poseFix[i].identity)
    //        {
    //            result = DataCamera.poseFix[i].duration;
    //            break;
    //        }
    //    }
    //    return result;
    //}
    void OnAnimatorMove()
    {

    }
}
