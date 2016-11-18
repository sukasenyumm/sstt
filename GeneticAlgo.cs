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
    public GeneticAlgo()
    {
        mutationRate = 0.05f;
        crossoverRate = 0.80f;
        populationSize = 100;
        generationSize = 2000;
        strFitness = "";
    }

    public GeneticAlgo(string Id)
    {
        mutationRate = 0.05f;
        crossoverRate = 0.80f;
        populationSize = 100;
        generationSize = 2000;
        poseId = Id;
        strFitness = "";
    }
    // Mulai algoritma Genetika
    public void Compute()
    {
       
        //  Create the fitness table.
        fitnessTable = new ArrayList();
        thisGeneration = new ArrayList(generationSize);
        nextGeneration = new ArrayList(generationSize);
        //Genome.MutationRate = m_mutationRate;

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
                    double d = (double)((Genome)thisGeneration[populationSize - 1]).Fitness;
                    outputFitness.WriteLine("{0},{1}", i, d);
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
            g.ComputeObjectiveFunction();
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
        double fitness = 0.0;
        fitnessTable.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            fitness += ((Genome)thisGeneration[i]).Fitness;
            fitnessTable.Add((double)fitness);
        }
 
    }

    private void CreateNextGeneration()
    {
        nextGeneration.Clear();
        Genome g = null;

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
            //child1.Mutate();
            //child2.Mutate();

            nextGeneration.Add(child1);
            nextGeneration.Add(child2);
        }

        thisGeneration.Clear();
        for (int i = 0; i < populationSize; i++)
            thisGeneration.Add(nextGeneration[i]);
    }

    private int RouletteSelection()
    {
        double randomFitness = UnityEngine.Random.value* totalFitness;
        int idx = -1;
        int mid;
        int first = 0;
        int last = populationSize - 1;
        mid = (last - first) / 2;

        //  ArrayList's BinarySearch is for exact values only
        //  so do this by hand.
        while (idx == -1 && first <= last)
        {
            if (randomFitness < (double)fitnessTable[mid])
            {
                last = mid;
            }
            else if (randomFitness > (double)fitnessTable[mid])
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

    public void GetBest(out float[] values, out float fitness, out string idPose)
    {
        Genome g = ((Genome)thisGeneration[populationSize - 1]);
        values = new float[6];
        g.GetValues(ref values);
        fitness = g.Fitness;
        idPose = g.PoseId;
    }
}
