# Channel-SDK
用于QQ频道的辣鸡C# SDK

## 使用说明
0. 启动[Channel-Core](https://github.com/Hellobaka/Channel-Core)
1. 给你的项目安装这个这个[nuget](https://github.com/Hellobaka/Channel-SDK/releases/latest)
2. 写完逻辑就启动程序
```csharp
using Channel_SDK;

static void Main(string[] args)
{
    string url = "ws://127.0.0.1:6235/main";//Channel-Core 的ws
    Channel channel = new (url);
    channel.PluginInfo = new()
    {
        Author = "Hellobaka",
        Version = "1.0.0",
        Name = "无情摇骰机",
        Description = "发送r就会回复0-6的数字"
    };
    channel.OnATMessage += Channel_OnATMessage;
    channel.Connect();

    while (true)
    {
        Console.ReadLine();
    }
}

private static void Channel_OnATMessage(Channel_SDK.Model.Message msg)
{
    if(msg.nonATMsg == "r")
    {
        msg.Answer($"r: {new Random().Next(0, 6)}");
    }
    Channel.Send_CallResult(CallResult.Block);//必需，否则主程序会卡壳
}
```
## 接口进度
- [x] 发送普通图文信息
- [x] 接收AT消息
...
## 待填坑
- [ ] 完善接口
- [x] 服务端退出自动退出
