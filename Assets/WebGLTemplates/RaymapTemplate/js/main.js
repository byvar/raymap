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
let levelsJSON = null;
let baseTitle = "Raymap"
let notificationTimeout = null;
let dialogueMsg = null;
let fullData = null;
let objectsList = [];
let currentSO = null;
let gameInstance = null;
let inputHasFocus = false;
let mode, lvl, folder;

let currentBehavior = null;
let currentScriptIndex = 0;
let wrapper, objects_content, unity_content, description_content, description_column;
let btn_close_description, stateSelector, objectListSelector, perso_tooltip;

// FUNCTIONS
function sendMessage(jsonObj) {
	if(gameInstance != null) {
		gameInstance.SendMessage("Loader", "ParseMessage", JSON.stringify(jsonObj));
	}
}

function initContent() {
	$.getJSON( "json/content.json", function( data ) {
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
					items.push("<div class='levels-item level current-levels-item' title='" + value.name + "'>" + value.name + "</div>");
					document.title = " [" + value_game.name + "] " + value.name + " - " + baseTitle;
				} else {
					items.push("<a class='levels-item level' href='index.html?mode=" + value_game.mode + "&folder=" + levelFolder + "&lvl=" + value.level + "' title='" + value.name + "'>" + value.name + "</a>");
				}
				totalEm += 2;
			});
		});
		api.getContentPane().append(items.join(""));
		$('#sidebar-levels-content').append(sidebarItems.join(""));
		sidebarUpdateArrows($('#sidebar-levels'));
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
function parseSuperObject(so, level) {
	let items = [];
	objectsList.push(so);
	let type = "Unknown";
	if(so.hasOwnProperty("type")) {
		type = so.type;
	}
	switch(type) {
		case "World":
			items.push("<div class='objects-item object-world level-" + level + "' alt='" + so.name + "'>" + so.name + "</div>");
			break;
		case "Perso":
			let family = "Family";
			let model = "Model";
			let instance = "Instance";
			if(so.hasOwnProperty("perso")) {
				family = so.perso.nameFamily;
				model = so.perso.nameModel;
				instance = so.perso.nameInstance;
			}
			items.push("<div class='objects-item object-perso' title='" + so.name + "'><div class='name-family'>" + family + "</div><div class='name-model'>" + model + "</div><div class='name-instance'>" + instance + "</div></div>");
			break;
		case "Sector":
			items.push("<div class='objects-item object-sector level-" + level + "' title='" + so.name + "'>" + so.name + "</div>");
			break;
		case "IPO":
		case "IPO_2":
			items.push("<div class='objects-item object-IPO level-" + level + "' title='" + so.name + "'>" + so.name + "</div>");
			break;
		default:
			items.push("<div class='objects-item object-regular' title='" + so.name + "'>" + so.name + "</div>");
			break;
			
	}
	if(so.hasOwnProperty("children")) {
		$.each(so.children, function(i, child) {
			items = items.concat(parseSuperObject(child, level+1));
		});
	}
	return items;
}
function parseAlways(alwaysData) {
	let items = [];
	let so = {
		offset: "null",
		type: "AlwaysWorld",
		position: [0,0,0],
		rotation: [0,0,0],
		scale:    [0,0,0]
	};
	objectsList.push(so);
	items.push("<div class='objects-item object-world level-0' alt='Spawnable objects'>Spawnable objects</div>");
	
	if(alwaysData.hasOwnProperty("spawnablePersos")) {
		$.each(alwaysData.spawnablePersos, function(i, child) {
			let family = alwaysData.spawnablePersos[i].nameFamily;
			let model = alwaysData.spawnablePersos[i].nameModel;
			let instance = alwaysData.spawnablePersos[i].nameInstance;
			items.push("<div class='objects-item object-always object-perso' title='Spawnable'><div class='name-family'>" + family + "</div><div class='name-model'>" + model + "</div><div class='name-instance'>" + instance + "</div></div>");
			let persoSO = {
				offset: alwaysData.spawnablePersos[i].offset,
				type: "Always",
				perso: alwaysData.spawnablePersos[i],
				position: alwaysData.spawnablePersos[i].position,
				rotation: alwaysData.spawnablePersos[i].rotation,
				scale:    alwaysData.spawnablePersos[i].scale
			};
			objectsList.push(persoSO);
		});
	}
	return items;
}
function handleMessage_settings(msg) {
	if(msg.hasOwnProperty("settings")) {
		$(".settings-toggle").removeClass("disabled-button");
		$("#btn-lighting").removeClass("disabled-button");
		if(msg.settings.enableLighting) {
			$(".lighting-settings").removeClass("disabled-button");
		} else {
			$(".lighting-settings").addClass("disabled-button");
		}
		selectButton($("#btn-lighting"), msg.settings.enableLighting);
		selectButton($("#btn-saturate"), !msg.settings.saturate);
		selectButton($("#btn-viewCollision"), msg.settings.viewCollision);
		selectButton($("#btn-viewGraphs"), msg.settings.viewGraphs);
		selectButton($("#btn-viewInvisible"), msg.settings.viewInvisible);
		selectButton($("#btn-displayInactive"), msg.settings.displayInactive);
		selectButton($("#btn-showPersos"), msg.settings.showPersos);
		selectButton($("#btn-playAnimations"), msg.settings.playAnimations);
		selectButton($("#btn-playTextureAnimations"), msg.settings.playTextureAnimations);
		$("#range-luminosity").val(msg.settings.luminosity);
	}
}
function setAllJSON(jsonString) {
	//alert(jsonString);
	//console.log(jsonString); 
	fullData = $.parseJSON(jsonString);
	if(fullData != null) {
		let totalWorld = [];
		if(fullData.hasOwnProperty("always")) {
			let fakeAlwaysWorld = parseAlways(fullData.always);
			totalWorld = totalWorld.concat(fakeAlwaysWorld);
		}
		if(fullData.hasOwnProperty("transitDynamicWorld")) {
			let transitDynamicWorld = parseSuperObject(fullData.transitDynamicWorld, 0);
			totalWorld = totalWorld.concat(transitDynamicWorld);
		}
		if(fullData.hasOwnProperty("actualWorld")) {
			let actualWorld = parseSuperObject(fullData.actualWorld, 0);
			totalWorld = totalWorld.concat(actualWorld);
		}
		if(totalWorld.length > 0) {
			let api = objects_content.data('jsp');
			api.getContentPane().append(totalWorld.join(""));
			// hack, but append (in chrome) is asynchronous so we could reinit with non-full scrollpane
			setTimeout(function(){
				api.reinitialise();
			}, 100);
		}
		if(fullData.hasOwnProperty("settings")) {
			handleMessage_settings(fullData.settings);
		}
	}
}

