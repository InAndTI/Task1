using System.Security.Cryptography;
using System.Text;

namespace Task1.Worker.Services
{
    public static class BruteForceService
    {
        private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        public static List<string> FindHash(string hash, int maxLength, int partNumber, int partCount)
        {
            List<string> found = new();
            int totalCombinations = (int)Math.Pow(Alphabet.Length, maxLength);

            int start = (totalCombinations / partCount) * partNumber;
            int end = (partNumber == partCount - 1) ? totalCombinations : (totalCombinations / partCount) * (partNumber + 1);

            foreach (var word in GenerateWords(maxLength, start, end))
            {
                if (ComputeMD5(word) == hash)
                {
                    found.Add(word);
                }
            }
            return found;
        }




        private static IEnumerable<string> GenerateWords(int maxLength, int start, int end)
        {
            int counter = 0;
            foreach (var word in GenerateCombinations(Alphabet, maxLength))
            {
                if (counter >= start && counter < end)
                    yield return word;

                counter++;
                if (counter >= end) break;
            }
        }

        private static IEnumerable<string> GenerateCombinations(char[] alphabet, int length)
        {
            var stack = new Stack<(string, int)>();
            stack.Push(("", 0));

            while (stack.Count > 0)
            {
                var (prefix, depth) = stack.Pop();
                if (depth == length)
                {
                    yield return prefix;
                    continue;
                }

                foreach (var letter in alphabet)
                {
                    stack.Push((prefix + letter, depth + 1));
                }
            }
        }

        private static string ComputeMD5(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
