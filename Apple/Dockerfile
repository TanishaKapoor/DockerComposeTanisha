FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Apple/Apple.csproj Apple/
RUN dotnet restore Apple/Apple.csproj
COPY . .
WORKDIR /src/Apple
RUN dotnet build Apple.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Apple.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Apple.dll"]