// SCRIPT
function setBehavior(behaviorIndex) {
	if(currentSO != null && currentSO.hasOwnProperty("perso") && currentSO.perso.hasOwnProperty("brain") && behaviorIndex >= 0) {
		let allBehaviors = [];
		let brain = currentSO.perso.brain;
		if(brain.hasOwnProperty("ruleBehaviors") && brain.ruleBehaviors.length > 0) {
			allBehaviors = allBehaviors.concat(brain.ruleBehaviors);
		}
		if(brain.hasOwnProperty("reflexBehaviors") && brain.reflexBehaviors.length > 0) {
			allBehaviors = allBehaviors.concat(brain.reflexBehaviors);
		}
		if(brain.hasOwnProperty("macros") && brain.macros.length > 0) {
			allBehaviors = allBehaviors.concat(brain.macros);
		}
		if(behaviorIndex < allBehaviors.length) {
			currentBehavior = allBehaviors[behaviorIndex];
			currentScriptIndex = 0;
			$("#header-script-text").text(currentBehavior.name + ".Scripts[0]");
			$("#content-script-code").text("");
		}
	}
}

function setScript(scriptIndex) {
	if(currentBehavior != null) {
		let scripts = [];
		if(currentBehavior.hasOwnProperty("script")) {
			scripts.push(currentBehavior.script);
		}
		if(currentBehavior.hasOwnProperty("firstScript")) {
			scripts.push(currentBehavior.firstScript);
		}
		if(currentBehavior.hasOwnProperty("scripts")) {
			scripts = scripts.concat(currentBehavior.scripts);
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
			$("#header-script-text").text(currentBehavior.name + ".Scripts[" + scriptIndex + "]");
			
			let jsonObj = {
				request: {
					type: "script",
					scriptOffset: scripts[scriptIndex].offset,
					behaviorType: currentBehavior.type
				}
			};
			sendMessage(jsonObj);
		}
	}
}

