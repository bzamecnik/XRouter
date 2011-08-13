using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.ObjectExploring
{
    public static class ArrayIndicesEnumeratingExtension
    {
        public static IEnumerable<int[]> GetAllIndices(this Array array)
        {
            var rank = array.Rank;
            var indices = new int[rank];
            var lowerBounds = new int[rank];
            var upperBounds = new int[rank];
            for (int i = 0; i < rank; i++) {
                lowerBounds[i] = array.GetLowerBound(i);
                upperBounds[i] = array.GetUpperBound(i);
                indices[i] = lowerBounds[i];
            }

            while (indices[0] <= upperBounds[0]) {
                yield return indices;

                int indexToIncrease = rank - 1;
                while ((indexToIncrease > 0) && (indices[indexToIncrease] == upperBounds[indexToIncrease])) {
                    indexToIncrease--;
                }

                indices[indexToIncrease]++;
                for (int i = indexToIncrease + 1; i < rank; i--) { indices[i] = lowerBounds[i]; }
            }
        }
    }
}
