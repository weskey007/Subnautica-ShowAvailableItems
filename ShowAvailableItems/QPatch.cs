using HarmonyLib;
using System;
using System.Reflection;

namespace ShowAvailableItems
{
	public static class QPatch
	{
#if SN1
        internal const string EASYCRAFT_ASSEMBLY = "EasyCraft";
#elif BZ
        internal const string EASYCRAFT_ASSEMBLY = "EasyCraft_BZ";
#endif
        
		public static void Patch()
		{
			Logger.Log("Patching...");
			var harmony = new Harmony("weskey.subnautica.showavailableitems.mod");
			
			var tooltipFactoryTranspiler = AccessTools.Method(typeof(ToolTipFactory_HarmonyPatch), "Transpiler");

			PatchIfExists(harmony, EASYCRAFT_ASSEMBLY, "EasyCraft.Main", "WriteIngredients", null, null, new HarmonyMethod(tooltipFactoryTranspiler));
            PatchIfExists(harmony, "Assembly-CSharp", "TooltipFactory", "WriteIngredients", null, null, new HarmonyMethod(tooltipFactoryTranspiler));

            Logger.Log("Patching complete");
		}

        public static void PatchIfExists(Harmony harmony, string assemblyName, string typeName, string methodName, HarmonyMethod prefix, HarmonyMethod postfix, HarmonyMethod transpiler)
        {
            var assembly = FindAssembly(assemblyName);
            if (assembly == null)
            {
                Logger.Log("Could not find assembly " + assemblyName + ", don't worry this probably just means you don't have the mod installed");
                return;
            }

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                Logger.Log("Could not find class/type " + typeName + ", the mod/assembly " + assemblyName + " might have changed");
                return;
            }

            Logger.Log("Found targetClass "+ typeName);
            var targetMethod = AccessTools.Method(targetType, methodName);
			if (targetMethod != null)
			{
				Logger.Log("Found targetMethod "+ typeName + "."+methodName+", Patching...");
				harmony.Patch(targetMethod, prefix, postfix, transpiler);
				Logger.Log("Patched "+ typeName + "."+methodName);
			}
			else
			{
				Logger.Log("Could not find method "+ typeName + "."+methodName+ ", the mod/assembly " + assemblyName + " might have been changed");
			}
		}

        private static Assembly FindAssembly(string assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.FullName.StartsWith(assemblyName + ","))
                    return assembly;

            return null;
        }
	}
}
