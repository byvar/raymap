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


function getRandomInt(max) {
	return Math.floor(Math.random() * Math.floor(max));
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
var entityMap = {
	'&': '&amp;',
	'<': '&lt;',
	'>': '&gt;',
	'"': '&quot;',
	"'": '&#39;',
	'/': '&#x2F;',
	'`': '&#x60;',
	'=': '&#x3D;'
  };
function escapeHTML (string) {
	return String(string).replace(/[&<>"'`=\/]/g, function (s) {
		return entityMap[s];
	});
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
let baseURL_local = "https://maps.raym.app/"
let gameLoading = true;
let currentJSON = null;
let gamesJSON = null;
let levelsJSON = null;
let baseTitle = "Raymap"
let notificationTimeout = null;
let versionsReinitTimeout = null;
let levelsReinitTimeout = null;
let dialogueMsg = null;
let fullData = null;
let hierarchy = null;
let objectsList = [];
let currentSO = null;
let gameInstance = null;
let inputHasFocus = false;
let gameSettings = null;
let mode, lvl, folder;

let currentBehavior = null;
let currentBehaviorType = "";
let wrapper, objects_content, unity_content, description_content, description_column;
let gameColumnContent, screenshotResolutionRadio, screenshotSizeFactorRadio, screenshotResolutionW, screenshotResolutionH, screenshotSizeFactor = null;
let screenshotResolutionSelected = false;
let btn_close_description, stateSelector, objectListSelector, languageSelector, cameraPosSelector, cinematicSelector, cinematicActorSelector, highlight_tooltip, text_highlight_tooltip, text_highlight_content, objectListInputGroup;
let previousState = -1;
let games_content, versions_content, levels_content, levels_sidebar = null;
let games_header, versions_header, levels_header = null;
let current_game, current_version = null;
let levels_actors_group, actor1_group, actor2_group, actor1_selector, actor2_selector = null;

// FUNCTIONS
function getNoCacheURL() {
	return '?nocache=' + (new Date()).getTime();
}
function sendMessage(jsonObj) {
	if(gameInstance != null) {
		//console.log("Message: " + JSON.stringify(jsonObj));
		gameInstance.SendMessage("Loader", "ParseMessage", JSON.stringify(jsonObj));
	}
}

function initContent() {
	$.getJSON( "json/content.json" + getNoCacheURL(), function( data ) {
		let api = games_content.data('jsp');
		gamesJSON = data;
		let items = [];
		let categories = data.categories;
		let isLoading = false;
		let gameSelected = false;
		$.each( categories, function(index_cat, value_cat) {
			let games = value_cat.games;
			items.push("<div class='game-category-item' alt='" + escapeHTML(value_cat.name) + "'>" + escapeHTML(value_cat.name) + "</div>");
			$.each( games, function(index, value) {
				let thisGameSelected = false;
				if(!gameSelected) {
					$.each( value.versions, function(idx_ver, val_ver) {
						if(!gameSelected && folder !== null && val_ver.hasOwnProperty("folder") && (folder === val_ver.folder || folder.startsWith(val_ver.folder + "/"))) {
							gameSelected = true;
							thisGameSelected = true;
							return false;
						}
					});
				}
				let titleHTML = escapeHTML(value.title);
				if(thisGameSelected) {
					versions_content.off(transEndEventName);
					initGame(value);
					$(".current-game-item").removeClass("current-game-item");
					items.push("<div class='game-item current-game-item' data-game='" + value.json + "' title='" + titleHTML + "' data-logo='" + encodeURI(value.image) + "'>");
				} else {
					items.push("<div class='game-item' data-game='" + value.json + "' title='" + titleHTML + "' data-logo='" + encodeURI(value.image) + "'>");
				}
				items.push("<div class='game-item-logo' style='background-image: url(\"" + encodeURI(value.image) + "\");' alt='" + titleHTML + "'></div>");
				items.push("<div class='game-item-title'>" + titleHTML + "</div></div>");
			});
		});
		/*if(!isLoading) {
			$("#btn-home").addClass("current-game-item");
			clickGame(null);
		}*/
		api.getContentPane().append(items.join(""));
		// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
		setTimeout(function(){
			games_header.removeClass('loading-header');
			games_content.removeClass('loading');
			api.reinitialise();
		}, 100);
	});
	/*$.getJSON( "json/content.json?1", function( data ) {
		$('#sidebar-levels').removeClass('hidden-sidebar');
		$('.sidebar-button').remove();
		$('#sidebar-levels-slider').css('top','0px');
		$('#sidebar-levels').scrollTop(0);
		let sidebarItems = [];
		levelsJSON = data;
		let api = $("#content-levels").data('jsp');
		let items = [];
		let games = data.games;
		let totalEm = 0;
		$.each( games, function(index_game, value_game) {
			let levels = value_game.levels;
			items.push("<div class='levels-item game' alt='" + value_game.name + "'>" + value_game.name + "</div>");
			totalEm += 3;
			let iconClass = "world-image";
			if(levels.length < 7) {
				iconClass = iconClass + " small";
			}
			let iconSidebar = "<div class='sidebar-button' style='background-image: url(\"" + encodeURI(value_game.image) + "\");'></div>";
			items.push("<div class='" + iconClass + "' style='background-image: url(\"" + encodeURI(value_game.image) + "\"); top: " + (totalEm+1) + "em;'></div>");
			sidebarItems.push(iconSidebar);
			$.each( levels, function(index, value) {
				let levelFolder = value.hasOwnProperty("folder") ? value.folder : value_game.folder;
				//items.push("<a class='logo-item' href='#" + value.json + "' title='" + value.title + "'><img src='" + encodeURI(value.image) + "' alt='" + value.title + "'></a>");
				if(value_game.mode === mode && folder === levelFolder && value.level === lvl) {
					items.push("<div class='levels-item level current-levels-item' title='" + value.name + "'><div class='name'>" + value.name + "</div><div class='internal-name'>" + value.level + "</div></div>");
					document.title = " [" + value_game.name + "] " + value.name + " - " + baseTitle;
				} else {
					items.push("<a class='levels-item level' href='index.html?mode=" + value_game.mode + "&folder=" + levelFolder + "&lvl=" + value.level + "' title='" + value.name + "'><div class='name'>" + value.name + "</div><div class='internal-name'>" + value.level + "</div></a>");
				}
				totalEm += 2;
			});
		});
		api.getContentPane().append(items.join(""));
		$('#sidebar-levels-content').append(sidebarItems.join(""));
		sidebarUpdateArrows($(".column-sidebar-content"));
		// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
		setTimeout(function(){
			api.reinitialise();
		}, 100);
	});*/
}

function refreshScroll() {
	waitForFinalEvent(function(){
		$(".column-content-scroll").each( function(index) {
			let api = $( this ).data('jsp');
			api.reinitialise();
		});
		sidebarUpdateArrows($(".column-sidebar-content"));
		recalculateAspectRatio();
	}, 3, "some unique string");
}

function setLevelsSidebarSlider(pos, isAtTop, isAtBottom) {
	if(levelsJSON != null && levelsJSON.hasOwnProperty("icons") && $(".levels-item.level").length > 0) {
		// Find which section you're scrolling in
		let i = 0;
		let highlight_i = 0;
		let allow_margin = 50;
		for(i = 0; i < levelsJSON.icons.length; i++) {
			let trackNum = levelsJSON.icons[i].level;
			let trackRef = $(".levels-item.level").eq(trackNum);
			if(pos + allow_margin >= trackRef.position().top) {
				highlight_i = i;
			} else {
				break;
			}
		}
		// Calculate height for special cases
		let height = 1;
		if(isAtBottom && isAtTop) {
			highlight_i = 0;
			height = levelsJSON.icons.length;
		} else if(isAtTop && highlight_i !== 0) {
			height = 1 + highlight_i;
			highlight_i = 0;
		} else if(isAtBottom && highlight_i !== levelsJSON.icons.length - 1) {
			height = levelsJSON.icons.length - highlight_i;
		}

		// Add class
		//let butt = $(".sidebar-button").eq(highlight_i);
		//let buttPos = butt.position().top;
		//$('#sidebar-soundtrack-slider').css('top',buttPos + 'px');
		$('#sidebar-levels-slider').css({
			'top': (highlight_i * 4) + 'em',
			'height': (height * 4) + 'em'
		});
		/*if(!butt.hasClass('sidebar-button-active')) {
			$(".sidebar-button").removeClass('sidebar-button-active');
			butt.addClass('sidebar-button-active');
		}*/
	}
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

function selectButton(button, selected) {
	if(selected) {
		button.addClass("selected");
	} else {
		button.removeClass("selected");
	}
}

// SUPEROBJECT PARSING
function getSuperObjectByIndex(index) {
	return objectsList[index];
}
function getPersoNameHTML(perso) {
	let nameStr = "";
	if(perso.hasOwnProperty("NameFamily")) nameStr += "<div class='name-family'>" + perso.NameFamily + "</div>";
	if(perso.hasOwnProperty("NameModel")) nameStr += "<div class='name-model'>" + perso.NameModel + "</div>";
	if(perso.hasOwnProperty("NameInstance")) nameStr += "<div class='name-instance'>" + perso.NameInstance + "</div>";
	return nameStr;
}
function parseSuperObject(so, level) {
	let items = [];
	objectsList.push(so);
	let type = "Unknown";
	if(so.hasOwnProperty("Type")) {
		type = so.Type;
	}
	let nameHTML = escapeHTML(so.Name);
	switch(type) {
		case "World":
			items.push("<div class='objects-item object-world level-" + level + "' alt='" + nameHTML + "'>" + nameHTML + "</div>");
			break;
		case "Perso":
			items.push("<div class='objects-item object-perso' title='" + nameHTML + "'>");
			if(so.hasOwnProperty("Perso")) {
				items.push(getPersoNameHTML(so.Perso));
			}
			items.push("</div>");
			break;
		case "Sector":
			items.push("<div class='objects-item object-sector level-" + level + "' title='" + nameHTML + "'>" + nameHTML + "</div>");
			break;
		case "IPO":
		case "IPO_2":
			items.push("<div class='objects-item object-IPO level-" + level + "' title='" + nameHTML + "'>" + nameHTML + "</div>");
			break;
		default:
			items.push("<div class='objects-item object-regular' title='" + nameHTML + "'>" + nameHTML + "</div>");
			break;
			
	}
	if(so.hasOwnProperty("Children")) {
		$.each(so.Children, function(i, child) {
			items = items.concat(parseSuperObject(child, level+1));
		});
	}
	return items;
}
function parseAlways(alwaysData) {
	let items = [];
	let so = {
		Offset: "null",
		Type: "AlwaysWorld",
		Position: { x: 0, y: 0, z: 0 },
		Rotation: { x: 0, y: 0, z: 0 },
		Scale:    { x: 0, y: 0, z: 0 }
	};
	objectsList.push(so);
	items.push("<div class='objects-item object-world level-0' alt='Spawnable objects'>Spawnable objects</div>");
	
	if(alwaysData.hasOwnProperty("SpawnablePersos")) {
		$.each(alwaysData.SpawnablePersos, function(i, child) {
			let perso = alwaysData.SpawnablePersos[i];
			let family = perso.NameFamily;
			let model = perso.NameModel;
			let instance = perso.NameInstance;
			items.push("<div class='objects-item object-always object-perso' title='Spawnable'>" + getPersoNameHTML(perso) + "</div>");
			let persoSO = {
				Offset:   perso.Offset,
				Type:     "Perso",
				Perso:    perso,
				Position: perso.Position,
				Rotation: perso.Rotation,
				Scale:    perso.Scale
			};
			objectsList.push(persoSO);
		});
	}
	return items;
}

function parseDynamicSuperObjects(dynamicSO) {
	let items = [];
	let so = {
		Offset: "null",
		Type: "World",
		Position: { x: 0, y: 0, z: 0 },
		Rotation: { x: 0, y: 0, z: 0 },
		Scale:    { x: 0, y: 0, z: 0 }
	};
	objectsList.push(so);
	items.push("<div class='objects-item object-world level-0' alt='Dynamic World'>Dynamic World</div>");
	
	$.each(dynamicSO, function(i, child) {
		items = items.concat(parseSuperObject(child, 1));
	});
	return items;
}
function handleMessage_settings(msg) {
	$(".settings-toggle").removeClass("disabled-button");
	$("#btn-lighting").removeClass("disabled-button");
	if(msg.EnableLighting) {
		$(".lighting-settings").removeClass("disabled-button");
	} else {
		$(".lighting-settings").addClass("disabled-button");
	}
	selectButton($("#btn-lighting"), msg.EnableLighting);
	selectButton($("#btn-saturate"), !msg.Saturate);
	selectButton($("#btn-fog"), msg.EnableFog);
	selectButton($("#btn-viewCollision"), msg.ViewCollision);
	selectButton($("#btn-viewGraphs"), msg.ViewGraphs);
	selectButton($("#btn-viewInvisible"), msg.ViewInvisible);
	selectButton($("#btn-displayInactive"), msg.DisplayInactive);
	selectButton($("#btn-showPersos"), msg.ShowPersos);
	selectButton($("#btn-playAnimations"), msg.PlayAnimations);
	selectButton($("#btn-playTextureAnimations"), msg.PlayTextureAnimations);
	$("#range-luminosity").val(msg.Luminosity);
}
function updateCinematic() {
	let index = cinematicSelector.prop("selectedIndex");
	
	let jsonObj = {
		CineData: {
			CinematicIndex: index
		}
	};
	let animationSpeed = $('#cinematicSpeed').val();
	if($.isNumeric(animationSpeed) && parseInt(animationSpeed) >= 0) {
		jsonObj.CineData.AnimationSpeed = parseInt(animationSpeed);
	}
	sendMessage(jsonObj);
}
function selectCinematicActor() {
	let index = cinematicActorSelector.prop("selectedIndex");
	if(index !== 0) {
		let persoIndex = cinematicActorSelector.val();
		cinematicActorSelector.prop("selectedIndex", 0);
		if($.isNumeric(persoIndex) && persoIndex > -1) {
			let so = getSuperObjectByIndex(persoIndex);
			if(so.hasOwnProperty("Perso")) {
				setSelectionPerso(so.Perso);
			}
		}
	}
}
function getIndexFromPerso(perso) {
	if(hierarchy != null) {
		for (let i = 0; i < objectsList.length; i++) {
			if(!objectsList[i].hasOwnProperty("Perso")) {
				continue;
			}
			if(objectsList[i].Perso.Offset === perso.Offset) {
				return i;
			}
		}
	}
	return -1;
}
function handleMessage_cineData(msg) {
	$("#btn-cine").removeClass("disabled-button");
	if(msg.hasOwnProperty("CinematicNames")) {
		cinematicSelector.empty();
		$.each(msg.CinematicNames, function (idx, cine) {
			cinematicSelector.append("<option value='" + idx + "'>" + escapeHTML(cine) + "</option>");
		});
	}
	if(msg.hasOwnProperty("CinematicIndex")) {
		cinematicSelector.prop("selectedIndex", msg.CinematicIndex);
	}
	if(msg.hasOwnProperty("AnimationSpeed")) {
		$('#cinematicSpeed').val(msg.AnimationSpeed);
		$("#cine-speed-group").removeClass("invisible");
	} else {
		$("#cine-speed-group").addClass("invisible");
	}
	cinematicActorSelector.empty();
	cinematicActorSelector.append("<option value='null'>Select an actor...</option>");
	if(msg.hasOwnProperty("Actors")) {
		$("#cine-actor-group").removeClass("invisible");
		$.each(msg.Actors, function (idx, act) {
			cinematicActorSelector.append("<option value='" + getIndexFromPerso(act) + "'>" + escapeHTML(act.NameInstance) + "</option>");
		});
	} else {
		$("#cine-actor-group").addClass("invisible");
	}
	cinematicActorSelector.prop("selectedIndex", 0);
}
function updateCameraPos() {
	let selectedCameraPos = cameraPosSelector.val();
	let classString = "";
	switch(selectedCameraPos) {
		case "Front":
			classString = "front";
			break;
		case "Back":
			classString = "back";
			break;
		case "Left":
			classString = "left";
			break;
		case "Right":
			classString = "right";
			break;
		case "Top":
			classString = "top";
			break;
		case "Bottom":
			classString = "bottom";
			break;
		case "Initial":
			classString = "initial";
			break;
		case "IsometricFront":
			classString = "isometric-front";
			break;
		case "IsometricBack":
			classString = "isometric-back";
			break;
		case "IsometricLeft":
			classString = "isometric-left";
			break;
		case "IsometricRight":
			classString = "isometric-right";
			break;
	}
	if(classString !== "") {
		$("#camera-cube").removeClass (function (index, className) {
			return (className.match (/(^|\s)show-\S+/g) || []).join(' ');
		});
		$("#camera-cube").addClass("show-" + classString);
		let jsonObj = {
			Type: "Camera",
			Camera: {
				CameraPos: selectedCameraPos
			}
		};
		sendMessage(jsonObj);
	}
}
function recalculateAspectRatio() {
	if(!$("#camera-popup").hasClass("hidden-popup") && $("input[name='screenshotRadio']:checked").val() === "resolution") {
		let resW = screenshotResolutionW.val();
		let resH = screenshotResolutionH.val();
		if($.isNumeric(resW) && $.isNumeric(resH) && resW > 0 && resH > 0) {
			let width = gameColumnContent.outerWidth();
			let height = gameColumnContent.outerHeight();
			if(width > 0 && height > 0) {
				let aspectRatio = width*1.0 / height;
				let resAspectRatio = resW*1.0 / resH;
				if(aspectRatio !== 0 && resAspectRatio !== 0) {
					if(resAspectRatio > aspectRatio) {
						let paddingY = (height - (width*1.0 / resAspectRatio)) / 2.0;
						gameColumnContent.css('padding', paddingY + "px 0 " + paddingY + "px 0");
					} else if(resAspectRatio < aspectRatio) {
						let paddingX = (width - (height*1.0 * resAspectRatio)) / 2.0;
						gameColumnContent.css('padding', "0 " + paddingX + "px 0 " + paddingX + "px");
					} else {
						gameColumnContent.css('padding', "");
					}
				}
			}
		}
	} else {
		gameColumnContent.css('padding', "");
	}

}
function updateResolutionSelection() {
	let radioValue = $("input:radio[name='screenshotRadio']:checked").val();

	if(radioValue) {
		switch(radioValue) {
			case "sizeFactor":
				screenshotResolutionW.prop('disabled', 'true');
				screenshotResolutionH.prop('disabled', 'true');
				screenshotSizeFactor.removeProp('disabled');
				break;
			case "resolution":
				screenshotResolutionW.removeProp('disabled');
				screenshotResolutionH.removeProp('disabled');
				screenshotSizeFactor.prop('disabled', 'true');
				break;
		}
	}
	recalculateAspectRatio();
}
function takeScreenshot() {
	let radioValue = $("input:radio[name='screenshotRadio']:checked").val();
	let isTransparent = $("#btn-photo-transparency").hasClass("selected");

	if(radioValue) {
		switch(radioValue) {
			case "sizeFactor":
				let fact = screenshotSizeFactor.val();
				if($.isNumeric(fact) && fact > 0) {
					let jsonObj = {
						Request: {
							Type: "Screenshot",
							Screenshot: {
								SizeFactor: fact,
								IsTransparent: isTransparent
							}
						}
					};
					sendMessage(jsonObj);
				}
				break;
			case "resolution":
				let resW = screenshotResolutionW.val();
				let resH = screenshotResolutionH.val();
				if($.isNumeric(resW) && $.isNumeric(resH) && parseInt(resW) > 0 && parseInt(resH) > 0) {
					let jsonObj = {
						Request: {
							Type: "Screenshot",
							Screenshot: {
								Width: parseInt(resW),
								Height: parseInt(resH),
								IsTransparent: isTransparent
							}
						}
					};
					sendMessage(jsonObj);
				}
				break;
		}
	}
}
function clickCameraCube(view) {
	let selectedCameraPos = cameraPosSelector.val();
	if(selectedCameraPos === view) {
		switch(selectedCameraPos) {
			case "Front":
				view = "IsometricFront";
				break;
			case "Back":
				view = "IsometricBack";
				break;
			case "Left":
				view = "IsometricLeft";
				break;
			case "Right":
				view = "IsometricRight";
				break;
			case "Top":
				view = "Initial";
				break;
			case "Bottom":
				view = "Initial";
				break;
			case "Initial":
				view = "IsometricFront";
				break;
			case "IsometricFront":
				view= "Front";
				break;
			case "IsometricBack":
				view = "Back";
				break;
			case "IsometricLeft":
				view = "Left";
				break;
			case "IsometricRight":
				view = "Right";
				break;
		}
	}
	cameraPosSelector.val(view);
	updateCameraPos();
}
function handleMessage_camera(msg) {
	$("#btn-camera").removeClass("disabled-button");
	if(msg.hasOwnProperty("CameraPos")) {
		let cameraPos = msg.CameraPos;
		cameraPosSelector.val(cameraPos);
	}
}
function toggleCinePopup() {
	if($("#btn-cine").hasClass("selected")) {
		$("#cine-popup").addClass("hidden-popup");
		$("#btn-cine").removeClass("selected");
	} else {
		$("#cine-popup").removeClass("hidden-popup");
		$("#btn-cine").addClass("selected");
	}
}
function toggleCameraPopup() {
	if($("#btn-camera").hasClass("selected")) {
		$("#camera-popup").addClass("hidden-popup");
		$("#btn-camera").removeClass("selected");
	} else {
		$("#camera-popup").removeClass("hidden-popup");
		$("#btn-camera").addClass("selected");
	}
	recalculateAspectRatio();
}

let formattedTexts = {};

function formatOpenSpaceTextR2(text) {
	let orgText = text;

	if(gameSettings.Game === "RRR") {
		let regexColors = RegExp("\\/[oO]([0-9]{1,3}):([0-9]{1,3}):([0-9]{1,3}):(.*?(?=\\/[oO]|$))", 'g');
	
		text = text.replace(regexColors, function (match, p1, p2, p3, p4, offset, string, groups) {
			return `<span style="color: rgba(${p1}, ${p2}, ${p3}, 1);">${p4}</span>`;
		});
	} else {
		let regexColors = RegExp("\/[oO]([0-9]{1,3}):(.*?(?=\/[oO]|$))", 'g');
	
		text = text.replace(regexColors, function (match, p1, p2, offset, string, groups) {
			return `<span class="dialog-color color-${p1.toLowerCase()}">${p2}</span>`;
		});
	}
	text = text.replace(/\/L:/gi, "<br/>"); // New Lines

	let regexEvent = RegExp("/[eE][0-9]{0,5}: (.*?(?=\/|$|<))", 'g');
	let regexCenter = RegExp("/C:(.*)$", 'gi');
	let regexMisc = RegExp("/[a-zA-Z][0-9]{0,5}:", 'g');

	text = text.replace(regexEvent, ""); // Replace event characters
	text = text.replace(regexCenter, function (match, p1, offset, string, groups) { // Center text if necessary
		return `<div class="center">${p1}</div>`;
	});
	text = text.replace(regexMisc, ""); // Replace non-visible control characters
	
	if(gameSettings.Game !== "RRR") {
		text = text.replace(":", ""); // remove :
	}

	let equalsSignRegex = RegExp("(?<!<[^>]*)=", 'g');
	text = text.replace(equalsSignRegex, ":"); // = becomes : unless in a html tag :) (TODO: check if Rayman 2 only

	return text;
}
function formatOpenSpaceTextR3(text) {
	let regexColors = RegExp("\\\\[cC]([0-9]{1,3}):([0-9]{1,3}):([0-9]{1,3}):(.*?(?=\\\\[cC]|$))", 'g');

	text = text.replace(regexColors, function (match, p1, p2, p3, p4, offset, string, groups) {
		return `<span style="color: rgba(${p1}, ${p2}, ${p3}, 1);">${p4}</span>`;
	});
	text = text.replace(/\\L/gi, "<br/>"); // New Lines

	//let regexEvent = RegExp("/[eE][0-9]{0,5}: (.*?(?=\/|$|<))", 'g');
	let regexCenter = RegExp("\\\\jc(.*)$", 'gi');
	let regexBold = RegExp("\\\\b(.*)$", 'gi');
	//let regexMisc = RegExp("/[a-zA-Z][0-9]{0,5}:", 'g');

	//text = text.replace(regexEvent, ""); // Replace event characters
	text = text.replace(regexCenter, function (match, p1, offset, string, groups) { // Center text if necessary
		return `<div class="center">${p1}</div>`;
	});
	text = text.replace(regexBold, function (match, p1, offset, string, groups) { // Bold text
		return `<b>${p1}</b>`;
	});
	//text = text.replace(regexMisc, ""); // Replace non-visible control characters

	//text = text.replace(":", ""); // remove :

	let equalsSignRegex = RegExp("(?<!<[^>]*)=", 'g');
	text = text.replace(equalsSignRegex, ":"); // = becomes : unless in a html tag :) (TODO: check if Rayman 2 only
	text = text.replace(/\$/g, "\""); // Special character, apparently.

	return text;
}
function formatOpenSpaceText(text) {
	let orgText = text;
	if (formattedTexts[text]!==undefined) {
		// Regexes are expen$ive - RTS
		return formattedTexts[text];
	}
	if(gameSettings != null){
		if(gameSettings.EngineVersion === "R2") {
			text = formatOpenSpaceTextR2(text);
		} else if(gameSettings.EngineVersion === "R3") {
			
			text = formatOpenSpaceTextR3(text);
		} else {
			text = formatOpenSpaceTextR2(text);
		}
	} else {
		text = formatOpenSpaceTextR2(text);
	}
	formattedTexts[orgText] = text;

	return text;
}
function getLanguageHTML(lang, langStart) {
	let fullHTML = [];
	fullHTML.push("<div class='localization-item category'>" + lang.Name + " (" + lang.NameLocalized + ")</div>");
	$.each(lang.Entries, function (idx, val) {
		fullHTML.push("<div class='localization-item localization-item-highlight' data-loc-item='" + (idx + langStart) + "'><div class='localization-item-index'>" + (idx + langStart) + "</div><div class='localization-item-text'>" + escapeHTML(val) + "</div></div>");
	});
	//fullHTML.push("</div>");
	return fullHTML.join("");
}
function updateLanguageDisplayed() {
	if(fullData != null && fullData.hasOwnProperty("Localization")) {
		let loc = fullData.Localization;
		let selectedLanguage = languageSelector.prop("selectedIndex");
		if(loc.hasOwnProperty("Languages") && loc.Languages.length > selectedLanguage) {
			$("#language-localized").html(getLanguageHTML(loc.Languages[selectedLanguage], loc.LanguageStart));
			
			let api = $("#content-localization").data('jsp');
			setTimeout(function(){
				api.reinitialise();
			}, 100);
		}
	}
}
function handleMessage_localization(msg) {
	$("#btn-localization").removeClass("disabled-button");
	if(gameSettings != null){
		if(gameSettings.Game === "R2" || gameSettings.Game === "R2Demo" || gameSettings.Game === "R2Revolution" || gameSettings.Game === "RRR" || gameSettings.Game === "RRush" || gameSettings.Game === "RedPlanet" || gameSettings.Game === "DD") {
			if(gameSettings.EngineMode === "PS1" || gameSettings.EngineMode === "ROM") {
				text_highlight_tooltip.addClass("rom");
				if(gameSettings.Game === "RRR") {
					text_highlight_tooltip.addClass("rrr");
				} else {
					text_highlight_tooltip.addClass("rayman-2");
				}
			} else {
				text_highlight_tooltip.addClass("rayman-2");
			}
		} else if(gameSettings.Game === "R3" || gameSettings.Game === "RA" || gameSettings.Game === "RM") {
			text_highlight_tooltip.addClass("rayman-3");
			if(gameSettings.Game === "R3") {
				text_highlight_tooltip.addClass("blue");
			}
		}
	}
	let fullHTML = [];
	let api = $("#content-localization").data('jsp');	
	if(msg.hasOwnProperty("Languages")) {
		languageSelector.empty();
		$.each(msg.Languages, function (idx, language) {
			languageSelector.append("<option value='" + idx + "'>" + escapeHTML(language.Name) + " (" + escapeHTML(language.NameLocalized)+")</option>");
		});
		languageSelector.prop("selectedIndex", 0);

		
		fullHTML.push("<div id='language-localized'>");
		if(msg.Languages.length > 0) {
			fullHTML.push(getLanguageHTML(msg.Languages[0], msg.LanguageStart));
		}
		fullHTML.push("</div>");
	}
	if(msg.hasOwnProperty("Common")) {
		fullHTML.push("<div id='language-common'>");
		fullHTML.push(getLanguageHTML(msg.Common, msg.CommonStart));
		fullHTML.push("</div>");
	}
	api.getContentPane().append(fullHTML.join(""));
	setTimeout(function(){
		api.reinitialise();
	}, 100);
}
function handleMessage_input(msg) {
	$("#btn-entryactions").removeClass("disabled-button");
	let fullHTML = [];
	let api = $("#content-entryactions").data('jsp');	
	if(msg.hasOwnProperty("EntryActions")) {
		$.each(msg.EntryActions, function (idx, val) {
			fullHTML.push("<div class='entryactions-item'><div class='entryactions-item-name'>" + escapeHTML(val.Name) + "</div><div class='entryactions-item-input'>" + escapeHTML(val.Input) + "</div></div>");
		});
	}
	api.getContentPane().append(fullHTML.join(""));
	setTimeout(function(){
		api.reinitialise();
	}, 100);
}
function setAllJSON(jsonString) {
	//alert(jsonString);
	//console.log(JSON.stringify(jsonString)); 
	let msg = $.parseJSON(jsonString);
	fullData = msg;
	if(msg.hasOwnProperty("GameSettings")) {
		gameSettings = msg.GameSettings;
	}
	if(msg.hasOwnProperty("Hierarchy")) {
		hierarchy = msg.Hierarchy;
		if(hierarchy != null) {
			let totalWorld = [];
			if(hierarchy.hasOwnProperty("Always")) {
				let fakeAlwaysWorld = parseAlways(hierarchy.Always);
				totalWorld = totalWorld.concat(fakeAlwaysWorld);
			}
			if(hierarchy.hasOwnProperty("TransitDynamicWorld")) {
				let transitDynamicWorld = parseSuperObject(hierarchy.TransitDynamicWorld, 0);
				totalWorld = totalWorld.concat(transitDynamicWorld);
			}
			if(hierarchy.hasOwnProperty("ActualWorld")) {
				let actualWorld = parseSuperObject(hierarchy.ActualWorld, 0);
				totalWorld = totalWorld.concat(actualWorld);
			}
			if(hierarchy.hasOwnProperty("DynamicWorld")) {
				let dynamicWorld = parseSuperObject(hierarchy.DynamicWorld, 0);
				totalWorld = totalWorld.concat(dynamicWorld);
			}
			if(hierarchy.hasOwnProperty("DynamicSuperObjects")) {
				let dynamicWorld = parseDynamicSuperObjects(hierarchy.DynamicSuperObjects);
				totalWorld = totalWorld.concat(dynamicWorld);
			}
			if(hierarchy.hasOwnProperty("FatherSector")) {
				let fatherSector = parseSuperObject(hierarchy.FatherSector, 0);
				totalWorld = totalWorld.concat(fatherSector);
			}
			if(totalWorld.length > 0) {
				let api = objects_content.data('jsp');
				api.getContentPane().append(totalWorld.join(""));
				// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
				setTimeout(function(){
					api.reinitialise();
				}, 100);
			}
		}
	}
	if(msg.hasOwnProperty("Settings")) {
		handleMessage_settings(msg.Settings);
	}
	if(msg.hasOwnProperty("Camera")) {
		handleMessage_camera(msg.Camera);
	}
	if(msg.hasOwnProperty("CineData")) {
		handleMessage_cineData(msg.CineData);
	}
	if(msg.hasOwnProperty("Localization")) {
		handleMessage_localization(msg.Localization);
	}
	if(msg.hasOwnProperty("Input")) {
		handleMessage_input(msg.Input);
	}
}
// SCRIPT
function setBehavior(behaviorIndex) {
	if(currentSO != null && currentSO.hasOwnProperty("Perso") && currentSO.Perso.hasOwnProperty("Brain") && behaviorIndex >= 0) {
		let brain = currentSO.Perso.Brain;
		let modelname = currentSO.Perso.hasOwnProperty("NameModel") ? currentSO.Perso.NameModel : currentSO.Perso.NameInstance;
		currentBehavior = null;
		let curIndex = behaviorIndex;
		if(currentBehavior == null && brain.hasOwnProperty("Intelligence") && brain.Intelligence.length > 0) {
			if(curIndex < brain.Intelligence.length) {
				currentBehavior = brain.Intelligence[curIndex];
				$("#header-script-text").text(modelname + ".Intelligence[" + curIndex + "]: " + currentBehavior.Name);
				currentBehaviorType = "Intelligence";
			} else {
				curIndex -= brain.Intelligence.length;
			}
		}
		if(currentBehavior == null && brain.hasOwnProperty("Reflex") && brain.Reflex.length > 0) {
			if(curIndex < brain.Reflex.length) {
				currentBehavior = brain.Reflex[curIndex];
				$("#header-script-text").text(modelname + ".Reflex[" + curIndex + "]: " + currentBehavior.Name);
				currentBehaviorType = "Reflex";
			} else {
				curIndex -= brain.Reflex.length;
			}
		}
		if(currentBehavior == null && brain.hasOwnProperty("Macros") && brain.Macros.length > 0) {
			if(curIndex < brain.Macros.length) {
				currentBehavior = brain.Macros[curIndex];
				$("#header-script-text").text(modelname + ".Macros[" + curIndex + "]: " + currentBehavior.Name);
				currentBehaviorType = "Macro";
			} else {
				curIndex -= brain.Macros.length;
			}
		}
		if(currentBehavior != null) {
			requestComport();
		}
	}
}

function requestComport() {
	let api = $("#content-script").data('jsp');
	api.getContentPane().html("");
	api.scrollTo(0,0, false);
	refreshScroll();
	$('#btn-next-script').addClass('disabled-button');
	$('#btn-prev-script').addClass('disabled-button');
	
	if(currentBehaviorType == "Macro") {
		let jsonObj = {
			Request: {
				Type: "Macro",
				Offset: currentBehavior.Offset,
				BehaviorType: currentBehaviorType
			}
		};
		sendMessage(jsonObj);
	} else {
		let jsonObj = {
			Request: {
				Type: "Comport",
				Offset: currentBehavior.Offset,
				BehaviorType: currentBehaviorType
			}
		};
		sendMessage(jsonObj);
	}
}

function getScriptHTML(title, script) {
	let scriptHTML = "";
	scriptHTML += "<div class='script-item category'><div class='script-item-name'>" + escapeHTML(title) + " - " + escapeHTML(script.Offset) + "</div></div>";
	scriptHTML += "<div class='script-item script'><pre><code class='script-item-code cs'>" + escapeHTML(script.Translation) + "</code></pre></div>";
	return scriptHTML;
}
function handleMessage_comport(msg) {
	if(currentBehavior != null) {
		currentBehavior = msg;
		let comportHTML = "";
		if(currentBehavior.hasOwnProperty("Script")) {
			comportHTML += getScriptHTML("Script", currentBehavior.Script);
		}
		if(currentBehavior.hasOwnProperty("FirstScript")) {
			comportHTML += getScriptHTML("First Script", currentBehavior.FirstScript);
		}
		if(currentBehavior.hasOwnProperty("Scripts")) {
			$.each(currentBehavior.Scripts, function (idx, script) {
				comportHTML += getScriptHTML("Scripts[" + idx + "]", script);
			});
		}
		let api = $("#content-script").data('jsp');
		api.getContentPane().html(comportHTML);
		$(".script-item-code").each(function() {
			hljs.highlightBlock($(this).get(0));
		})
		api.scrollTo(0,0, false);
		refreshScroll();
	}
}

// PERSO OBJECT DESCRIPTION
function showObjectDescription(so, isSOChanged) {
	$('#posX').val(so.Position.x);
	$('#posY').val(so.Position.y);
	$('#posZ').val(so.Position.z);
	
	$('#rotX').val(so.Rotation.x);
	$('#rotY').val(so.Rotation.y);
	$('#rotZ').val(so.Rotation.z);
	
	$('#sclX').val(so.Scale.x);
	$('#sclY').val(so.Scale.y);
	$('#sclZ').val(so.Scale.z);
	
	if(so.hasOwnProperty("Perso")) {
		let perso = so.Perso;
		$('.perso-description').removeClass('invisible');
		if(isSOChanged) {
			stateSelector.empty();
			objectListSelector.empty();
			//objectListSelector.append("<option value='0'>None</option>");

			if(perso.hasOwnProperty("States")) {
				$.each(perso.States, function (idx, state) {
					stateSelector.append("<option value='" + idx + "'>" + escapeHTML(state.Name) + "</option>");
				});
				stateSelector.prop("selectedIndex", perso.State);
			}
			if(perso.hasOwnProperty("ObjectLists")) {
				$.each(perso.ObjectLists, function (idx, poList) {
					objectListSelector.append("<option value='" + idx + "'>" + escapeHTML(poList) + "</option>");
				});
				objectListSelector.prop("selectedIndex", perso.ObjectList);
			}
		} else {
			stateSelector.prop("selectedIndex", perso.State);
			objectListSelector.prop("selectedIndex", perso.ObjectList);
		}
		if(!perso.hasOwnProperty("ObjectLists") || perso.ObjectLists.length == 0 || (perso.ObjectLists.length == 1 && perso.ObjectLists[0] == "Null")) {
			objectListInputGroup.addClass('invisible');
		}
		
		selectButton($("#btn-enabled"), perso.IsEnabled);

		$("#objectName").html(getPersoNameHTML(perso));
		
		// Animation stuff
		selectButton($("#btn-playAnimation"), perso.PlayAnimation);
		selectButton($("#btn-autoNextState"), perso.AutoNextState);
		$('#animationSpeed').val(perso.AnimationSpeed);

		if(isSOChanged || previousState !== perso.State) {
			// State transitions
			$("#content-state-transitions").empty();
			if(perso.hasOwnProperty("States") && perso.States[perso.State].hasOwnProperty("Transitions") && perso.States[perso.State].Transitions.length > 0) {
				let state = perso.States[perso.State];
				let transitionsHTML = [];
				transitionsHTML.push("<div class='transitions-item category collapsible' data-collapse='transitions-collapse'><div class='collapse-sign'>+</div>State transitions</div><div id='transitions-collapse' style='display: none;'>");
				
				transitionsHTML.push("<div class='transitions-item transitions-header'><div class='transitions-targetstate'>Target state</div><div class='transitions-linkingtype'></div><div class='transitions-statetogo'>Redirect to</div></div>");
				$.each(state.Transitions, function (idx, val) {
					transitionsHTML.push("<div class='transitions-item'>")
					transitionsHTML.push("<div class='transitions-targetstate selectState' data-select-state='" + val.TargetState + "'>" + escapeHTML(perso.States[val.TargetState].Name) + "</div>");
					if(val.LinkingType === 1) {
						transitionsHTML.push("<div class='transitions-linkingtype'><i class='icon-media-fast-forward'></i></div>");
					} else {
						transitionsHTML.push("<div class='transitions-linkingtype'><i class='icon-media-play'></i></div>");
					}
					transitionsHTML.push("<div class='transitions-statetogo selectState' data-select-state='" + val.StateToGo + "'>" + escapeHTML(perso.States[val.StateToGo].Name) + "</div>");
					transitionsHTML.push("</div>");
				});
				transitionsHTML.push("</div>");
				$("#content-state-transitions").append(transitionsHTML.join(""));
			} else {
				
			}
		}
		previousState = perso.State;
		
		// Scripts
		if(isSOChanged) {
			$("#content-brain").empty();
			if(perso.hasOwnProperty("Brain")) {
				let allBehaviors = [];
				let brain = perso.Brain;
				if(perso.hasOwnProperty("StateTransitionExportAvailable") && perso.StateTransitionExportAvailable) {
					allBehaviors.push('<div class="behaviors-item category stateTransitionExport"><div class="collapse-sign">+</div>Behavior transition diagram</div>');
				}
				//let reg = /^.*\.(.*?)\[(\d*?)\](?:\[\"(.*?)\"\])?$/;
				if(brain.hasOwnProperty("Intelligence") && brain.Intelligence.length > 0) {
					allBehaviors.push("<div class='behaviors-item category collapsible' data-collapse='behaviors-intelligence-collapse'><div class='collapse-sign'>+</div>Intelligence behaviors</div><div id='behaviors-intelligence-collapse' style='display: none;'>");
					$.each(brain.Intelligence, function (idx, val) {
						let name = val.hasOwnProperty("Name") ? val.Name : "";
						//if(idx == 0) name += (name === "" ? "" : " ") + "(Init)";
						if(!val.hasOwnProperty("FirstScript") && (!val.hasOwnProperty("Scripts") || val.Scripts.length == 0)) name += (name === "" ? "" : " ") + "(Empty)";
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>Intelligence " + idx + "</div><div class='behavior-name'>" + name + "</div></div>");
					});
					allBehaviors.push("</div>");
				}
				if(brain.hasOwnProperty("Reflex") && brain.Reflex.length > 0) {
					allBehaviors.push("<div class='behaviors-item category collapsible' data-collapse='behaviors-reflex-collapse'><div class='collapse-sign'>+</div>Reflex behaviors</div><div id='behaviors-reflex-collapse' style='display: none;'>");
					$.each(brain.Reflex, function (idx, val) {
						let name = val.hasOwnProperty("Name") ? val.Name : "";
						if(!val.hasOwnProperty("FirstScript") && (!val.hasOwnProperty("Scripts") || val.Scripts.length == 0)) name += (name === "" ? "" : " ") + "(Empty)";
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>Reflex " + idx + "</div><div class='behavior-name'>" + name + "</div></div>");
					});
					allBehaviors.push("</div>");
				}
				if(brain.hasOwnProperty("Macros") && brain.Macros.length > 0) {
					allBehaviors.push("<div class='behaviors-item category collapsible' data-collapse='macros-collapse'><div class='collapse-sign'>+</div>Macros</div><div id='macros-collapse' style='display: none;'>");
					$.each(brain.Macros, function (idx, val) {
						let name = val.hasOwnProperty("Name") ? val.Name : "";
						if(!val.hasOwnProperty("Script")) name += (name === "" ? "" : " ") + "(Empty)";
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>Macro " + idx + "</div><div class='behavior-name'>" + name + "</div></div>");
					});
					allBehaviors.push("</div>");
				}
				if(brain.hasOwnProperty("DsgVars") && brain.DsgVars.length > 0) {
					allBehaviors.push("<div class='behaviors-item category collapsible' data-collapse='dsgvars-collapse'><div class='collapse-sign'>+</div>DSG Variables</div><div id='dsgvars-collapse' style='display: none;'>");
					let hasCurrent = brain.DsgVars.some(d => d.hasOwnProperty("ValueCurrent"));
					let hasInitial = brain.DsgVars.some(d => d.hasOwnProperty("ValueInitial"));
					let hasModel = brain.DsgVars.some(d => d.hasOwnProperty("ValueModel"));
					// Header
					allBehaviors.push("<div class='dsgvars-item dsgvars-header'><div class='dsgvar-type'></div><div class='dsgvar-name'></div>")
					if(hasCurrent) allBehaviors.push("<div class='dsgvar-value'>Current</div>");
					if(hasInitial) allBehaviors.push("<div class='dsgvar-value'>Initial</div>");
					if(hasModel) allBehaviors.push("<div class='dsgvar-value'>Model</div>");
					allBehaviors.push("</div>")
					// DsgVars
					$.each(brain.DsgVars, function (idx, dsg) {
						let dsgString = getDsgVarString(idx, dsg, hasCurrent, hasInitial, hasModel);
						allBehaviors.push(dsgString);
					});
					allBehaviors.push("</div>");
				}
				$("#content-brain").append(allBehaviors.join(""));
			}
		}
		
	} else {
		$('.perso-description').addClass('invisible');
	}
	let api = description_content.data('jsp');
	setTimeout(function(){
		api.reinitialise();
		//recalculateAspectRatio();
	}, 100);
	btn_close_description.removeClass('disabled-button');
	description_column.removeClass('invisible');
}

function getDsgVarString(index, dsg, hasCurrent, hasInitial, hasModel) {
	if(dsg.IsArray && dsg.ArrayLength > 0) {
		let dsgString = "<div class='dsgvars-item dsgvar dsgvar-array collapsible' data-collapse='dsgvar-" + index + "-collapse'><div class='dsgvar-type collapse-sign'>+</div>";
		dsgString += "<div class='dsgvar-name'>" + dsg.Name + " (Length: " + dsg.ArrayLength + ")</div></div>";
		dsgString += "<div id='dsgvar-" + index + "-collapse' style='display: none;'>"
		for (let i = 0; i < dsg.ArrayLength; i++) {
			dsgString += "<div class='dsgvars-item dsgvar'><div class='dsgvar-type dsgvar-type-" + dsg.Type + "'>" + getDsgVarIcon(dsg.ArrayType) + "</div>";
			dsgString += "<div class='dsgvar-name'>" +  "[" + i + "]</div>";
			if(hasCurrent) dsgString += getDsgVarTypeString("current", dsg.ValueCurrent, i);
			if(hasInitial) dsgString += getDsgVarTypeString("initial", dsg.ValueInitial, i);
			if(hasModel) dsgString += getDsgVarTypeString("model", dsg.ValueModel, i);
			dsgString += "</div>";
		}
		dsgString += "</div>";
		return dsgString;
	} else if(dsg.IsArray) {
		let dsgString = "<div class='dsgvars-item dsgvar'><div class='dsgvar-type dsgvar-type-" + dsg.Type + "'>" + getDsgVarIcon(dsg.Type) + "</div>";
		dsgString += "<div class='dsgvar-name'>" + dsg.Name + " (Length: 0)</div>";
		dsgString += "</div>";
		return dsgString;
	} else {
		let dsgString = "<div class='dsgvars-item dsgvar'><div class='dsgvar-type dsgvar-type-" + dsg.Type + "'>" + getDsgVarIcon(dsg.Type) + "</div>";
		dsgString += "<div class='dsgvar-name'>" + dsg.Name + "</div>";
		if(hasCurrent) dsgString += getDsgVarTypeString("current", dsg.ValueCurrent, 0);
		if(hasInitial) dsgString += getDsgVarTypeString("initial", dsg.ValueInitial, 0);
		if(hasModel) dsgString += getDsgVarTypeString("model", dsg.ValueModel, 0);
		dsgString += "</div>";
		return dsgString;
	}
}

function getDsgVarIcon(type) {
	let dsgString = "<div class='dsgvar-icon'>";
	switch(type) {
		case "Boolean":
			//dsgString += "<i class='icon-input-checked'></i>";
			break;
		case "Byte":
			break;
		case "UByte":
			break;
		case "Short":
			break;
		case "UShort":
			break;
		case "Int":
			break;
		case "UInt":
			break;
		case "Float":
			break;
		case "Caps":
			break;
		case "Text":
			dsgString += "<i class='icon-commenting'></i>";
			break;
		case "Vector":
			break;
		case "Perso":
			dsgString += "<i class='icon-user'></i>";
			break;
		case "SuperObject":
			break;
		case "WayPoint":
			dsgString += "<i class='icon-location-pin'></i>";
			break;
		case "Graph":
			dsgString += "<i class='icon-flow-children'></i>";
			break;
		case "Action":
			dsgString += "<i class='icon-media-play'></i>";
			break;
		case "SoundEvent":
			dsgString += "<i class='icon-volume-medium'></i>";
		default:
			break;
	}
	dsgString += "</div>";
	return dsgString;
}

function getDsgVarTypeString(valueType, val, idx) {
	if(val === undefined || val === null) {
		let dsgString = "<div class='dsgvar-value dsgvar-value-null'></div>";
		return dsgString;
	}
	if(val.hasOwnProperty("AsArray")) {
		if(idx >= val.AsArray.length) {
			let dsgString = "<div class='dsgvar-value dsgvar-value-null'></div>";
			return dsgString;
		} else {
			return getDsgVarTypeString(valueType, val.AsArray[idx], idx);
		}
	}
	let dsgString = "<div class='dsgvar-value dsgvar-value-" + valueType + " dsgvar-value-" + val.Type;
	switch(val.Type) {
		case "Boolean":
			dsgString += "'>" + val.AsBoolean;
			break;
		case "Byte":
			dsgString += "'>" + val.AsByte;
			break;
		case "UByte":
			dsgString += "'>" + val.AsUByte;
			break;
		case "Short":
			dsgString += "'>" + val.AsShort;
			break;
		case "UShort":
			dsgString += "'>" + val.AsUShort;
			break;
		case "Int":
			dsgString += "'>" + val.AsInt;
			break;
		case "UInt":
			dsgString += "'>" + val.AsUInt;
			break;
		case "Float":
			dsgString += "'>" + val.AsFloat;
			break;
		case "Caps":
			dsgString += "'>" + val.AsCaps;
			break;
		case "Text":
			dsgString += " text' data-localization-item='" + val.AsText + "'>" + val.AsText;
			let text = $("#content-localization").find(`.localization-item[data-loc-item='${val.AsText}']`).find(".localization-item-text").text();
			if(text !== undefined && text != null) {
				dsgString += " - " + escapeHTML(text);
			}
			break;
		case "Vector":
			dsgString += " vector'>(" + val.AsVector.x + ", " + val.AsVector.y + ", " + val.AsVector.z + ")";
			break;
		case "Perso":
			if(val.hasOwnProperty("AsPerso")) {
				dsgString += " perso' data-offset='" + val.AsPerso.Offset + "'>" + val.AsPerso.NameInstance;
			} else {
				dsgString +="'>"
			}
			break;
		case "SuperObject":
			dsgString += "'>";
			if(val.hasOwnProperty("AsSuperObject")) {
				if(val.AsSuperObject.hasOwnProperty("Name")) {
					dsgString += val.AsSuperObject.Name;
				}
			}
			break;
		case "WayPoint":
			dsgString += "'>";
			if(val.hasOwnProperty("AsWayPoint")) {
				if(val.AsWayPoint.hasOwnProperty("Name")) {
					dsgString += val.AsWayPoint.Name;
				}
			}
			break;
		case "Graph":
			dsgString += "'>";
			if(val.hasOwnProperty("AsGraph")) {
				if(val.AsGraph.hasOwnProperty("Name")) {
					dsgString += val.AsGraph.Name;
				}
			}
			break;
		case "Action":
			dsgString += "'>";
			if(val.hasOwnProperty("AsAction")) {
				if(val.AsAction.hasOwnProperty("Name")) {
					dsgString += val.AsAction.Name;
				}
			}
			break;
		default:
			dsgString += "'>";
			break;
	}
	dsgString += "</div>";
	return dsgString;
}

function sendPerso() {
	if(currentSO != null && currentSO.hasOwnProperty("Perso")) {
		let animationSpeed = $('#animationSpeed').val();
		let jsonObj = {
			Perso: {
				Offset: currentSO.Perso.Offset,
				ObjectList: $("#objectList").prop('selectedIndex'),
				State: $("#state").prop('selectedIndex'),
				IsEnabled: $("#btn-enabled").hasClass("selected"),
				PlayAnimation: $("#btn-playAnimation").hasClass("selected"),
				AutoNextState: $("#btn-autoNextState").hasClass("selected"),
				AnimationSpeed: $.isNumeric(animationSpeed) ? animationSpeed : currentSO.Perso.AnimationSpeed
			}
		}
		sendMessage(jsonObj);
	}
}
function setObjectTransform() {
	if(currentSO != null) {
		let posX = $('#posX').val();
		let posY = $('#posY').val();
		let posZ = $('#posZ').val();
		
		let rotX = $('#rotX').val();
		let rotY = $('#rotY').val();
		let rotZ = $('#rotZ').val();
		
		let sclX = $('#sclX').val();
		let sclY = $('#sclY').val();
		let sclZ = $('#sclZ').val();
		
		if($.isNumeric(posX) && $.isNumeric(posY) && $.isNumeric(posZ) &&
		   $.isNumeric(rotX) && $.isNumeric(rotY) && $.isNumeric(rotZ) &&
		   $.isNumeric(sclX) && $.isNumeric(sclY) && $.isNumeric(sclZ)) {
			let jsonObj = {
				Superobject: {
					Offset:   currentSO.Offset,
					Type:     currentSO.Type,
					Position: { x: posX, y: posY, z: posZ},
					Rotation: { x: rotX, y: rotY, z: rotZ},
					Scale:    { x: sclX, y: sclY, z: sclZ}
				}
			}
			sendMessage(jsonObj);
		}
	}
}

// SELECTION
function setSelectionPerso(perso) {
	let jsonObj = {
		Selection: {
			Perso: {
				Offset: perso.Offset
			},
			View: true
		}
	}
	sendMessage(jsonObj);
}
function setSelection(so) {
	if(so.hasOwnProperty("Perso")) {
		setSelectionPerso(so.Perso);
	} else {
		let jsonObj = {
			Selection: {
				SuperObject: {
					Offset: so.Offset
				},
				View: true
			}
		}
		sendMessage(jsonObj);
	}
}
function clearSelection() {
	description_column.addClass('invisible');
	$(".objects-item").removeClass("current-objects-item");
	/*setTimeout(function(){
		recalculateAspectRatio();
	}, 100);*/
	currentSO = null;
	let jsonObj = {
		Selection: {
			//Offset: "null"
		}
	}
	sendMessage(jsonObj);
}
function handleMessage_selection_updatePerso(oldPerso, newPerso) {
	if(newPerso.hasOwnProperty("IsEnabled")) { // IncludeDetails
		oldPerso.IsEnabled = newPerso.IsEnabled;
		oldPerso.State = newPerso.State;
		oldPerso.ObjectList = newPerso.ObjectList;
		oldPerso.PlayAnimation = newPerso.PlayAnimation;
		oldPerso.AnimationSpeed = newPerso.AnimationSpeed;
		oldPerso.AutoNextState = newPerso.AutoNextState;
	}
	oldPerso.Position = newPerso.Position;
	oldPerso.Rotation = newPerso.Rotation;
	oldPerso.Scale = newPerso.Scale;
	if(newPerso.hasOwnProperty("States")) oldPerso.States = newPerso.States;
	if(newPerso.hasOwnProperty("ObjectLists")) oldPerso.ObjectLists = newPerso.ObjectLists;
	if(newPerso.hasOwnProperty("Brain")) oldPerso.Brain = newPerso.Brain;
	if(newPerso.hasOwnProperty("StateTransitionExportAvailable")) oldPerso.StateTransitionExportAvailable = newPerso.StateTransitionExportAvailable;
}
function handleMessage_selection(msg) {
	let selection = msg;
	if(!selection.hasOwnProperty("Perso")) {
		// Deselection. Can only happen from web version, so do nothing
		return;
	}
	let perso = selection.Perso;
	let perso_index = getIndexFromPerso(perso);
	if(perso_index > -1) {
		handleMessage_selection_updatePerso(objectsList[perso_index].Perso, perso);
		objectsList[perso_index].Position = perso.Position;
		objectsList[perso_index].Rotation = perso.Rotation;
		objectsList[perso_index].Scale = perso.Scale;

		$(".objects-item").removeClass("current-objects-item");
		$(".objects-item:eq(" + perso_index + ")").addClass("current-objects-item");
		let newcurrentSO = objectsList[perso_index];
		let isSOChanged = newcurrentSO != currentSO;
		currentSO = newcurrentSO;
		showObjectDescription(currentSO, isSOChanged);
	}
}
function handleMessage_highlight(msg) {
	let highlight = "";
	highlight_tooltip.removeClass("perso");
	highlight_tooltip.removeClass("waypoint");
	highlight_tooltip.removeClass("collider");
	if(msg.hasOwnProperty("Perso")) {
		highlight_tooltip.addClass("perso");
		highlight += "<div class='highlight-perso'>" + getPersoNameHTML(msg.Perso) + "</div>";
	}
	if(msg.hasOwnProperty("WayPoint")) {
		if(msg.WayPoint.hasOwnProperty("Graphs")) {
			highlight_tooltip.addClass("waypoint");
			highlight += "<div class='highlight-header'>Graph:</div><div class='highlight-waypoint'>";
			$.each(msg.WayPoint.Graphs, function (idx, graph) {
				if(graph !== null) {
					highlight += "<div class='highlight-graphName'>" + escapeHTML(graph.Name) + "</div>";
				}
			});
			highlight += "</div>";
		}
	}
	if(msg.hasOwnProperty("Collider")) {
		highlight_tooltip.addClass("collider");
		highlight += "<div class='highlight-header'>Collide Type:</div><div class='highlight-collider'>";
		if(msg.Collider.hasOwnProperty("CollideTypes")) {
			if(msg.Collider.CollideTypes.length > 0) {
				$.each(msg.Collider.CollideTypes, function (idx, col) {
					highlight += "<div class='highlight-collideType'>" + col + "</div>";
				});
			} else {
				highlight += "<div class='highlight-collideType'>Default</div>"
			}
		} else {
			highlight += "<div class='highlight-collideType'>Default</div>"
		}
		highlight += "</div>";
	}
	if(highlight !== "") {
		highlight_tooltip.html(highlight);
		highlight_tooltip.removeClass("hidden-tooltip");
	} else {
		highlight_tooltip.addClass("hidden-tooltip");
	}
}
function requestTransitionExport() {
	let jsonObj = {
		Request: {
			Type: "TransitionExport"
		}
	}
	sendMessage(jsonObj);
}
function handleMessage_transitionExport(msg) {
	//console.log(msg);
	let popupWin = window.open('statediagram.html','transitionExport','');
	popupWin.inputJSON = msg;
	popupWin.addEventListener('load', (event) => {
		popupWin.CreateBehaviorDiagram();
	});
}

// SETTINGS
function sendSettings() {
	let jsonObj = {
		Settings: {
			EnableLighting: $("#btn-lighting").hasClass("selected"),
			EnableFog: $("#btn-fog").hasClass("selected"),
			Luminosity: $("#range-luminosity").val(),
			Saturate: !$("#btn-saturate").hasClass("selected"),
			ViewCollision: $("#btn-viewCollision").hasClass("selected"),
			ViewGraphs: $("#btn-viewGraphs").hasClass("selected"),
			ViewInvisible: $("#btn-viewInvisible").hasClass("selected"),
			DisplayInactive: $("#btn-displayInactive").hasClass("selected"),
			ShowPersos: $("#btn-showPersos").hasClass("selected"),
			PlayAnimations: $("#btn-playAnimations").hasClass("selected"),
			PlayTextureAnimations: $("#btn-playTextureAnimations").hasClass("selected")
		}
	}
	sendMessage(jsonObj);
}

// MESSAGE
function handleMessage(jsonString) {
	let msg = $.parseJSON(jsonString);
	if(msg != null && msg.hasOwnProperty("Type")) {
		switch(msg.Type) {
			case "Highlight":
				handleMessage_highlight(msg.Highlight); break;
			case "Selection":
				handleMessage_selection(msg.Selection); break;
			case "Settings":
				handleMessage_settings(msg.Settings); break;
			case "Macro":
				handleMessage_comport(msg.Macro); break;
			case "Comport":
				handleMessage_comport(msg.Comport); break;
			case "Camera":
				handleMessage_camera(msg.Camera); break;
			case "CineData":
				handleMessage_cineData(msg.CineData); break;
			case "TransitionExport":
				handleMessage_transitionExport(msg.TransitionExport); break;
			default:
				console.log('default');break;
		}
	}
}

function initGame(gameJSON) {
	current_game = gameJSON;

	var api = versions_content.data('jsp');
	api.getContentPane().empty();
	let items = [];
	let versions = gameJSON.versions;
	let versionSelected = false;
	$.each(versions, function(index_version, value) {
		let thisVersionSelected = false;
		if(!versionSelected && folder !== null && value.hasOwnProperty("folder") && (folder === value.folder || folder.startsWith(value.folder + "/"))) {
			versionSelected = true;
			thisVersionSelected = true;
		}
		let nameHTML = escapeHTML(value.name);
		if(thisVersionSelected) {
			levels_sidebar.addClass('loading-sidebar');
			levels_header.addClass('loading-header');
			levels_actors_group.addClass('loading');
			clickVersion_onEndLoading(value.json);
			$(".current-version-item").removeClass("current-version-item");
			items.push("<div class='version-item current-version-item' data-version='" + value.json + "' title='" + nameHTML + "' data-logo='" + encodeURI(value.image) + "'>");
		} else {
			items.push("<div class='version-item' data-version='" + value.json + "' title='" + nameHTML + "' data-logo='" + encodeURI(value.image) + "'>");
		}
		items.push("<div class='version-item-image' style='background-image: url(\"" + encodeURI(value.image) + "\");' alt='" + nameHTML + "'></div>");
		items.push("<div class='version-item-name'>" + nameHTML + "</div></div>");
	});
	/*if(!isLoading) {
		$("#btn-home").addClass("current-game-item");
		clickGame(null);
	}*/
	api.getContentPane().append(items.join(""));
	// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
	if(versionsReinitTimeout != null) clearTimeout(versionsReinitTimeout);
	versionsReinitTimeout = setTimeout(function(){
		versions_header.removeClass('loading-header');
		versions_content.removeClass('loading');
		api.reinitialise();
		api.scrollToPercentY(0, false);
	}, 100);
}

function initActors() {
	if(levelsJSON.hasOwnProperty("numActors")) {
		if(levelsJSON.numActors >= 1) {
			let actors = levelsJSON.hasOwnProperty("actors1") ? levelsJSON.actors1 : levelsJSON.actors;

			let actorItems = "";
			$.each(actors, function (idx, act) {
				actorItems += "<option value='" + act.actor + "'>" + escapeHTML(act.name) + "</option>";
			});


			actor1_selector.html(actorItems);
			actor1_group.removeClass("invisible");
			let actorIndex = getRandomInt(actors.length);
			actor1_selector.val(actors[actorIndex].actor);
		} else {
			actor1_group.addClass("invisible");
		}
		if(levelsJSON.numActors == 2) {
			let actors = levelsJSON.hasOwnProperty("actors2") ? levelsJSON.actors2 : levelsJSON.actors;

			let actorItems = "";
			$.each(actors, function (idx, act) {
				actorItems += "<option value='" + act.actor + "'>" + escapeHTML(act.name) + "</option>";
			});

			actor2_selector.html(actorItems);
			actor2_group.removeClass("invisible");
			let actorIndex = getRandomInt(actors.length);
			actor2_selector.val(actors[actorIndex].actor);
		} else {
			actor2_group.addClass("invisible");
		}
		levels_actors_group.removeClass("hidden");
	} else {
		levels_actors_group.addClass("hidden");
	}
}

function updateLevelLinksActors() {
	$("a.levels-item.level").each(function( index ) {
		let lastActor1 = $( this ).data("actor1");
		let lastActor2 = $( this ).data("actor2");
		let urlParams = $(this).data("urlParams");
		let newActor1 = actor1_selector.val();
		let newActor2 = actor2_selector.val();

		let newActorString = "";
		if(lastActor1 !== undefined) {
			$(this).data("actor1", newActor1);
			newActorString += "&a1=" + newActor1;
		}
		if(lastActor2 !== undefined) {
			$(this).data("actor2", newActor2);
			newActorString += "&a2=" + newActor2;
		}
		if(urlParams !== undefined) {
			$(this).attr("href", urlParams + newActorString);
		}
	});
}

function initVersion(versionJSON) {
	$('.sidebar-button').remove();
	$('#sidebar-levels-slider').css('top','0px');
	levels_sidebar.scrollTop(0);
	let sidebarItems = [];
	levelsJSON = versionJSON;
	let api = levels_content.data('jsp');
	api.getContentPane().empty();
	let items = [];
	let totalEm = 0;
	levels_header.text(levelsJSON.name);

	// Fill in actors
	initActors();

	// Fill in levels
	if(levelsJSON.hasOwnProperty("levels")) {
		let levels = levelsJSON.levels;
		$.each( levels, function(index, value) {
			let levelFolder = levelsJSON.folder;
			if(value.hasOwnProperty("folder")) {
				levelFolder += "/" + value.folder;
			}
			let urlParams = "?mode=" + levelsJSON.mode + "&folder=" + levelFolder + "&lvl=" + value.level;
			if(value.hasOwnProperty("additionalParams")) {
				$.each(value.additionalParams, function(inx_param, val_param) {
					urlParams += "&" + val_param.key + "=" + val_param.value;
				});
			}
			let actorParams = "";
			let requiresActor1 = value.hasOwnProperty("actor1") && value.actor1;
			let requiresActor2 = value.hasOwnProperty("actor2") && value.actor2;

			if(levelsJSON.hasOwnProperty("numActors")) {
				if(requiresActor1) actorParams += "&a1=" + actor1_selector.val();
				if(requiresActor2) actorParams += "&a2=" + actor2_selector.val();
			}

			let nameHTML = escapeHTML(value.name);

			//items.push("<a class='logo-item' href='#" + value.json + "' title='" + value.title + "'><img src='" + encodeURI(value.image) + "' alt='" + value.title + "'></a>");
			if(levelsJSON.mode === mode && folder === levelFolder && value.level === lvl) {
				items.push("<div class='levels-item level current-levels-item' title='" + nameHTML + "'><div class='name'>" + nameHTML + "</div><div class='internal-name'>" + value.level + "</div></div>");
				document.title = " [" + levelsJSON.name + "] " + value.name + " - " + baseTitle;
			} else {
				let actorHTML = "";
				if(levelsJSON.hasOwnProperty("numActors")) {
					if(requiresActor1) actorHTML += "data-actor1='" + actor1_selector.val() + "' ";
					if(requiresActor2) actorHTML += "data-actor2='" + actor2_selector.val() + "' ";
					if(requiresActor1 || requiresActor2) actorHTML += "' data-url-params='" + escapeHTML(urlParams) + "' ";
				}
				items.push("<a class='levels-item level' " + actorHTML + "href='index.html" + urlParams + actorParams + "' title='" + nameHTML + "'><div class='name'>" + nameHTML + "</div><div class='internal-name'>" + value.level + "</div></a>");
			}
			totalEm += 2;
		});
	}
	if(levelsJSON.hasOwnProperty("icons")) {
		levels_sidebar.removeClass('hidden-sidebar');
		let emDistance = 2;
		$.each( levelsJSON.icons, function(index, value) {
			let iconClass = "world-image";
			if(levelsJSON.icons.length > index+1 && levelsJSON.icons[index+1].level < value.level+7) {
				iconClass = iconClass + " small";
			}
			let iconSidebar = "<div class='sidebar-button'><div class='sidebar-button-image' style='background-image: url(\"" + encodeURI(value.image) + "\");'></div></div>";
			items.push("<div class='" + iconClass + "' style='background-image: url(\"" + encodeURI(value.image) + "\"); top: " + emDistance*(value.level) + "em;'></div>");
			sidebarItems.push(iconSidebar);
		});

	} else {
		levels_sidebar.addClass('hidden-sidebar');
	}
	api.getContentPane().append(items.join(""));
	$('#sidebar-levels-content').append(sidebarItems.join(""));
	sidebarUpdateArrows($(".column-sidebar-content"));
	// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
	if(levelsReinitTimeout != null) clearTimeout(levelsReinitTimeout);
	levelsReinitTimeout = setTimeout(function(){
		levels_header.removeClass('loading-header');
		levels_sidebar.removeClass('loading-sidebar');
		levels_content.removeClass('loading');
		levels_actors_group.removeClass('loading');
		api.reinitialise();
		setLevelsSidebarSlider(0, true, api.getContentPane().height() <= levels_content.height());
		api.scrollToPercentY(0, false);
	}, 100);
}


function clickVersion_onEndLoading(versionItem) {
	if(versionItem !== null && current_game !== null) {
		levels_content.off(transEndEventName);
		$.getJSON( "json/" + current_game.viewer + "/" + current_game.json + "/" + versionItem + ".json" + getNoCacheURL(), function( data ) {
			initVersion(data);
		});
	}
}

function clickVersion(versionItem) {
	if( !support || levels_content.hasClass('loading')) {
		levels_sidebar.addClass('loading-sidebar');
		levels_header.addClass('loading-header');
		levels_actors_group.addClass('loading');
		clickVersion_onEndLoading(versionItem);
	} else {
		levels_content.off(transEndEventName);
		levels_content.on(transEndEventName, function() {
			clickVersion_onEndLoading(versionItem);
		});
		levels_content.addClass('loading');
		levels_sidebar.addClass('loading-sidebar');
		levels_header.addClass('loading-header');
		levels_actors_group.addClass('loading');
	}
}

function clickGame_onEndLoading(gameItem) {
	if(gameItem !== null) {
		let gameFound = false;
		let gameJSON = null;
		$.each(gamesJSON.categories, function(index_cat, cat) {
			$.each(cat.games, function(index_game, game) {
				if(game.json === gameItem) {
					gameFound = true;
					gameJSON = game;
					return false;
				}
			});
			if(gameFound) return false;
		});
		if(gameJSON !== null) {
			versions_content.off(transEndEventName);
			initGame(gameJSON);

		}
	}
}

function clickGame(gameItem) {
	if( !support || versions_content.hasClass('loading')) {
		levels_content.addClass('loading');
		levels_sidebar.addClass('loading-sidebar');
		levels_header.addClass('loading-header');
		levels_actors_group.addClass('loading');
		clickGame_onEndLoading(gameItem);
	} else {
		levels_content.addClass('loading');
		levels_sidebar.addClass('loading-sidebar');
		levels_header.addClass('loading-header');
		levels_actors_group.addClass('loading');
		versions_content.off(transEndEventName);
		levels_content.off(transEndEventName);
		versions_content.on(transEndEventName, function() {
			clickGame_onEndLoading(gameItem);
		});
		versions_content.addClass('loading');
		versions_header.addClass('loading-header');
	}
}


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
	$('#dialogue-popup').addClass('hidden-popup');
	if(gameInstance != null) {
		$('#popup-overlay').addClass('hidden-overlay');
		$('#levelselect-popup').addClass('hidden-popup');
		$('#localization-popup').addClass('hidden-popup');
		$('#entryactions-popup').addClass('hidden-popup');
		$('#script-popup').addClass('hidden-popup');
		$('#config-popup').addClass('hidden-popup');
		$('#info-popup').addClass('hidden-popup');
		selectButton($('#btn-levelselect'), false);
		selectButton($('#btn-localization'), false);
		selectButton($('#btn-entryactions'), false);
		selectButton($('#btn-info'), false);
		selectButton($('#btn-config'), false);
	}
	dialogueMsg = null;
}

function startGame() {
	gameInstance = UnityLoader.instantiate("gameContainer", "Build/raymap.json", {onProgress: UnityProgress});
}

function showLevelSelect() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#levelselect-popup").removeClass('hidden-popup');
	selectButton($('#btn-levelselect'), true);
}
function showLocalizationWindow() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#localization-popup").removeClass('hidden-popup');
	selectButton($('#btn-localization'), true);
}
function showEntryActionsWindow() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#entryactions-popup").removeClass('hidden-popup');
	selectButton($('#btn-entryactions'), true);
}

