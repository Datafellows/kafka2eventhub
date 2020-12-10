FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY src/bin/Release/net5.0/publish/ app/
WORKDIR /app
ENTRYPOINT ["dotnet", "kafka2eventhub.dll"]