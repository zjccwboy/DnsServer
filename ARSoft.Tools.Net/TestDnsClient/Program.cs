using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace TestDnsClient
{
    class Program
    {
        const int QUERY_TIMEOUT = 30000;
        static void Main(string[] args)
        {

            bool isSuccess;
            IPAddress ip = DnsResolver("127.0.0.1", 200, "www.sina.com.cn", out isSuccess);
            if (isSuccess)
                Console.WriteLine(ip);

            Console.Read();
        }

        /// 

        /// DNS解析
        /// 
        /// DNS服务器IP
        /// 解析超时时间
        /// 解析网址
        /// 是否解析成功
        /// 解析到的IP信息
        public static IPAddress DnsResolver(string dnsServer, int timeOut, string url, out bool isSuccess)
        {
            //初始化DnsClient，第一个参数为DNS服务器的IP，第二个参数为超时时间
            var dnsClient = new DnsClient(IPAddress.Parse(dnsServer), timeOut);
            //解析域名。将域名请求发送至DNS服务器解析，第一个参数为需要解析的域名，第二个参数为
            //解析类型， RecordType.A为IPV4类型
            //DnsMessage dnsMessage = dnsClient.Resolve("www.sina.com", RecordType.A);
            var s = new Stopwatch();
            s.Start();
            var dnsMessage = dnsClient.Resolve(DomainName.Parse(url));
            s.Stop();
            Console.WriteLine(s.Elapsed.Milliseconds);
            //若返回结果为空，或者存在错误，则该请求失败。
            if (dnsMessage == null || (dnsMessage.ReturnCode != ReturnCode.NoError && dnsMessage.ReturnCode != ReturnCode.NxDomain))
            {
                isSuccess = false;
            }
            //循环遍历返回结果，将返回的IPV4记录添加到结果集List中。
            if (dnsMessage != null)
                foreach (var dnsRecord in dnsMessage.AnswerRecords)
                {
                    var aRecord = dnsRecord as ARecord;
                    if (aRecord == null) continue;
                    isSuccess = true;
                    return aRecord.Address;
                }
            isSuccess = false;
            return null;
        }

    }
}