function showScript() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#script-popup").removeClass('hidden-popup');
}
function showConfig() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#config-popup").removeClass('hidden-popup');
	selectButton($('#btn-config'), true);
}
function showInfo() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#info-popup").removeClass('hidden-popup');
	selectButton($('#btn-info'), true);
}
function selectNewStyleItem(styleName) {
	if(styleName != null) {
		let styleItem = $('.style-item[data-style=' + styleName + ']');
		if(styleItem.length > 0) {
			$(".current-style-item").removeClass("current-style-item");
			styleItem.addClass("current-style-item");
		}
	}
}

function init() {
	initContent();
	let url = new URL(window.location.href);
	mode = url.searchParams.get("mode");
	lvl = url.searchParams.get("lvl");
	folder = url.searchParams.get("folder");
	if(mode != null && lvl != null && folder != null) {
		startGame();
	} else {
		//showConfig();
		showLevelSelect();
	}
}


// DOCUMENT INIT
$(function() {
	window.addEventListener('allJSON', function (e) {
		setAllJSON(e.detail);
	}, false);
	window.addEventListener('unityJSMessage', function (e) {
		handleMessage(e.detail);
	}, false);
	
	
	wrapper = $('#wrapper');
	objects_content = $('#content-objects');
	unity_content = $('#content-unity');
	description_content = $('#content-description');
	description_column = $('.column-description');
	btn_close_description = $('#btn-close-description');
	stateSelector = $('#state');
	objectListSelector = $('#objectList');
	languageSelector = $('#languageSelector');
	cameraPosSelector = $('#cameraPosSelector');
	cinematicSelector = $('#cinematicSelector');
	cinematicActorSelector = $('#cinematicActorSelector');
	objectListInputGroup = $('#objectListInputGroup')
	highlight_tooltip = $("#highlight-tooltip");
	text_highlight_tooltip = $('#text-highlight-tooltip');
	text_highlight_content = $('#text-highlight-content');
	gameColumnContent = $('#game-content');
	screenshotResolutionRadio = $('#screenshotResolutionRadio');
	screenshotSizeFactorRadio = $('#screenshotSizeFactorRadio');
	screenshotResolutionW = $('#screenshotResolutionW');
	screenshotResolutionH = $('#screenshotResolutionH');
	screenshotSizeFactor = $('#screenshotSizeFactor');

	games_content = $('#content-games');
	versions_content = $('#content-versions');
	levels_content = $('#content-levels');
	levels_sidebar = $('#sidebar-levels');
	levels_header = $('#header-levels');
	games_header = $('#header-games');
	versions_header = $('#header-versions');

	levels_actors_group = $('#levels-actors-group');
	actor1_group = $('#actor1-group');
	actor2_group = $('#actor2-group');
	actor1_selector = $('#actor1Selector');
	actor2_selector = $('#actor2Selector');
	
	if(window.location.protocol == "file:") {
		baseURL = baseURL_local;
	}
	
	$(document).mousemove(function( event ) {
		highlight_tooltip.css({'left': (event.pageX + 3) + 'px', 'top': (event.pageY + 25) + 'px'});
		text_highlight_tooltip.css({'left': (event.pageX + 3) + 'px', 'right': ($(window).width() - event.pageX - 3) + 'px', 'top': (event.pageY + 25) + 'px'});
	});
	$(document).on('mouseenter', ".localization-item-highlight", function() {
		let text = $(this).find(".localization-item-text").text();
		let formatted = formatOpenSpaceText(text);
		if(/\S/.test(formatted)) {
			// found something other than a space or line break
			text_highlight_content.html(formatOpenSpaceText(text));
			text_highlight_tooltip.removeClass("hidden-tooltip");
			text_highlight_tooltip.removeClass("right");
		}
	});
	$(document).on('mouseleave', ".localization-item-highlight", function() {
		text_highlight_content.html("");
		text_highlight_tooltip.addClass("hidden-tooltip");
	});
	$(document).on('mouseenter', ".dsgvar-value-Text", function() {
		let locItem = $(this).data("localizationItem");
		if(locItem !== undefined && locItem != null) {
			let text = $("#content-localization").find(`.localization-item[data-loc-item='${locItem}']`).find(".localization-item-text").text();
			if(text !== undefined && text != null) {
				let formatted = formatOpenSpaceText(text);
				if(/\S/.test(formatted)) {
					// found something other than a space or line break
					text_highlight_content.html(formatOpenSpaceText(text));
					text_highlight_tooltip.removeClass("hidden-tooltip");
					text_highlight_tooltip.addClass("right");
				}
			}
		}
	});
	$(document).on('mouseleave', ".dsgvar-value-Text", function() {
		text_highlight_content.html("");
		text_highlight_tooltip.addClass("hidden-tooltip");
	});
	$(document).on('click', ".objects-item.object-perso", function() {
		let index = $(".objects-item").index(this);
		//$(".objects-item").removeClass("current-objects-item");
		//$(this).addClass("current-objects-item");
		let so = getSuperObjectByIndex(index);
		if(so.hasOwnProperty("Perso")) {
			setSelectionPerso(so.Perso);
			//currentSO = so;
			//showObjectDescription(so);
		}
		return false;
	});
	
	$(document).on('click', ".objects-item.object-IPO, .objects-item.object-sector", function() {
		let index = $(".objects-item").index(this);
		//$(".objects-item").removeClass("current-objects-item");
		//$(this).addClass("current-objects-item");
		let so = getSuperObjectByIndex(index);
		setSelection(so);
		return false;
	});
	
	$(document).on('click', "#btn-close-description", function() {
		clearSelection();
		$(this).addClass("disabled-button");
		return false;
	});
	
	$(document).on('click', "#btn-fullscreen", function() {
		gameInstance.SetFullscreen(1);
		return false;
	});
	
	$(document).on('click', "#btn-levelselect", function() {
		showLevelSelect();
		return false;
	});
	$(document).on('click', "#btn-localization", function() {
		showLocalizationWindow();
		return false;
	});
	$(document).on('click', "#btn-entryactions", function() {
		showEntryActionsWindow();
		return false;
	});
	$(document).on('click', "#btn-info", function() {
		showInfo();
		return false;
	});
	$(document).on('click', "#btn-config", function() {
		showConfig();
		return false;
	});
	
	$(document).on('click', "#btn-photo-transparency", function() {
		selectButton($(this), !$(this).hasClass("selected"));
		if($(this).hasClass("selected")) {
			$(".icon-transparency").removeClass("hidden");
			$(".icon-opaque").addClass("hidden");
		} else {
			$(".icon-transparency").addClass("hidden");
			$(".icon-opaque").removeClass("hidden");
		}
		return false;
	});
	$(document).on('click', "#btn-lighting", function() {
		if($(this).hasClass("selected")) {
			$(".lighting-settings").addClass("disabled-button");
		} else {
			$(".lighting-settings").removeClass("disabled-button");
		}
		selectButton($(this), !$(this).hasClass("selected"));
		sendSettings();
		return false;
	});
	$(document).on('input', "#range-luminosity", function() {
		sendSettings();
		return false;
	});
	
	$(document).on('click', ".settings-toggle", function() {
		selectButton($(this), !$(this).hasClass("selected"));
		sendSettings();
		return false;
	});
	
	$(document).on('click', "#btn-enabled, #btn-autoNextState, #btn-playAnimation", function() {
		selectButton($(this), !$(this).hasClass("selected"));
		sendPerso();
		return false;
	});
	$(document).on('click', "#btn-photo", function() {
		takeScreenshot();
		return false;
	});
	$("input:radio[name='screenshotRadio']").change(function(){
		updateResolutionSelection();
		return false;
	});
	description_column.on('transitionend', function () {
		// your event handler
		waitForFinalEvent(function(){
			recalculateAspectRatio();
		}, 3, "eventWaiterTransitionEnd");
	});
	
	$(document).on('input', "#screenshotResolutionW, #screenshotResolutionH", function() {
		recalculateAspectRatio();
		return false;
	});
	
	$(document).on('change', "#objectList", function() {
		//let selectedIndex = $(this).prop('selectedIndex');
		//setObjectList(selectedIndex);
		sendPerso();
		$(this).blur();
		return false;
	});
	$(document).on('click', ".selectState", function() {
		//let selectedIndex = $(this).prop('selectedIndex');
		//setState(selectedIndex);
		let newState = parseInt($(this).data("selectState"));
		if(currentSO != null && currentSO.hasOwnProperty("Perso") && currentSO.Perso.State != newState) {
			stateSelector.prop("selectedIndex", newState);
			sendPerso();
		}
		$(this).blur();
		return false;
	});
	$(document).on('change', "#state", function() {
		//let selectedIndex = $(this).prop('selectedIndex');
		//setState(selectedIndex);
		sendPerso();
		$(this).blur();
		return false;
	});
	$(document).on('change', "#actor1Selector, #actor2Selector", function() {
		updateLevelLinksActors();
	});
	$(document).on('change', "#languageSelector", function() {
		updateLanguageDisplayed();
		$(this).blur();
		return false;
	});
	$(document).on('change', "#cameraPosSelector", function() {
		updateCameraPos();
		$(this).blur();
		return false;
	});
	$(document).on('change', "#cinematicSelector", function() {
		updateCinematic();
		$(this).blur();
		return false;
	});
	$(document).on('change', "#cinematicActorSelector", function() {
		selectCinematicActor();
		$(this).blur();
		return false;
	});
	$(document).on('click', '.cube__face', function() {
		let view = $(this).data('view');
		clickCameraCube(view);
		return false;
	});
	$(document).on('focusin', ".input-typing", function() {
		if(!inputHasFocus && gameInstance != null) {
			for (var i in gameInstance.Module.getJSEvents().eventHandlers) {
				var event = gameInstance.Module.getJSEvents().eventHandlers[i];
				if (event.eventTypeString == 'keydown' || event.eventTypeString == 'keypress' || event.eventTypeString == 'keyup') {
					window.removeEventListener(event.eventTypeString, event.eventListenerFunc, event.useCapture);
				}
			}
		}
		inputHasFocus = true;
	});
	$(document).on('focusout', ".input-typing", function() {
		if(inputHasFocus && !$(".input-typing").is(":focus") && gameInstance != null) {
			for (var i in gameInstance.Module.getJSEvents().eventHandlers) {
				var event = gameInstance.Module.getJSEvents().eventHandlers[i];
				if (event.eventTypeString == 'keydown' || event.eventTypeString == 'keypress' || event.eventTypeString == 'keyup') {
					window.addEventListener(event.eventTypeString, event.eventListenerFunc, event.useCapture);
				}
			}
			inputHasFocus = false;
		}
	});
	$(document).on('input', ".input-transform", function() {
		setObjectTransform();
		return false;
	});
	
	$(document).on('input', "#animationSpeed", function() {
		sendPerso();
		return false;
	});
	
	$(document).on('input', "#cinematicSpeed", function() {
		updateCinematic();
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
	$(document).on('click', ".collapsible", function() {
		let collapse_id = $(this).attr('data-collapse');
		let collapse = $("#"+collapse_id);
		if(collapse.is(":hidden")) {
			collapse.show("fast", refreshScroll);
			$(this).find(".collapse-sign").text("-");
		} else {
			collapse.hide("fast", refreshScroll);
			$(this).find(".collapse-sign").text("+");
		}
		return false;
	});
	$(document).on('click', ".stateTransitionExport", function () {
		requestTransitionExport();
		return false;
	});
	
	$(document).on('click', ".behaviors-item.behavior", function() {
		let index = $(".behaviors-item.behavior").index(this);
		setBehavior(index);
		showScript();
		return false;
	});

	$(document).on('click', "#btn-cine", function() {
		toggleCinePopup();
		if($("#btn-camera").hasClass("selected")) {
			toggleCameraPopup();
		}
		return false;		
	});
	$(document).on('click', "#btn-camera", function() {
		toggleCameraPopup();
		if($("#btn-cine").hasClass("selected")) {
			toggleCinePopup();
		}
		return false;		
	});
	$(document).on('click', ".game-item", function() {
		let json = jQuery(this).data('game');
		$(".current-game-item").removeClass("current-game-item");
		jQuery(this).addClass("current-game-item");
		clickGame(json);
		return false;
	});
	$(document).on('click', ".version-item", function() {
		let json = jQuery(this).data('version');
		$(".current-version-item").removeClass("current-version-item");
		jQuery(this).addClass("current-version-item");
		clickVersion(json);
		return false;
	});
	$(document).on('click', ".style-item", function() {
		let styleName = $(this).data("style");
		setActiveStyleSheet(styleName);
		selectNewStyleItem(styleName);
		$(".current-style-item").removeClass("current-style-item");
		$(this).addClass("current-style-item");
		
		waitForFinalEvent(function(){
			setStyleCookie();
		}, 100, "styleChange");
		return false;
	});

	$(document).on('click', ".sidebar-button", function() {
		let butt = jQuery(this);
		let buttIndex = $(".sidebar-button").index(butt);
		if(levelsJSON !== null && levelsJSON.hasOwnProperty("icons") && levelsJSON.icons.length > buttIndex) {
			var trackNum = levelsJSON.icons[buttIndex].level;
			var levelRef = $(".levels-item.level").eq(trackNum);
			let api = $("#content-levels").data('jsp');
			api.scrollToY(levelRef.position().top, true);
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
	
	$('#content-levels').bind('jsp-scroll-y', function(event, scrollPositionY, isAtTop, isAtBottom) {
		waitForFinalEvent(function(){
				setLevelsSidebarSlider(scrollPositionY, isAtTop, isAtBottom);
			}, 20, "scrolly scrolly");
		}
	)
	
	let pane = $('.column-content-scroll');
	let settings = {
		horizontalGutter: 0,
		verticalGutter: 0,
		animateEase: "swing"
	};
	pane.jScrollPane(settings);

	
	// Select the right style item
	let styleName = getActiveStyleSheet();
	selectNewStyleItem(styleName);
	
	init();
	
});

$( window ).resize(refreshScroll);