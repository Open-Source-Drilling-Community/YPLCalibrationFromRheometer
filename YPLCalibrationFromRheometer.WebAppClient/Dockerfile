FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

WORKDIR /src

COPY ["YPLCalibrationFromRheometer.WebAppClient/YPLCalibrationFromRheometer.WebAppClient.csproj", "YPLCalibrationFromRheometer.WebAppClient/"]
COPY ./nuget.config .
RUN dotnet restore "YPLCalibrationFromRheometer.WebAppClient/YPLCalibrationFromRheometer.WebAppClient.csproj"

COPY . .
WORKDIR "/src/YPLCalibrationFromRheometer.WebAppClient"
RUN dotnet build "YPLCalibrationFromRheometer.WebAppClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YPLCalibrationFromRheometer.WebAppClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YPLCalibrationFromRheometer.WebAppClient.dll"]