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
let levelsJSON = null;
let baseTitle = "Raymap"
let notificationTimeout = null;
let dialogueMsg = null;
let fullData = null;
let hierarchy = null;
let objectsList = [];
let currentSO = null;
let gameInstance = null;
let inputHasFocus = false;
let mode, lvl, folder;

let currentBehavior = null;
let currentBehaviorType = "";
let currentScriptIndex = 0;
let wrapper, objects_content, unity_content, description_content, description_column;
let btn_close_description, stateSelector, objectListSelector, languageSelector, highlight_tooltip, text_highlight_tooltip, text_highlight_content, objectListInputGroup;
let previousState = -1;

// FUNCTIONS
function sendMessage(jsonObj) {
	if(gameInstance != null) {
		console.log("Message: " + JSON.stringify(jsonObj));
		gameInstance.SendMessage("Loader", "ParseMessage", JSON.stringify(jsonObj));
	}
}

function initContent() {
	$.getJSON( "json/content.json?1", function( data ) {
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
	});
}

function refreshScroll() {
	waitForFinalEvent(function(){
		$(".column-content-scroll").each( function(index) {
			let api = $( this ).data('jsp');
			api.reinitialise();
		});
		sidebarUpdateArrows($(".column-sidebar-content"));
	}, 3, "some unique string");
}

