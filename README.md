# UniExt (`com.underdogg.uniext`)

Utility package with runtime extensions and lightweight state-machine helpers for Unity.

## Included modules

- `Runtime/Dictionary`:
  - `UniDict<TKey, TValue>`: serializable dictionary with duplicate-key policy for deserialization.
- `Runtime/CFSM`:
  - classic finite-state machine with typed state transitions.
- `Runtime/PayloadedFSM`:
  - payload-driven FSM + builder (`FSMBuilder`, `State`, `Action`, `Decision`, `Transition`).
- `Runtime/Extensions`:
  - collection helpers, random/math utilities, vector helpers, serialization helpers, UI helpers.

## UniDict notes

`UniDict<TKey, TValue>` stores entries in a serialized list and keeps runtime dictionary cache synchronized.

Important behavior:

- duplicate keys from serialized data are handled by policy:
  - `Throw`
  - `KeepFirst`
  - `KeepLast` (default)
- null keys are ignored during rebuild with optional warning logs.
- `CopyTo` now follows `IDictionary` contract checks.

## Unity compatibility

Target baseline: Unity `2022.3 LTS+`.

`UniDict` inspector drawer supports:

- IMGUI (`OnGUI`)
- UI Toolkit (`CreatePropertyGUI`)

## Breaking changes in current revision

- `CFSM.States` is now read-only (`IReadOnlyDictionary`) and validated on construct.
- `FSMBuilder` is now single-use and enforces build invariants.
- `IObservable<T>` now includes `SetValue(T value, bool forceNotify = false)`.
- `ListExt.Trim` is obsolete in favor of `PopLast`.
- `ListExt.Peek` is obsolete in favor of `PeekFirst`.
- `MathTool.GetRandomKeyFromWeightDict` is obsolete in favor of `WeightedRandom.TryPickKey`.
- `MathTool.GetVec3ByString` is obsolete in favor of `MathTool.TryParseVector3`.

## Quick usage examples

```csharp
using com.underdogg.uniext.Runtime.Dictionary;

[Serializable]
public class LootTable
{
    public UniDict<string, int> Weights = new();
}
```

```csharp
using com.underdogg.uniext.Runtime.Extensions;

if (WeightedRandom.TryPickKey(lootWeights, out var key))
{
    Debug.Log($"Picked: {key}");
}
```
