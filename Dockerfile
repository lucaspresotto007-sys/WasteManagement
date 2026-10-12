# =============================================================================
# Waste Management ESG API — Multi-Stage Dockerfile
# =============================================================================

# ─── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (better layer caching)
COPY ["WasteManagementAPI/WasteManagementAPI.csproj", "WasteManagementAPI/"]
RUN dotnet restore "WasteManagementAPI/WasteManagementAPI.csproj"

# Copy the full source and build in Release mode
COPY . .
WORKDIR "/src/WasteManagementAPI"
RUN dotnet build "WasteManagementAPI.csproj" -c Release -o /app/build

# ─── Stage 2: Publish ────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "WasteManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ─── Stage 3: Runtime ────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "WasteManagementAPI.dll"]
