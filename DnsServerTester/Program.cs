using ARSoft.Tools.Net.Dns;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DnsServerTester
{
    class Program
    {
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        static void Main(string[] args)
        {

            var maxConnection = 100;
            DnsServer dnsServer = new DnsServer(maxConnection, maxConnection);
            dnsServer.QueryReceived += DnsServer_QueryReceived;
            dnsServer.Start();

            Console.Read();
        }

        private static Task DnsServer_QueryReceived(object sender, QueryReceivedEventArgs eventArgs)
        {
            eventArgs.Query.IsQuery = false;
            DnsMessage query = eventArgs.Query as DnsMessage;
            if (query == null || query.Questions.Count <= 0)
                query.ReturnCode = ReturnCode.ServerFailure;
            else
            {
                if (query.Questions[0].RecordType == RecordType.A)
                {                    
                    foreach (DnsQuestion dnsQuestion in query.Questions)
                    {
                        //string resolvedIp = Resolve(clientAddress.ToString(), dnsQuestion.Name);
                        ARecord aRecord = new ARecord(dnsQuestion.Name, 36000, ipAddress);
                        query.AnswerRecords.Add(aRecord);
                    }
                }
                else
                    return null;
            }
            eventArgs.Response = eventArgs.Query;
            return Task.CompletedTask;
        }
    }
}
