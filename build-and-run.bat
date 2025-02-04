@echo off
set BUILD_CONFIGURATION=%1
if "%BUILD_CONFIGURATION%"=="" set BUILD_CONFIGURATION=Release

echo Building and running with configuration: %BUILD_CONFIGURATION%
docker-compose up --build
