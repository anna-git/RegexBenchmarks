using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Benchmarks.Dotnet
{
    public class ManualVsBuildTimeout
    {
        public const string oldPattern = @"(?i)(?:p(?:ass)?w(?:or)?d|pass(?:_?phrase)?|secret|(?:api_?|private_?|public_?|access_?|secret_?)key(?:_?id)?|token|consumer_?(?:id|key|secret)|sign(?:ed|ature)?|auth(?:entication|orization)?)(?:\s*=[^&]+|""\s*:\s*""[^""]+"")|bearer\s+[a-z0-9\._\-]|token:[a-z0-9]{13}|gh[opsu]_[0-9a-zA-Z]{36}|ey[I-L][\w=-]+\.ey[I-L][\w=-]+(?:\.[\w.+\/=-]+)?|[\-]{5}BEGIN[a-z\s]+PRIVATE\sKEY[\-]{5}[^\-]+[\-]{5}END[a-z\s]+PRIVATE\sKEY|ssh-rsa\s*[a-z0-9\/\.+]{100,}";
        public const string Pattern = @"((?i)(?:p(?:ass)?w(?:or)?d|pass(?:_?phrase)?|secret|(?:api_?|private_?|public_?|access_?|secret_?)key(?:_?id)?|token|consumer_?(?:id|key|secret)|sign(?:ed|ature)?|auth(?:entication|orization)?)(?:(?:\s|%20)*(?:=|%3D)[^&]+|(?:""|%22)(?:\s|%20)*(?::|%3A)(?:\s|%20)*(?:""|%22)(?:%2[^2]|%[^2]|[^""%])+(?:""|%22))|bearer(?:\s|%20)+[a-z0-9\._\-]|token(?::|%3A)[a-z0-9]{13}|gh[opsu]_[0-9a-zA-Z]{36}|ey[I-L](?:[\w=-]|%3D)+\.ey[I-L](?:[\w=-]|%3D)+(?:\.(?:[\w.+\/=-]|%3D|%2F|%2B)+)?|[\-]{5}BEGIN(?:[a-z\s]|%20)+PRIVATE(?:\s|%20)KEY[\-]{5}[^\-]+[\-]{5}END(?:[a-z\s]|%20)+PRIVATE(?:\s|%20)KEY|ssh-rsa(?:\s|%20)*(?:[a-z0-9\/\.+]|%2F|%5C|%2B){100,})";
        private Regex regex;
        private string input = $"?token=?authentic1=val1&token=a0b21ce2-006f-4cc6-95d5-d7b550698482&key2=val2&authentic2=val2&token2=a0b21ce2-006f-4cc6-95d5-d7b550698482&{string.Concat(Enumerable.Repeat("a", 6000))}";

        [GlobalSetup(Target = (nameof(ManualTimeoutTest)))]
        public void SetupManual()
        {
            regex = new Regex(Pattern, RegexOptions.Compiled);
        }

        [GlobalSetup(Target = (nameof(TimeoutTest)))]
        public void SetupTimeout()
        {
            regex = new Regex(Pattern, RegexOptions.Compiled, timeSpan);
        }

        private readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(100);
        
        [Benchmark]
        public void ManualTimeoutTest()
        {

            try
            {
                Task<string> task = Task.Factory.StartNew(() => ExecRegex());
                task.Wait(timeSpan);
                if (task.IsCompletedSuccessfully)
                {
                    var res = task.Result;
                }
            }
            catch (AggregateException)
            {
            }
        }

        [Benchmark]
        public void TimeoutTest()
        {
            try
            {
                var res = ExecRegex();
            }
            catch (RegexMatchTimeoutException)
            {
            }
        }

        private string ExecRegex() => regex.Replace(input, "<redacted>");
    }
}
