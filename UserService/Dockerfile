FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 8000
EXPOSE 8001

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY UserService/UserService.csproj UserService/
RUN dotnet restore UserService/UserService.csproj
COPY . .
WORKDIR /src/UserService
RUN dotnet build UserService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish UserService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UserService.dll"]
