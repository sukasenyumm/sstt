using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class GeneticAlgo  {
   // public delegate float GAFunction(int[] values);
    private float totalFitness;
    
    static private float getFitness;
    private ArrayList thisGeneration;
    private ArrayList nextGeneration;
    //Definisi tabel fitness untuk seluruh gen
    private ArrayList fitnessTable;
 
    //Definisi pose ID
    private string poseId;
    public string PoseId
    {
        get
        {
            return poseId;
        }
        set
        {
            poseId = value;
        }
    }

    //Definisi populasi
    private int populationSize;
    public int PopulationSize
    {
        get
        {
            return populationSize;
        }
        set
        {
            populationSize = value;
        }
    }

    // definisi ukuran generasi
    private int generationSize;
    public int Generations
    {
        get
        {
            return generationSize;
        }
        set
        {
            generationSize = value;
        }
    }
    //definisi tingkat crossover
    private float crossoverRate;
    public float CrossoverRate
    {
        get
        {
            return crossoverRate;
        }
        set
        {
            crossoverRate = value;
        }
    }

    //definisi tingkat mutasi
    private float mutationRate;
    public float MutationRate
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

    //definisi keterangan tulisan fitness
    private string strFitness;
    public string FitnessFile
    {
        get
        {
            return strFitness;
        }
        set
        {
            strFitness = value;
        }
    }

    private bool elitism;
    public bool Elitism
    {
        get
        {
            return elitism;
        }
        set
        {
            elitism = value;
        }
    }

    public GeneticAlgo()
    {
        mutationRate = 0.05f;
        crossoverRate = 0.80f;
        populationSize = 100;
        generationSize = 1000;
        strFitness = "";
    }

    public GeneticAlgo(string Id)
    {
        mutationRate = 0.05f;
        crossoverRate = 0.80f;
        populationSize = 100;
        generationSize = 1000;
        poseId = Id;
        strFitness = "";
        elitism = true;
    }
    // Mulai algoritma Genetika
    public void Compute()
    {
       //int temp = 0;
        //  Create the fitness table.
        fitnessTable = new ArrayList();
        thisGeneration = new ArrayList(generationSize);
        nextGeneration = new ArrayList(generationSize);
        Genome.MutationRate = mutationRate;

        CreateGenomes(poseId);
        RankPopulation();

        StreamWriter outputFitness = null;
        bool write = false;
        if (strFitness != "")
        {
            write = true;
            outputFitness = new StreamWriter(strFitness);
        }

        for (int i = 0; i < generationSize; i++)
        {
            CreateNextGeneration();
            RankPopulation();
            if (write)
            {
                if (outputFitness != null)
                {
                    for (int j = 0; j < populationSize; j++)
                    {
                        float d = (float)((Genome)thisGeneration[j]).Fitness;
                        float[] kr = (float[])((Genome)thisGeneration[j]).Genes();
                        string pose = (string)((Genome)thisGeneration[j]).PoseId;
                        outputFitness.WriteLine("Generation: {0},Genome: {1},[x: {2},y: {3},z: {4},rx: {5},ry: {6},rz: {7}],Pose id: {8} Fitness: {9}"
                            , i,j,kr[0],kr[1],kr[2],kr[3],kr[4],kr[5],pose, d);
                    }
                }
            }
                
        }
        if (outputFitness != null)
            outputFitness.Close();

     
    }
    //Ciptakan Genome-genome
    private void CreateGenomes(string Id)
    {
        for (int i = 0; i < populationSize; i++)
        {
            Genome g = new Genome(Id);
            thisGeneration.Add(g);
        }
    }

    //Rangking populasi dan sorting berdasarkan fitness
    private void RankPopulation()
    {
        //Hitung total fitness setiap gen
        totalFitness = 0;
        for (int i = 0; i < populationSize; i++)
        {
            Genome g = ((Genome)thisGeneration[i]);
            totalFitness += g.Fitness;
        }
        thisGeneration.Sort(new GenomeComparer());

        //  sorting fitness setiap gen
        float fitness = 0f;
        fitnessTable.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            fitness += ((Genome)thisGeneration[i]).Fitness;
            fitnessTable.Add((float)fitness);
        }
 
    }

    //Buat Generasi selanjutnya
    private void CreateNextGeneration()
    {
        nextGeneration.Clear();
        Genome g = null;
        if (elitism)
            g = (Genome)thisGeneration[populationSize - 1];

        for (int i = 0; i < populationSize; i += 2)
        {
            int pidx1 = RouletteSelection();
            int pidx2 = RouletteSelection();
            Genome parent1, parent2, child1, child2;
            parent1 = ((Genome)thisGeneration[pidx1]);
            parent2 = ((Genome)thisGeneration[pidx2]);

            if (UnityEngine.Random.value < crossoverRate)
            {
                parent1.Crossover(ref parent2, out child1, out child2);
            }
            else
            {
                child1 = parent1;
                child2 = parent2;
            }
            child1.Mutate(populationSize);
            child2.Mutate(populationSize);

            // aktifkan kembali semua property yg dibutuhkan setelah di clear 
            for (int k = 0; k < child1.PoseRanges.Length; k++)
            {
                if (child1.PoseRanges[k].identity == child1.PoseId)
                {
                    child1.LockPose = child1.PoseRanges[k];
                }
            }
            for (int k = 0; k < child2.PoseRanges.Length; k++)
            {
                if (child2.PoseRanges[k].identity == child2.PoseId)
                {
                    child2.LockPose = child2.PoseRanges[k];
                }
            }

            float[] fixedPoseMin = new float[6] { child1.LockPose.minPosition.x, child1.LockPose.minPosition.y, child1.LockPose.minPosition.z,
                                            child1.LockPose.minRotation.x, child1.LockPose.minRotation.y, child1.LockPose.minRotation.z};

            float[] fixedPoseMax = new float[6] { child1.LockPose.maxPosition.x, child1.LockPose.maxPosition.y, child1.LockPose.maxPosition.z,
                                            child1.LockPose.maxRotation.x, child1.LockPose.maxRotation.y, child1.LockPose.maxRotation.z};

            for (int j = 0; j < 6; j++)
            {
                if (child1.genes[j] >= fixedPoseMin[j] && child1.genes[j] <= fixedPoseMax[j])
                {
                    child1.binaryGenes[j] = true;
                }
                else
                {
                    child1.binaryGenes[j] = false;
                }
            }

            float[] fixedPoseMin2 = new float[6] { child2.LockPose.minPosition.x, child2.LockPose.minPosition.y, child2.LockPose.minPosition.z,
                                            child2.LockPose.minRotation.x, child2.LockPose.minRotation.y, child2.LockPose.minRotation.z};

            float[] fixedPoseMax2 = new float[6] { child2.LockPose.maxPosition.x, child2.LockPose.maxPosition.y, child2.LockPose.maxPosition.z,
                                            child2.LockPose.maxRotation.x, child2.LockPose.maxRotation.y, child2.LockPose.maxRotation.z};
            for (int j = 0; j < 6; j++)
            {
                if (child2.genes[j] >= fixedPoseMin2[j] && child2.genes[j] <= fixedPoseMax2[j])
                {
                    child2.binaryGenes[j] = true;
                }
                else
                {
                    child2.binaryGenes[j] = false;
                }
            }

            //evaluasi kembali fungsi objektif
            child1.ComputeObjectiveFunction();
            child2.ComputeObjectiveFunction();

            nextGeneration.Add(child1);
            nextGeneration.Add(child2);
        }

        if (elitism && g != null)
            nextGeneration[0] = g;
        thisGeneration.Clear();
        for (int i = 0; i < populationSize; i++)
            thisGeneration.Add(nextGeneration[i]);
    }
    // Seleksi dengan mesin roullete
    private int RouletteSelection()
    {
        float randomFitness = UnityEngine.Random.value* totalFitness;
        int idx = -1;
        int mid;
        int first = 0;
        int last = populationSize - 1;
        mid = (last - first) / 2;

        //  ArrayList's BinarySearch is for exact values only
        //  so do this by hand.
        while (idx == -1 && first <= last)
        {
            if (randomFitness < (float)fitnessTable[mid])
            {
                last = mid;
            }
            else if (randomFitness > (float)fitnessTable[mid])
            {
                first = mid;
            }
            mid = (first + last) / 2;
            //  lies between i and i+1
            if ((last - first) == 1)
                idx = last;
        }
        return idx;
    }
    // Ambil Hasil
    public void GetBest(out float[] values, out float fitness, out string idPose, out float duration)
    {
        Genome g = ((Genome)thisGeneration[populationSize - 1]);
        values = new float[6];
        g.GetValues(ref values);
        fitness = g.Fitness;
        idPose = g.PoseId;
        duration = g.Duration;
    }
}
