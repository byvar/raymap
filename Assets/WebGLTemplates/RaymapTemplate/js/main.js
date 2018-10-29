'use strict';

function fmtMSS(s){return(s-(s%=60))/60+(9<s?':':':0')+s}

function fmtHMSS(s) {
	let secs = s % 60;
	let minshours = ((s - secs) / 60)
	let mins = minshours % 60;
	let hours = (minshours - mins) / 60;
	return (hours>0? (hours+':'+(9<mins?'':'0')):'')+mins+ (9<secs?':':':0') + secs;
}


function absolutizeURL(url) {
	let el = document.createElement('div');
	let escapedHTML = url.toString().split('&').join('&amp;').split('<').join('&lt;').split('"').join('&quot;');
	el.innerHTML = '<a href="' + escapedHTML + '">x</a>';
	return el.firstChild.href;
}

function formatBytes(bytes,decimals) {
	if(bytes == 0) return '0 bytes';
	let k = 1024,
	    dm = decimals + 1 || 3,
		sizes = ['bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
        i = Math.floor(Math.log(bytes) / Math.log(k));
	return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
}

let waitForFinalEvent = (function () {
  let timers = {};
  return function (callback, ms, uniqueId) {
	if (!uniqueId) {
	  uniqueId = "Don't call this twice without a uniqueId";
	}
	if (timers[uniqueId]) {
	  clearTimeout (timers[uniqueId]);
	}
	timers[uniqueId] = setTimeout(callback, ms);
  };
})();

function basename(str) {
   let base = new String(str).substring(str.lastIndexOf('/') + 1); 
    if(base.lastIndexOf(".") != -1)       
        base = base.substring(0, base.lastIndexOf("."));
   return base;
}


// Animation support
let transEndEventNames = {
	'WebkitTransition' : 'webkitTransitionEnd',
	'MozTransition'    : 'transitionend',
	'transition'       : 'transitionend'
};
let transEndEventName = transEndEventNames[ Modernizr.prefixed('transition') ];
let support = Modernizr.csstransitions;

// GLOBAL VARIABLES
let baseURL = "./"
let baseURL_local = "https://getramone.com/raymap/"
let gameLoading = true;
let currentJSON = null;
let baseTitle = "Raymap"
let notificationTimeout = null;
let dialogueMsg = null;
let fullData = null;
let objectsList = [];
let currentSO = null;

let wrapper, objects_content, unity_content, description_content, description_column;
let btn_close_description, stateSelector, objectListSelector;

// FUNCTIONS	
function addSongsToPlaylist(songsJSON) {
	let api = playlist_content.data('jsp');
	//api.getContentPane().empty();
	let items = [];
	playlistJSON = playlistJSON.concat(songsJSON);
	$.each( songsJSON, function(index, value) {
		items.push("<li class='playlist-item'>");
		items.push("<div class='song-handle'><i class='icon-menu'></i></div>");
		items.push("<div class='song-details song-game'>" + value.album_short_title + "</div>");
		items.push("<div class='song-details song-number'>" + value.track + "</div>");
		items.push("<div class='song-details song-title'>" + value.title + "</div>");
		items.push("<div class='song-details song-time'>" + fmtMSS(value.length_seconds) +"</div>");
		items.push("<div class='song-details song-remove' title='Remove from playlist'><i class=\"icon-remove\"></i></div></li>");
	});
	playlist_content_list.append(items.join(""));
	btn_clear_playlist.removeClass('disabled-button');
	refreshPlaylistText();
	if(cur_tab_index != 2) {
		if(songsJSON.length == 1) {
			let value = songsJSON[0];
			showNotification("Added to playlist: (" + value.album_short_title + ") " +  value.track + " - " + value.title, true);
		} else {
			showNotification("Added " + songsJSON.length + " tracks to the playlist.", true);
		}
	}
	//api.getContentPane().append(items.join(""));
	// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
	setTimeout(function(){
		refreshPlaylistSortable(true);
		api.reinitialise();
		api.scrollToBottom(true);
	}, 100);
}

function addCurrentSoundtrack() {
	if(currentJSON == null) return;
	let newJSON = currentJSON.songs;
	
	let api = playlist_content.data('jsp');
	let items = [];
	$.each( newJSON, function(index, value) {
		newJSON[index]["album_link"] = currentJSON.link;
		newJSON[index]["album_title"] = currentJSON.album;
		newJSON[index]["album_short_title"] = currentJSON.short_title;
		items.push("<li class='playlist-item'>");
		items.push("<div class='song-handle'><i class='icon-menu'></i></div>");
		items.push("<div class='song-details song-game'>" + currentJSON.short_title + "</div>");
		items.push("<div class='song-details song-number'>" + value.track + "</div>");
		items.push("<div class='song-details song-title'>" + value.title + "</div>");
		items.push("<div class='song-details song-time'>" + fmtMSS(value.length_seconds) +"</div>");
		items.push("<div class='song-details song-remove' title='Remove from playlist'><i class=\"icon-remove\"></i></div></li>");
	});
	playlistJSON = playlistJSON.concat(newJSON);
	playlist_content_list.append(items.join(""));
	btn_clear_playlist.removeClass('disabled-button');
	refreshPlaylistText();
	if(cur_tab_index != 2) {
		if(newJSON.length == 1) {
			let value = newJSON[0];
			showNotification("Added to playlist: (" + value.album_short_title + ") " +  value.track + " - " + value.title, true);
		} else {
			showNotification("Added " + newJSON.length + " tracks to the playlist.", true);
		}
	}
	//api.getContentPane().append(items.join(""));
	// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
	setTimeout(function(){
		refreshPlaylistSortable(true);
		api.reinitialise();
		//api.scrollToBottom(true);
	}, 100);
}

function clearPlaylist() {
	if(playlistJSON.length > 0) {
		playIndex(-1);
		playlistJSON = [];
		refreshPlaylistText();
		$(".playlist-item").remove();
		btn_clear_playlist.addClass('disabled-button');
		let api = playlist_content.data('jsp');
		setTimeout(function(){
			refreshPlaylistSortable(true);
			api.reinitialise();
			api.scrollToY(0, false);
		}, 100);
	}
}

function initContent() {
	$.getJSON( "json/content.json", function( data ) {
		let api = games_content.data('jsp');
		let items = [];
		let categories = data.categories;
		let locationHash = window.location.hash;
		let isLoading = false;
		if(locationHash.length > 1) locationHash = locationHash.substr(1);
		$.each( categories, function(index_cat, value_cat) {
			let soundtracks = value_cat.soundtracks;
			items.push("<div class='game-category-item' alt='" + value_cat.name + "'>" + value_cat.name + "</div>");
			$.each( soundtracks, function(index, value) {
				//items.push("<a class='logo-item' href='#" + value.json + "' title='" + value.title + "'><img src='" + encodeURI(value.image) + "' alt='" + value.title + "'></a>");
				if(locationHash != null && locationHash == value.json) {
					isLoading = true;
					clickSoundtrack("./json/" + locationHash + ".json");
					items.push("<a class='game-item current-game-item' href='#" + value.json + "' title='" + value.title + "' data-logo='" + encodeURI(value.image) + "'>");
				} else {
					items.push("<a class='game-item' href='#" + value.json + "' title='" + value.title + "' data-logo='" + encodeURI(value.image) + "'>");
				}
				items.push("<div class='game-item-logo' style='background-image: url(\"" + encodeURI(value.image) + "\");' alt='" + value.title + "'></div>");
				items.push("<div class='game-item-title'>" + value.title + "</div></a>");
			});
		});
		if(!isLoading) {
			$("#btn-home").addClass("current-game-item");
			clickSoundtrack(null);
		}
		api.getContentPane().append(items.join(""));
		// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
		setTimeout(function(){
			api.reinitialise();
		}, 100);
	});
}

function sidebarUpdateArrows(sidebar_content) {
	let sidebar = sidebar_content.parent();
	let arrowUp = sidebar.find(".sidebar-arrow-up");
	let arrowDown = sidebar.find(".sidebar-arrow-down");
	let scrollTop = sidebar_content.scrollTop();
	let scrollBottom = sidebar_content.get(0).scrollHeight - sidebar_content.outerHeight() - scrollTop;
	if(scrollTop > 0) {
		arrowUp.removeClass('sidebar-arrow-disabled')
	} else {
		arrowUp.addClass('sidebar-arrow-disabled');
	}
	if(scrollBottom > 0) {
		arrowDown.removeClass('sidebar-arrow-disabled');
	} else {
		arrowDown.addClass('sidebar-arrow-disabled');
	}
}

function sidebarScrollUp(sidebar_content) {
	let scrollTop = sidebar_content.scrollTop();
	sidebar_content.animate({
        scrollTop: scrollTop - 25
    }, 100, "linear");
}

function sidebarScrollDown(sidebar_content) {
	let scrollTop = sidebar_content.scrollTop();
	sidebar_content.animate({
        scrollTop: scrollTop + 25
    }, 100, "linear");
}


function showNotification(msg,mobile_only) {
	if(mobile_only && $("#mobile-tabswitch-l").css('display')=='none') return;
	let notifPopup = $("#notification-popup");
	notifPopup.text(msg);
	notifPopup.removeClass('hidden-popup');
	if(notificationTimeout != null) clearTimeout(notificationTimeout);
	notificationTimeout = setTimeout(function(){
		notifPopup.addClass('hidden-popup');
	}, 3000);
}

function getSuperObjectByIndex(index) {
	return objectsList[index];
}

function parseSuperObject(so) {
	let items = [];
	objectsList.push(so);
	let type = "Unknown";
	if(so.hasOwnProperty("type")) {
		type = so.type;
	}
	if(type === "World") {
		items.push("<div class='objects-item object-world' alt='" + so.name + "'>" + so.name + "</div>");
	} else if(type === "Perso") {
		let family = "Family";
		let model = "Model";
		let instance = "Instance";
		if(so.hasOwnProperty("perso")) {
			family = so.perso.nameFamily;
			model = so.perso.nameModel;
			instance = so.perso.nameInstance;
		}
		items.push("<div class='objects-item object-perso' title='" + so.name + "'><div class='name-family'>" + family + "</div><div class='name-model'>" + model + "</div><div class='name-instance'>" + instance + "</div></div>");
	} else {
		items.push("<div class='objects-item object-regular' title='" + so.name + "'>" + so.name + "</div>");
	}
	if(so.hasOwnProperty("children")) {
		$.each(so.children, function(i, child) {
			items = items.concat(parseSuperObject(child));
		});
	}
	return items;
}

function setAllJSON(jsonString) {
	//alert(jsonString);
	console.log(jsonString); 
	fullData = $.parseJSON(jsonString);
	if(fullData != null) {
		if(fullData.hasOwnProperty("actualWorld")) {
			let api = objects_content.data('jsp');
			let actualWorld = parseSuperObject(fullData.actualWorld);
			api.getContentPane().append(actualWorld.join(""));
			// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
			setTimeout(function(){
				api.reinitialise();
			}, 100);
		}
	}
}

function showObjectDescription(so) {
	$('#posX').val(so.position[0]);
	$('#posY').val(so.position[1]);
	$('#posZ').val(so.position[2]);
	
	$('#rotX').val(so.rotation[0]);
	$('#rotY').val(so.rotation[1]);
	$('#rotZ').val(so.rotation[2]);
	
	$('#sclX').val(so.scale[0]);
	$('#sclY').val(so.scale[1]);
	$('#sclZ').val(so.scale[2]);
	
	if(so.hasOwnProperty("perso")) {
		$('.perso-description').removeClass('invisible');
		stateSelector.empty();
		objectListSelector.empty();
		objectListSelector.append("<option value='0'>None</option>");
		let family = fullData.families[so.perso.family];
		if(family != null) {
			if(family.hasOwnProperty("states")) {
				$.each(family.states, function (idx, val) {
					stateSelector.append("<option value='" + idx + "'>" + val + "</option>");
				});
				stateSelector.prop("selectedIndex", so.perso.state);
			}
			if(family.hasOwnProperty("objectLists")) {
				$.each(family.objectLists, function (idx, val) {
					objectListSelector.append("<option value='" + idx + "'>" + val + "</option>");
				});
			}
		}
		if(fullData.hasOwnProperty("uncategorizedObjectLists")) {
			$.each(fullData.uncategorizedObjectLists, function (idx, val) {
				objectListSelector.append("<option value='" + idx + "'>" + val + "</option>");
			});
		}
		objectListSelector.prop("selectedIndex", so.perso.objectList);
	} else {
		$('.perso-description').addClass('invisible');
	}
	let api = description_content.data('jsp');
	setTimeout(function(){
		api.reinitialise();
	}, 100);
	btn_close_description.removeClass('disabled-button');
	description_column.removeClass('invisible');
}

function setObjectList(ol) {
	if(currentSO != null && currentSO.hasOwnProperty("perso")) {
		currentSO.perso.objectList = ol;
		let jsonObj = {}
		let perso = {}
		perso["offset"] = currentSO.perso.offset;
		perso["objectList"] = ol;
        jsonObj["perso"] = perso;
		gameInstance.SendMessage("Loader", "ParseMessage", JSON.stringify(jsonObj));
	}
}
function setState(state) {
	if(currentSO != null && currentSO.hasOwnProperty("perso")) {
		currentSO.perso.state = state;
		let jsonObj = {}
		let perso = {}
		perso["offset"] = currentSO.perso.offset;
		perso["state"] = state;
        jsonObj["perso"] = perso;
		gameInstance.SendMessage("Loader", "ParseMessage", JSON.stringify(jsonObj));
	}
}

/*function clickSoundtrack(jsonFile) {
	mobileTab(1);
	if( !support ) {
		onEndLoading(jsonFile);
	} else {
		ost_content.addClass('loading');
		ost_header.addClass('loading-header');
		ost_footer.addClass('loading-header');
		ost_sidebar.addClass('loading-sidebar');
		btn_play_soundtrack.addClass('invisible-button');
		btn_download_mp3.addClass('invisible-button');
		btn_download_flac.addClass('invisible-button');
		btn_soundtrack_info.addClass('invisible-button');
		ost_content.off(transEndEventName);
		ost_content.on(transEndEventName, function() {
			onEndLoading(jsonFile);
		});
	}
}*/


// POPUPS
function startDialogue(message,initDelay) {
	$('#dialogue-content').empty();
	let spans = '<span>' + message.split('').join('</span><span>') + '</span>';
	let addDelay = 0;
	if(initDelay) {
		addDelay += 300;
	}
	dialogueMsg = message;
	$(spans).hide().appendTo('#dialogue-content').each(function (i) {
		$(this).delay(50 * i + addDelay).css({
			display: 'inline',
			opacity: 0
		}).animate({
			opacity: 1
		}, 50);
		let text = $(this).text();
		if(text == "." || text == "," || text == "?" || text == "!") {
			addDelay += 200;
		}
	});
}

function skipDialogue() {
	if(dialogueMsg != null) {
		$('#dialogue-content').empty().text(dialogueMsg);
	}
}

function showDialogue(image, message, style, share, covers) {
	if(image != null) {
		$('#dialogue-image').css('background-image',"url(\"" + encodeURI(image) + "\")");
	} else {
		$('#dialogue-image').css('background-image','none');
	}
	$('#popup-overlay').removeClass('hidden-overlay');
	let initDelay = false;
	if($('#dialogue-popup').hasClass('hidden-popup')) {
		$('#dialogue-popup').removeClass('hidden-popup');
		initDelay = true;
	}
	if(share) {
		$('#share-popup').removeClass('hidden-popup');
	}
	if(covers && currentJSON.hasOwnProperty("covers")) {
		setPopupCover(0);
		$('#cover-popup').removeClass('hidden-popup');
	}
	startDialogue(message, initDelay);
}

function hideDialogue() {
	$('#popup-overlay').addClass('hidden-overlay');
	$('#dialogue-popup').addClass('hidden-popup');
	dialogueMsg = null;
}


// DOCUMENT INIT
$(function() {
	window.addEventListener('allJSON', function (e) {
		setAllJSON(e.detail);
	}, false);
	
	
	wrapper = $('#wrapper');
	objects_content = $('#content-objects');
	unity_content = $('#content-unity');
	description_content = $('#content-description');
	description_column = $('.column-description');
	btn_close_description = $('#btn-close-description');
	stateSelector = $('#state');
	objectListSelector = $('#objectList');
	
	if(window.location.protocol == "file:") {
		baseURL = baseURL_local;
	}
	
	$(document).on('click', ".objects-item", function() {
		let index = $(".objects-item").index(this);
		$(".objects-item").removeClass("current-objects-item");
		$(this).addClass("current-objects-item");
		let so = getSuperObjectByIndex(index);
		currentSO = so;
		showObjectDescription(so);
		return false;
	});
	
	$(document).on('click', "#btn-close-description", function() {
		description_column.addClass('invisible');
		$(".objects-item").removeClass("current-objects-item");
		currentSO = null;
		$(this).addClass("disabled-button");
		return false;
	});
	
	$(document).on('click', "#btn-fullscreen", function() {
		gameInstance.SetFullscreen(1);
		return false;
	});
	
	$(document).on('change', "#objectList", function() {
		let selectedIndex = $(this).prop('selectedIndex');
		setObjectList(selectedIndex);
		$(this).blur();
		return false;
	});
	$(document).on('change', "#state", function() {
		let selectedIndex = $(this).prop('selectedIndex');
		setState(selectedIndex);
		$(this).blur();
		return false;
	});
	
	
	
	$(document).on('click', "a.logo-item", function() {
		let hash = jQuery(this).attr("href").substr(1);
		clickSoundtrack("./json/" + hash + ".json");
		return false;
	});
	
	$(document).on('click', "a.game-item", function() {
		let hash = jQuery(this).attr("href").substr(1);
		$(".current-game-item").removeClass("current-game-item");
		jQuery(this).addClass("current-game-item");
		clickSoundtrack("./json/" + hash + ".json");
		return false;
	});
	
	$(document).on('click', ".song-item", function() {
		if(currentJSON != null) {
			let songItem = jQuery(this);
			let songsJSON = [getSongJSON(songItem)];
			addSongsToPlaylist(songsJSON);
			//mobileTab(2);
		}
		return false;
	});
	
	$(document).on('click', ".song-button-play", function() {
		if(currentJSON != null) {
			let songItem = $(this).parent().parent();
			let songsJSON = [getSongJSON(songItem)];
			mobileTab(2);
			addSongsToPlaylist(songsJSON);
			playIndex(playlistJSON.length-1);
		}
		return false;
	});
	
	$(document).on('click', ".song-button-download", function() {
		if(currentJSON != null) {
			let songItem = $(this).parent().parent();
			let songJSON = getSongJSON(songItem);
			downloadSong(songJSON,false);
		}
		return false;
	});
	$(document).on('click', ".song-button-info", function() {
		if(currentJSON != null) {
			let songItem = $(this).parent().parent();
			let ind = songItem.index(".song-item");
			let songJSON = getSongJSON(songItem);
			showSongDialogue(songJSON, ind);
		}
		return false;
	});
	
	$(document).on('click', "#popup-overlay", function() {
		hideDialogue();
		return false;
	});
	$(document).on('click', "#dialogue-content", function() {
		skipDialogue();
		return false;
	});
	$(document).on('click', ".style-item", function() {
		let title = $(this).find(".style-item-text").text();
		setActiveStyleSheet(title);
		$(".current-style-item").removeClass("current-style-item");
		$(this).addClass("current-style-item");
		return false;
	});
	
	
	$(document).on('click', ".sidebar-button", function() {
		if(currentJSON != null) {
			let butt = jQuery(this);
			let buttIndex = $(".sidebar-button").index(butt);
			if(currentJSON != null && currentJSON.hasOwnProperty("icons") && currentJSON.icons.length > buttIndex) {
				let trackNum = currentJSON.icons[buttIndex].track-1;
				let trackRef = $(".song-item").eq(trackNum);
				let api = ost_content.data('jsp');
				api.scrollToY(trackRef.position().top, true);
			}
		}
		return false;
	});
	
	$(".column-sidebar-content").scroll(function() {
		let cont = $(this);
		waitForFinalEvent(function(){
			sidebarUpdateArrows(cont);
		}, 20, "sidebar scrolly");
	});
	
	let sidebarScrollInterval = false;
	$('.sidebar-arrow-up').mouseover(function(){
	   clearInterval(sidebarScrollInterval);
		let cont = $(this).parent().find(".column-sidebar-content");
	    sidebarScrollInterval = setInterval(function(){
		   sidebarScrollUp(cont);
	   }, 100);
	});
	$('.sidebar-arrow-up').mousedown(function(){
	   clearInterval(sidebarScrollInterval);
		let cont = $(this).parent().find(".column-sidebar-content");
	    sidebarScrollInterval = setInterval(function(){
		   sidebarScrollUp(cont);
	   }, 100);
	});
	$('.sidebar-arrow-down').mouseover(function(){
	   clearInterval(sidebarScrollInterval);
		let cont = $(this).parent().find(".column-sidebar-content");
	    sidebarScrollInterval = setInterval(function(){
		   sidebarScrollDown(cont);
	   }, 100);
	});
	$('.sidebar-arrow-down').mousedown(function(){
	   clearInterval(sidebarScrollInterval);
		let cont = $(this).parent().find(".column-sidebar-content");
	    sidebarScrollInterval = setInterval(function(){
		   sidebarScrollDown(cont);
	   }, 100);
	});
	$('.sidebar-arrow').mouseout(function(){
	   clearInterval(sidebarScrollInterval);
	   sidebarScrollInterval = false;
	});
	$('.sidebar-arrow').mouseup(function(){
	   clearInterval(sidebarScrollInterval);
	   sidebarScrollInterval = false;
	});
	
	let pane = $('.column-content-scroll');
	let settings = {
		horizontalGutter: 0,
		verticalGutter: 0,
		animateEase: "swing"
	};
	pane.jScrollPane(settings);
	initContent();
	
});

$( window ).resize(function() {
	waitForFinalEvent(function(){
		
		$(".column-content-scroll").each( function(index) {
			let api = $( this ).data('jsp');
			api.reinitialise();
		});
	}, 3, "some unique string");
});