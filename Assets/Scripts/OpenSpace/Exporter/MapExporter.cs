using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    public partial class MapExporter {
        private MapLoader loader;
        private string exportPath;
        private string gameName;

        public static JsonSerializerSettings JsonExportSettings
        {
            get
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                settings.ContractResolver = JsonIgnorePointersResolver.Instance;
                settings.SerializationBinder = new OpenSpaceTypesBinder();

                return settings;
            }
        }

        public MapExporter(MapLoader loader, string exportPath)
        {
            this.loader = loader;
            this.exportPath = exportPath;
            this.gameName = Settings.s.mode.ToString();
        }

        public void Export()
        {

            string exportDirectoryLevels = Path.Combine(this.exportPath, "Levels");
            string exportDirectoryAIModels = Path.Combine(this.exportPath, "AIModels");
            string exportDirectoryMaterials = Path.Combine(this.exportPath, "Materials");
            string exportDirectoryFamilies = Path.Combine(this.exportPath, "Families");
            string exportDirectoryGeneral = Path.Combine(this.exportPath, "General");
            string exportDirectoryTextures = Path.Combine(this.exportPath, "Resources", "Textures");

            if (!Directory.Exists(exportDirectoryLevels)) {
                Directory.CreateDirectory(exportDirectoryLevels);
            }
            if (!Directory.Exists(exportDirectoryAIModels)) {
                Directory.CreateDirectory(exportDirectoryAIModels);
            }
            if (!Directory.Exists(exportDirectoryMaterials)) {
                Directory.CreateDirectory(exportDirectoryMaterials);
            }
            if (!Directory.Exists(exportDirectoryTextures)) {
                Directory.CreateDirectory(exportDirectoryTextures);
            }
            if (!Directory.Exists(exportDirectoryFamilies)) {
                Directory.CreateDirectory(exportDirectoryFamilies);
            }
            if (!Directory.Exists(exportDirectoryGeneral)) {
                Directory.CreateDirectory(exportDirectoryGeneral);
            }

            ExportTextures(exportDirectoryTextures);
            ExportMaterials(exportDirectoryMaterials, exportDirectoryLevels);
            ExportFamilies(exportDirectoryFamilies);
            //ExportAIModels(exportDirectoryAIModels);
            ExportEntryActions(exportDirectoryGeneral);
            ExportTextTable(exportDirectoryGeneral);
            ExportScene(exportDirectoryLevels);
        }

        private void ExportScene(string path)
        {
            SerializedScene es = new SerializedScene(loader);
            string sceneJSON = es.ToJSON();

            string filePath = Path.Combine(path, loader.lvlName + ".json");
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            using (StreamWriter sceneFileStream = File.CreateText(filePath)) {

                sceneFileStream.Write(sceneJSON);
                sceneFileStream.Flush();
                sceneFileStream.Close();
            }
        }

        private void ExportTextures(string texturePath)
        {
            foreach (TextureInfo texture in loader.textures) {

                if (texture == null) {
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
        }

        private void ExportFamilies(string path)
        {
            foreach (Family fam in loader.families) {

                string familyDirectory = Path.Combine(path, fam.name);

                if (!Directory.Exists(familyDirectory)) {
                    Directory.CreateDirectory(familyDirectory);
                }

                string filePath = Path.Combine(familyDirectory, "Family_" + fam.name + ".json");
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }

                using (StreamWriter aiModelFileStream = File.CreateText(filePath)) {

                    aiModelFileStream.Write(fam.ToJSON());
                    aiModelFileStream.Flush();
                    aiModelFileStream.Close();
                }

                foreach (ObjectList objectList in fam.objectLists) {
                    string objectListJSON = objectList.ToJSON();
                    string objectListHash = HashUtils.MD5Hash(objectListJSON);

                    string objectListFilePath = Path.Combine(familyDirectory, "ObjectList_" + objectListHash + ".json");
                    if (File.Exists(objectListFilePath)) {
                        File.Delete(objectListFilePath);
                    }

                    using (StreamWriter aiModelFileStream = File.CreateText(objectListFilePath)) {

                        aiModelFileStream.Write(objectListJSON);
                        aiModelFileStream.Flush();
                        aiModelFileStream.Close();
                    }
                }
            }
        }

        private void ExportAIModels(string path)
        {
            foreach (AIModel aiModel in loader.aiModels) {

                // TODO: this is Rayman 2 specific for now :(
                string aiModelCSharp = AIModelExporter.AIModelToCSharp_R2(gameName + ".AIModels", aiModel);

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
            if (str != null)
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

            foreach (FontStructure.TextTable textTable in loader.fontStruct.languages) {
                if (textTable.num_entries > numTextsPerLanguage) {
                    numTextsPerLanguage = textTable.num_entries;
                }

                int entryIndex = 0;
                foreach (string entry in textTable.entries) {
                    textEntries.Add("SetStringForLanguage(" + languageIndex + ", " + (entryIndex++) + ", \"" + EscapeStringForCSharp(entry) + "\");");
                }

                textEntries.Add(""); // Extra line break

                languageIndex++;
            }

            string textManagerConstructor = "public TextManager() {" + Environment.NewLine +
                "InitTable(" + loader.fontStruct.num_languages + ", " + loader.fontStruct.languages[0].num_entries_max + ");" + Environment.NewLine + Environment.NewLine +
                string.Join(Environment.NewLine, textEntries) +
                Environment.NewLine +
                "}";

            string content = "public class TextManager : OpenSpaceImplementation.Strings.TextManager {" + Environment.NewLine +
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