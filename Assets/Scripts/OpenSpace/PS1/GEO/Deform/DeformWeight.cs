
namespace OpenSpace.PS1 {
	public class DeformWeight : OpenSpaceStruct {

		public ushort ind_bone;
		public ushort weight;

		protected override void ReadInternal(Reader reader) {
			ind_bone = reader.ReadUInt16();
			weight = reader.ReadUInt16();
		}
	}
}
