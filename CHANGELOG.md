# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-preview.1] - 2021-05-04
### Added
- Added UPM support.
- Added support for Unity 2019.4

## [1.0.0-preview.2] - 2021-05-05
### Fixed
- Fixed typos in recent README changes.
- Fixed import errors for Samples.

## [1.0.0-preview.3] - 2021-05-06
### Fixed
- Lighting settings on Affective Video demo rebuilt for Unity 2019.4.x
- Fixed missing event data and empty recording status.

## [1.0.0-preview.4] - 2021-05-10
### Changed
- Data Points demo has new buttons to show how to start and stop recording data.
- Changed the layer for tracked objects on EmteqVREyeManager.

## [1.0.0-preview.5] - 2021-05-17
### Changed
- Calibration now shows ExpressionType and FaceSide as strings on the session json file rather than an enum value.
### Fixed
- Unity would hang after calibration because thread processes were not being properly disposed.
- Missing serialisation data on messages sent to SuperVision.
- Missing data on Affective Video demo.

## [1.0.0-preview.6] - 2021-05-21
### Changed
- Mask fit adjustment prompt now waits 5 seconds before alerting the user to a mask fit change.

## [1.0.0] - 2021-05-25
### Changed
- More robust logic was introduced for processing the BlockingCollections.
- Mask fit adjustment prompt's 3D model was made transparent.

## [1.1.0] - 2021-08-24
### Changed
- Added local video streaming support to Supervision app (1.1.3+).
- Added systems required for supporting DAB file playback. Note that it is not yet operational, it will be completed in a future release.
- Improved performance and reliability of mask USB connection.

## [1.2.0] - 2021-12-03
### Changed
- Updated UI design of sensor debug widget.
- Replaced previous calibration prefab with a new design.
### Fixed
- Fixed issue relating to mismatch in data frequency on hardware version 9.4.0 masks.

## [1.2.1] - 2021-12-06
### Fixed
- Fixed issue where the sensor debug widget and the Mask fit adjustment prompt's sensor icons were not changing appropriately to reflect their respective fit states.

## [1.3.0] - 2022-01-21
### Changed
- Added feature to automatically start and stop a data section that shows whethere the mask is on or off the users face
- Added eye data saving to an "<datetime>.eyedata" file when the EyeTrackingManager is in use
- Added quick switch bool to calibration prefab allowing switching between using a button or a timer for stage progression
- Replaced all text elements within project with TextMeshPro versions with a new high resolution font
### Fixed
- Fixed code within CustomCalibration.cs that was incompatible with Unity 2019
- Fixed desktop scenes to enable Vsync so that they do not run with an unlimited frame rate which was causing high GPU usage
## [1.3.1] - 2022-01-21
### Fixed
- Fixed minor UI bug within contact prompt and video stream status widget.

## [1.3.2] - 2022-02-26
### Fixed
- Fixed issues with mask reconnect logic

## [1.4.0] - 2022-02-11
### Added
- Added bool value to VR Manager to read if data recording is turned on or not
- Added exposed option to customise distance mask contact prompt is placed from camera
### Changed
- Completed overhaul of the UI in sample scenes and prefabs
### Fixed
- Fixed EMG sensor data frequency
- Fixed various issues with Emteq prefabs and UI in samples
- Fixed bugs in display and deactivation of contact prompt
- Fixed calibration screen in affective video sample

## [1.4.1] - 2022-02-17
### Fixed
- Replaced font asset with smaller version


## [1.5.0] - 2022-04-15
### Added
- Added a readable label in json output when saving data points and sections
- Added second timestamp field within json output that gives the time as seconds in UNIX time
- Added baseline heartrate calculation stage at the start of calibration
### Fixed
- Fixed bug preventing video stream camera from rendering correctly


## [1.5.1] - 2022-05-13
### Added
- Added Unix timestamp to .eyedata file
### Fixed
- Fixed issue with host clock forwarding to mask device firmware
- Fixed supervision recording start/stop behaviour


## [1.5.2] - 2022-05-18
### Fixed
- Fixed code that was incompatible with Unity 2019.4
- Updated WebRTC and Renderstreaming packages to fix support for 2021.3


## [1.5.3] - 2022-05-19
### Added
- Added data section to cover full calibration sequence


## [1.5.4] - 2022-05-26
### Fixed
- Fixed end of calibration data section triggering too late

## [2.0.0-exp.1] - 2022-08-17
### Breaking Change
- Replaced references to "EmteqVR" within the package, in code, documentation, and within the Unity editor. Note that this is a breaking change, the general rule for updating is to replace "EmteqVR" with "Emteq".
### Added
- Added support for android builds, primarily to support Piceo Neo3, but generic android devices may also work
### Fixed
- Fixed bug with player input on progress bar in calibration sample
- Upgraded plugin to MQTTnet 4

## [2.0.0-exp.2] - 2022-08-23
### Fixed
- Fixed issue with Emteq sample prefab references

## [2.0.0-exp.3] - 2022-10-12
### Fixed
- Improved serial port connection on both Windows and Android. Includes many bug fixes.
- Fixed an internal calibration code issue on Android

## [2.0.0-exp.4] - 2022-10-17
### Fixed
- Fixed issue calculating heartrate baseline on all platforms
- Improved robustness of event handling within EmteqManager

## [2.0.0-exp.5] - 2022-11-24
### Fixed
- Fixed issue causing unreliable sending of datapoint MQTT events

## [2.0.0-exp.6] - 2022-12-06
### Added
- Added support for broadcasting data streams using LabStreamingLayer
