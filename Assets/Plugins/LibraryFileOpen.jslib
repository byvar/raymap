var LibraryFileOpen = {
   Alert: function(msgptr) {
	 window.alert(Pointer_stringify(msgptr));
   },
   GetFileData: function(filenameptr) {
	 var filename = Pointer_stringify(filenameptr);
	 var filedata = window.filedata[filename];
	 var ptr = (window.fileptr = window.fileptr ? window.fileptr : {})[filename] = _malloc(filedata.byteLength);
	 var dataHeap = new Uint8Array(HEAPU8.buffer, ptr, filedata.byteLength);
	 dataHeap.set(new Uint8Array(filedata));
	 return ptr;
   },
   GetFileDataLength: function(filenameptr) {
	 var filename = Pointer_stringify(filenameptr);
	 console.log("GetFileDataLength");
	 console.log(filename);
	 console.log(window);
	 console.log(window.filedata);
	 return window.filedata[filename].byteLength;
   },
   FreeFileData: function(filenameptr) {
	 var filename = Pointer_stringify(filenameptr);
	 _free(window.fileptr[filename]);
	 delete window.fileptr[filename];
	 delete window.filedata[filename];
   }
};
 
mergeInto(LibraryManager.library, LibraryFileOpen);
