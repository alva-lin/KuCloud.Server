FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY  ["src/", "."]
#COPY ["src/KuCloud.Api/KuCloud.Api.csproj", "KuCloud.Api/"]
RUN dotnet restore "KuCloud.Api/KuCloud.Api.csproj"
COPY . .
WORKDIR "/src/KuCloud.Api"
RUN dotnet build "KuCloud.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KuCloud.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KuCloud.Api.dll"]
