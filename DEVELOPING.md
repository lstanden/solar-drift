# Developing

Development is built and tested on Linux. Paths below use Linux path conventions and the default Steam install location under `$HOME/.local/share/Steam`.

## Requirements

- .NET SDK 8 or newer
- Solar Expanse installed through Steam
- BepInEx 5 installed into the Solar Expanse game folder

The project references Solar Expanse, Unity, BepInEx, and Sirenix DLLs from the installed game. Those assemblies are not vendored in this repository.

## Build

By default, the project looks for the game at:

```text
$HOME/.local/share/Steam/steamapps/common/Solar Expanse
```

Build with:

```bash
dotnet restore src/SolarDrift/SolarDrift.csproj --locked-mode
dotnet build src/SolarDrift/SolarDrift.csproj -c Release --no-restore
```

For a different install location, pass `GameRoot` explicitly:

```bash
dotnet build src/SolarDrift/SolarDrift.csproj -c Release -p:GameRoot="/path/to/Solar Expanse"
```

## Install From Source

```bash
dotnet msbuild src/SolarDrift/SolarDrift.csproj -t:Install -p:Configuration=Release
```

This installs the mod DLL and PDB into:

```text
$HOME/.local/share/Steam/steamapps/common/Solar Expanse/BepInEx/plugins/SolarDrift
```

After launch or reload, check:

```text
$HOME/.local/share/Steam/steamapps/common/Solar Expanse/BepInEx/LogOutput.log
```

for:

```text
Solar Drift 0.1.0 loaded
```

## Release Builds

Releases are built by `.github/workflows/release.yml` when a `v*` tag is pushed, or manually with the workflow dispatch form.

The release workflow:

- checks out the public source at the release commit;
- restores NuGet dependencies in locked mode from `packages.lock.json`;
- builds with deterministic/CI build settings;
- packages `SolarDrift.dll` and `SolarDrift.pdb`;
- writes `SOURCE_COMMIT.txt` and `BUILDINFO.txt` into the zip;
- publishes `checksums.sha256`;
- requests a GitHub build provenance attestation for the zip.

Solar Expanse, Unity, BepInEx, and Sirenix assemblies are required as compile-time references. Public release builds use the `solar-drift-build` self-hosted runner group. The runner needs the game installed locally.

If the game is not installed in the default Linux Steam location for the runner user, set this repository variable:

```text
SOLAR_EXPANSE_GAME_ROOT=/path/to/Solar Expanse
```

`SOLAR_EXPANSE_GAME_ROOT` must contain `BepInEx/` and `Solar Expanse_Data/`. The runner also needs .NET 8, `python3`, and `sha256sum` available on `PATH`.

## Test Release Packaging Locally

Run the same build/package script used by GitHub Actions:

```bash
RELEASE_VERSION=v0.0.0-local scripts/build-release.sh
```

For a nonstandard game install:

```bash
GAME_ROOT="/path/to/Solar Expanse" RELEASE_VERSION=v0.0.0-local scripts/build-release.sh
```

This writes:

```text
dist/SolarDrift-v0.0.0-local.zip
dist/checksums.sha256
```

`act` can simulate more of GitHub Actions locally, but it still needs access to the game reference assemblies and may not support release publishing or GitHub provenance attestations exactly like GitHub-hosted Actions.

To run the build/package portion through `act`, mount the local game install into the container and enable the local test input:

```bash
mkdir -p .act
ln -sfn "$HOME/.local/share/Steam/steamapps/common/Solar Expanse" .act/solar-expanse

act workflow_dispatch \
  -j build-release \
  -P ubuntu-latest=catthehacker/ubuntu:act-latest \
  --input version=v0.0.0-act \
  --input local_test=true \
  --var SOLAR_EXPANSE_GAME_ROOT=/opt/solar-expanse \
  --container-options "--volume=$PWD/.act/solar-expanse:/opt/solar-expanse:ro"
```

The `.act/solar-expanse` symlink avoids Docker/Act quoting issues with the space in `Solar Expanse`.
