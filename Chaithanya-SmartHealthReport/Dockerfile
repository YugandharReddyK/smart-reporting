# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY SmartHealthReport.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime with Playwright + Chromium
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Install Playwright dependencies (Chromium system libs)
RUN echo 'Acquire::http::Pipeline-Depth "0";' > /etc/apt/apt.conf.d/99fixbadproxy \
    && echo 'Acquire::http::No-Cache=True;' >> /etc/apt/apt.conf.d/99fixbadproxy \
    && echo 'Acquire::BrokenProxy=true;' >> /etc/apt/apt.conf.d/99fixbadproxy \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
    wget \
    ca-certificates \
    fonts-liberation \
    fonts-noto-color-emoji \
    libasound2t64 \
    libatk-bridge2.0-0t64 \
    libatk1.0-0t64 \
    libcairo2 \
    libcups2t64 \
    libdbus-1-3 \
    libdrm2 \
    libgbm1 \
    libglib2.0-0t64 \
    libgtk-3-0t64 \
    libnspr4 \
    libnss3 \
    libpango-1.0-0 \
    libx11-6 \
    libxcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxkbcommon0 \
    libxrandr2 \
    xdg-utils \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Install Playwright Chromium browser (use the published app's deps to run Playwright CLI)
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
RUN dotnet exec --depsfile SmartHealthReport.deps.json \
    --runtimeconfig SmartHealthReport.runtimeconfig.json \
    Microsoft.Playwright.dll install chromium

# Output directory for generated PDFs
RUN mkdir -p /output

# Run as non-root for security
RUN groupadd -r appuser && useradd -r -g appuser -G audio,video appuser \
    && chown -R appuser:appuser /app /output \
    && (test -d /ms-playwright && chown -R appuser:appuser /ms-playwright || true)
USER appuser

ENV REPORT_OUTPUT_DIR=/output

ENTRYPOINT ["dotnet", "SmartHealthReport.dll"]
