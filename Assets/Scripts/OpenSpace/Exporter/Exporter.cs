using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.OpenSpace.Exporter {
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

            if (!Directory.Exists(exportDirectoryLevel)) {
                Directory.CreateDirectory(exportDirectoryLevel);
            }
            if (!Directory.Exists(exportDirectoryCommon)) {
                Directory.CreateDirectory(exportDirectoryCommon);
            }
            if (!Directory.Exists(exportDirectoryAIModels)) {
                Directory.CreateDirectory(exportDirectoryAIModels);
            }

            ExportAIModels(exportDirectoryAIModels);
            ExportEntryActions(exportDirectoryCommon);
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
    }
}
