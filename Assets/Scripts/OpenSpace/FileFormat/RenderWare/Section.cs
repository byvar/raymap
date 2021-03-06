﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.FileFormat.RenderWare {
	public class Section {
		public Type type;
		public uint size;
		public uint version;

		public List<Section> children = new List<Section>();
		public Dictionary<string, object> variables = new Dictionary<string, object>();
		public byte[] data;

		public object this[string key] {
			get { return variables[key]; }
			set { variables[key] = value; }
		}

		public Section this[int key] {
			get { return children[key]; }
			set { children[key] = value; }
		}

		public Section(Type type, uint size, uint version) {
			this.type = type;
			this.size = size;
			this.version = version;
		}

		public void ReadChild(Reader reader, SectionRead specialRead = null) {
			children.Add(Section.Read(reader, specialRead: specialRead));
		}
		
		public static Section Read(Reader reader, SectionRead specialRead = null) {
			MapLoader l = MapLoader.Loader;
			Type type = (Type)reader.ReadUInt32();
			uint size = reader.ReadUInt32();
			uint version = reader.ReadUInt32();
			Section sec = new Section(type, size, version);
			if(specialRead != null) specialRead(sec);
			switch (sec.type) {
				case Type.String:
					sec.data = reader.ReadBytes((int)size);
					sec["string"] = System.Text.Encoding.UTF8.GetString(sec.data).TrimEnd('\0');
					break;
				case Type.TextureDictionary:
					ushort numTextures = 0;
					sec.ReadChild(reader, (s) => {
						numTextures = reader.ReadUInt16();
						s["numTextures"] = numTextures;
						s["unk"] = reader.ReadUInt16();
					}); // Info Struct
					for (uint i = 0; i < numTextures; i++) {
						sec.ReadChild(reader); // TextureNative
					}
					sec.ReadChild(reader); // Extension
					break;
				case Type.TextureNative:
					sec.ReadChild(reader, (s) => {
						s["platform"] = reader.ReadUInt32();
						s["filterMode"] = reader.ReadByte();
						s["addressingMode"] = reader.ReadByte();
						reader.ReadUInt16(); // padding
					}); // Info Struct
					sec.ReadChild(reader); // Texture name
					sec.ReadChild(reader); // Alpha name
					sec.ReadChild(reader, (st) => {
						st.ReadChild(reader, (s) => {
							s["width"] = reader.ReadUInt32();
							s["height"] = reader.ReadUInt32();
							s["bpp"] = reader.ReadUInt32();
							s["rasterFormat"] = reader.ReadUInt32();
							reader.ReadUInt32(); // PS2 TEX0 GS register
							reader.ReadUInt32(); // PS2 TEX0 GS register
							reader.ReadUInt32(); // PS2 TEX1 GS register
							reader.ReadUInt32(); // PS2 TEX1 GS register
							reader.ReadUInt32(); // PS2 MIPTBP1 GS register
							reader.ReadUInt32(); // PS2 MIPTBP1 GS register
							reader.ReadUInt32(); // PS2 MIPTBP2 GS register
							reader.ReadUInt32(); // PS2 MIPTBP2 GS register
							s["textureDataSize"] = reader.ReadUInt32();
							s["paletteDataSize"] = reader.ReadUInt32();
							s["gpuDataAlignedSize"] = reader.ReadUInt32();
							s["skyMipmapVal"] = reader.ReadUInt32();
						}); // Texture - info struct
						st.ReadChild(reader); // Texture - data
					}); // Texture
					sec.ReadChild(reader, (se) => {
						se.ReadChild(reader); // Extension - sky mipmap val - 4 bytes unknown
					}); // Extension
					break;
				case Type.Clump:
					uint numAtomics = 0;
					sec.ReadChild(reader, (s) => {
						numAtomics = reader.ReadUInt32();
						s["numAtomics"] = numAtomics;
					}); // Info Struct
					sec.ReadChild(reader); // Frame list
					sec.ReadChild(reader); // Geometry list
					for (int i = 0; i < numAtomics; i++) {
						sec.ReadChild(reader);
					}
					sec.ReadChild(reader); // Extension
					//l.print("Clump size: " + reader.BaseStream.Position);
					break;
				case Type.FrameList:
					uint numFrames = 0;
					sec.ReadChild(reader, (s) => {
						numFrames = reader.ReadUInt32();
						s["numFrames"] = numFrames;
						Frame[] frames = new Frame[numFrames];
						for (int i = 0; i < numFrames; i++) {
							frames[i] = new Frame();
							frames[i].matrix = Matrix.ReadRenderware(reader);
							frames[i].index = reader.ReadInt32();
							frames[i].flags = reader.ReadUInt32();
						}
						s["frames"] = frames;
					}); // Info Struct
					for (int i = 0; i < numFrames; i++) {
						sec.ReadChild(reader); // Extension
					}
					break;
				case Type.GeometryList:
					uint numMeshes = 0;
					sec.ReadChild(reader, (s) => {
						numMeshes = reader.ReadUInt32();
						s["numMeshes"] = numMeshes;
					}); // Info Struct
					for (int i = 0; i < numMeshes; i++) {
						sec.ReadChild(reader); // Geometry
					}
					break;
				case Type.Geometry:
					sec.ReadChild(reader, (s) => {
						s["geometry"] = Geometry.Read(reader);
					}); // Info Struct
					sec.ReadChild(reader); // Material list
					sec.ReadChild(reader, (s) => {
						if (s.size > 0) {
							s.ReadChild(reader); // Bin Mesh
						}
					}); // Extension
					break;
				case Type.MaterialList:
					uint numMaterials = 0;
					uint numUniqueMaterials = 0;
					sec.ReadChild(reader, (s) => {
						numMaterials = reader.ReadUInt32();
						int[] materialIndices = new int[numMaterials];
						for (int i = 0; i < numMaterials; i++) {
							materialIndices[i] = reader.ReadInt32();
							if (materialIndices[i] == -1) numUniqueMaterials++;
						}
						s["numMaterials"] = numMaterials;
						s["numUniqueMaterials"] = numUniqueMaterials;
						s["materialIndices"] = materialIndices;
					}); // Info struct
					for (int i = 0; i < numUniqueMaterials; i++) {
						sec.ReadChild(reader); // Material
					}
					break;
				case Type.Material:
					bool isTextured = false;
					 sec.ReadChild(reader, (s) => {
						Material mat = Material.Read(reader);
						s["material"] = mat;
						isTextured = mat.isTextured;
					}); // Material struct
					if (isTextured) {
						sec.ReadChild(reader);
					}
					sec.ReadChild(reader); // Extension
					if (sec.children.Last().data != null && sec.children.Last().data.Length > 0) {
						l.print("Check material extension ending at " + reader.BaseStream.Position + "!");
					}
					break;
				case Type.Texture:
					sec.ReadChild(reader, (s) => {
						s["texture"] = MaterialTexture.Read(reader);
					}); // Info struct
					sec.ReadChild(reader); // Texture name
					sec.ReadChild(reader); // Alpha name
					sec.ReadChild(reader); // Extension
					if (sec.children.Last().data != null && sec.children.Last().data.Length > 0) {
						l.print("Check texturematerial extension ending at " + reader.BaseStream.Position + "!");
					}
					break;
				case Type.BinMeshPlg:
					sec["binMesh"] = BinMesh.Read(reader);
					break;
				case Type.Atomic:
					sec.ReadChild(reader, (s) => {
						s["frameIndex"] = reader.ReadUInt32();
						s["geometryIndex"] = reader.ReadUInt32();
						s["flags"] = reader.ReadUInt32();
						s["unused"] = reader.ReadUInt32();
					}); // Struct
					sec.ReadChild(reader); // Extension
					break;
				default:
					if(specialRead == null) sec.data = reader.ReadBytes((int)size);
					break;
			}

			return sec;
		}

		public delegate void SectionRead(Section section);

		public enum Type {
			Struct = 1,
			String = 2,
			Extension = 3,
			Camera = 5,
			Texture = 6,
			Material = 7,
			MaterialList = 8,
			AtomicSection = 9,
			PlaneSection = 10,
			World = 11,
			Spline = 12,
			Matrix = 13,
			FrameList = 14,
			Geometry = 15,
			Clump = 16,
			Light = 18,
			UnicodeString = 19,
			Atomic = 20,
			TextureNative = 21,
			TextureDictionary = 22,
			AnimationDatabase = 23,
			Image = 24,
			SkinAnimation = 25,
			GeometryList = 26,
			AnimAnimation = 27,
			Team = 28,
			Crowd = 29,
			DeltaMorphAnimation = 30,
			RightToRender = 31,
			MultitextureEffectNative = 32,
			MultitextureEffectDictionary = 33,
			TeamDictionary = 34,
			PlatformIndependentTextureDictionary = 35,
			TableOfContents = 36,
			ParticleStandardGlobalData = 37,
			Altpipe = 38,
			PlatformIndependentPeds = 39,
			PatchMesh = 40,
			ChunkGroupStart = 41,
			ChunkGroupEnd = 42,
			UvAnimationDictionary = 43,
			CollTree = 44,
			MetricsPlg = 257,
			SplinePlg = 258,
			StereoPlg = 259,
			VrmlPlg = 260,
			MorphPlg = 261,
			PvsPlg = 262,
			MemoryLeakPlg = 263,
			AnimationPlg = 264,
			GlossPlg = 265,
			LogoPlg = 266,
			MemoryInfoPlg = 267,
			RandomPlg = 268,
			PngImagePlg = 269,
			BonePlg = 270,
			VrmlAnimPlg = 271,
			SkyMipmapVal = 272,
			MrmPlg = 273,
			LodAtomicPlg = 274,
			MePlg = 275,
			LightmapPlg = 276,
			RefinePlg = 277,
			SkinPlg = 278,
			LabelPlg = 279,
			ParticlesPlg = 280,
			GeomtxPlg = 281,
			SynthCorePlg = 282,
			StqppPlg = 283,
			PartPpPlg = 284,
			CollisionPlg = 285,
			HanimPlg = 286,
			UserDataPlg = 287,
			MaterialEffectsPlg = 288,
			ParticleSystemPlg = 289,
			DeltaMorphPlg = 290,
			PatchPlg = 291,
			TeamPlg = 292,
			CrowdPpPlg = 293,
			MipSplitPlg = 294,
			AnisotropyPlg = 295,
			GcnMaterialPlg = 297,
			GeometricPvsPlg = 298,
			XboxMaterialPlg = 299,
			MultiTexturePlg = 300,
			ChainPlg = 301,
			ToonPlg = 302,
			PtankPlg = 303,
			ParticleStandardPlg = 304,
			PdsPlg = 305,
			PrtadvPlg = 306,
			NormalMapPlg = 307,
			AdcPlg = 308,
			UvAnimationPlg = 309,
			CharacterSetPlg = 384,
			NohsWorldPlg = 385,
			ImportUtilPlg = 386,
			SlerpPlg = 387,
			OptimPlg = 388,
			TlWorldPlg = 389,
			DatabasePlg = 390,
			RaytracePlg = 391,
			RayPlg = 392,
			LibraryPlg = 393,
			Plg2d = 400,
			TileRenderPlg = 401,
			JpegImagePlg = 402,
			TgaImagePlg = 403,
			GifImagePlg = 404,
			QuatPlg = 405,
			SplinePvsPlg = 406,
			MipmapPlg = 407,
			MipmapkPlg = 408,
			Font2d = 409,
			IntersectionPlg = 410,
			TiffImagePlg = 411,
			PickPlg = 412,
			BmpImagePlg = 413,
			RasImagePlg = 414,
			SkinFxPlg = 415,
			VcatPlg = 416,
			Path2d = 417,
			Brush2d = 418,
			Object2d = 419,
			Shape2d = 420,
			Scene2d = 421,
			PickRegion2d = 422,
			ObjectString2d = 423,
			AnimationPlg2d = 424,
			Animation2d = 425,
			Keyframe2d = 432,
			Maestro2d = 433,
			Barycentric = 434,
			PlatformIndependentTextureDictionaryTk = 435,
			TocTk = 436,
			TplTk = 437,
			AltpipeTk = 438,
			AnimationTk = 439,
			SkinSplitTookit = 440,
			CompressedKeyTk = 441,
			GeometryConditioningPlg = 442,
			WingPlg = 443,
			GenericPipelineTk = 444,
			LightmapConversionTk = 445,
			FilesystemPlg = 446,
			DictionaryTk = 447,
			UvAnimationLinear = 448,
			UvAnimationParameter = 449,
			BinMeshPlg = 1294,
			NativeDataPlg = 1296,
			ZmodelerLock = 61982,
			AtomicVisibilityDistance = 39055872,
			ClumpVisibilityDistance = 39055873,
			FrameVisibilityDistance = 39055874,
			PipelineSet = 39056115,
			Unused5 = 39056116,
			TexdictionaryLink = 39056117,
			SpecularMaterial = 39056118,
			Unused8 = 39056119,
			Effect2d = 39056120,
			ExtraVertColour = 39056121,
			CollisionModel = 39056122,
			GtaHanim = 39056123,
			ReflectionMaterial = 39056124,
			Breakable = 39056125,
			Frame = 39056126,
			Unused16 = 39056127,
		}
	}
}
