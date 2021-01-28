## CAVAPA - Computer Assisted Analyses of Physical Activity

A Windows 10 GUI application that computes a measurement of movement (physical activity). Select a video file and a CSV file containing the motion measurement is generated.

![CAVAPA Screenshot](https://cavapa.ruthenbeck.io/images/cavapa2.png)

<https://cavapa.ruthenbeck.io>

<https://github.com/gregruthenbeck/cavapa>

### Dependencies

* C# .NET Framework 4.2.7 (built for Windows 10)
* OpenCV via [EmguCV](http://www.emgu.com/wiki/index.php/Download_And_Installation): Image processing (with hardware acceleration)
* FFmpeg via [FFmpeg.AutoGen](https://github.com/sdcb/FFmpeg.AutoGen): Decodes video frames (with hardware acceleration)

### Installer

The Windows installer  (Setup .msi) builder project is <https://github.com/gregruthenbeck/CavapaInstaller/>. It is intended to sit beside the source code folder of this project (cavapa.sln links to ../CavapaInstaller/CavapaInstaller.vdproj).
