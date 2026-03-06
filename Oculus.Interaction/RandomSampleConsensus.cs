using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RandomSampleConsensus<TModel>
	{
		public RandomSampleConsensus(int maxDataPoints = 10, int exclusionZone = 2)
		{
			this._maxDataPoints = maxDataPoints;
			this._exclusionZone = exclusionZone;
			this._modelSet = new TModel[maxDataPoints, maxDataPoints];
		}

		public TModel FindOptimalModel(RandomSampleConsensus<TModel>.GenerateModel modelGenerator, RandomSampleConsensus<TModel>.EvaluateModelScore modelScorer)
		{
			return this.FindOptimalModel(modelGenerator, modelScorer, this._maxDataPoints);
		}

		public TModel FindOptimalModel(RandomSampleConsensus<TModel>.GenerateModel modelGenerator, RandomSampleConsensus<TModel>.EvaluateModelScore modelScorer, int dataPointsCount)
		{
			for (int i = 0; i < dataPointsCount; i++)
			{
				for (int j = i + 1; j < dataPointsCount; j++)
				{
					this._modelSet[i, j] = modelGenerator((i + this._exclusionZone) % dataPointsCount, (j + this._exclusionZone) % dataPointsCount);
				}
			}
			TModel result = default(TModel);
			float num = float.PositiveInfinity;
			for (int k = 0; k < dataPointsCount; k++)
			{
				int num2 = Random.Range(0, dataPointsCount - 1);
				int num3 = Random.Range(num2 + 1, dataPointsCount);
				TModel tmodel = this._modelSet[num2, num3];
				float num4 = modelScorer(tmodel, this._modelSet);
				if (num4 < num)
				{
					result = tmodel;
					num = num4;
				}
			}
			return result;
		}

		private readonly TModel[,] _modelSet;

		private readonly int _exclusionZone;

		private readonly int _maxDataPoints;

		public delegate TModel GenerateModel(int index1, int index2);

		public delegate float EvaluateModelScore(TModel model, TModel[,] modelSet);
	}
}
