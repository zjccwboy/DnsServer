using ARSoft.Tools.Net.Dns;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TestDnsServer
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
            Console.WriteLine("Hello World!");
        }

        private static Task DnsServer_QueryReceived(object sender, QueryReceivedEventArgs eventArgs)
        {
            eventArgs.Response = ProcessQuery(eventArgs.Query, eventArgs.RemoteEndpoint.Address, eventArgs.ProtocolType);
            return Task.CompletedTask;
        }

        //委托实现方法，可自定义解析规则
        static DnsMessageBase ProcessQuery(DnsMessageBase message, IPAddress clientAddress, ProtocolType protocol)
        {
            message.IsQuery = false;
            DnsMessage query = message as DnsMessage;
            if (query == null || query.Questions.Count <= 0)
                message.ReturnCode = ReturnCode.ServerFailure;
            else
            {
                if (query.Questions[0].RecordType == RecordType.A)
                {
                    //自定义解析规则，clientAddress即客户端的IP，dnsQuestion.Name即客户端请求的域名，Resolve为自定义的方法（代码不再贴出），返回解析后的ip，将其加入AnswerRecords中
                    foreach (DnsQuestion dnsQuestion in query.Questions)
                    {
                        //string resolvedIp = Resolve(clientAddress.ToString(), dnsQuestion.Name);
                        ARecord aRecord = new ARecord(dnsQuestion.Name, 36000, ipAddress);
                        query.AnswerRecords.Add(aRecord);
                    }
                }
                else
                    return null;
                //如果为IPV6请求，则交给上级DNS服务器处理，代码不再贴出
            }
            return message;
        }
    }
}
