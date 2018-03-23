# Evolutionary Algorithms

Genetic algorithms are inspired by biological evolution and concepts such as reproduction, mutation, recombination, and netural selection. Genetic algorithms are commonly used to generate solutions to optimization and search problems.

In this example we look at how to optimise the ability of move and jump to maximise the total distance agents travel within a given time period.

![Screenshot](screenshot.gif)

We initialise the algorithm with random start values for 'walk speed' and 'distance from object to jump'. Evolution occurs according to the below.

## Strength
In general we only let the strongest individuals survive. It is hence critical that we have a good definition of how to define what strongest means. We also need to determine what percentage of the population should 'survive' and be used for breeding new generations.

In this demo the fitness function considers total distance travelled, and penalise for time in the air as that is presumably sub optimal using more energy.

## Breeding Strategy
How should individuals that survive breed? There are many methods here including cross breading with the strongest, random breeding, biased towards the strongest, considering different genders etc.

We also need to consider whether the population size is allowed to change, or our algorithm needs to consider a fixed population size.

In this demo we simply breed in pairs, starting with the strongest and moving down.

## Mixing of genes
Once we have 2 (or more individuals) a strategy needs defining for how to combine genes. This can for example be done at random, from specific individuals, biased to the strongest, etc. We can also in our case chose to take values literally, take a mean or perform some other way of combining them.

In this demo we just chose genes at random from the parents.

## Mutation Rate
How often should genes mutate, how many genes can mutate and should mutations be at random or a deviation of existing gene values.

In this demo we have a mutation rate of 1% setting all genes to random values within their valid range.


SkyBox: CC-BY-SA 3.0 NoLogoGames, https://opengameart.org/content/space-nebulas-skybox
