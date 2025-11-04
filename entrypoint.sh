#!/bin/bash
set -e

echo "Running migrations..."
dotnet ef database update --project /app/AnydeskTracker.csproj --no-build || true

echo "Starting app..."
exec dotnet AnydeskTracker.dll
