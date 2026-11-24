# Game Quest Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## Unreleased

## [1.0.4] - 2026-11-24

### Fixed
- sample: flowers only count for quests that have started and are not over (a quest starting later could be completed and rewarded early)
- sample: today's quests are launched when the game starts, not only when the quest popup is opened
- sample: a quest waiting for its reward is no longer relaunched when the popup opens

## [1.0.3] - 2026-11-24

### Fixed
- quest progress created at runtime is linked to its quest info, so quests progress even without saved player data (first launch, WebGL)

## [1.0.2] - 2022-10-18

### Fixed
- ability to have multiple quests for same target id

### Changed
- use StringDataList with object pool and update of required packages

## [1.0.1] - 2022-04-06

### Fixed
- Auto dependencies in package

## [1.0.0] - 2021-10-13

### Fixed

### Added
- First publish of this new package: player's quests and rewards management.

### Changed

### Deprecated

### Removed

### Fixed

### Security
