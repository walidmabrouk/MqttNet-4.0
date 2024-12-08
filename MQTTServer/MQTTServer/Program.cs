// See https://aka.ms/new-console-template for more information
// Configurer les options du serveur MQTT
using MQTTnet;
using MQTTnet.Server;
using System.Net;
using System.Text;



// Create the options for MQTT Broker
var options = new MqttServerOptionsBuilder()
    //Set endpoint to localhost
    //.WithDefaultEndpointBoundIPAddress(IPAddress.Parse("192.168.1.14"))
    .WithDefaultEndpoint()
    // Port is going to use 5004
    .WithDefaultEndpointPort(5004);

// Create a new mqtt server
var server = new MqttFactory().CreateMqttServer(options.Build());
//Add Interceptor for logging incoming messages
server.InterceptingPublishAsync += Server_InterceptingPublishAsync;


server.ClientConnectedAsync += e =>
{
    Console.WriteLine($"Client connecté : {e.ClientId}");
    return Task.CompletedTask;
};

server.ClientDisconnectedAsync += e =>
{
    Console.WriteLine($"Client déconnecté : {e.ClientId}");
    return Task.CompletedTask;
};
try
{
    // Démarrage du serveur MQTT
    await server.StartAsync();
    Console.WriteLine($"Serveur MQTT démarré sur localhost:{5004}");
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur lors du démarrage du serveur : {ex.Message}");
}


// Attente pour voir si le serveur reste actif
Console.WriteLine("Appuyez sur Enter pour arrêter le serveur.");
Console.ReadLine();



Task Server_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
{
    // Convert Payload to string
    var payload = arg.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage?.Payload);


    Console.WriteLine(
        $" TimeStamp: {DateTime.Now} -- Message: ClientId = {arg.ClientId}, Topic = {arg.ApplicationMessage?.Topic}," +
        $" Payload = {payload}"
        );
    return Task.CompletedTask;
}