#!/usr/bin/env bash

dotnet publish -c Release
now --prod bin/Release/netstandard2.1/publish/wwwroot/

chmod +x deploy.sh
./deploy.sh