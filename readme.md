# Channel-SDK
用于QQ频道的辣鸡C# SDK

## 使用说明
1. 打开`Channel-Core`
2. 引用本项目
```csharp
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
```