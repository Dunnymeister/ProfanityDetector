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
using System.Linq;
using Xunit;

namespace Ebooks.ProfanityDetector.Tests.Unit
{
    public class ProfanityTests
    {
        [Fact]
        public void Constructor_Default_TermObjectIsNotNotNull()
        {
            IProfanityFilter filter = new ProfanityFilter();
            Assert.NotNull(filter.Terms);
        }

        [Fact]
        public void IsProfanity_TermContainsProhibitedTerm_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("arsehole");
            Assert.True(filter.IsProfanity("arsehole"));
        }

        [Fact]
        public void IsProfanity_TermDoesNotContainProhibitedTerm_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            Assert.False(filter.IsProfanity("fluffy"));
        }


        [Fact]
        public void IsProfanity_TermIsEmpty_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            Assert.False(filter.IsProfanity(string.Empty));
        }

        [Fact]
        public void IsProfanity_TermIsNull_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            Assert.False(filter.IsProfanity(null));
        }

        [Fact]
        public void IsProfanity_TermIsProhibitedAndPermitted_ReturnsFalse()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Permitted.Add("shitty");
            Assert.False(filter.IsProfanity("shitty"));
        }

        [Fact]
        public void IsProfanity_TermIsPermittedAndMixedCase_ReturnsFalse()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Permitted.Add("shitty");
            Assert.False(filter.IsProfanity("ShiTty"));
        }

        [Fact]
        public void GetProfanities_InputIsEmpty_ReturnsEmptyCollection()
        {
            var filter = new ProfanityFilter();
            var swearList = filter.GetProfanities(string.Empty);

            Assert.Equal(0, swearList.Count);
        }

        [Fact]
        public void GetProfanities_InputIsNull_ReturnsEmptyCollection()
        {
            var filter = new ProfanityFilter();
            var swearList = filter.GetProfanities(null);
            Assert.Equal(0, swearList.Count);
        }

        [Fact]
        public void GetProfanities_InputContainsTwoProhibitedTerms_ReturnsTwoTerms()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("twat");
            filter.Terms.Prohibited.Add("dick");
            var swearList = filter.GetProfanities("You are a complete twat and a dick.");
            Assert.Equal(2, swearList.Count);
        }

        [Fact]
        public void GetProfanities_InputContainsProhibitedTermWithTrailingComma_ReturnsTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("twat");

            var swearList = filter.GetProfanities("You sir are, a complete twat, and a gentleman");
            Assert.Equal(1, swearList.Count);
            Assert.Equal("twat", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_InputContainsProhibitedTermWithMixedCase_ReturnsTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("twat");
            filter.Terms.Prohibited.Add("dick");
            var swearList = filter.GetProfanities("You are a complete tWaT");
            
            Assert.Equal(1, swearList.Count);
            Assert.Equal("twat", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_InputContainsProhibitedTerm_ReturnsTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("2 girls 1 cup");
            var swearList = filter.GetProfanities("2 girls 1 cup is my favourite video");

            Assert.Equal(1, swearList.Count);
            Assert.Equal("2 girls 1 cup", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_InputContainsScunthorpeProblem_ReturnsTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fucking");
            filter.Terms.Prohibited.Add("cock");
            filter.Terms.Prohibited.Add("fuck");
            filter.Terms.Prohibited.Add("shit");
            filter.Terms.Prohibited.Add("cunt");
            filter.Terms.Permitted.Add("scunthorpe");
            filter.Terms.Permitted.Add("penistone");

            var swearList = filter.GetProfanities("I fucking live in Scunthorpe and it is a shit place to live. I would much rather live in penistone you great big cock fuck.");

            Assert.Equal(4, swearList.Count);
            Assert.Contains("fucking", swearList);
            Assert.Contains("cock", swearList);
            Assert.Contains("shit", swearList);
            Assert.Contains("fuck", swearList);
        }

        [Fact]
        public void GetProfanities_BothProhibitedAndPermittedTermsPresent_ReturnsProhibitedTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("twat");
            filter.Terms.Permitted.Add("tit");

            var swearList = filter.GetProfanities("You are a complete twat and a total tit.");

            Assert.Equal(1, swearList.Count);
            Assert.Equal("twat", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_MultiplePermittedTermsPresent_NoProhibitedWordsReturned()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Permitted.Add("scunthorpe");
            var swearList = filter.GetProfanities("Scunthorpe Scunthorpe");
            Assert.Equal(0, swearList.Count);
        }

        [Fact]
        public void GetProfanities_PermittedTermsPresentMultipleTimesAndProhibitedWordPresent_ReturnsProhibitedTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var swearList = filter.GetProfanities("Scunthorpe cunt Scunthorpe");
            Assert.Equal(1, swearList.Count);
            Assert.Equal("cunt", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_MultiplePermittedTermsPresentAndMultipleProhibitedWordsPresent_ReturnsProhibitedTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var swearList = filter.GetProfanities("Scunthorpe cunt Scunthorpe cunt");
            Assert.Equal(1, swearList.Count);
            Assert.Equal("cunt", swearList.ElementAt(0));
        }

        [Fact]
        public void GetProfanities_InputIsProhibitedWordExactMatch_ReturnsProhibitedTermCorrectly()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cock");

            var swearList = filter.GetProfanities("cock");
            Assert.Equal(1, swearList.Count);
            Assert.Equal("cock", swearList.ElementAt(0));
        }

        [Fact]
        public void Censor_HasPermittedTermsAndInputContainsProhibitedTerms_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fuck");
            filter.Terms.Prohibited.Add("fucking");
            filter.Terms.Prohibited.Add("cock");
            filter.Terms.Prohibited.Add("shit");
            filter.Terms.Permitted.Add("scunthorpe");
            filter.Terms.Permitted.Add("penistone");

            var result = filter.Censor("I fucking live in Scunthorpe and it is a shit place to live. I would much rather live in penistone you great big cock fuck.", '*');
            const string expected = "I ******* live in Scunthorpe and it is a **** place to live. I would much rather live in penistone you great big **** ****.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsProhibitedTerms_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fuck");
            filter.Terms.Prohibited.Add("fucking");
            filter.Terms.Prohibited.Add("cock");
            filter.Terms.Prohibited.Add("shit");
            filter.Terms.Prohibited.Add("cunt");

            var result = filter.Censor("I fucking live in Scunthorpe and it is a shit place to live. I would much rather live in penistone you great big cock fuck.", '*');
            const string expected = "I ******* live in Scunthorpe and it is a **** place to live. I would much rather live in penistone you great big **** ****.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsCapitalisedProhibitedTerms_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fuck");
            filter.Terms.Prohibited.Add("fucking");
            filter.Terms.Prohibited.Add("cock");
            filter.Terms.Prohibited.Add("shit");
            filter.Terms.Prohibited.Add("cunt");

            var result = filter.Censor("I Fucking Live In Scunthorpe And It Is A Shit Place To Live. I Would Much Rather Live In Penistone You Great Big Cock Fuck.", '*');
            const string expected = "I ******* Live In Scunthorpe And It Is A **** Place To Live. I Would Much Rather Live In Penistone You Great Big **** ****.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsMultipleProhibitedTerms_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("2 girls 1 cup");
            filter.Terms.Prohibited.Add("twatting");

            var result = filter.Censor("2 girls 1 cup, is my favourite twatting video.");
            const string expected = "* ***** * ***, is my favourite ******** video.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsProhibitedTermWithTrailingPeriod_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fucker");
            filter.Terms.Prohibited.Add("shit");

            var result = filter.Censor("Mary had a little shit lamb who was a little fucker.");
            const string expected = "Mary had a little **** lamb who was a little ******.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsProhibitedTermWithTrailingComma_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("twat");

            var result = filter.Censor("You are a stupid little twat, did you know that?");
            const string expected = "You are a stupid little ****, did you know that?";

            Assert.Equal(expected, result);
        }


        [Fact]
        public void Censor_InputContainsPermittedTerm_ReturnsOriginalString()
        {
            var filter = new ProfanityFilter();

            var result = filter.Censor("Scunthorpe");
            const string expected = "Scunthorpe";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsPermittedTermInMixedCase_ReturnsOriginalString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Permitted.Add("scunthrope");
            var result = filter.Censor("ScUnThOrPe");
            const string expected = "ScUnThOrPe";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsPermittedTermInLowerCase_ReturnsOriginalString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            filter.Terms.Permitted.Add("scunthorpe");

            var result = filter.Censor("scunthorpe");
            const string expected = "scunthorpe";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsProhibitedTermTwice_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var result = filter.Censor("cunt cunt");
            const string expected = "**** ****";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputEndsWithProhibited_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var result = filter.Censor("scunthorpe cunt");
            const string expected = "scunthorpe ****";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsProhibitedTermsAndAlternateReplacementCharProvided_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("2 girls 1 cup");
            filter.Terms.Prohibited.Add("twatting");
            var result = filter.Censor("2 girls 1 cup, is my favourite twatting video.", '@');
            const string expected = "@ @@@@@ @ @@@, is my favourite @@@@@@@@ video.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputIsEmptyAndAlternateReplacementCharProvided_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            var result = filter.Censor("", '@');
            const string expected = "";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsNoProhibitedTermsAndAlternateReplacementCharProvided_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("2 girls 1 cup");
            filter.Terms.Prohibited.Add("twatting");
            var result = filter.Censor("Hello, I am a fish.", '*');
            const string expected = "Hello, I am a fish.";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputHasProhibitedTermAndTrailingSpace_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("fuck");
            var result = filter.Censor("Hello you little fuck ");
            const string expected = "Hello you little **** ";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputContainsOnlyWhitespace_ReturnsOriginalString()
        {
            var filter = new ProfanityFilter();

            var result = filter.Censor("     ");
            const string expected = "     ";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputIsOnlyNonAlphaNumericCharacters_ReturnsOriginalString()
        {
            var filter = new ProfanityFilter();

            var result = filter.Censor("!@£$*&^&$%^$£%£$@£$£@$£$%%^");
            const string expected = "!@£$*&^&$%^$£%£$@£$£@$£$%%^";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputHasNumberAppendedToProhibitedTerm_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("motherfucker");
            var result = filter.Censor("You are a motherfucker1", '*');
            const string expected = "You are a *************";

            Assert.Equal(expected, result);
        }


        [Fact]
        public void Censor_InputHasMultipleNumbersAppendedToProhibitedTerm_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("motherfucker");
            var result = filter.Censor("You are a motherfucker123", '*');
            const string expected = "You are a ***************";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputHasNumberPrependedToProhibitedTerm_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("motherfucker");
            var result = filter.Censor("You are a 1motherfucker", '*');
            const string expected = "You are a *************";

            Assert.Equal(expected, result);
        }


        [Fact]
        public void Censor_InputHasMultipleNumbersPrependedToProhibitedTerm_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("motherfucker");
            var result = filter.Censor("You are a 123motherfucker", '*');
            const string expected = "You are a ***************";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputHasMultipleNumbersAppendedAndPrependedToProhibitedTerm_ReturnsCensoredString()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("motherfucker");
            var result = filter.Censor("You are a 123motherfucker123", '*');
            const string expected = "You are a ******************";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Censor_InputIsEmpty_ReturnsEmptyCollection()
        {
            var filter = new ProfanityFilter();

            var result = filter.Censor("");
            const string expected = "";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ContainsProfanity_InputContainsProhibitedTermEmbeddedInWord_ReturnsFalse()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var result = filter.ContainsProfanity("Scunthorpe");
            Assert.False(result);
        }

        [Fact]
        public void ContainsProfanity_InputContainsProhibitedTermWithTrailingNumbers_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            var result = filter.ContainsProfanity("cunt123");
            Assert.True(result);
        }
        
        [Fact]
        public void ContainsProfanity_InputContainsTwoProhibitedTermsThatAreAlsoPermittedWords_ReturnsFalse()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("cunt");
            filter.Terms.Prohibited.Add("arse");
            filter.Terms.Permitted.Add("cunt");
            filter.Terms.Permitted.Add("arse");
            var result = filter.ContainsProfanity("Scuntarsehorpe cunt arse");
            Assert.False(result);
        }

        [Fact]
        public void ContainsProfanity_InputContainsNoProhibitedTerms_ReturnsFalse()
        {
            var filter = new ProfanityFilter();
            var result = filter.ContainsProfanity("Ireland");
            Assert.False(result);
        }

        [Fact]
        public void ContainsProfanity_InputContainsDisguisedProhibitedTerm_ReturnsTrue()
        {
            var filter = new ProfanityFilter();
            filter.Terms.Prohibited.Add("ass");
            var result = filter.ContainsProfanity("a$$");
            Assert.True(result);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullTerms()
        {
            Assert.Throws<ArgumentNullException>(() => {
                _ = new ProfanityFilter(null);
            });
        }

        [Fact]
        public void Constructor_TermsProvided_UsesTermsProvided()
        {          
            var terms = new Terms();
            terms.Prohibited.Add("fuck");
            terms.Prohibited.Add("shit");
            terms.Prohibited.Add("bollocks");

            IProfanityFilter filter = new ProfanityFilter(terms);
            Assert.Equal(3, filter.Terms.Prohibited.Count());
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullWordList()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ProfanityFilter(null));
        }
    }
}