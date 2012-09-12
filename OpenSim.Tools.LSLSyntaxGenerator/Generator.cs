﻿/*
 * Copyright (c) Contributors, Teesside University Centre for Construction Innovation and Research
 * See CONTRIBUTORS.md for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using OpenMetaverse.StructuredData;

using OpenSim.Region.ScriptEngine.Shared.ScriptBase;
using OpenSim.Region.ScriptEngine.Shared.Api.Interfaces;

namespace TeessideUniversity.CCIR.OpenSim.Tools
{
    class LSLSyntaxGenerator
    {
        /// <summary>
        /// Generates and writes the LSL Syntax helper data to a JSON file
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            OSDMap output = SyntaxHelpers();

            File.WriteAllText("../doc/opensim.lsl.syntax-helper.json",
                    OSDParser.SerializeJson(output, true).ToJson().ToString(),
                    Encoding.UTF8);

            Console.Write("Done");
            while (true)
            {
            }
        }

        /// <summary>
        /// Gets the LSL Syntax helper data
        /// </summary>
        /// <returns>an OSDMap of script methods and constants</returns>
        public static OSDMap SyntaxHelpers()
        {
            OSDMap output = new OSDMap();

            output["LSL"] = (OSD)ToOSDArray(GetMethods(typeof(ILSL_Api)));
            output["OSSL"] = (OSD)ToOSDArray(GetMethods(typeof(IOSSL_Api)));
            output["LS"] = (OSD)ToOSDArray(GetMethods(typeof(ILS_Api)));
            output["MOD"] = (OSD)ToOSDArray(GetMethods(typeof(IMOD_Api)));
            output["ScriptConstants"] = (OSD)ToOSDArray(ScriptConstants());

            return output;
        }

        /// <summary>
        /// Deduplicates the code for converting a string List to an OSDArray
        /// </summary>
        /// <param name="input">The list to be converted</param>
        /// <returns>The OSDArray representing the input</returns>
        private static OSDArray ToOSDArray(List<string> input)
        {
            return (new OSDArray(input.ConvertAll<OSD>(x =>
            {
                return (OSDString)x;
            })));
        }

        /// <summary>
        /// Uses reflection to get the methods defined on the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<string> GetMethods(Type type)
        {
            List<string> resp = new List<string>();

            foreach (MethodInfo method in type.GetMethods())
            {
                resp.Add(method.Name);
            }

            return resp;
        }

        /// <summary>
        /// Gets the constants defined on ScriptBaseClass
        /// </summary>
        /// <returns>A list of script constants</returns>
        private static List<string> ScriptConstants()
        {
            List<string> resp = new List<string>();

            foreach (FieldInfo field in typeof(ScriptBaseClass).GetFields(
                    BindingFlags.Public | BindingFlags.Static))
            {
                if (field.IsLiteral && !field.IsInitOnly)
                    resp.Add(field.Name);
            }

            return resp;
        }
    }
}