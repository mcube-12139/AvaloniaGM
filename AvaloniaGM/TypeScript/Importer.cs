using System;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class Importer(UndertaleData data) {
        readonly List<(UndertaleCode replaced, string code)> replacements = [];

        internal void AddReplacement(UndertaleCode replaced, string code) {
            replacements.Add((replaced, code));
        }

        internal void Import() {
            Parser parser = new();
            Generator generator = new(data);

            foreach ((UndertaleCode replaced, string code) in replacements) {
                string source = replaced.Name.Content;

                CodeRoot root = parser.Parse(source, code);
                generator.Generate(source, root, replaced);
                Console.WriteLine(generator);
            }
        }
    }
}
