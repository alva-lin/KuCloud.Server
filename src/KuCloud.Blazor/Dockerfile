FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/KuCloud.Blazor/KuCloud.Blazor.csproj", "src/KuCloud.Blazor/"]
RUN dotnet restore "src/KuCloud.Blazor/KuCloud.Blazor.csproj"
COPY . .
WORKDIR "/src/src/KuCloud.Blazor"
RUN dotnet build "KuCloud.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KuCloud.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KuCloud.Blazor.dll"]
