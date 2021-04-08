FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AaaaperoBack/AaaaperoBack.csproj", ""]
RUN dotnet restore "./AaaaperoBack.csproj"
COPY AaaaperoBack .
WORKDIR "/src/."
RUN dotnet build "AaaaperoBack.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "AaaaperoBack.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet AaaaperoBack.dll
