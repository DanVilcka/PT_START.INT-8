FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Fibonacci.csproj Fibonacci.csproj
RUN dotnet restore Fibonacci.csproj
COPY . .
WORKDIR /src
RUN dotnet build Fibonacci.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Fibonacci.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "run"]