function handleMessage_script(msg) {
	if(currentBehavior != null) {
		let scripts = [];
		if(msg.hasOwnProperty("translation")) {
			$("#content-script-code").text(msg.translation);
			hljs.highlightBlock($("#content-script-code").get(0));
			let api = $("#content-script").data('jsp');
			api.scrollTo(0,0, false);
			/*waitForFinalEvent(function(){
				hljs.highlightBlock($("#content-script-code").get(0));
			}, 3, "highlight");*/
			refreshScroll();
		}
		if(currentBehavior.hasOwnProperty("script")) {
			scripts.push(currentBehavior.script);
		}
		if(currentBehavior.hasOwnProperty("firstScript")) {
			scripts.push(currentBehavior.firstScript);
		}
		if(currentBehavior.hasOwnProperty("scripts")) {
			scripts = scripts.concat(currentBehavior.scripts);
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
		
		selectButton($("#btn-enabled"), so.perso.enabled);
		$("#objectName").html("<div class='name-family'>" + so.perso.nameFamily + "</div><div class='name-model'>" + so.perso.nameModel + "</div><div class='name-instance'>" + so.perso.nameInstance + "</div>");
		
		// Animation stuff
		selectButton($("#btn-playAnimation"), so.perso.playAnimation);
		selectButton($("#btn-autoNextState"), so.perso.autoNextState);
		$('#animationSpeed').val(so.perso.animationSpeed);
		
		// Scripts
		$("#content-brain").empty();
		if(so.perso.hasOwnProperty("brain")) {
			let allBehaviors = [];
			let brain = so.perso.brain;
			let reg = /^.*\.(.*?)\[(\d*?)\](?:\[\"(.*?)\"\])?$/;
			if(brain.hasOwnProperty("ruleBehaviors") && brain.ruleBehaviors.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='behaviors-rule-collapse'><div class='collapse-sign'>+</div>Rule behaviors</div><div id='behaviors-rule-collapse' style='display: none;'>");
				$.each(brain.ruleBehaviors, function (idx, val) {
					let match = reg.exec(val.name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			if(brain.hasOwnProperty("reflexBehaviors") && brain.reflexBehaviors.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='behaviors-reflex-collapse'><div class='collapse-sign'>+</div>Reflex behaviors</div><div id='behaviors-reflex-collapse' style='display: none;'>");
				$.each(brain.reflexBehaviors, function (idx, val) {
					let match = reg.exec(val.name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			if(brain.hasOwnProperty("macros") && brain.macros.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='macros-collapse'><div class='collapse-sign'>+</div>Macros</div><div id='macros-collapse' style='display: none;'>");
				$.each(brain.macros, function (idx, val) {
					let match = reg.exec(val.name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			$("#content-brain").append(allBehaviors.join(""));
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

function sendPerso() {
	if(currentSO != null && currentSO.hasOwnProperty("perso")) {
		let animationSpeed = $('#animationSpeed').val();
		let jsonObj = {
			perso: {
				offset: currentSO.perso.offset,
				objectList: $("#objectList").prop('selectedIndex'),
				state: $("#state").prop('selectedIndex'),
				enabled: $("#btn-enabled").hasClass("selected"),
				playAnimation: $("#btn-playAnimation").hasClass("selected"),
				autoNextState: $("#btn-autoNextState").hasClass("selected"),
				animationSpeed: $.isNumeric(animationSpeed) ? animationSpeed : currentSO.perso.animationSpeed
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
				superobject: {
					offset:   currentSO.offset,
					type:     currentSO.type,
					position: [posX, posY, posZ],
					rotation: [rotX, rotY, rotZ],
					scale:    [sclX, sclY, sclZ]
				}
			}
			sendMessage(jsonObj);
		}
	}
}

// SELECTION
function setSelectionPerso(perso) {
	let jsonObj = {
		selection: {
			offset: perso.offset,
			type: "Perso",
			view: true
		}
	}
	sendMessage(jsonObj);
}
function setSelection(so) {
	let jsonObj = {
		selection: {
			offset: so.hasOwnProperty("perso") ? so.perso.offset : so.offset,
			type: so.type,
			view: true
		}
	}
	sendMessage(jsonObj);
}
function clearSelection() {
	description_column.addClass('invisible');
	$(".objects-item").removeClass("current-objects-item");
	currentSO = null;
	let jsonObj = {
		selection: {
			offset: "null"
		}
	}
	sendMessage(jsonObj);
}
function handleMessage_selection(msg) {
	if(msg.selectionType === "superobject") {
		let so_selection, index_selection = -1;
		if(fullData != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(objectsList[i].offset === msg.selection.offset) {
					objectsList[i] = msg.selection;
					index_selection = i;
					break;
				}
			}
		}
		if(index_selection > -1) {
			$(".objects-item").removeClass("current-objects-item");
			$(".objects-item:eq(" + index_selection + ")").addClass("current-objects-item");
			currentSO = msg.selection;
			showObjectDescription(currentSO);
		}
	} else if(msg.selectionType === "always") {
		let perso_selection, index_selection = -1;
		if(fullData != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(objectsList[i].offset === msg.selection.offset) {
					objectsList[i].perso = msg.selection;
					objectsList[i].position = msg.selection.position;
					objectsList[i].rotation = msg.selection.rotation;
					objectsList[i].scale = msg.selection.scale;
					index_selection = i;
					break;
				}
			}
		}			
		if(index_selection > -1) {
			$(".objects-item").removeClass("current-objects-item");
			$(".objects-item:eq(" + index_selection + ")").addClass("current-objects-item");
			currentSO = objectsList[index_selection];
			showObjectDescription(currentSO);
		}
	}
}
function handleMessage_highlight(msg) {
	if(msg.hasOwnProperty("perso")) {
		perso_tooltip.html("<div class='name-family'>" + msg.perso.nameFamily + "</div><div class='name-model'>" + msg.perso.nameModel + "</div><div class='name-instance'>" + msg.perso.nameInstance + "</div>");
		perso_tooltip.removeClass("hidden-tooltip");
	} else {
		perso_tooltip.addClass("hidden-tooltip");
	}
}

// SETTINGS
function sendSettings() {
	let jsonObj = {
		settings: {
			enableLighting: $("#btn-lighting").hasClass("selected"),
			luminosity: $("#range-luminosity").val(),
			saturate: !$("#btn-saturate").hasClass("selected"),
			viewCollision: $("#btn-viewCollision").hasClass("selected"),
			viewGraphs: $("#btn-viewGraphs").hasClass("selected"),
			viewInvisible: $("#btn-viewInvisible").hasClass("selected"),
			displayInactive: $("#btn-displayInactive").hasClass("selected"),
			showPersos: $("#btn-showPersos").hasClass("selected"),
			playAnimations: $("#btn-playAnimations").hasClass("selected"),
			playTextureAnimations: $("#btn-playTextureAnimations").hasClass("selected")
		}
	}
	sendMessage(jsonObj);
}

// MESSAGE
function handleMessage(jsonString) {
	let msg = $.parseJSON(jsonString);
	if(msg != null && msg.hasOwnProperty("type")) {
		switch(msg.type) {
			case "highlight":
				handleMessage_highlight(msg); break;
			case "selection":
				handleMessage_selection(msg); break;
			case "settings":
				handleMessage_settings(msg); break;
			case "script":
				handleMessage_script(msg); break;
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
		$('#script-popup').addClass('hidden-popup');
		selectButton($('#btn-levelselect'), false);
	}
	dialogueMsg = null;
}

function startGame() {
	gameInstance = UnityLoader.instantiate("gameContainer", "Build/WebGL.json", {onProgress: UnityProgress});
}

function showLevelSelect() {
	$('#popup-overlay').removeClass('hidden-overlay');
	$("#levelselect-popup").removeClass('hidden-popup');
	selectButton($('#btn-levelselect'), true);
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
	perso_tooltip = $("#perso-tooltip");
	
	if(window.location.protocol == "file:") {
		baseURL = baseURL_local;
	}
	
	$(document).mousemove(function( event ) {
		perso_tooltip.css({'left': (event.pageX + 3) + 'px', 'top': (event.pageY + 25) + 'px'});
	});
	
	$(document).on('click', ".objects-item.object-perso", function() {
		let index = $(".objects-item").index(this);
		//$(".objects-item").removeClass("current-objects-item");
		//$(this).addClass("current-objects-item");
		let so = getSuperObjectByIndex(index);
		if(so.hasOwnProperty("perso")) {
			setSelectionPerso(so.perso);
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
	$(document).on('change', "#state", function() {
		//let selectedIndex = $(this).prop('selectedIndex');
		//setState(selectedIndex);
		sendPerso();
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
	$(document).on('click', ".behaviors-item.category", function() {
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