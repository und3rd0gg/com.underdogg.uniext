# Changelog

All notable changes to this package are documented in this file.

## [3.0.0] - 2026-02-28

### Added

- `UniDict` duplicate-key handling policy (`Throw`, `KeepFirst`, `KeepLast`).
- `UniDict` APIs:
  - `TryAdd`
  - `SetOrReplace`
- UI Toolkit support for `UniDict` inspector drawer.
- `WeightedRandom.TryPickKey(...)` utility.
- `MathTool.TryParseVector3(...)`.
- `ObservableVariable<T>.SetValue(...)`.
- `ListExt` safe helpers:
  - `PeekFirst`
  - `TryPeekFirst`
  - `PopLast`
  - `TryPopLast`
  - `TryGetKeyByValue`

### Changed

- Reworked `UniDict` serialization sync strategy to prevent inspector data loss.
- `UniDict` now validates null/duplicate keys during rebuild.
- `UniDict.CopyTo(...)` now follows `IDictionary` argument validation contract.
- `CFSM` state storage is now encapsulated and validated.
- `PayloadedFSM` (`FSM`, `FSMBuilder`, `Transition`) now enforces construction/runtime invariants.
- `ComponentExt` now uses cached reflection metadata and reports copy skips explicitly.
- `CanvasGroupExt`, `LayerMaskExt`, `RandomExt`, `DataExt`, `EnumerableExt`, `DrawExt` now validate inputs.
- `Underdogg.UniExt.asmdef`: `allowUnsafeCode` set to `false`.

### Deprecated

- `ListExt.Peek(...)` in favor of `PeekFirst(...)`.
- `ListExt.Trim(...)` in favor of `PopLast(...)`.
- `ListExt.KeyByValue(...)` in favor of `TryGetKeyByValue(...)`.
- `MathTool.GetRandomKeyFromWeightDict(...)` in favor of `WeightedRandom.TryPickKey(...)`.
- `MathTool.GetVec3ByString(...)` in favor of `MathTool.TryParseVector3(...)`.

### Breaking

- Package version bumped to `3.0.0`.
- `IObservable<T>` now requires `SetValue(T value, bool forceNotify = false)`.
- `CFSM.States` changed from mutable field to read-only property.
- `FSMBuilder` is now single-use and throws on invalid build pipeline usage.
