# DetourContext.Dispose Fix

This BepInEx Patcher applies the fix from [Fix DetourContext.Dispose #102](https://github.com/MonoMod/MonoMod/pull/102), which makes `MonoMod.RuntimeDetour.DetourContext.Dispose` work properly. This is only useful if you are using a version of **MonoMod.RuntimeDetour** which doesn't have this patch.

If the patcher doesn't find the IL code it is looking to patch, it will print `Nothing to patch.` to the BepInEx console. If this is the case, this patcher isn't needed.

## Why This Matters

If you are using `DetourContext`, e.g.:
```cs
using(new DetourContext(){ Priority = 100 })
{
    On.StartOfRound.Awake += Hook_2;
}
On.StartOfRound.Awake += Hook_1;
```
The `DetourContext` wouldn't dispose of itself, and `Hook_1` would run before `Hook_2` because it also got applied to it.  
With this patcher, the `DetourContext` will dispose of itself and `Hook_2` will run before `Hook_1` like it should.