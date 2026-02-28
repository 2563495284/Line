#!/bin/bash

WORKSPACE=$(cd "$(dirname "$0")/.." && pwd)
LUBAN_DLL=/Users/yijin/Documents/luban_examples-main/Tools/Luban/Luban.dll
CONF_ROOT=$(cd "$(dirname "$0")" && pwd)

dotnet "$LUBAN_DLL" \
    -t client \
    -c cs-simple-json \
    -d json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir="$WORKSPACE/Assets/Scripts/GameConfig/Gen" \
    -x outputDataDir="$WORKSPACE/Assets/Resources/Config"

echo ""
echo "生成完成！"
echo "  - C#代码 -> Assets/Scripts/GameConfig/Gen"
echo "  - JSON数据 -> Assets/Resources/Config"
