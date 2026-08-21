#!/bin/sh
set -e
# A leftover GCHeapHardLimit from the dashboard aborts .NET with exit 134.
unset DOTNET_GCHeapHardLimit
unset COMPlus_GCHeapHardLimit
exec dotnet SafeProject.Web.dll --urls "http://0.0.0.0:${PORT:-8080}"
