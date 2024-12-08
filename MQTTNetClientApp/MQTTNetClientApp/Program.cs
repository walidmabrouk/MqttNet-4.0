// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client;

// Create TCP based options using the builder.

var factory = new MqttFactory();
var _mqttClient = factory.CreateMqttClient();
Console.Write("MQTT Client ID :");
string? mqttID = Console.ReadLine();
// Create client options object
MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId(mqttID)
                                        .WithTcpServer("127.0.0.1");
var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 5004) // Adresse et port du serveur MQTT
            .WithClientId(mqttID)        // Identifiant unique du client
            .WithCleanSession()               // Nettoie les sessions persistantes
            .Build();



// Set up handlers
_mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync;


_mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;
// Connect to the broker
var connectResult = await _mqttClient.ConnectAsync(mqttClientOptions);
Console.WriteLine("client connected");

Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
{
    Console.WriteLine("Connected x");
    return Task.CompletedTask;
};
Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
{
    Console.WriteLine("Disconnected");
    return Task.CompletedTask;
};
Console.WriteLine("");
Console.Write("Sender or Receiver : ");
string? choice = Console.ReadLine()?.ToLower();

switch (choice)
{
    case "sender":
       
        publish("topic/temperature", true);
        break;
    case "receiver":
        Subsribe("topic/temperature");
        break;
    default: Console.WriteLine("bad choise :( "); break;
}





async void Subsribe(string topic)
{
    var mqttSubscribeOptions = factory.CreateSubscribeOptionsBuilder().WithTopicFilter(topic).Build();

    await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    Console.WriteLine($"MQTT client subscribed to {topic}.");
    _mqttClient.ApplicationMessageReceivedAsync += async e =>
    {
        string payload = e.ApplicationMessage.ConvertPayloadToString();
        double temperature = Convert.ToDouble(payload); 
        if (temperature > 37.5)
            Console.ForegroundColor= ConsoleColor.Red;
        else
            Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine($"{payload}");
    };
}
async void publish(string topic,  bool loop)
{
    Console.WriteLine($"MQTT client Push Msg to {topic}.");
    Random random = new Random();
    do
    {
        
        string msg = (random.NextDouble() * (40.0 - 30.0) + 30.0).ToString("F2");
        var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(msg)
                    .Build();
        await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        await Task.Delay(3000);
    } while (loop);


}

Thread.Sleep(2000);

Console.WriteLine("Press enter to exit.");
Console.ReadLine();

