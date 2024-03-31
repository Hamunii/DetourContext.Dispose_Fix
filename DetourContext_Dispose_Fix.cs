using BepInEx.Logging;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using HarmonyLib;
using Mono.Cecil;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace DetourContext_Dispose_Fix;

internal class Patcher
{
    internal static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Hamunii.DetourContext.Dispose_Fix");
    private static ILHook disposeHook = null!;

    public static void Initialize()
    {
        disposeHook = new(AccessTools.DeclaredMethod(typeof(DetourContext), nameof(DetourContext.Dispose)), Fix_DetourContext_Dispose);
    }

    private static void Fix_DetourContext_Dispose(ILContext il)
    {
        /*
        // Implements the fix from https://github.com/MonoMod/MonoMod/pull/102
        // 
        // TL;DL: This patch changes this:
        // if (!IsDisposed)
        //     return;
        //
        // To this:
        // if (IsDisposed)
        //     return;
        */

        ILCursor c = new(il);

        bool found = c.TryGotoNext(
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<DetourContext>("IsDisposed"),
            x => x.MatchBrtrue(out _),
            x => x.MatchRet()
        );

        if (!found)
        {
            Logger.LogInfo("Nothing to patch.");
            return;
        }
        
        c.Index += 2;
        c.Next.OpCode = OpCodes.Brfalse_S;

        // Logger.LogInfo(il.ToString());
    }

    // Load us https://docs.bepinex.dev/articles/dev_guide/preloader_patchers.html
    public static IEnumerable<string> TargetDLLs { get; } = new string[] { };
    public static void Patch(AssemblyDefinition _) { }
}
