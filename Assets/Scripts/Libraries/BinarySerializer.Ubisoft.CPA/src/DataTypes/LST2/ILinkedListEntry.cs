namespace BinarySerializer.Ubisoft.CPA {
    interface ILinkedListEntry {
        Pointer NextEntry { get; }
        Pointer PreviousEntry { get; }
    }
}
