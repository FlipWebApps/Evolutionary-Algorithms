using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// DNA holds a list of genes that define an brain / agents behaviour
/// </summary>
/// Create an instance of the DNA class, specifying the number of genes that it should hold and the value ranges for each gene.
/// All genes will initially be randomised within the specified ranges and can later be combined from two other dna strands as
/// a part of breeding or mutated.
public class DNA {

	public List<int> Genes { get; private set; } 
	public int NumberOfGenes { get; private set; }
    public int[] GeneRanges { get; private set; }


    /// <summary>
    /// Construct a DNA with the specified number of genes, range and a list of passed gene values.
    /// </summary>
    /// <param name="numberOfGenes"></param>
    /// <param name="geneRanges"></param>
    /// <param name="genes"></param>
    public DNA(int numberOfGenes, int[] geneRanges, List<int> genes)
    {
        NumberOfGenes = numberOfGenes;
        GeneRanges = geneRanges;
        Genes = genes;
    }


    /// <summary>
    /// Construct a DNA with the specified number of genes, range and random gene values.
    /// </summary>
    /// <param name="numberOfGenes"></param>
    /// <param name="geneRanges"></param>
    /// <param name="genes"></param>
    public DNA(int numberOfGenes, int[] geneRanges)
	{
		NumberOfGenes = numberOfGenes;
		GeneRanges = geneRanges;
        Genes = new List<int>();
        for (int i = 0; i < NumberOfGenes; i++)
        {
            Genes.Add(Random.Range(0, GeneRanges[i]));
        }
    }


    /// <summary>
    /// Get the value of a particular gene
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetGene(int pos)
    {
        return Genes[pos];
    }


    /// <summary>
    /// Set a particular gene to a given value
    /// </summary>
    /// <param name="position"></param>
    /// <param name="value"></param>
    public void SetGene(int position, int value)
	{
		Genes[position] = value;
	}


    /// <summary>
    /// Combine DNA by taking half each from two seperate dna strands
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
	public static DNA CombineHalf(DNA d1, DNA d2)
	{
        Assert.AreEqual(d1.NumberOfGenes, d2.NumberOfGenes, "Dna strands should be the same length.");

        var dnaLength = d1.NumberOfGenes;
        List<int> genes = new List<int>();
        for (int i = 0; i < dnaLength; i++)
		{
			if(i < dnaLength / 2.0)
				genes.Add(d1.Genes[i]);
			else
				genes.Add(d2.Genes[i]);
		}
        return new DNA(d1.NumberOfGenes, d1.GeneRanges, genes);
    }

    /// <summary>
    /// Combining genes selected at random from two seperate dna strands
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    public static DNA CombineRandom(DNA d1, DNA d2)
    {
        Assert.AreEqual(d1.NumberOfGenes, d2.NumberOfGenes, "Dna strands should be the same length.");

        var dnaLength = d1.NumberOfGenes;
        List<int> genes = new List<int>();
        for (int i = 0; i < dnaLength; i++)
        {
            if (Random.value < 0.5f)
                genes.Add(d1.Genes[i]);
            else
                genes.Add(d2.Genes[i]);
        }
        return new DNA(d1.NumberOfGenes, d1.GeneRanges, genes);
    }

    /// <summary>
    /// Returns a mutated version of the DNA by mutating a single random gene.
    /// </summary>
    /// In genetics a gene can rarely mutate in place so we return a new DNA strand.
    public static DNA Mutate(DNA dna)
	{
        var geneToMutate = Random.Range(0, dna.NumberOfGenes);
        var newGenes = new List<int>(dna.Genes);
        newGenes[geneToMutate] = Random.Range(0, dna.GeneRanges[geneToMutate]);
        return new DNA(dna.NumberOfGenes, dna.GeneRanges, newGenes);
	}
}

