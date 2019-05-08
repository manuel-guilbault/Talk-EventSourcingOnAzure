# Event Sourcing on Azure

This is a sample project showing how to use various Azure PaaS services as event store for CQRS & event sourced applications.
Two different event store implementations are illustrated, each on its own branch.

> The code samples may seem weird, with only write operations and no read operations. This is because they implement no queries and no read models, but only a couple of commands, in order to keep the focus on illustrating the retrieval and storage of events.

## With Table Storage

The `table-storage` branch shows an event store implementation which uses a table of an Azure Table Storage account to store events.
The events store implementation also writes events to an Azure Service Bus queue, and an Azure Functions application asynchronously 
consumes the events coming from the queue.

### Running the sample

1. Perform a `git checkout` of the `table-storage` branch.
2. Using the `Deployment/azuredeploy.json` ARM template, create the required resources on your Azure account.
   **Watch out, as some services may cost some money.**
3. Rename `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.sample.json` to 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.json` and set the following keys:
    1. `AzureWebJobsStorage` & `AzureWebJobsDashboard`: the connection string of the Azure Storage account.
    2. `ServiceBus:ConnectionString` : the connection string of the Azure Service Bus namespace.
4. Rename `AzureTableEventSourcingTest.WebApi/appsettings.sample.json` to `AzureTableEventSourcingTest.WebApi/appsettings.json`
   and set the following keys:
    1. `Azure` / `Storage` / `ConnectionString`: the connection string of the Azure Storage account.
    2. `Azure` / `ServiceBus` / `ConnectionString`: the connection string of the Azure Service Bus namespace.
5. Launch the application (make sure to run both `AzureTableEventSourcingTest.WebApi` and 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions`).

## With Cosmos Db

The `cosmos-db` branch shows an event store implementation which uses a collection from an Azure CosmosDb database to store events.
New events are consumed asynchronously by an Azure Functions application using the CosmosDb collection's change feed.

### Running the sample

1. Perform a `git checkout` of the `cosmos-db` branch.
2. Using the `Deployment/azuredeploy.json` ARM template, create the required resources on your Azure account.
   **Watch out, as some services may cost some money.**
3. Rename `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.sample.json` to 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.json` and set the following keys:
    1. `AzureWebJobsStorage` & `AzureWebJobsDashboard`: the connection string of the Azure Storage account.
    2. `CosmosDb:ConnectionString` : the connection string of the Azure CosmosDb account.
4. Rename `AzureTableEventSourcingTest.WebApi/appsettings.sample.json` to `AzureTableEventSourcingTest.WebApi/appsettings.json`
   and set the following keys:
    1. `Azure` / `CosmosDb` / `AccountEndpoint`: the URI of the Azure CosmosDb account.
    2. `Azure` / `CosmosDb` / `AccountKey`: the secret key of the Azure CosmosDb account.
5. Launch the application (make sure to run both `AzureTableEventSourcingTest.WebApi` and 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions`).
