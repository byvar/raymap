var LibraryRaymap = {
	SetAllJSON: function(msgptr) {
		var allJSON = Pointer_stringify(msgptr);
		var event = new CustomEvent('allJSON', { detail: allJSON });
        window.dispatchEvent(event);
	}
};
 
mergeInto(LibraryManager.library, LibraryRaymap);
