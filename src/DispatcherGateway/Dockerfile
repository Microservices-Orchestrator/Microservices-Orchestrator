# --- 1. AÞAMA: ÇALIÞTIRMA ORTAMI (Sadece çalýþtýrmak için gereken hafif kalýp) ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- 2. AÞAMA: ÝNÞA ORTAMI (Ýçinde derleyicilerin olduðu aðýr kalýp) ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sadece csproj dosyasýný kopyalayýp paketleri indiriyoruz (Cache avantajý için)
COPY ["DispatcherGateway.csproj", "./"]
RUN dotnet restore "./DispatcherGateway.csproj"

# Kalan tüm dosyalarý kopyalayýp projeyi "Release" modunda derliyoruz
COPY . .
RUN dotnet publish "DispatcherGateway.csproj" -c Release -o /app/publish

# --- 3. AÞAMA: FÝNAL (Derlenmiþ cillop gibi dosyalarý hafif kalýba taþýma) ---
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Konteyner ayaða kalktýðýnda çalýþacak ilk komut
ENTRYPOINT ["dotnet", "DispatcherGateway.dll"]