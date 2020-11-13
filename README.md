# kafka2eventhub
This code mirrors a Confluent Kafka stream to Azure Event Hub. This can be usefull in scenario's where it is not possible to connect to Kafka directly or when further analysis of the data is not possible from the Kafka stream.

## Microservice
The code is build to be run as a microservice inside a container. There are several options to pass in a configuration:
- Create a new container based on this one with a config.json file in the same folder as the binaries.
- Map a volume as local folder and use an environment variable to point to the configuration file.
- Pass options as environment variables.

## Configuration file
The configuration file is a json file where you can set several variables.
``` json
{
    "kafka": {
        "groupId": "kafka-consumer",
        "bootstrapServers": "localhost:9092",
        "topics": [],
        "sslCertificateLocation": null,
        "sslKeyLocation": null
    },
    "eventHub": {
        "connectionString": "<Event Hub connectionstring>"
    }
}
```

By default the configuration file is searched within the current directory. You can override this by providing a path to a json file on the promt. Eg. ``` kafka2eventhub <pathtoconfig.json>```. Another option is to pass it as an environment variable named: DF_CONFIG. Configurations set by environment variables always take precedence over the command prompt or configuration file.

## Environment variables
Some settings can be set through environment variables.
- DF_BOOTSTRAPSERVERS: To set bootstrapservers.
- DF_GROUPID: To set the groupid.
- DF_TOPICS: A comma seperated list of topics.
- DF_EVENTHUB: The connection to Azure Event Hub.

## Docker
There is a Docker image available at <https://hub.docker.com/r/datafellows/kafka2eventhub>.
