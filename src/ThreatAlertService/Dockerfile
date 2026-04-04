# Aşama 1: Derleme (Build) ortamı
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Sadece proje dosyasını kopyalayıp bağımlılıkları yüklüyoruz
COPY ["ThreatAlertService.csproj", "./"]
RUN dotnet restore "ThreatAlertService.csproj"

# Tüm kodları kopyalayıp derliyoruz
COPY . .
RUN dotnet publish "ThreatAlertService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Aşama 2: Çalıştırma (Runtime) ortamı
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Uygulamayı 80 portunda çalışmaya zorluyoruz
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ThreatAlertService.dll"]