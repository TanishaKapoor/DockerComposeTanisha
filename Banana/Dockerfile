FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Banana/Banana.csproj Banana/
RUN dotnet restore Banana/Banana.csproj
COPY . .
WORKDIR /src/Banana
RUN dotnet build Banana.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Banana.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Banana.dll"]
