// See https://aka.ms/new-console-template for more information
using Garnet;
using Microsoft.Extensions.DependencyInjection;
using Server;

IServiceCollection services = new ServiceCollection();
services.AddSingleton<VectorSum>();
await using ServiceProvider provider = services.BuildServiceProvider();

try {
    using var server = new GarnetServer(args);

    // Start the server
    server.Start();
    VectorSum vectorSum = provider.GetRequiredService<VectorSum>();
    server.Register.NewProcedure("VECTOR.SUM", vectorSum);
    Thread.Sleep(Timeout.Infinite);
}
catch (Exception ex) {
    Console.WriteLine($"Unable to initialize server due to exception: {ex.Message}");
}