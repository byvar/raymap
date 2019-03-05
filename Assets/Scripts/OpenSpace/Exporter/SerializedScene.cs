using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenSpace.AI;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenSpace.Exporter {

    public class SerializedScene {

        // Scene properties

        public SerializedPersoData PersoData;
        public SerializedGraphData GraphData;
        public SerializedWorldData WorldData;

        public SerializedScene(MapLoader loader)
        {
            PersoData = new SerializedPersoData(loader.persos);
            GraphData = new SerializedGraphData(loader.graphs, loader.graphNodes, loader.waypoints);
            WorldData = new SerializedWorldData(loader.sectors);
        }

        public string ToJSON()
        {
            JsonSerializerSettings settings = MapExporter.JsonExportSettings;

            settings.Converters.Add(new UnityConverter());
            settings.Converters.Add(new VisualMaterial.VisualMaterialReferenceJsonConverter());
            settings.Converters.Add(new GameMaterial.GameMaterialReferenceJsonConverter());
            settings.Converters.Add(new WayPoint.WayPointReferenceJsonConverter());

            return JsonConvert.SerializeObject(this, settings);
        }
    }


}