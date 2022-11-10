FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["smooms.app/smooms.app.csproj", "smooms.app/"]
RUN dotnet restore "smooms.app/smooms.app.csproj"
COPY . .
WORKDIR "/src/smooms.app"
RUN dotnet build "smooms.app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "smooms.app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "smooms.app.dll"]
