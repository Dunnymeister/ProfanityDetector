/*
MIT License
Copyright (c) 2019
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ebooks.ProfanityDetector
{
    /// <inheritdoc />
    public class ProfanityFilter : IProfanityFilter
    {
        public Terms Terms { get; set; }
        public Dictionary<string, string> Grawlixes { get; set; }

        public ProfanityFilter()
        {
            Terms = new Terms();
            Grawlixes = new Dictionary<string, string>();
            Grawlixes.Add("$", "s");
            Grawlixes.Add("5", "s");
            Grawlixes.Add("!", "i");
            Grawlixes.Add("7", "t");
        }

        public ProfanityFilter(Terms terms)
        {
            if (terms == null)
            {
                throw new ArgumentNullException("Parameter terms must not be null");
            }

            Terms = terms;
        }

        public bool IsProfanity(string term)
        {
            return GetProfanities(term).Count() > 0;
        }

        public bool ContainsProfanity(string term)
        {
            return GetProfanities(term).Count() > 0;
        }

        public IReadOnlyCollection<string> GetProfanities(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }

            // Perform character normalisation to prevent punctuation
            // getting strings snuck through
            var normalisedInput = input.ToLower(CultureInfo.InvariantCulture);
            normalisedInput = normalisedInput.Replace(".", "");
            normalisedInput = normalisedInput.Replace(",", "");
            normalisedInput = normalisedInput.Replace("-", "");

            foreach (var grawlix in Grawlixes)
            {
                normalisedInput = normalisedInput.Replace(grawlix.Key, grawlix.Value);
            }

            var potentials = new List<string>();
            foreach (var prohibitedTerm in Terms.Prohibited)
            {
                string pattern = string.Format(@"(?:\w*{0}\w*)", prohibitedTerm);
                foreach (Match m in Regex.Matches(normalisedInput, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    // If it's an exact match we can continue without further analysis
                    if (prohibitedTerm.Equals(m.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        potentials.Add(m.Value);
                        continue;
                    }

                    // The match at this point contains prohibited words embedded in larger word
                    // This means that Scunthorpe will be flagged when it's clearly ok
                    // but fucking will be flagged correctly
                    // This scenario is hard to deal with but let's do our best
                    var adjectiveEndings = new List<string>() { "ing", "ed", "s", "er", "y", "a" };
                    foreach (var adjectiveEnding in adjectiveEndings)
                    {
                        var test = $"{prohibitedTerm}{adjectiveEnding}";
                        if (m.Value.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                        {
                            potentials.Add(m.Value);
                            break;
                        }
                    }

                    // Double check that the phrase isn't just surrounded by garbage characters                           
                    var regex = new Regex(@"[^a-zA-Z_]*", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    var result = regex.Replace(m.Value, "");
                    if (prohibitedTerm.Equals(result, StringComparison.InvariantCultureIgnoreCase))
                    {
                        potentials.Add(m.Value);
                        break;
                    }
                }
            }

            potentials = potentials.Distinct().ToList();

            // Remove any matches that are on the permitted list
            potentials.RemoveAll(x => Terms.Permitted.Any(y => x.Equals(y, StringComparison.InvariantCultureIgnoreCase)));

            // Deduplicate any partial matches, ie, if the word "twatting" is in a sentence, don't include "twat" if part of the same word.
            return potentials.ToArray();
        }

        public string Censor(string sentence, char censorCharacter = '*')
        {
            if (string.IsNullOrEmpty(sentence))
            {
                return string.Empty;
            }

            string noPunctuation = sentence.Trim();
            noPunctuation = noPunctuation.ToLower();

            noPunctuation = Regex.Replace(noPunctuation, @"[^\w\s]", "");

            var words = noPunctuation.Split(' ');

            var postAllowList = FilterWordListByAllowList(words);
            var swearList = new List<string>();

            AddMultiWordProfanities(swearList, string.Join(" ", postAllowList));

            StringBuilder censored = new StringBuilder(sentence);
            StringBuilder tracker = new StringBuilder(sentence);

            return CensorStringByProfanityList(censorCharacter, swearList, censored, tracker).ToString();
        }

        /// <summary>
        /// For a given sentence, look for the specified profanity. If it is found, look to see
        /// if it is part of a containing word. If it is, then return the containing work and the start
        /// and end positions of that word in the string.
        ///
        /// For example, if the string contains "scunthorpe" and the passed in profanity is "cunt",
        /// then this method will find "cunt" and work out that it is part of an enclosed word.
        /// </summary>
        /// <param name="toCheck">Sentence to check.</param>
        /// <param name="profanity">Profanity to look for.</param>
        /// <returns>Tuple of the following format (start character, end character, found enclosed word).
        /// If no enclosed word is found then return null.</returns>
        public (int, int, string)? GetCompleteWord(string toCheck, string profanity)
        {
            if (string.IsNullOrEmpty(toCheck))
            {
                return null;
            }

            string profanityLowerCase = profanity.ToLower(CultureInfo.InvariantCulture);
            string toCheckLowerCase = toCheck.ToLower(CultureInfo.InvariantCulture);

            if (toCheckLowerCase.Contains(profanityLowerCase))
            {
                var startIndex = toCheckLowerCase.IndexOf(profanityLowerCase, StringComparison.Ordinal);
                var endIndex = startIndex;

                // Work backwards in string to get to the start of the word.
                while (startIndex > 0)
                {
                    if (toCheck[startIndex - 1] == ' ' || char.IsPunctuation(toCheck[startIndex - 1]))
                    {
                        break;
                    }

                    startIndex -= 1;
                }

                // Work forwards to get to the end of the word.
                while (endIndex < toCheck.Length)
                {
                    if (toCheck[endIndex] == ' ' || char.IsPunctuation(toCheck[endIndex]))
                    {
                        break;
                    }

                    endIndex += 1;
                }

                return (startIndex, endIndex, toCheckLowerCase.Substring(startIndex, endIndex - startIndex).ToLower(CultureInfo.InvariantCulture));
            }

            return null;
        }

        private StringBuilder CensorStringByProfanityList(char censorCharacter, List<string> swearList, StringBuilder censored, StringBuilder tracker)
        {
            foreach (string word in swearList.OrderByDescending(x => x.Length))
            {
                (int, int, string)? result = (0, 0, "");
                var multiWord = word.Split(' ');

                if (multiWord.Length == 1)
                {
                    do
                    {
                        result = GetCompleteWord(tracker.ToString(), word);

                        if (result != null)
                        {
                            string filtered = result.Value.Item3;

                            filtered = Regex.Replace(result.Value.Item3, @"[\d-]", string.Empty);

                            if (filtered == word)
                            {
                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    censored[i] = censorCharacter;
                                    tracker[i] = censorCharacter;
                                }
                            }
                            else
                            {
                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    tracker[i] = censorCharacter;
                                }
                            }
                        }
                    }
                    while (result != null);
                }
                else
                {
                    censored = censored.Replace(word, CreateCensoredString(word, censorCharacter));
                }
            }

            return censored;
        }

        private List<string> FilterSwearListForCompleteWordsOnly(string sentence, List<string> swearList)
        {
            List<string> filteredSwearList = new List<string>();
            StringBuilder tracker = new StringBuilder(sentence);

            foreach (string word in swearList.OrderByDescending(x => x.Length))
            {
                (int, int, string)? result = (0, 0, "");
                var multiWord = word.Split(' ');

                if (multiWord.Length == 1)
                {
                    do
                    {
                        result = GetCompleteWord(tracker.ToString(), word);

                        if (result != null)
                        {
                            if (result.Value.Item3 == word)
                            {
                                filteredSwearList.Add(word);

                                for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                                {
                                    tracker[i] = '*';
                                }
                                break;
                            }

                            for (int i = result.Value.Item1; i < result.Value.Item2; i++)
                            {
                                tracker[i] = '*';
                            }
                        }
                    }
                    while (result != null);
                }
                else
                {
                    filteredSwearList.Add(word);
                    tracker.Replace(word, " ");
                }
            }

            return filteredSwearList;
        }

        private List<string> FilterWordListByAllowList(string[] words)
        {
            List<string> postAllowList = new List<string>();
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    if (!Terms.Permitted.Contains(word.ToLower(CultureInfo.InvariantCulture)))
                    {
                        postAllowList.Add(word);
                    }
                }
            }

            return postAllowList;
        }

        private void AddMultiWordProfanities(List<string> swearList, string postAllowListSentence)
        {
            swearList.AddRange(
                from string profanity in Terms.Prohibited
                where postAllowListSentence.ToLower(CultureInfo.InvariantCulture).Contains(profanity)
                select profanity);
        }

        private static string CreateCensoredString(string word, char censorCharacter)
        {
            string censoredWord = string.Empty;

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] != ' ')
                {
                    censoredWord += censorCharacter;
                }
                else
                {
                    censoredWord += ' ';
                }
            }

            return censoredWord;
        }
    }
}