# MySpyImageResizer
![Static Badge](https://img.shields.io/badge/dotnet-v10.0-blue)
![Static Badge](https://img.shields.io/badge/platform-Windows-lightgrey)
![Static Badge](https://img.shields.io/badge/license-MIT-green)

**MySpyImageResizer** is a desktop application developed in **C# (.NET / WPF)** designed for **batch image resizing and format conversion**.  
It allows you to select a source folder, define custom dimensions, choose the output format, and decide whether to preserve the original image proportions (aspect ratio).

The project focuses on **simplicity**, **asynchronous processing**, and **real-time visual feedback** during image processing.

---

## Features

- **Batch Image Resizing**
  - Automatically processes multiple images from a selected folder.
  - Displays detailed progress (processed count and percentage).

- **Format Conversion**
  - Choose the desired output image format (PNG, JPG, BMP, WEBP, etc).
  - Option to convert format only, without resizing.

- **Dimension Control**
  - Manual width and height configuration.
  - Configurable default values.

- **Keep Aspect Ratio**
  - Proportional resizing based on image width.
  - Prevents image distortion.

- **Asynchronous Processing**
  - Responsive UI while processing images.
  - Ability to cancel the operation at any time.

- **Automatic Output Folder Creation**
  - If no output folder is defined, a `resized` folder is created automatically.

- **Visual Feedback**
  - Dynamic progress bar.
  - Visual indicators for in-progress and completed tasks.

---

## Supported Formats

Input and output support for the following formats:

- `.jpg`
- `.jpeg`
- `.png`
- `.bmp`
- `.gif`
- `.webp`
- `.tiff`
- `.pdf`

---

## Possible Uses

### 📸 Photo Organization
> Resize large photo collections for storage, backup, or sharing while preserving quality and proportions.

### 🌐 Web Image Preparation
> Adjust images to ideal sizes for websites, web systems, or e-commerce platforms, reducing file size and improving performance.

### 📱 Application Asset Standardization
> Generate images with fixed dimensions for desktop, mobile applications, or games.

### 🧰 Fast Format Conversion
> Convert entire folders of images to another format without changing their dimensions.

### 🧪 Educational and Technical Use
> A great example of a WPF application using:
> - Asynchronous tasks  
> - CancellationToken  
> - UI updates via Dispatcher  
> - Background processing  

---

## Download (Releases)

You can download the compiled version directly from the **Releases** tab on GitHub:

👉 **Releases → [Download](https://github.com/Spylher/MySpyImageResizer/releases/latest)**

> The published version does not require the .NET SDK to run.

---

## How to Use

1. **Select the source folder** containing the images.
2. *(Optional)* **Choose the output folder**.
   - If not selected, it will be **automatically generated**.
3. Configure the desired options:
   - **Width and height**
   - **Output format**
   - **Keep aspect ratio**
   - **Convert format only** (no resizing)
4. Click **Start** to begin processing.
5. **Monitor the progress** in real time.
6. **Cancel the process** at any moment if needed.

<img width="600" height="800" alt="screenshot" src="https://github.com/user-attachments/assets/5bd2e45f-3610-48f6-bebc-256f687e97ea" />

---
## Manual Build

### Requirements

- **Windows**
- **.NET SDK 10.0 or higher**
- **Visual Studio 2022+** with:
  - .NET Desktop Development
  - WPF workload enabled

### Clone this repository

```bash
git clone https://github.com/Spylher/MySpyImageResizer.git
```
- **`Open the project in Visual Studio:`** Navigate to the cloned project directory and open the .csproj file with Visual Studio.
  
- **`Build the project:`** Build the project in Visual Studio using either the `Release` or `Debug` build mode, depending on your need.

---

## Contribution
Contributions are welcome! If you find any issues, please open an issue or submit a pull request.

## License
This project is licensed under the [licença MIT](./LICENSE.txt).

