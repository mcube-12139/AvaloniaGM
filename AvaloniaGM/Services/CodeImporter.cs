using AvaloniaGM.Entities;
using System;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services {
    internal class CodeImporter(UndertaleData data) {
        readonly List<(UndertaleCode replaced, string code)> replacements = [];

        internal void AddReplacement(UndertaleCode replaced, string code) {
            replacements.Add((replaced, code));
        }

        internal void Import() {
            CodeParser parser = new();
            CodeGenerator generator = new(data);

            foreach ((_, string code) in replacements) {
                CodeRoot root = parser.Parse(code);
                generator.Generate(root);
                Console.WriteLine(generator);
            }
        }
    }
}
