var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer.AddDatabase("marketlens-db");

var redis = builder.AddRedis("marketlens-cache")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.MarketLens_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(redis)
    .WaitFor(redis)
    .WithExternalHttpEndpoints();

builder.Build().Run();
