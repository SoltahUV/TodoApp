FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY WebAPI/WebAPI.csproj WebAPI/
COPY Auth/Auth.csproj Auth/
COPY BusinessLogic/BusinessLogic.csproj BusinessLogic/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore WebAPI/WebAPI.csproj

COPY . .
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebAPI.dll"]
