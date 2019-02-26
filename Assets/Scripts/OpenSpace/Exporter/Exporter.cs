using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Input;
using OpenSpace.Object.Properties;
using OpenSpace.Text;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class Exporter {
        private MapLoader loader;
        private string exportPath;
        private string gameName;

        public Exporter(MapLoader loader, string exportPath)
        {
            this.loader = loader;
            this.exportPath = exportPath;
            this.gameName = Settings.s.mode.ToString();
        }

        public void Export()
        {

            string exportDirectoryLevel = Path.Combine(this.exportPath, loader.lvlName);
            string exportDirectoryCommon = Path.Combine(this.exportPath, "Common");

            string exportDirectoryAIModels = Path.Combine(exportDirectoryCommon, "AIModels");
            string exportDirectoryMaterialsCommon = Path.Combine(exportDirectoryCommon, "Materials");
            string exportDirectoryTexturesCommon = Path.Combine(exportDirectoryCommon, "Textures");
            string exportDirectoryFamiliesCommon = Path.Combine(exportDirectoryCommon, "Families");

            if (!Directory.Exists(exportDirectoryLevel)) {
                Directory.CreateDirectory(exportDirectoryLevel);
            }
            if (!Directory.Exists(exportDirectoryCommon)) {
                Directory.CreateDirectory(exportDirectoryCommon);
            }
            if (!Directory.Exists(exportDirectoryAIModels)) {
                Directory.CreateDirectory(exportDirectoryAIModels);
            }
            if (!Directory.Exists(exportDirectoryMaterialsCommon)) {
                Directory.CreateDirectory(exportDirectoryMaterialsCommon);
            }
            if (!Directory.Exists(exportDirectoryTexturesCommon)) {
                Directory.CreateDirectory(exportDirectoryTexturesCommon);
            }
            if (!Directory.Exists(exportDirectoryFamiliesCommon)) {
                Directory.CreateDirectory(exportDirectoryFamiliesCommon);
            }

            ExportTextures(exportDirectoryTexturesCommon);
            ExportMaterials(exportDirectoryMaterialsCommon, exportDirectoryLevel);
            ExportFamilies(exportDirectoryFamiliesCommon);
            ExportAIModels(exportDirectoryAIModels);
            ExportEntryActions(exportDirectoryCommon);
            ExportTextTable(exportDirectoryCommon);
        }

        private void ExportTextures(string texturePath)
        {
            foreach(TextureInfo texture in loader.textures) {

                if (texture==null) {
                    continue;
                }
                string textureName = texture.name != null ? texture.name : texture.offset.offset.ToString("X8");
                string path = Path.Combine(texturePath, texture.name + ".png");
                if (!Directory.Exists(Path.GetDirectoryName(path))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                if (File.Exists(path)) {
                    File.Delete(path);
                }

                using (FileStream fileStream = File.Create(path)) {

                    byte[] pngData = ImageConversion.EncodeToPNG(texture.Texture);
                    fileStream.Write(pngData, 0, pngData.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        private void ExportMaterials(string commonPath, string levelPath)
        {
            foreach (VisualMaterial vismat in loader.visualMaterials) {

                string path = commonPath;

                string contents = vismat.ToJSON();
                string contentsHash = HashUtils.MD5Hash(contents); // Identify material by its hash

                string matFileName = "VisualMaterial_" + contentsHash;

                string matFilePath = Path.Combine(path, matFileName + ".json");
                if (File.Exists(matFilePath)) {
                    File.Delete(matFilePath);
                }

                using (StreamWriter fileStream = File.CreateText(matFilePath)) {

                    fileStream.Write(contents);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }

            foreach (GameMaterial mat in loader.gameMaterials) {

                string path = commonPath;

                string contents = mat.ToJSON();
                string contentsHash = HashUtils.MD5Hash(contents); // Identify material by its hash

                string matFileName = "GameMaterial_" + contentsHash;

                string matFilePath = Path.Combine(path, matFileName+".json");
                if (File.Exists(matFilePath)) {
                    File.Delete(matFilePath);
                }

                using (StreamWriter fileStream = File.CreateText(matFilePath)) {

                    fileStream.Write(contents);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        private void ExportFamilies(string path)
        {

            foreach (Family fam in loader.families) {
                
                string filePath = Path.Combine(path, fam.name + ".json");
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }

                using (StreamWriter aiModelFileStream = File.CreateText(filePath)) {

                    aiModelFileStream.Write(fam.ToJSON());
                    aiModelFileStream.Flush();
                    aiModelFileStream.Close();
                }
            }
        }

        private void ExportAIModels(string path)
        {
            foreach (AIModel aiModel in loader.aiModels) {

                // TODO: this is Rayman 2 specific for now :(
                string aiModelCSharp = AIModelExporter.AIModelToCSharp_R2(gameName+".AIModels", aiModel);

                string filePath = Path.Combine(path, aiModel.name + ".cs");
                if (File.Exists(filePath)) {
                    File.Delete(filePath);

                }

                using (StreamWriter aiModelFileStream = File.CreateText(filePath)) {

                    aiModelFileStream.Write(aiModelCSharp);
                    aiModelFileStream.Flush();
                    aiModelFileStream.Close();
                }
            }
        }

        private void ExportEntryActions(string path)
        {
            List<string> entryActions = new List<string>();
            foreach (EntryAction action in loader.inputStruct.entryActions) {
                entryActions.Add(EntryActionExporter.EntryActionToCSharpClass(action));
            }


            string filePath = Path.Combine(path, "EntryActions.cs");
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            string[] usingItems = new string[] { "UnityEngine", "OpenSpaceImplementation.Input", "OpenSpaceImplementation" };
            string usingBlock = string.Join(Environment.NewLine, usingItems.Select(i => "using " + i + ";"));

            string fileContents = usingBlock + Environment.NewLine + "namespace " + gameName + ".EntryActions {" + Environment.NewLine +
                string.Join(Environment.NewLine, entryActions) + Environment.NewLine +
                "}";

            using (StreamWriter entryActionsFileStream = File.CreateText(filePath)) {

                entryActionsFileStream.Write(fileContents);
                entryActionsFileStream.Flush();
                entryActionsFileStream.Close();
            }
        }

        private static string EscapeStringForCSharp(string str)
        {
            if (str!=null)
            str = str.Replace((char)0x85, '\0'); // For some reason there's a ... whitespace character used sometimes
            return str;
        }

        private void ExportTextTable(string path)
        {
            string filePath = Path.Combine(path, "TextTable.cs");
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            string[] usingItems = new string[] { "UnityEngine", "OpenSpaceImplementation.Strings", "OpenSpaceImplementation" };
            string usingBlock = string.Join(Environment.NewLine, usingItems.Select(i => "using " + i + ";"));

            int numLanguages = loader.fontStruct.languages.Length;
            int numTextsPerLanguage = 0;

            List<string> textEntries = new List<string>();
            int languageIndex = 0;

            foreach(FontStructure.TextTable textTable in loader.fontStruct.languages) {
                if (textTable.num_entries>numTextsPerLanguage) {
                    numTextsPerLanguage = textTable.num_entries;
                }

                int entryIndex = 0;
                foreach(string entry in textTable.entries) {
                    textEntries.Add("SetString(" + languageIndex + ", "+(entryIndex++)+", \"" + EscapeStringForCSharp(entry) + "\");");
                }

                textEntries.Add(""); // Extra line break

                languageIndex++;
            }

            string textManagerConstructor = "public StringManager() {" + Environment.NewLine +
                "InitTable(" + loader.fontStruct.num_languages + ", " + loader.fontStruct.languages[0].num_entries_max + ");" + Environment.NewLine + Environment.NewLine +
                string.Join(Environment.NewLine, textEntries) +
                Environment.NewLine +
                "}";

            string content = "public class StringManager : OpenSpaceImplementation.Strings.StringManager {" + Environment.NewLine +
                    textManagerConstructor +
                    Environment.NewLine + 
                "}";

            string fileContents = usingBlock + Environment.NewLine + "namespace " + gameName + ".Text {" + Environment.NewLine +
                content + Environment.NewLine +
                "}";

            using (StreamWriter fileStream = File.CreateText(filePath)) {

                fileStream.Write(fileContents);
                fileStream.Flush();
                fileStream.Close();
            }
        }
    }
}
