#!/bin/bash

printenv

script_dir=`dirname "$0"` 
cd $script_dir/../Source 

test_exit_code=0

for project_name in $(find . -type d -name '*Tests' | cut -c3-)
do
    dll_path=./$project_name/bin/Release/netcoreapp2.0/$project_name.dll
    echo "--- $project_name ---"
    
    dotnet test $project_name \
        -c Release \
        --logger:Appveyor \
        /p:CollectCoverage=true \
        /p:CoverletOutputFormat=opencover \
        /p:CoverletOutput=./$project_name.opencover.xml
    
    if [ $test_exit_code -eq 0 ]
    then
        test_exit_code=$?
    fi
done

# temporarily using a clear-text token; it will be regenerated
find . -type f -name '*.opencover.xml' | xargs -n1 $script_dir/codecov.sh -t 0effad4c-9e98-48c8-a598-47e3e4c77e93 -f 

exit $test_exit_code
