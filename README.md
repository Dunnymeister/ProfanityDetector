# ProfanityDetector

## About

This is a .NET library for detecting profanities within a text string.

### Thanks
eBooks.com would like to thank Stephen Haunts, the original author of this library, for their work on both the code and and for compiling the initial profanity list from other lists on the internet that are allegedly used by social media sites.

## Using the Library via Nuget 

##### Using the Package Manager Console

Installation via the Package Manager Console:

```
Install-Package Ebooks.ProfanityDetector
```

Installation via Visual Studio's NuGet Package Manager:
1. Open Solution Explorer
2. Right-click on the project within your solution you want to add ProfanityDetector to
3. Click on *Manage NuGet Packages...*
4. Click on the Browse tab and search for *Ebooks.ProfanityDetector*
5. Click on the *Ebooks.ProfanityDetector* package, select the version you wish to install in the right-tab and then click *Install*

## Usage

### Getting Started: Hello, world!
```csharp
var filter = new ProfanityFilter();
filter.Terms.Prohibited.Add("world");
var result = filter.Censor("Hello, world!");
Console.WriteLine(result); // Prints "Hello, *****!"
```

### Check if a Word is Classed as a Profanity

```csharp
  var filter = new ProfanityFilter();
  filter.Terms.Prohibited.Add("blue");

  // Console output will 
  if (filter.IsProfanity("blue")) 
  {
      Console.WriteLine("Input is prohibited!");
  }
```

### Return All Profanities Within a Sentence

```csharp
var filter = new ProfanityFilter();
filter.Terms.Prohibited.Add("green");
filter.Terms.Prohibited.Add("blue");
filter.Terms.Prohibited.Add("red");

var prohibitedTerms = filter.GetProfanities("A secondary color is formed by the sum of two primary colors of equal intensity: cyan is green+blue, magenta is blue+red, and yellow is red+green.");

// Writes "Found prohibited terms: green, blue, red" to console
Console.WriteLine($"Found prohibited terms: {string.Join(", ", prohibitedTerms)}");
```

### Censor Profanities Within a Sentence

```csharp
  var filter = new ProfanityFilter();
  filter.Terms.Prohibited.Add("stealeth");
  filter.Terms.Prohibited.Add("rend");
  var result = filter.Censor("For him that stealeth, or borroweth and returneth not, this book from its owner, let it change into a serpent in his hand & rend him.");
  
  // Writes "For him that ********, or borroweth and returneth not, this book from its owner, let it change into a serpent in his hand & **** him." to console
  Console.WriteLine(result);
```

## Issues and Limitations

Any profanity filter based on a word list is going to fail at some point as users attempt to obfuscate input. We do not recommend using this library in place of moderation, but we hope that it will make your moderators lives easier by detecting simple profanity before it reaches them.

### The Scunthorpe Problem

A common problem with the profanity detector is solving what is called the [Scunthorpe Problem](https://en.wikipedia.org/wiki/Scunthorpe_problem). This is where you get a false-positive result from a profanity detector because a profanity pattern is found inside a non-profane word. For example, with "Scunthorpe" (which is a town in the United Kingdom), it will get reported as containing the word "c@nt". 

This profanity detector library guards against this problem in two ways:

1. Adding known "good" words to an allow list of words that are to be excluded from the profanity detector.
2. If a profanity is enclosed in a larger word, the parent word is also checked against the profanity list. If that is not in the list, which Scunthorpe isn't, then the word is ignored. If the parent word is in the profanity list, then it will be reported as so.

## Adding and Removing Profanties

Our understanding is that most organisations will maintain their own list of prohibited terms. As a result we have chosen to ship the ProfanityFilter class without any profanities populated by default.

We have created a second NuGet package *Ebooks.ProfanityDetectorExtensions* you can reference to easily populate your prohibited terms with commonly banned English words. This list was initially populated from Stephen Haunts' curated list and will be expanded upon over time.

#### Using the Sample List

First make sure you've included the base *Ebooks.ProfanityDetector* NuGet package and then
add a reference to the NuGet package *Ebooks.ProfanityDetector.Extensions*

You can then create a ProfanityFilter and populate it with the default list of English prohibited terms by:

```csharp
var filter = new ProfanityFilter().UseDefaults();
```

## Using Your Own Profanity List

Add a single term:
```csharp
filter.Terms.Prohibited.Add("naughty");
```

Overwrite the entire list:
```csharp
var filter = new ProfanityFilter(new Terms() 
{ 
  Prohibited = new HashSet<string>() { "naughty", "bad", "terrible", "alarming" }
});
```

or:

```csharp
var myProhibitedWords = new HashSet<string>() { "naughty", "bad", "terrible", "alarming" };
filter.Terms.Prohibited = myProhibitedWords;
```