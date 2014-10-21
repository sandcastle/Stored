#!/bin/sh -x

mono --runtime=v4.0 .nuget/NuGet.exe push ../build/Stored.%system.build.number%.nupkg  978b8639-edcf-41cb-ac65-ca1914ffc1b4
mono --runtime=v4.0 .nuget/NuGet.exe push ../build/Stored.Postgres.%system.build.number%.nupkg  978b8639-edcf-41cb-ac65-ca1914ffc1b4