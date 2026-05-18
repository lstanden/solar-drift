# Solar Drift

A BepInEx plugin for Solar Expanse.

Solar Drift is an unofficial fan project. It is not affiliated with, endorsed by, sponsored by, or approved by the developers or publishers of Solar Expanse.

This mod is built and tested on Linux. Paths in this README use Linux path conventions and the default Steam install location under `$HOME/.local/share/Steam`.

## Features

- Camera zoom preservation, enabled by default. Space pause/unpause and normal target changes preserve the current zoom.
- Ctrl-click focus zoom. Hold Ctrl while clicking a target to allow the game's normal focus zoom reset.
- Optional non-depleting mining.
- Optional non-declining player funds.
- Optional player-only mining multiplier.
- Optional player-only research multiplier.

## Install

1. Install BepInEx 5 for Solar Expanse.
2. Download the latest `SolarDrift-*.zip` from GitHub Releases.
3. Extract the zip into your Solar Expanse game folder.

After extraction, the plugin should be here:

```text
$HOME/.local/share/Steam/steamapps/common/Solar Expanse/BepInEx/plugins/SolarDrift/SolarDrift.dll
```

Launch the game once to generate the config file.

## Configuration

After the first launch, edit:

```text
$HOME/.local/share/Steam/steamapps/common/Solar Expanse/BepInEx/config/local.solar-drift.cfg
```

Default settings are intended for public use: the camera fix is on, and gameplay modifiers are off.

```ini
[Camera]
PreserveCameraZoom = true

[Cheats]
NonDepletingMining = false
NonDecliningFunds = false
EnablePlayerMiningMultiplier = false
PlayerMiningMultiplier = 1
EnablePlayerResearchMultiplier = false
PlayerResearchMultiplier = 1
```

The mining options can be combined. For example, this makes player machines mine faster while deposits stay intact:

```ini
[Cheats]
NonDepletingMining = true
EnablePlayerMiningMultiplier = true
PlayerMiningMultiplier = 5
```

## Verify A Release

Release zips include `SOURCE_COMMIT.txt` and `BUILDINFO.txt`. GitHub Releases also include `checksums.sha256`.

For stronger provenance, compare the release asset against the public GitHub Actions run attached to the release. The workflow builds from the visible source, publishes checksums, and requests a GitHub build provenance attestation.

## Development

See [DEVELOPING.md](DEVELOPING.md) for build, install-from-source, and release workflow details.

## License

MIT. See [LICENSE](LICENSE).
