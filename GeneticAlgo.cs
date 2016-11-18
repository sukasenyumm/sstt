using UnityEngine;
using System.Collections;
using System;

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
	
}
