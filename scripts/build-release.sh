#!/usr/bin/env bash
set -euo pipefail

project="${PROJECT:-src/SolarDrift/SolarDrift.csproj}"
configuration="${CONFIGURATION:-Release}"
release_version="${RELEASE_VERSION:-v0.0.0-local}"
game_root="${GAME_ROOT:-${HOME}/.local/share/Steam/steamapps/common/Solar Expanse}"

required=(
  "$game_root/BepInEx/core/BepInEx.dll"
  "$game_root/BepInEx/core/0Harmony.dll"
  "$game_root/Solar Expanse_Data/Managed/Assembly-CSharp.dll"
  "$game_root/Solar Expanse_Data/Managed/UnityEngine.dll"
  "$game_root/Solar Expanse_Data/Managed/UnityEngine.CoreModule.dll"
  "$game_root/Solar Expanse_Data/Managed/UnityEngine.InputLegacyModule.dll"
  "$game_root/Solar Expanse_Data/Managed/UnityEngine.UI.dll"
  "$game_root/Solar Expanse_Data/Managed/Sirenix.Serialization.dll"
)

missing=0
for file in "${required[@]}"; do
  if [ ! -f "$file" ]; then
    echo "Missing required build reference: $file" >&2
    missing=1
  fi
done

if [ "$missing" -ne 0 ]; then
  {
    echo "This project builds against the installed Solar Expanse/BepInEx assemblies."
    echo "Set GAME_ROOT to the root that contains BepInEx/ and Solar Expanse_Data/."
  } >&2
  exit 1
fi

python3 --version >/dev/null

dotnet restore "$project" --locked-mode -p:GameRoot="$game_root"
dotnet build "$project" --configuration "$configuration" --no-restore -p:GameRoot="$game_root" -p:SourceRevisionId="${GITHUB_SHA:-local}"

package_root="dist/package/SolarDrift"
plugin_root="$package_root/BepInEx/plugins/SolarDrift"
output_dir="src/SolarDrift/bin/$configuration/net472"

rm -rf dist
mkdir -p "$plugin_root"

cp "$output_dir/SolarDrift.dll" "$plugin_root/"
cp "$output_dir/SolarDrift.pdb" "$plugin_root/"

cat > "$package_root/SOURCE_COMMIT.txt" <<EOF
Repository: ${GITHUB_REPOSITORY:-local/solar-drift}
Commit: ${GITHUB_SHA:-local}
Ref: ${GITHUB_REF:-local}
Workflow run: ${GITHUB_SERVER_URL:-local}/${GITHUB_REPOSITORY:-local/solar-drift}/actions/runs/${GITHUB_RUN_ID:-local}
EOF

cat > "$package_root/BUILDINFO.txt" <<EOF
Artifact: SolarDrift-$release_version.zip
Configuration: $configuration
Target framework: net472
Build runner: ${RUNNER_NAME:-local} (${RUNNER_OS:-$(uname -s)}/${RUNNER_ARCH:-$(uname -m)})
Built from clean GitHub checkout: ${GITHUB_ACTIONS:-no}
EOF

(
  cd dist/package
  python3 -m zipfile -c "../SolarDrift-$release_version.zip" SolarDrift
)

(
  cd dist
  sha256sum "SolarDrift-$release_version.zip" > checksums.sha256
  sha256sum package/SolarDrift/BepInEx/plugins/SolarDrift/SolarDrift.dll >> checksums.sha256
  sha256sum package/SolarDrift/BepInEx/plugins/SolarDrift/SolarDrift.pdb >> checksums.sha256
)

echo "Created dist/SolarDrift-$release_version.zip"
echo "Created dist/checksums.sha256"
