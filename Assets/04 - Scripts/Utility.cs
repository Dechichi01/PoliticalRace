using UnityEngine;
using System.Collections;

namespace Utility
{
    public class Randomness
    {
        //Returns a shuffled array (can be used to avoid repetition)
        public static T[] ShuffledArray<T>(T[] array, int seed)
        {
            System.Random prng = new System.Random(seed);

            for (int i = 0; i < array.Length; i++)
            {
                int randomIndex = prng.Next(i, array.Length);

                //Swaps current item with a random item in the array
                T tempItem = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = tempItem;
            }
            return array;
        }

        //Get a random value with probability (see ProbabilityElement class)
        public static T GetRandomValue<T>(params ProbabilityElement<T>[] selections)
        {
            float rand = Random.value;
            float currentProb = 0;
            foreach (var selection in selections)
            {
                currentProb += selection.probability;
                if (rand <= currentProb)
                    return selection.element;
            }

            //will happen if the input's probabilities sums to less than 1
            //In ths case will just return a equal probability random selection
            return selections[Random.Range(0, selections.Length)].element;
        }
    }

    public class ProbabilityElement<T>
    {
        public T element;
        public float probability;

        public ProbabilityElement(T _element, float _probability)
        {
            element = _element;
            probability = _probability;
        }
    }

    /*
     * PseudoRandomDistribution container class. Used to create a PRD element, with avg probability p,
     * which guarantees to trigger after a certain amount of failed tries (N)
    */
    public class PRD
    {
        private int N;
        private decimal C;

        public PRD(float p)
        {
            N = 0;
            C = CfromP((decimal) p);
        }

        public float GetProbability()
        {
            N++;
            float p = Mathf.Clamp01((float)C * N);
            if (p >= 1f) N = 0;
            return p;
        }

        public void ResetTries() { N = 0; }

        public decimal CfromP(decimal p)
        {
            decimal Cupper = p;
            decimal Clower = 0m;
            decimal Cmid;
            decimal p1;
            decimal p2 = 1m;
            while (true)
            {
                Cmid = (Cupper + Clower) / 2m;
                p1 = PfromC(Cmid);
                if (System.Math.Abs(p1 - p2) <= 0m) break;

                if (p1 > p)
                    Cupper = Cmid;
                else
                    Clower = Cmid;

                p2 = p1;
            }

            return Cmid;
        }

        private decimal PfromC(decimal C)
        {
            decimal pProcOnN = 0m;
            decimal pProcByN = 0m;
            decimal sumNpProcOnN = 0m;

            int maxFails = (int)System.Math.Ceiling(1m / C);
            for (int N = 1; N <= maxFails; ++N)
            {
                pProcOnN = System.Math.Min(1m, N * C) * (1m - pProcByN);
                pProcByN += pProcOnN;
                sumNpProcOnN += N * pProcOnN;
            }

            return (1m / sumNpProcOnN);
        }
    }
    
}
