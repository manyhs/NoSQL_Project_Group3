#!/bin/sh
cd "$(dirname "$0")"

DOTNET=""
for d in "$(command -v dotnet 2>/dev/null)" /usr/local/share/dotnet/dotnet "$HOME/.dotnet/dotnet" /opt/homebrew/bin/dotnet /usr/local/bin/dotnet; do
  if [ -n "$d" ] && [ -x "$d" ]; then DOTNET="$d"; break; fi
done

if [ -z "$DOTNET" ]; then
  echo "Could not find .NET. Install the .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
else
  "$DOTNET" run --project NoSQL_Project -- --demo
fi

echo
echo "Press Enter to close this window."
read _
