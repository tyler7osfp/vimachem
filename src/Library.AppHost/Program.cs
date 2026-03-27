var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var libraryDb = postgres.AddDatabase("librarydb");

var mongo = builder.AddMongoDB("mongo");
var eventDb = mongo.AddDatabase("eventdb");

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

builder.AddProject<Projects.Library_Api>("library-api")
    .WithReference(libraryDb)
    .WithReference(rabbitmq);

builder.AddProject<Projects.EventService>("event-api")
    .WithReference(eventDb)
    .WithReference(rabbitmq);

builder.Build().Run();
