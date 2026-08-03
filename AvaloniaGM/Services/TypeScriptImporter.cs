using AvaloniaGM.Entities;
using System;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services {
    internal class TypeScriptImporter(UndertaleData data) {
        readonly List<(UndertaleCode replaced, string code)> replacements = [];

        internal void AddReplacement(UndertaleCode replaced, string code) {
            replacements.Add((replaced, code));
        }

        internal void Import() {
            TypeScriptParser parser = new();
            TypeScriptGenerator generator = new(data);

            foreach ((UndertaleCode replaced, string code) in replacements) {
                CodeRoot root = parser.Parse(replaced.Name.Content, code);
                generator.Generate(root, replaced);
                Console.WriteLine(generator);
            }
        }
    }
}
