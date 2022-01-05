using Channel_SDK.Events;
using Channel_SDK.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WebSocket4Net;

namespace Channel_SDK
{
    public class Channel
    {
        public enum MessageType
        {
            PlainMsg,
            EmbedMsg,
            ArkMsg
        }
        public static WebSocket Instance;
        public event Action<Message> OnDispatchMessage;
        public Channel(string url = "ws://localhost:6235/main")
        {
            Instance = new(url);
        }
        private void WebsocketInit()
        {
            Instance.Opened += WebSocket_Opened;
            Instance.MessageReceived += WebSocket_MessageReceived;
        }
        public void Connect()
        {
            WebsocketInit();
            Instance.Open();
        }
        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            JObject json = JObject.Parse(e.Message);
            switch (json["type"].ToString())
            {
                case "Dispatch":
                    var message = json["data"]["msg"]["d"].ToObject<Message>();
                    Console.WriteLine($"receive msg: {message.content}");
                    OnDispatchMessage(message);
                    break;
                default:
                    break;
            }
        }
        private void WebSocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }
        public static void Send_PlainMessage(string channelID, string msg_id = "", string msg = "", string imageUrl = "")
        {
            if (string.IsNullOrWhiteSpace(msg) && string.IsNullOrWhiteSpace(imageUrl))
            {
                Helper.OutError("文本消息与图片消息不可同时为空");
                return;
            }
            var json = new { channelID, content = new { msg_id , image = imageUrl, content = msg } };
            SendMsg(MessageType.PlainMsg, json);
        }
        private static void SendMsg(MessageType msgType, object msg)
        {
            JObject msgToSend = new()
            {
                { "type", (int)msgType },
            };
            if (msg is null)
                msgToSend.Add("data", null);
            else
                msgToSend.Add("data", JToken.FromObject(msg));
            Helper.OutLog($"推送消息:{msgToSend.ToString(Formatting.None)}");
            Instance.Send(msgToSend.ToString(Formatting.None));
        }
    }
}
