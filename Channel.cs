using Channel_SDK.Events;
using Channel_SDK.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using WebSocket4Net;

namespace Channel_SDK
{
    public class Channel
    {
        public enum MessageType
        {
            PlainMsg,
            EmbedMsg,
            ArkMsg,
            PluginInfo,
            CallResult,
        }
        public enum CallResult
        {
            Pass,
            Block
        }
        public static WebSocket Instance;
        public event Action<Message> OnDispatchMessage;
        public event Action<Message> OnATMessage;

        private string url;
        private bool isConnected = false;
        public Channel(string url = "ws://localhost:6235/main")
        {
            this.url = url;
            Instance = new(url);
        }
        private void WebsocketInit()
        {
            Instance.Opened += WebSocket_Opened;
            Instance.MessageReceived += WebSocket_MessageReceived;
            Instance.Error += Instance_Error;
            Instance.Closed += Instance_Closed;
        }

        private void Instance_Closed(object sender, EventArgs e)
        {
            isConnected = false;
            Helper.OutError($"与服务端连接断开...3秒后尝试重连，按Ctrl+C中断...");
            Thread.Sleep(1000 * 3);
            Instance.Dispose();
            Instance = new(url);
            Connect();
        }

        private void Instance_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Helper.OutError($"WebSocket连接出错:{e.Exception.Message}");
        }

        public void Connect()
        {
            WebsocketInit();
            Instance.Open();
        }
        private static int msgSeq { get; set; } = 0;
        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            JObject json = JObject.Parse(e.Message);
            msgSeq = ((int)json["seq"]);
            switch (json["type"].ToString())
            {
                case "Dispatch":
                    var message = json["data"]["msg"]["d"].ToObject<Message>();
                    Console.WriteLine($"receive msg: {message.content}");
                    OnDispatchMessage?.Invoke(message);
                    if (json["data"]["msg"]["t"].ToString() == "AT_MESSAGE_CREATE")
                    {
                        OnATMessage?.Invoke(message);
                    }
                    break;
                default:
                    break;
            }
            if(OnATMessage==null)
            {
                Send_CallResult(CallResult.Pass);
            }
        }
        private void WebSocket_Opened(object sender, EventArgs e)
        {
            isConnected = true;
            Helper.OutLog("Connected");
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
        public static void Send_CallResult(CallResult result)
        {
            SendMsg(MessageType.CallResult, new { result });
        }
        private static void SendMsg(MessageType msgType, object msg)
        {
            JObject msgToSend = new()
            {
                { "type", (int)msgType },
                { "seq", msgSeq}
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
