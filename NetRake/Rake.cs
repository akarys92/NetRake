using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Resources;

namespace NetRake
{
    public class Rake
    {
        private String StopWordRegex;
        private String reSplit = @"(?:[_,.;:!?\(\)\[\]\{\}\/\|\\\*\#\%\^\&\-\=\+\n])";
        private static int MIN_LENGTH = 3;

        public Rake()
        {
            var list = Properties.Resources.StopWordList;
            String[] stopListString = list.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None); ;
            List<String> stopList = stopListString.ToList<String>();
            this.StopWordRegex = CreateStopwordRegex(stopList);
        }
        public Dictionary<String, double> Run(String text)
        {
            // Generate candidate words
            String candidateWords = GenerateCandidates(text);

            // Generate phrase list
            List<String> sentences = GeneratePhrases(candidateWords);

            // Generate word scores
            Dictionary<String, double> wordRanks = GetWordScores(sentences);

            // Generate Keyword scores
            Dictionary<String, double> wordScores = GetPhraseScores(sentences, wordRanks);

            return wordScores;
        }

        // Create the stop word Regex 
        private String CreateStopwordRegex(List<String> stopWordList)
        {
            List<String> patterns = new List<string>();
            string pattern = @"\b(";
            string end = @")\b";
            String temp = String.Join("|", stopWordList);
            String output = pattern + temp + end;
            return output;
        }

        // Apply the stop filter
        private String GenerateCandidates(String text)
        {
            String match = Regex.Replace(text, this.StopWordRegex, "", RegexOptions.IgnoreCase);
            String output = match.Replace("  ", " ").Replace("  ", "");
            return output;
        }
        // Get Ranks of individual words
        private Dictionary<String, double> GetWordScores(List<String> phraseList)
        {
            Dictionary<String, int> wordFrequency = new Dictionary<String, int>();
            Dictionary<String, int> wordDegree = new Dictionary<String, int>();
            foreach (string phrase in phraseList)
            {
                List<String> wordList = SeperateWords(phrase, MIN_LENGTH);
                int wordListLength = wordList.Count;
                int wordListDegree = wordListLength - 1;
                foreach (string word in wordList)
                {
                    if (wordFrequency.ContainsKey(word))
                    {
                        wordFrequency[word] += 1;
                    }
                    else {
                        wordFrequency[word] = 1;
                    }
                    if (wordDegree.ContainsKey(word))
                    {
                        wordDegree[word] += wordListDegree;
                    }
                    else {
                        wordDegree[word] = wordListDegree;
                    }
                }
                foreach (string word in wordFrequency.Keys)
                {
                    wordDegree[word] = wordDegree[word] + wordFrequency[word];
                }
            }
            Dictionary<String, double> wordScores = new Dictionary<String, double>();
            foreach (string word in wordDegree.Keys)
            {
                wordScores[word] = (double)wordDegree[word] / (double)wordFrequency[word];
            }

            return wordScores;
        }

        private Dictionary<String, double> GetPhraseScores(List<String> candidateWords, Dictionary<String, double> wordScores)
        {
            Dictionary<String, double> phraseScores = new Dictionary<String, double>();
            foreach (String phrase in candidateWords)
            {
                // Break into words
                List<String> words = SeperateWords(phrase, MIN_LENGTH);
                double score = 0;
                // Add composite score
                foreach (string word in words)
                {
                    score += wordScores[word];
                }
                // Add that to output
                phraseScores[phrase] = score;
            }

            return phraseScores;
        }

        private List<string> GeneratePhrases(string text)
        {
            List<string> sentences = new List<string>();
            var matches = Regex.Matches(text, reSplit, RegexOptions.ExplicitCapture);
            foreach (string s in Regex.Split(text, reSplit))
            {
                string ns = s.Trim();
                if (!string.IsNullOrEmpty(ns))
                {
                    sentences.Add(ns.ToLower());
                }
            }

            return sentences;
        }
        // Break phrases into individual words with min length
        private List<String> SeperateWords(string phrase, int minLength)
        {
            List<String> wordList = new List<string>();
            string[] words = phrase.Split(' ');
            foreach (string word in words)
            {
                if (word.Length >= minLength)
                    wordList.Add(word);
            }
            return wordList;
        }
    }
}
