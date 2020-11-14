FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine as sdk
WORKDIR /src
COPY . .
RUN dotnet publish --configuration=Release GrpcChat.Server/GrpcChat.Server.csproj -o /app

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine as runtime
WORKDIR /app
COPY --from=sdk /app .

EXPOSE 9696

ENTRYPOINT [ "dotnet", "GrpcChat.Server.dll" ]
