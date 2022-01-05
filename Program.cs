using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WebSocket4Net;

namespace Channel_SDK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "ws://127.0.0.1:6235/main";
            Channel channel = new (url);
            channel.Connect();
            channel.OnDispatchMessage += Channel_OnDispatchMessage;
            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void Channel_OnDispatchMessage(Model.Message msg)
        {
            if(msg.nonATMsg == "r")
            {
                msg.Answer($"r: {new Random().Next(0, 6)}");
            }
        }
    }
}
