FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["YPLCalibrationFromRheometer.WebApp.Client/YPLCalibrationFromRheometer.WebApp.Client.csproj", "YPLCalibrationFromRheometer.WebApp.Client/"]
RUN dotnet restore "YPLCalibrationFromRheometer.WebApp.Client/YPLCalibrationFromRheometer.WebApp.Client.csproj"

COPY . .
WORKDIR "/src/YPLCalibrationFromRheometer.WebApp.Client"
RUN dotnet build "YPLCalibrationFromRheometer.WebApp.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YPLCalibrationFromRheometer.WebApp.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YPLCalibrationFromRheometer.WebApp.Client.dll"]