using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MerkleTree
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> words = new List<string> { "Anatolii", "Borachuk", "Work", "Csharp", "Language", "Nothing", "Tree" };
            Console.WriteLine("List of words: " + string.Join(", ", words));

            List<string> hashValueWords = new List<string>();
            using (SHA256 sha256 = SHA256.Create())
            {
                foreach (string word in words)
                {
                    byte[] wordBytes = Encoding.UTF8.GetBytes(word);
                    byte[] hashBytes = sha256.ComputeHash(wordBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    hashValueWords.Add(hash);
                }
            }

            Console.WriteLine("\nMerkle tree leaves:");
            Console.WriteLine(string.Join("\n", hashValueWords));

            MerkleTree tree = new MerkleTree();
            int branchLevel = 1;
            (string root, int level) = tree.RootHashCalculation(hashValueWords, branchLevel);

            Console.WriteLine($"\nThe only node in the branches level number {level} is the hash of the root of the Merkle tree:\n{root}");

            Console.ReadKey();
        }
    }

    public class MerkleTree
    {
        public (string, int) RootHashCalculation(List<string> hashes, int level)
        {
            if (hashes.Count % 2 != 0)
            {
                hashes.Add(hashes[hashes.Count - 1]);
                Console.WriteLine($"\nSince the number of hashes is not even, we added the following hash to the list:\n{hashes.Count - 1}");
                Console.WriteLine("\nNow we have the following list of hashes:");
                Console.WriteLine(string.Join("\n", hashes));
            }

            List<string> nodes = new List<string>();

            int firstElem = 0;
            int secondElem = 1;
            int count = 1;

            while (count <= Math.Round(hashes.Count / 2.0))
            {
                int nodeNumber = count - 1;
                string nodeHash = GetHash(hashes[firstElem] + hashes[secondElem]);
                nodes.Add(nodeHash);
                Console.WriteLine($"\nThe hash of the node of pair number {count}:\n{hashes[firstElem]} + {hashes[secondElem]}");
                Console.WriteLine($"The level of branches number {level} is this:\n{nodes[nodeNumber]}\n");

                firstElem += 2;
                secondElem += 2;
                count++;
            }

            Console.WriteLine($"In total, at the level of branches number {level}, we have {nodes.Count} nodes:");
            Console.WriteLine(string.Join("\n", nodes));

            hashes.Clear();
            level++;

            if (nodes.Count == 1)
            {
                level--;
                return (nodes[0], level);
            }
            else
            {
                return RootHashCalculation(nodes, level);
            }
        }

        private string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }
    }
}
