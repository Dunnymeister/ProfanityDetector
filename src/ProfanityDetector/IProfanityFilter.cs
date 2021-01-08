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
using System.Collections.Generic;

namespace Ebooks.ProfanityDetector
{
    /// <summary>
    /// Detects profanity and racial slurs contained within some text and return an indication flag.<br />
    /// All words are treated as case insensitive.
    /// </summary>
    public interface IProfanityFilter
    {
        /// <summary>
        /// An object containing the pProhibited terms and 
        /// </summary>
        Terms Terms { get; set; }

        /// <summary>
        /// Check if a single word is in the profanity list
        /// </summary>
        /// <param name="term">The term to check</param>
        /// <returns>True if considered to be profanity, False otherwise</returns>
        bool IsProfanity(string term);

        /// <summary>
        /// Check if a string contains any profanity
        /// </summary>
        /// <param name="input">The string to check</param>
        /// <returns>True if the string contains profanity, False otherwise</returns>
        bool ContainsProfanity(string input);

        /// <summary>
        /// Gets a list of all terms in the input strings considered to be profanity
        /// </summary>
        /// <param name="input">The string to check</param>
        /// <returns>A read-only collection of any offensive terms found in the input</returns>
        IReadOnlyCollection<string> GetProfanities(string input);


        /// <summary>
        /// For any given string, censor any profanities from the list using the specified
        /// censoring character.
        /// </summary>
        /// <param name="sentence">The string to censor.</param>
        /// <param name="censorCharacter">The character to use for censoring.</param>
        /// <param name="ignoreNumbers">Ignore any numbers that appear in a word.</param>
        /// <returns></returns>
        string Censor(string input, char censorCharacter = '*');
    }
}