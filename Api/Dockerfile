FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Api/Api.csproj Api/
COPY Service/Service.csproj Service/
COPY Data/Data.csproj Data/
RUN dotnet restore Api/Api.csproj
COPY . .
WORKDIR /src/Api
RUN dotnet build Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Api.dll"]
