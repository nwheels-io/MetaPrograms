#!/bin/bash

script_dir=`dirname "$0"` 
cd $script_dir/../Source 

for project_name in $(find . -type d -name '*Tests' | cut -c3-)
do
    dll_path=./$project_name/bin/Release/netcoreapp2.0/$project_name.dll
    echo "--- $project_name ---"
    dotnet test $project_name /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./$project_name.opencover.xml
done

find . -type f -name '*.opencover.xml' | xargs -n1 $script_dir/codecov.sh -t $CODECOV_TOKEN -f 
