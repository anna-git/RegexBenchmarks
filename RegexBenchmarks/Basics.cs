using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Benchmarks.Dotnet
{
    public class Basics
    {
        public const string Pattern = @"((?i)(?:p(?:ass)?w(?:or)?d|pass(?:_?phrase)?|secret|(?:api_?|private_?|public_?|access_?|secret_?)key(?:_?id)?|token|consumer_?(?:id|key|secret)|sign(?:ed|ature)?|auth(?:entication|orization)?)(?:(?:\s|%20)*(?:=|%3D)[^&]+|(?:""|%22)(?:\s|%20)*(?::|%3A)(?:\s|%20)*(?:""|%22)(?:%2[^2]|%[^2]|[^""%])+(?:""|%22))|bearer(?:\s|%20)+[a-z0-9\._\-]|token(?::|%3A)[a-z0-9]{13}|gh[opsu]_[0-9a-zA-Z]{36}|ey[I-L](?:[\w=-]|%3D)+\.ey[I-L](?:[\w=-]|%3D)+(?:\.(?:[\w.+\/=-]|%3D|%2F|%2B)+)?|[\-]{5}BEGIN(?:[a-z\s]|%20)+PRIVATE(?:\s|%20)KEY[\-]{5}[^\-]+[\-]{5}END(?:[a-z\s]|%20)+PRIVATE(?:\s|%20)KEY|ssh-rsa(?:\s|%20)*(?:[a-z0-9\/\.+]|%2F|%5C|%2B){100,})";
        #region awfullongstring
        public IEnumerable<object[]> QueryStrings() // for multiple arguments it's an IEnumerable of array of objects (object[])
        {
            for (int i = 1000; i < 3001; i+=1000)
            {
                yield return new object[] { string.Concat(Enumerable.Repeat("a", i)) };
            }
        }
        private Regex? regex;
        #endregion

        [GlobalSetup]
        public void Setup()
        {
            regex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));
        }
        [Benchmark]
        [ArgumentsSource(nameof(QueryStrings))]
        public void BasicTestIncremental(string queryString)
        {
            var m = regex.Match(queryString);
        }
    }
}
