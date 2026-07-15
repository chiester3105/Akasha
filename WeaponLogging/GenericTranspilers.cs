using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Akasha.WeaponLogging
{

    public static class GenericTranspiler
    {
        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName,
            MethodInfo original,
            MethodInfo replacement)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(original))
                {
                    // stack before:
                    // Unit, PersistentID, float

                    foreach (var emit in loader)
                        yield return emit.Clone();

                    // stack after:
                    // Unit, PersistentID, float, string

                    yield return new CodeInstruction(OpCodes.Call, replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName,
            MethodInfo original,
            MethodInfo replacement)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(original))
                {
                    // stack currently:
                    // Unit, PersistentID, float

                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);

                    // stack becomes:
                    // Unit, PersistentID, float, string

                    yield return new CodeInstruction(OpCodes.Call, replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    public static class TakeDamageTranspiler
    {
        private static readonly Type[] OriginalParameters =
        [
            typeof(float),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(PersistentID)
        ];

        private static readonly Type[] NewParameters =
        [
            typeof(IDamageable),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(PersistentID),
        typeof(string)
        ];

        private static readonly MethodInfo Original =
            AccessTools.Method(typeof(IDamageable), nameof(IDamageable.TakeDamage), OriginalParameters);

        private static readonly MethodInfo Replacement =
            AccessTools.Method(typeof(TakeDamageExtensions), nameof(TakeDamageExtensions.TakeDamage), NewParameters);

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    foreach (var emit in loader)
                        yield return emit.Clone();

                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {

                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }


    public static class ArmorPenetrateTranspiler // TODOw
    {
        private static readonly Type[] OriginalParameters =
        [
            typeof(Vector3),
        typeof(Vector3),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(PersistentID)
        ];

        private static readonly Type[] NewParameters =
        [
            typeof(Vector3),
        typeof(Vector3),
        typeof(float),
        typeof(float),
        typeof(float),
        typeof(PersistentID),
        typeof(string)
        ];

        private static readonly MethodInfo Original =
            AccessTools.Method(typeof(DamageEffects), nameof(DamageEffects.ArmorPenetrate), OriginalParameters);

        private static readonly MethodInfo Replacement =
            AccessTools.Method(typeof(DamageEffectExtensions), nameof(DamageEffectExtensions.ArmorPenetrate),
                NewParameters);


        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    foreach (var emit in loader)
                        yield return emit.Clone();


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);

                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    public static class RecordDamageTranspiler
    {
        private static readonly MethodInfo Original =
            AccessTools.Method(typeof(Unit), nameof(Unit.RecordDamage), [typeof(PersistentID), typeof(float)]);

        private static readonly MethodInfo Replacement =
            AccessTools.Method(typeof(WeaponLoggingExtensions), nameof(WeaponLoggingExtensions.RecordDamage));


        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    foreach (var emit in loader)
                        yield return emit.Clone();


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }


    public static class BlastFragTranspiler
    {
        private static readonly MethodInfo Original =
            AccessTools.Method(typeof(DamageEffects), nameof(DamageEffects.BlastFrag));

        private static readonly MethodInfo Replacement =
            AccessTools.Method(typeof(DamageEffectExtensions), nameof(DamageEffectExtensions.BlastFrag));

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    foreach (var emit in loader)
                    {
                        yield return emit.Clone();
                    }


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);


                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
    public static class TakeShockwaveTranspiler
    {
        private static readonly Type[] OriginalParameters =
        [
            typeof(Vector3),
        typeof(float),
        typeof(float)
        ];

        private static readonly Type[] NewParameters =
        [
            typeof(IDamageable),
        typeof(Vector3),
        typeof(float),
        typeof(float),
        typeof(string)
        ];

        private static readonly MethodInfo Original =
            AccessTools.Method(typeof(IDamageable), nameof(IDamageable.TakeShockwave), OriginalParameters);

        private static readonly MethodInfo Replacement =
            AccessTools.Method(typeof(TakeShockwaveExtensions), nameof(TakeShockwaveExtensions.TakeShockwave),
                NewParameters);

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            IEnumerable<CodeInstruction> loadWeaponName)
        {
            var loader = loadWeaponName.ToList();
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {

                    foreach (var emit in loader)
                        yield return emit.Clone();

                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Inject(
            IEnumerable<CodeInstruction> instructions,
            string weaponName)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(Original))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, weaponName);

                    yield return new CodeInstruction(OpCodes.Call, Replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}