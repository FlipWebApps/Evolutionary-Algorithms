using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Monobehaviour representing the brain of any bot / agent.
/// </summary>
public class Brain : MonoBehaviour
{
    #region Configuration
    // 2 genes - 'speed' & 'distance from obstacle before jump'
    public enum Genes { Speed = 0, JumpDistance = 1}
    const int NumberOfGenes = 2;
    int[] GeneRanges = { 80, 80 };
    #endregion Configuration

    public GameObject DistanceSensor;
    public GameObject Explosion;
    public GameObject Body;
    public GameObject Glasses;
    public GameObject Skelaton;

    public DNA DNA { get; private set; }
    public float DistanceTravelled { get; private set; }
    public float TimeInAir { get; private set; }

    Vector3 _startPosition;
    ThirdPersonCharacter _thirdPersonCharacter;
    
	public void Init(DNA dna = null)
	{
        DNA = dna ?? new DNA(NumberOfGenes, GeneRanges);
        DistanceTravelled = 0;
        _startPosition = transform.position;
        _thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
    }

    private void Update()
    {
        // check if an obstacle is within range
        Debug.DrawRay(DistanceSensor.transform.position, DistanceSensor.transform.forward * 8, Color.red, 0);
        var seeObstacle = false;
        RaycastHit hit;
        if (Physics.Raycast(DistanceSensor.transform.position, DistanceSensor.transform.forward, out hit, 8, layerMask: 1 << LayerMask.NameToLayer("Obstacle")))
        {
            seeObstacle = true;
        }

        // read DNA
        var speed = DNA.GetGene((int)Genes.Speed) * 0.01f;              // range 0..0.8 per frame
        var jumpDistance = DNA.GetGene((int)Genes.JumpDistance) / 10f;  // range 0..8 in increments of 0.1

        if (!PopulationManager.episodeIsEnded)
        {
            // determine move vector and whether to jump
            var move = speed * Vector3.forward;
            var jump = (seeObstacle && hit.distance < jumpDistance);

            // act
            _thirdPersonCharacter.Move(move, false, jump);

            // update distance
            DistanceTravelled = transform.position.z - _startPosition.z;
            if (DistanceTravelled > PopulationManager.BestDistance)
                PopulationManager.BestDistance = DistanceTravelled;

            // update time in air
            if (!_thirdPersonCharacter.m_IsGrounded)
                TimeInAir += Time.deltaTime;
        }
        else
            _thirdPersonCharacter.Move(Vector3.zero, false, false);
    }
}