function setLevelsSidebarSlider(pos) {
	if(levelsJSON != null && levelsJSON.hasOwnProperty("games") && $(".levels-item.game").length > 0) {
		// Find which section you're scrolling in
		let i = 0;
		let highlight_i = 0;
		let allow_margin = 50;
		for(i = 0; i < levelsJSON.games.length; i++) {
			let gameRef = $(".levels-item.game").eq(i);
			if(pos + allow_margin >= gameRef.position().top) {
				highlight_i = i;
			} else {
				break;
			}
		}
		// Add class
		//let butt = $(".sidebar-button").eq(highlight_i);
		//let buttPos = butt.position().top;
		//$('#sidebar-soundtrack-slider').css('top',buttPos + 'px');
		$('#sidebar-levels-slider').css('top',(highlight_i * 4) + 'em');
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
	switch(type) {
		case "World":
			items.push("<div class='objects-item object-world level-" + level + "' alt='" + so.Name + "'>" + so.Name + "</div>");
			break;
		case "Perso":
			items.push("<div class='objects-item object-perso' title='" + so.Name + "'>");
			if(so.hasOwnProperty("Perso")) {
				items.push(getPersoNameHTML(so.Perso));
			}
			items.push("</div>");
			break;
		case "Sector":
			items.push("<div class='objects-item object-sector level-" + level + "' title='" + so.Name + "'>" + so.Name + "</div>");
			break;
		case "IPO":
		case "IPO_2":
			items.push("<div class='objects-item object-IPO level-" + level + "' title='" + so.Name + "'>" + so.Name + "</div>");
			break;
		default:
			items.push("<div class='objects-item object-regular' title='" + so.Name + "'>" + so.Name + "</div>");
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
function handleMessage_cineData(msg) {
	$("#btn-cine").removeClass("disabled-button");
}
function handleMessage_camera(msg) {
	$("#btn-camera").removeClass("disabled-button");
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
}

let formattedTexts = {};

function formatOpenSpaceText(text) {

	if (formattedTexts[text]!==undefined) {
		// Regexes are expen$ive - RTS
		return formattedTexts[text];
	}

	let orgText = text;

	let regexColors = RegExp("\/[oO]([0-9]{1,3}):(.*?(?=\/[oO]|$))", 'g');

	text = text.replace(regexColors, function (match, p1, p2, offset, string, groups) {
		return `<span class="dialog-color color-${p1.toLowerCase()}">${p2}</span>`;
	});
	text = text.replace(/\/L:/gi, "<br/>"); // New Lines

	let regexEvent = RegExp("/[eE][0-9]{0,5}: (.*?(?=\/|$|<))", 'g');
	let regexCenter = RegExp("/C:(.*)$", 'gi');
	let regexMisc = RegExp("/[a-zA-Z][0-9]{0,5}:", 'g');

	text = text.replace(regexEvent, ""); // Replace event characters
	text = text.replace(regexCenter, function (match, p1, p2, offset, string, groups) { // Center text if necessary
		return `<div class="center">${p1}</div>`;
	});
	text = text.replace(regexMisc, ""); // Replace non-visible control characters

	text = text.replace(":", ""); // remove :

	let equalsSignRegex = RegExp("(?<!<[^>]*)=", 'g');
	text = text.replace(equalsSignRegex, ":"); // = becomes : unless in a html tag :) (TODO: check if Rayman 2 only)

	formattedTexts[orgText] = text;

	return text;
}
function getLanguageHTML(lang, langStart) {
	let fullHTML = [];
	fullHTML.push("<div class='localization-item category'>" + lang.Name + " (" + lang.NameLocalized + ")</div>");
	$.each(lang.Entries, function (idx, val) {
		fullHTML.push("<div class='localization-item localization-item-highlight' data-loc-item='" + (idx + langStart) + "'><div class='localization-item-index'>" + (idx + langStart) + "</div><div class='localization-item-text'>" + escapeHTML(val) + "</div></div>");
	});
	fullHTML.push("</div>");
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
	text_highlight_tooltip.addClass("rayman-2");
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
function setAllJSON(jsonString) {
	//alert(jsonString);
	console.log(JSON.stringify(jsonString)); 
	let msg = $.parseJSON(jsonString);
	fullData = msg;
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
}
// SCRIPT
function setBehavior(behaviorIndex) {
	if(currentSO != null && currentSO.hasOwnProperty("Perso") && currentSO.Perso.hasOwnProperty("Brain") && behaviorIndex >= 0) {
		let brain = currentSO.Perso.Brain;
		currentBehavior = null;
		let curIndex = behaviorIndex;
		if(brain.hasOwnProperty("Intelligence") && brain.Intelligence.length > 0) {
			if(curIndex < brain.Intelligence.length) {
				currentBehavior = brain.Intelligence[curIndex];
				currentBehaviorType = "Intelligence";
			} else {
				curIndex -= brain.Intelligence.length;
			}
		}
		if(brain.hasOwnProperty("Reflex") && brain.Reflex.length > 0) {
			if(curIndex < brain.Reflex.length) {
				currentBehavior = brain.Reflex[curIndex];
				currentBehaviorType = "Reflex";
			} else {
				curIndex -= brain.Reflex.length;
			}
		}
		if(brain.hasOwnProperty("Macros") && brain.Macros.length > 0) {
			if(curIndex < brain.Macros.length) {
				currentBehavior = brain.Macros[curIndex];
				currentBehaviorType = "Macro";
			} else {
				curIndex -= brain.Macros.length;
			}
		}
		if(currentBehavior != null) {
			currentScriptIndex = 0;
			if(currentBehaviorType == "Macro") {
				$("#header-script-text").text(currentBehavior.Name + ".Script");
			} else {
				$("#header-script-text").text(currentBehavior.Name + ".Scripts[0]");
			}
			$("#content-script-code").text("");
		}
	}
}

function setScript(scriptIndex) {
	if(currentBehavior != null) {
		let scripts = [];
		if(currentBehavior.hasOwnProperty("Script")) {
			scripts.push(currentBehavior.Script);
		}
		if(currentBehavior.hasOwnProperty("FirstScript")) {
			scripts.push(currentBehavior.FirstScript);
		}
		if(currentBehavior.hasOwnProperty("Scripts")) {
			scripts = scripts.concat(currentBehavior.Scripts);
		}
		$("#content-script-code").text("");
		let api = $("#content-script").data('jsp');
		api.scrollTo(0,0, false);
		refreshScroll();
		if(scriptIndex < 0 || scriptIndex >= scripts.length) {
			$('#btn-next-script').addClass('disabled-button');
			$('#btn-prev-script').addClass('disabled-button');
			currentScriptIndex = 0;
		} else {
			$('#btn-next-script').addClass('disabled-button');
			$('#btn-prev-script').addClass('disabled-button');
			currentScriptIndex = scriptIndex;
			$("#header-script-text").text(currentBehavior.Name + ".Scripts[" + scriptIndex + "]");
			
			let jsonObj = {
				Request: {
					Type: "Script",
					ScriptOffset: scripts[scriptIndex].Offset,
					BehaviorType: currentBehaviorType
				}
			};
			sendMessage(jsonObj);
		}
	}
}

function handleMessage_script(msg) {
	if(currentBehavior != null) {
		let scripts = [];
		if(msg.hasOwnProperty("Translation")) {
			$("#content-script-code").text(msg.Translation);
			hljs.highlightBlock($("#content-script-code").get(0));
			let api = $("#content-script").data('jsp');
			api.scrollTo(0,0, false);
			/*waitForFinalEvent(function(){
				hljs.highlightBlock($("#content-script-code").get(0));
			}, 3, "highlight");*/
			refreshScroll();
		}
		if(currentBehavior.hasOwnProperty("Script")) {
			scripts.push(currentBehavior.Script);
		}
		if(currentBehavior.hasOwnProperty("FirstScript")) {
			scripts.push(currentBehavior.FirstScript);
		}
		if(currentBehavior.hasOwnProperty("Scripts")) {
			scripts = scripts.concat(currentBehavior.Scripts);
		}
		if(currentScriptIndex < scripts.length-1) {
			$('#btn-next-script').removeClass('disabled-button');
		} else {
			$('#btn-next-script').addClass('disabled-button');
		}
		if(currentScriptIndex > 0) {
			$('#btn-prev-script').removeClass('disabled-button');
		} else {
			$('#btn-prev-script').addClass('disabled-button');
		}
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
}
// TODO
function handleMessage_selection(msg) {
	let selection = msg;
	if(!selection.hasOwnProperty("Perso")) {
		// Deselection. Can only happen from web version, so do nothing
		return;
	}
	let perso = selection.Perso;
	if(perso.IsAlways) {
		let perso_selection, index_selection = -1;
		if(hierarchy != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(!objectsList[i].hasOwnProperty("Perso")) {
					continue;
				}
				if(objectsList[i].Perso.Offset === perso.Offset) {
					handleMessage_selection_updatePerso(objectsList[i].Perso, perso);
					objectsList[i].Position = perso.Position;
					objectsList[i].Rotation = perso.Rotation;
					objectsList[i].Scale = perso.Scale;
					index_selection = i;
					break;
				}
			}
		}			
		if(index_selection > -1) {
			$(".objects-item").removeClass("current-objects-item");
			$(".objects-item:eq(" + index_selection + ")").addClass("current-objects-item");
			let newcurrentSO = objectsList[index_selection];
			let isSOChanged = newcurrentSO != currentSO;
			currentSO = newcurrentSO;
			showObjectDescription(currentSO, isSOChanged);
		}
	} else {
		let so_selection, index_selection = -1;
		if(hierarchy != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(!objectsList[i].hasOwnProperty("Perso")) {
					continue;
				}
				if(objectsList[i].Perso.Offset === perso.Offset) {
					handleMessage_selection_updatePerso(objectsList[i].Perso, perso);
					objectsList[i].Position = perso.Position;
					objectsList[i].Rotation = perso.Rotation;
					objectsList[i].Scale = perso.Scale;
					index_selection = i;
					break;
				}
			}
		}
		if(index_selection > -1) {
			$(".objects-item").removeClass("current-objects-item");
			$(".objects-item:eq(" + index_selection + ")").addClass("current-objects-item");
			let newcurrentSO = objectsList[index_selection];
			let isSOChanged = newcurrentSO != currentSO;
			currentSO = newcurrentSO;
			showObjectDescription(currentSO, isSOChanged);
		}
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
					highlight += "<div class='highlight-graphName'>" + graph.Name + "</div>";
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
			case "Script":
				handleMessage_script(msg.Script); break;
			default:
				console.log('default');break;
		}
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
	$('#dialogue-popup').addClass('hidden-popup');
	if(gameInstance != null) {
		$('#popup-overlay').addClass('hidden-overlay');
		$('#levelselect-popup').addClass('hidden-popup');
		$('#localization-popup').addClass('hidden-popup');
		$('#script-popup').addClass('hidden-popup');
		selectButton($('#btn-levelselect'), false);
		selectButton($('#btn-localization'), false);
	}
	dialogueMsg = null;
}

function startGame() {
	gameInstance = UnityLoader.instantiate("gameContainer", "Build/WebGL_2020_07_08.json", {onProgress: UnityProgress});
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

function showScript() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#script-popup").removeClass('hidden-popup');
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
	objectListInputGroup = $('#objectListInputGroup')
	highlight_tooltip = $("#highlight-tooltip");
	text_highlight_tooltip = $('#text-highlight-tooltip');
	text_highlight_content = $('#text-highlight-content');
	
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
	$(document).on('change', "#languageSelector", function() {
		updateLanguageDisplayed();
		$(this).blur();
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
	
	$(document).on('click', ".behaviors-item.behavior", function() {
		let index = $(".behaviors-item.behavior").index(this);
		setBehavior(index);
		setScript(0);
		showScript();
		return false;
	});
	
	$(document).on('click', "#btn-next-script", function() {
		setScript(currentScriptIndex+1);
		return false;
	});
	
	$(document).on('click', "#btn-prev-script", function() {
		setScript(currentScriptIndex-1);
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

	$(document).on('click', ".sidebar-button", function() {
		let butt = jQuery(this);
		let buttIndex = $(".sidebar-button").index(butt);
		let gameRef = $(".levels-item.game").eq(buttIndex);
		let api = $("#content-levels").data('jsp');
		api.scrollToY(gameRef.position().top, true);
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
				setLevelsSidebarSlider(scrollPositionY);
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
	
	init();
	
});

$( window ).resize(refreshScroll);