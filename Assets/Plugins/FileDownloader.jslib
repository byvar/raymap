mergeInto(LibraryManager.library, {

	// To save files from Unity into the browser. Textures could be saved this way, for example.
	SaveFile : function(array, size, fileNamePtr)
    {
        var fileName = Pointer_stringify(fileNamePtr);
     
        var bytes = new Uint8Array(size);
        for (var i = 0; i < size; i++)
        {
           bytes[i] = HEAPU8[array + i];
        }
     
        var blob = new Blob([bytes]);
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
     
        var event = document.createEvent("MouseEvents");
        event.initMouseEvent("click", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        link.dispatchEvent(event);
    },

});