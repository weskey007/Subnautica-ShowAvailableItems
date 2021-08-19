using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;

namespace ShowAvailableItems
{
	public static class ToolTipFactory_HarmonyPatch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var foundTargetString = false;
			int startIndex = -1, endIndex = -1;
	
			var codes = new List<CodeInstruction>(instructions);
			
			for (int i = 0; i < codes.Count; i++)
			{
				if (!foundTargetString && codes[i].opcode == OpCodes.Pop)
				{
					startIndex = i + 1; // exclude current 'pop'
					
					for (int j = startIndex; j < codes.Count; j++)
					{
						if (codes[j].opcode == OpCodes.Pop)
							break;
						
						var strOperand = codes[j].operand as String;
						if (strOperand == " (")
						{
							foundTargetString = true;
							break;
						}
					}
				}
				
				if (foundTargetString && (codes[i].opcode == OpCodes.Bge || codes[i].opcode == OpCodes.Bge_S))
				{
					endIndex = i; // include 'bge'
					break;
				}
			}
			
			if (startIndex > -1 && endIndex > -1)
			{
				Logger.Log("Targeted section to transpile: "+startIndex+" till "+endIndex);

				codes[startIndex].opcode = OpCodes.Nop;
				codes.RemoveRange(startIndex + 1, endIndex - startIndex);
			}
			else
				Logger.Log("Could not find transpiling target");
			
			return codes;
		}
	}
}
