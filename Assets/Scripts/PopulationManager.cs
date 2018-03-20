using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Global population manager for controling the population and world state.
/// </summary>
public class PopulationManager : MonoBehaviour {

	public GameObject BotPrefab;
	public int PopulationSize = 50;
    public int MutationPercent = 1;
    public float EpisodeTime = 5;
    public Text StatusText;
    public Text MessageText;

    List<GameObject> population = new List<GameObject>();
	public static float elapsed = 0;
    public static float BestDistance = 0;
    public static bool episodeIsEnded = false;
    int generation = 1;
    double _meanSpeed, _meanJumpDistance;
    double _sdSpeed, _sdJumpDistance;


	void Start () {
		for(int i = 0; i < PopulationSize; i++)
		{
            var brain = CreateBot();
            population.Add(brain.gameObject);
		}
        UpdateGeneStatistics();
    }


    void Update()
    {
        if (!episodeIsEnded)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= EpisodeTime)
            {
                elapsed = EpisodeTime;
                episodeIsEnded = true;
                StartCoroutine(EpisodeEnded());
            }

            StatusText.text = string.Format(
                "Time: {0:0.00}\n" +
                "Generation: {1}\n" +
                "Max. Distance: {2:0.00}\n" +
                "Population: {3}\n" +
                "Speed: {4:0.00} ({5:0.00})\n" +
                "Jump Distance: {6:0.00} ({7:0.00})\n",
                elapsed, generation, BestDistance, population.Count, _meanSpeed, _sdSpeed, _meanJumpDistance, _sdJumpDistance
                );
        }
    }

    /// <summary>
    /// Create a new Bot instance, optionally with specified DNA
    /// </summary>
    /// <param name="dna"></param>
    /// <returns></returns>
    Brain CreateBot(DNA dna = null)
    {
        var startingPos = new Vector3(this.transform.position.x + Random.Range(-2f, 2f),
                                                this.transform.position.y,
                                                this.transform.position.z);
        var bot = Instantiate(BotPrefab, startingPos, this.transform.rotation);
        var brain = bot.GetComponent<Brain>();
        brain.Init(dna);
        return brain;
    }


    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        DNA dna;
        if (Random.Range(0, 100) < MutationPercent)
            dna = DNA.Mutate(parent1.GetComponent<Brain>().DNA);
        else
            dna = DNA.CombineRandom(parent1.GetComponent<Brain>().DNA, parent2.GetComponent<Brain>().DNA);

        var brain = CreateBot(dna);
        return brain.gameObject;
    }


    IEnumerator EpisodeEnded()
    {
        MessageText.gameObject.SetActive(true);
        MessageText.color = Color.red;
        MessageText.text = "Time UP - Weakest Die!";
        yield return new WaitForSeconds(0.5f);
        AnimateKillWeakest();
        yield return new WaitForSeconds(2);
        MessageText.color = Color.green;
        MessageText.text = string.Format("Breeding New Generation {0}", generation + 1);
        yield return new WaitForSeconds(2);
        BreedNewPopulation();
        MessageText.gameObject.SetActive(false);
        episodeIsEnded = false;
        elapsed = 0;
    }


    void AnimateKillWeakest()
    {
        var sortedList = GetPopulationSortedBySurvivalAscending();

        for (int i = 0; i < (int)(sortedList.Count / 2.0f) - 1; i++)
        {
            var brain = sortedList[i].GetComponent<Brain>();
            brain.Explosion.SetActive(true);
            brain.Body.SetActive(false);
            brain.Glasses.SetActive(false);
            brain.Skelaton.SetActive(false);
        }
    }

    /// <summary>
    /// Breed a new population from the top half strongest, pairing off from strongest.
    /// </summary>
    void BreedNewPopulation()
    {
        var sortedList = GetPopulationSortedBySurvivalAscending();

        population.Clear();

        // different strategies for breeding, best with best, best with worst etc..
        for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        // kill the previous population
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        // new stats
        generation++;
        UpdateGeneStatistics();
    }

    /// <summary>
    /// Return a list sorted by 'strength' or total reward. Rewarded for distance travelled, and penalised for time 
    /// in the air (as that uses energy).
    /// </summary>
    /// <returns></returns>
    List<GameObject> GetPopulationSortedBySurvivalAscending()
    {
        return population.OrderBy(o =>
                     (o.GetComponent<Brain>().DistanceTravelled - o.GetComponent<Brain>().TimeInAir)).ToList();
    }

    /// <summary>
    /// Update the gene statistics variables
    /// </summary>
    void UpdateGeneStatistics()
    {
        int totalSpeeds = 0;
        int totalJumpDistances = 0;
        for (int i = 0; i < population.Count; i++)
        {
            var brain = population[i].GetComponent<Brain>();
            totalSpeeds += brain.DNA.GetGene((int)Brain.Genes.Speed);
            totalJumpDistances += brain.DNA.GetGene((int)Brain.Genes.JumpDistance);
        }
        _meanSpeed = totalSpeeds / (double)population.Count;
        _meanJumpDistance = totalJumpDistances / (double)population.Count;

        double sumSquaredDifferencesSpeeds = 0;
        double sumSquaredDifferencesJumpDistances = 0;
        for (int i = 0; i < population.Count; i++)
        {
            var brain = population[i].GetComponent<Brain>();
            sumSquaredDifferencesSpeeds += System.Math.Pow(brain.DNA.GetGene((int)Brain.Genes.Speed) - _meanSpeed, 2f);
            sumSquaredDifferencesJumpDistances += System.Math.Pow(brain.DNA.GetGene((int)Brain.Genes.JumpDistance) - _meanJumpDistance, 2f);
        }
        _sdSpeed = System.Math.Sqrt(sumSquaredDifferencesSpeeds / (double)population.Count);
        _sdJumpDistance = System.Math.Sqrt(sumSquaredDifferencesJumpDistances / (double)population.Count);
    }
}

