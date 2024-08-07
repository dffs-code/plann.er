FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./src/Journey.Api/*.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out ./src/Journey.Api/Journey.Api.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

COPY ./src/Journey.Api/Data/JourneyDatabase.db ./Data/JourneyDatabase.db

EXPOSE 8080

ENTRYPOINT ["dotnet", "Journey.Api.dll"]
