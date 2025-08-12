#!/bin/bash

dotnet build Portfoli.slnx
dotnet tool restore
dotnet tool run dotnet-fm migrate -p sqlite -c "Data Source=src/Portfoli/Portfoli.db" -a "src/Portfoli/bin/Debug/net9.0/Portfoli.dll"
