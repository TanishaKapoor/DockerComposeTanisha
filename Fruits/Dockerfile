FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Fruits/Fruits.csproj Fruits/
RUN dotnet restore Fruits/Fruits.csproj
COPY . .
WORKDIR /src/Fruits
RUN dotnet build Fruits.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Fruits.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Fruits.dll"]
