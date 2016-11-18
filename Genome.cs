using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Genome{

    private const int numLength = 6;
    //definisi gen posisi dan rotasi
    private float[] genes;
    public float[] Genes
    {
        get { return genes; }
        
    }

    //definisi gen binary
    private bool[] binaryGenes;
    public bool[] BinaryGenes
    {
        get { return binaryGenes; }
    }
    //definisi kamera yang dikunci
    private PoseCamera lockPose = new PoseCamera();
    public PoseCamera LockPose
    {
        get
        {
            return lockPose;
        }
        set
        {
            lockPose = value;
        }
    }

    //definisi fungsi objektif
    private int objectiveResult;
    public int ObjectiveResult
    {
        get
        {
            return objectiveResult;
        }
        set
        {
            objectiveResult = value;
        }
    }
    private float fitness;
    public float Fitness
    {
        get
        {
            return fitness;
        }
    }
	public Genome()
    {

    }

    public Genome(string idPose)
    {
        CreateGenes(idPose);
    }
   
    //Mulai buat Gen/Kromosom
    private void CreateGenes(string initialId)
    {
        PoseCamera[] poseRange = ChooseRule(initialId);
    
        Vector3 hasilPosGenerated = GeneratePosition(poseRange);
        Vector3 hasilRotGenerated = GenerateRotation(poseRange);
        genes = new float[numLength]{hasilPosGenerated.x, hasilPosGenerated.y,hasilPosGenerated.z,
                hasilRotGenerated.x, hasilRotGenerated.y,hasilRotGenerated.z,
                };

        string lockedId = GetLockPoseId(initialId, poseRange);
        CreateBinaryGenes();
    }

    // Buat Gen berbentuk biner dari posisi yang sudah dilock terhadap posisi yang dipruning (a->b,c,d) <b,c,d adalah pilihan pruning>
    private void CreateBinaryGenes()
    {
        binaryGenes = new bool[genes.Length];
        float[] fixedPoseMin = new float[6] { lockPose.minPosition.x, lockPose.minPosition.y, lockPose.minPosition.z,
                                            lockPose.minRotation.x, lockPose.minRotation.y, lockPose.minRotation.z};

        float[] fixedPoseMax = new float[6] { lockPose.maxPosition.x, lockPose.maxPosition.y, lockPose.maxPosition.z,
                                            lockPose.maxRotation.x, lockPose.maxRotation.y, lockPose.maxRotation.z};

        for (int i = 0; i < genes.Length;i++ )
        {
            if (genes[i] >= fixedPoseMin[i] && genes[i] <= fixedPoseMax[i])
            {
                binaryGenes[i] = true;
            }
            else
            {
                binaryGenes[i] = false;
            }
        }
            
    }

    //Pilih aturan pruning
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

    //Generate posisi random berdasarkan min/max range dari pruning
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

    //Generate rotasi random berdasarkan min/max range dari pruning
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

    //Dapatkan pose yang nantinya akan diLock berdasarkan id Pose yang diberikan
    string GetLockPoseId(string nameId, PoseCamera[] pose)
    {
        int randomedLock = (int)Random.RandomRange(0f, (float)pose.Length);
        string result="";

        for (int i = 0; i < pose.Length; i++)
        {
            if (i == (randomedLock + 1))
            {
                lockPose = pose[i];
                result = pose[i].identity;
            }
        }

        return result;
    }

    //Hitung hasil fungsi objektif setiap gen
    public void ComputeObjectiveFunction()
    {
        int count = 0;
        for (int i = 0; i < binaryGenes.Length;i++ )
        {
            if (binaryGenes[i] == true)
                count++;
        }

        objectiveResult = 6 - count;
        fitness = 1f / (objectiveResult + 1f);
    }

}
