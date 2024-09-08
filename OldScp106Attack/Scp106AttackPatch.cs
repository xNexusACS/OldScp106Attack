using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp106;

namespace OldScp106Attack;

[HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
public static class Scp106AttackPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnServerShoot(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        const int offset = 2;
        int index = newInstructions.FindIndex(x => x.Calls(AccessTools.Method(typeof(Hitmarker), nameof(Hitmarker.SendHitmarkerDirectly), new[] { typeof(ReferenceHub), typeof(float), typeof(bool) }))) + offset;
        
        List<Label> labels = newInstructions.GetRange(index, 28).FindAll(x => x.labels.Count > 0).SelectMany(x => x.labels).ToList();
        
        newInstructions.RemoveRange(index, 28);
        
        newInstructions[index].labels.AddRange(labels);

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
