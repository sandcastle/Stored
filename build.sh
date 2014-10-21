#!/bin/sh -x
export EnableNuGetPackageRestore=true

# restore nuget packages
mono --runtime=v4.0 .nuget/NuGet.exe restore Stored.sln

# build project
xbuild build.proj /property:BUILD_VERSION=%system.build.number%

# build folder
rm -rf ../build
mkdir ../build -p

# nuget packaging
mono --runtime=v4.0 .nuget/NuGet.exe pack Stored/Stored.nuspec -NoDefaultExcludes -BasePath Stored/bin/Release -OutputDirectory ../build -Version %system.build.number%
mono --runtime=v4.0 .nuget/NuGet.exe pack Stored.Postgres/Stored.Postgres.nuspec -NoDefaultExcludes -BasePath Stored.Postgres/bin/Release -OutputDirectory ../build -Version %system.build.number%