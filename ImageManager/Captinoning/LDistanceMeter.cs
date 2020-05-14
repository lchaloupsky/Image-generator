using System;

namespace ImageManagement.Captioning
{
    /// <summary>
    /// Class for measuring the distance of two strings
    /// </summary>
    public class LDistanceMeter
    {
        /// <summary>
        /// Calculates standard Levenshtein distance of two strings with dynamic programming
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <returns>Calculated Levenshtein distance</returns>
        public int CalculateStringDistance(string s1, string s2)
        {
            int[,] distances = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i < s1.Length + 1; i++)
                distances[i, 0] = i;

            for (int i = 0; i < s2.Length + 1; i++)
                distances[0, i] = i;

            for (int i = 0; i < s1.Length; i++)
            {
                for (int j = 0; j < s2.Length; j++)
                {
                    int cost = 1;
                    if (s1[i] == s2[j])
                        cost = 0;

                    distances[i + 1, j + 1] = Math.Min(distances[i, j + 1] + 1, Math.Min(distances[i + 1, j] + 1, distances[i, j] + cost));

                    if (i > 0 && j > 0 && s1[i] == s2[j - 1] && s1[i - 1] == s2[j])
                        distances[i + 1, j + 1] = Math.Min(distances[i + 1, j + 1], distances[i - 1, j - 1] + 1);
                }
            }

            return distances[s1.Length, s2.Length];
        }
    }
}
