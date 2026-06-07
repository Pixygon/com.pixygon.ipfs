# Pixygon — IPFS

Fetch files from IPFS by hash and turn them into spawnable, self-playing Unity GameObjects.

## Overview

NFT and on-chain media reference their assets by IPFS content hash, but those assets can be anything — a PNG, a WebP, an animated GIF, or an MP4. This package resolves a hash to bytes over an IPFS gateway, sniffs the `Content-Type`, decodes it into the right Unity primitive (Sprite, animated `Gif`, or streamed video), and instantiates a ready-to-display prefab. It sits at the asset-ingestion layer of the engine, between a raw IPFS reference and anything that needs to render NFT media in-game.

## Key types

| Type | What it is |
| --- | --- |
| `IpfsConstructor` | MonoBehaviour entry point. `ConstructIpfsObject(hash)` resolves a hash and instantiates the matching image / gif / video prefab as a child. |
| `IpfsBridge` | Static fetch + decode layer. `GetIpfsFile<T>(hash, thumbnail)` downloads from the gateway and returns a typed Unity object based on `Content-Type`. |
| `IpfsGif` | MonoBehaviour that plays a decoded `Gif` frame-by-frame on a UI `Image`. Accepts a `Gif` or fetches one from a hash directly. |
| `IpfsVideo` | MonoBehaviour that streams an MP4 URL through a `VideoPlayer` into a `RenderTexture` on a `RawImage`. |
| `Gif` | Decoded animated GIF — an ordered `List<GifData>` of frames. |
| `GifData` | A single GIF frame: a `Sprite` plus its `delay` in seconds. |
| `VideoData` | Result wrapper carrying a resolved video `_url`. |
| `SpriteData` | Result wrapper carrying a decoded `Sprite`. |
| `ErrorData` | Result wrapper carrying an `_error` message when a fetch or decode fails. |

## How games use it

Drop an `IpfsConstructor` on a GameObject, assign its image / gif / video prefabs in the inspector, then hand it a hash:

```csharp
var constructor = GetComponent<IpfsConstructor>();

// Resolves the hash, decodes by content type, and spawns the right prefab as a child.
GameObject media = await constructor.ConstructIpfsObject("QmHash...");

// Pass a full gateway URL or a bare hash — both work. Use thumbnail mode for previews:
GameObject thumb = await constructor.ConstructIpfsObject("QmHash...", thumbnail: true);

// Swap or tear down the spawned media:
constructor.ClearIpfs();
```

For lower-level access, call `IpfsBridge.GetIpfsFile<Object>(hash)` directly and switch on the returned type (`Sprite`, `Gif`, `VideoData`, `ErrorData`) yourself.

## Dependencies

- `com.pixygon.debugtool` (0.5.4) — routed logging via `Log.DebugMessage` / `DebugGroup` for surfacing fetch errors.

This package also bundles a vendored GIF decoder (`ThreeDISevenZeroR.UnityGifDecoder`, under `GifDecoder/`) and uses a `WebP` decoder for `image/webp` content.

## Install

```json
"com.pixygon.ipfs": "https://github.com/Pixygon/com.pixygon.ipfs.git"
```
