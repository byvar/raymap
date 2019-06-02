using Autodesk.Fbx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fbx {
	public class OpenSpaceToFbx {
		public string tempFilePath;
		static bool exportCancelled = false;

		 bool ExportProgressCallback(float percentage, string status) {
			// Convert from percentage to [0,1].
			// Then convert from that to [0.5,1] because the first half of
			// the progress bar was for creating the scene.
			var progress01 = 0.5f * (1f + (percentage / 100.0f));

			bool cancel = false;
			if (cancel) {
				exportCancelled = true;
			}

			// Unity says "true" for "cancel"; FBX wants "true" for "continue"
			return !cancel;
		}

		public void CreateFBX() {
			using (FbxManager fbxManager = FbxManager.Create()) {
				// Configure fbx IO settings.
				fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT));

				// Create the exporter
				var fbxExporter = FbxExporter.Create(fbxManager, "Exporter");
				
				// Initialize the exporter.
				// fileFormat must be binary if we are embedding textures
				int fileFormat = -1;

				bool status = fbxExporter.Initialize(tempFilePath, fileFormat, fbxManager.GetIOSettings());
				// Check that initialization of the fbxExporter was successful
				if (!status)
					return;
				
				// Set compatibility to 2014
				fbxExporter.SetFileExportVersion("FBX201400");

				// Set the progress callback.
				fbxExporter.SetProgressCallback(ExportProgressCallback);

				// Create a scene
				var fbxScene = FbxScene.Create(fbxManager, "Scene");

				// set up the scene info
				FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create(fbxManager, "SceneInfo");
				fbxSceneInfo.mTitle = "Raymap scene";
				//fbxSceneInfo.mSubject = "Subject";
				fbxSceneInfo.mAuthor = "Byvar & Robin";
				// fbxSceneInfo.mSubject = "Subject";
				/*fbxSceneInfo.mRevision = "1.0";
				fbxSceneInfo.mKeywords = Keywords;
				fbxSceneInfo.mComment = Comments;*/
				fbxSceneInfo.Original_ApplicationName.Set("Raymap");
				fbxSceneInfo.LastSaved_ApplicationName.Set(fbxSceneInfo.Original_ApplicationName.Get());
				fbxScene.SetSceneInfo(fbxSceneInfo);

				// Set up the axes (Y up, Z forward, X to the right) and units (centimeters)
				// Exporting in centimeters as this is the default unit for FBX files, and easiest
				// to work with when importing into Maya or Max
				var fbxSettings = fbxScene.GetGlobalSettings();
				fbxSettings.SetSystemUnit(FbxSystemUnit.cm);

				// The Unity axis system has Y up, Z forward, X to the right (left handed system with odd parity).
				// The Maya axis system has Y up, Z forward, X to the left (right handed system with odd parity).
				// We need to export right-handed for Maya because ConvertScene can't switch handedness:
				// https://forums.autodesk.com/t5/fbx-forum/get-confused-with-fbxaxissystem-convertscene/td-p/4265472
				fbxSettings.SetAxisSystem(FbxAxisSystem.MayaYUp);

				// export set of object
				FbxNode fbxRootNode = fbxScene.GetRootNode();
				// stores how many objects we have exported, -1 if export was cancelled
				int exportProgress = 0;
				IEnumerable<GameObject> revisedExportSet = null;

				// Total # of objects to be exported
				// Used by progress bar to show how many objects will be exported in total
				// i.e. exporting x/count... 
				int count = 0;
				
			}
		}
	}
}
