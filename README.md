<div align="center">

<img src="Icons/ClipLabLogo-ICO.png" width="96" alt="Clip Lab logo" />

# Clip Lab

**A lightweight Windows desktop app for downloading YouTube video/audio and doing quick video edits.**

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D6.svg)](#requirements)
[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![UI](https://img.shields.io/badge/UI-WinForms-informational.svg)](#tech-stack)

![Clip Lab preview](https://github.com/booby1545/ClipLab/assets/107137294/0f56864a-cbe2-428a-ba5f-d6b895dbadb6)

</div>

> This is a polished fork of the original [ClipLab](https://github.com/flexeykinDev/ClipLab) — same core functionality, cleaned-up repo structure and docs.

## About

Clip Lab lets you download videos from YouTube as MP3 or the original video format, and do basic editing without a full video suite: merge two clips together, convert MP4 to MP3, and trim a video by seconds. It's a small, focused tool for the "I just need this one thing done" case.

**Features**

- Download YouTube videos as MP4 or extract audio as MP3
- Merge two video files into one
- Convert MP4 → MP3
- Trim video by start/end time
- Simple, themeable WinForms UI

## Tech stack

| Purpose | Library |
|---|---|
| Video/audio download | [VideoLibrary](https://github.com/omansak/libvideo) |
| Media conversion | [xFFmpeg.NET](https://github.com/cmxl/FFmpeg.NET) |
| Media conversion | [NReco.VideoConverter](https://www.nuget.org/packages/NReco.VideoConverter) |
| Runtime | .NET 6.0 (Windows Forms) |

## Requirements

- Windows 10/11
- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (or the SDK, if building from source)
- Visual Studio 2022 (only if building from source)

## Getting started

1. Clone the repository:

   ```bash
   git clone https://github.com/flexeykinDev/ClipLab-Pro.git
   ```

2. Unzip `( UNZIP.ME )FFmpeg.zip` into the project root — it contains the `ffmpeg.exe` binary the app depends on.

3. Open `Clip Lab.sln` in Visual Studio and build. Make sure NuGet packages are restored, or the build will fail.

4. Run the app from Visual Studio, or from the build output folder.

## Usage

1. **Download** tab — paste a YouTube link, optionally check "MP3 only", choose a save folder, and click **Download**.
2. **Edit** tab — merge two videos, convert MP4 to MP3, or trim a video by seconds, using the corresponding buttons.

## Contributing

1. Create a branch for your change.
2. Make your changes and test that they work as expected.
3. Commit, push, and open a pull request against `main`.

Ideas and improvements are welcome — feel free to fork the project and build on it under the terms of the license below.

## Author

**yiksnele** — Discord: `yiksnele#1068`
Email: [booby1546@gmail.com](mailto:booby1546@gmail.com)

## License

Distributed under the [Apache License 2.0](LICENSE).
Apache License 2.0 © [flexeykinDev](https://github.com/flexeykinDev), 2025. Free to use for commercial and non-commercial purposes; see [LICENSE](LICENSE) for details.
