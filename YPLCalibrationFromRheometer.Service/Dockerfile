FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["YPLCalibrationFromRheometer.Service/YPLCalibrationFromRheometer.Service.csproj", "YPLCalibrationFromRheometer.Service/"]
RUN dotnet restore "YPLCalibrationFromRheometer.Service/YPLCalibrationFromRheometer.Service.csproj"

COPY . .
WORKDIR "/src/YPLCalibrationFromRheometer.Service"
RUN dotnet build "YPLCalibrationFromRheometer.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "YPLCalibrationFromRheometer.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
VOLUME /home
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "YPLCalibrationFromRheometer.Service.dll"]