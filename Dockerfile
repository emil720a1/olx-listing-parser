FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY OlxParser.slnx ./
COPY OlxParser.Console/OlxParser.Console.csproj OlxParser.Console/

RUN dotnet restore OlxParser.Console/OlxParser.Console.csproj

COPY OlxParser.Console/ OlxParser.Console/

RUN dotnet publish OlxParser.Console/OlxParser.Console.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir --parents /app/data

ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "OlxParser.Console.dll"]
