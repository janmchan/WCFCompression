using System;
using WCFClient.WcfServiceDemoProxy;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new Service1Client();
            var result = proxy.GetData(1);
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
