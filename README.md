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
        
    },
    "eventHub": {
        "connectionString": "<Event Hub connectionstring>"
    }
}
```
