using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace DnsClientTester
{
    class Program
    {
        const int QUERY_TIMEOUT = 30000;
        static void Main(string[] args)
        {
            Start();

            Console.Read();
        }

        static int Count;
        static Stopwatch Stopwatch = new Stopwatch();

        static void Start()
        {
            Stopwatch.Start();
            var serverIP = "103.195.51.147";
            var timeOut = 200;
            for (var i = 0; i < 100; i++)
            {
                var dnsClient = new DnsClient(IPAddress.Parse(serverIP), timeOut);
                DnsResolver(dnsClient);
            }
        }

        static async void DnsResolver(DnsClient dnsClient)
        {
            const string domain = "www.sina.com.cn";
            while (true)
            {
                var ip = await DnsResolverAsync(dnsClient, domain);
                if (ip != null)
                {
                    Count++;
                    if(Count % 10000 == 0)
                    {
                        Console.WriteLine($"耗时:{Stopwatch.ElapsedMilliseconds}/ms 完成请求数:{Count}/次");
                        Stopwatch.Restart();
                        Count = 0;
                    }
                }
            }
        }

        static async Task<IPAddress> DnsResolverAsync(DnsClient dnsClient, string domain)
        {
            var dnsMessage = await dnsClient.ResolveAsync(DomainName.Parse(domain));
            if (dnsMessage == null || (dnsMessage.ReturnCode != ReturnCode.NoError && dnsMessage.ReturnCode != ReturnCode.NxDomain))
                return null;

            if (dnsMessage != null)
                foreach (var dnsRecord in dnsMessage.AnswerRecords)
                {
                    var aRecord = dnsRecord as ARecord;
                    if (aRecord == null) continue;
                    return aRecord.Address;
                }
            return null;
        }

    }
}
