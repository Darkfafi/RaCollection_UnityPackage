using System;
using System.Collections.Generic;

namespace RaCollection
{

	public static class RaCollectionRandomUtils
	{
		// List
		public static Random CreateRandom(int? seed = null)
		{
			return seed.HasValue ? new Random(seed.Value) : new Random(UnityEngine.Random.Range(0, int.MaxValue));
		}

		public static void Shuffle<T>(this IList<T> self, int? seed = null)
		{
			Random random = CreateRandom(seed);
			int n = self.Count;
			while(n > 1)
			{
				int k = random.Next(n--);
				T v = self[k];
				self[k] = self[n];
				self[n] = v;
			}
		}

		public static T GetRandomItem<T>(this IList<T> weights)
			where T : IRaCollectionWeightedItem
		{
			if(weights.Count == 0)
			{
				return default;
			}

			if(weights.Count == 1)
			{
				return weights[0];
			}

			int maxRoll = 1;

			List<T> sortedWeights = new List<T>(weights);
			sortedWeights.Sort((a, b) => b.RaItemWeight - a.RaItemWeight);
			for(int i = 0; i < sortedWeights.Count; i++)
			{
				maxRoll += sortedWeights[i].RaItemWeight;
			}

			int roll = UnityEngine.Random.Range(0, maxRoll);
			int currentValue = 0;

			for(int i = 0; i < sortedWeights.Count; i++)
			{
				T currentEntry = sortedWeights[i];
				if(roll > currentValue && roll <= currentValue + currentEntry.RaItemWeight)
				{
					return currentEntry;
				}
				currentValue += currentEntry.RaItemWeight;
			}

			return sortedWeights[0];
		}

		// ReadOnly List
		public static T GetRandomItemReadOnly<T>(this IReadOnlyList<T> weights)
			where T : IRaCollectionWeightedItem
		{
			if(weights.Count == 0)
			{
				return default;
			}

			if(weights.Count == 1)
			{
				return weights[0];
			}

			int maxRoll = 1;

			List<T> sortedWeights = new List<T>(weights);
			sortedWeights.Sort((a, b) => b.RaItemWeight - a.RaItemWeight);
			for(int i = 0; i < sortedWeights.Count; i++)
			{
				maxRoll += sortedWeights[i].RaItemWeight;
			}

			int roll = UnityEngine.Random.Range(0, maxRoll);
			int currentValue = 0;

			for(int i = 0; i < sortedWeights.Count; i++)
			{
				T currentEntry = sortedWeights[i];
				if(roll > currentValue && roll <= currentValue + currentEntry.RaItemWeight)
				{
					return currentEntry;
				}
				currentValue += currentEntry.RaItemWeight;
			}

			return sortedWeights[0];
		}
	}


	public interface IRaCollectionWeightedItem
	{
		int RaItemWeight
		{
			get;
		}
	}
}