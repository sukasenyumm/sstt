using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Genome{

    private const int numLength = 6;
    //definisi pose ID sebelumnya
    //private string poseId;
    //public string PoseId
    //{
    //    get { return poseId; }
    //}
    //definisi gen posisi dan rotasi
    public float[] genes;
    public float[] Genes()
    {
        return genes;
    }

    //definisi gen binary
    public bool[] binaryGenes;
    public bool[] BinaryGenes()
    {
        return binaryGenes; 
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

    private PoseCamera[] poseRanges;
    public PoseCamera[] PoseRanges
    {
        get
        {
            return poseRanges;
        }
        set
        {
            poseRanges = value;
        }
    }

    private float duration = 0f;
    public float Duration
    {
        get
        {
            return duration;
        }
        set
        {
            duration = value;
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

    private static float mutationRate;
    public static float MutationRate
    {
        get
        {
            return mutationRate;
        }
        set
        {
            mutationRate = value;
        }
    }

    public Genome(string idPose)
    {
        CreateGenes(idPose);
        CreateBinaryGenes();
        ComputeObjectiveFunction();
    }

    public Genome(string idPose, bool createGenes)
    {
        //poseId = idPose;
        genes = new float[6];
        binaryGenes = new bool[6];
        LockPose.identity = idPose;
        if (createGenes)
        {
            CreateGenes(idPose);
            CreateBinaryGenes();
            ComputeObjectiveFunction();
        }
    }

    //Mulai buat Gen/Kromosom
    private void CreateGenes(string initialId)
    {
        PoseCamera[] poseRange = ChooseRule(initialId);
        poseRanges = poseRange;
        Vector3 hasilPosGenerated = GeneratePosition(poseRange); //new Vector3(Random.Range(-5f, 5), Random.Range(-5f, 5), Random.Range(-5f, 5));//GeneratePosition(poseRange);
        Vector3 hasilRotGenerated = GenerateRotation(poseRange); //new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)); //GenerateRotation(poseRange);
        genes = new float[numLength]{hasilPosGenerated.x, hasilPosGenerated.y,hasilPosGenerated.z,
                hasilRotGenerated.x, hasilRotGenerated.y,hasilRotGenerated.z,
                };

        int index = poseRange.Length - 1;
        lockPose = poseRange[index];
        duration = Random.Range(poseRange[index].minduration, poseRange[index].maxduration);
        //GetLockPoseId(poseRange);
        //poseId = GetLockPoseId(initialId, poseRange);
        //poseId = lockedId;
       
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
        currId = Regex.Replace(currId, @"[\d-]", string.Empty);
        PoseCamera[] result = DataCamera.poseRangeFromCUF;
        //result.RandomShuffle();
        switch (currId)
        {
            case "CUF": result = DataCamera.poseRangeFromCUF;
                break;
            case "MSF": result = DataCamera.poseRangeFromMSF;
                break;
            case "MSHAL": result = DataCamera.poseRangeFromMSHAL;
                break;
            case "LSHAF": result = DataCamera.poseRangeFromLSHAF;
                break;
            case "MSHAF": result = DataCamera.poseRangeFromMSHAF;
                break;
            case "MSLAF": result = DataCamera.poseRangeFromMSLAF;
                break;
        }

        result.RandomShuffle();
        return result;

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
    //Crossover
    public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
    {
        int pos = (int)(Random.value * 6);
        child1 = new Genome(genome2.LockPose.identity, false);
        child2 = new Genome(genome2.LockPose.identity, false);
        child1.Duration = genome2.Duration;
        child2.Duration = genome2.Duration;
        child1.PoseRanges = genome2.PoseRanges;
        child2.PoseRanges = genome2.PoseRanges;

        for (int i = 0; i < 6; i++)
        {
            if (i < pos)
            {
                child1.genes[i] = genes[i];
                child2.genes[i] = genome2.genes[i];
            }
            else
            {
                child1.genes[i] = genome2.genes[i];
                child2.genes[i] = genes[i];
            }
        }

        
    }
    //Mutasi
    public void Mutate(int populationSize)
    {
        for (int pos = 0; pos < 6; pos++)
        {
            if (Random.value < mutationRate)
            {
                if(pos > 2 && (genes[pos] > 0.1f && genes[pos] < 0.2f))
                genes[pos] = genes[pos] + (Random.Range(0f,360f) / ((float)populationSize * mutationRate * 0.1f));
                else if(pos < 2 && (genes[pos] > 0.1f && genes[pos] < 0.2f))
                genes[pos] = genes[pos] + (Random.value / ((float)populationSize * mutationRate * 0.1f));
                else
                {
                    //do nothing
                }
            }
               
        }
    }
    // Dapatkan hasil gen
    public void GetValues(ref float[] values)
    {
        for (int i = 0; i < 6; i++)
            values[i] = genes[i];
    }

}
