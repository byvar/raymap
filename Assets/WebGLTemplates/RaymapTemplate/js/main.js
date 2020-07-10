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
let baseURL_local = "https://maps.raym.app/"
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
let currentBehaviorType = "";
let currentScriptIndex = 0;
let wrapper, objects_content, unity_content, description_content, description_column;
let btn_close_description, stateSelector, objectListSelector, perso_tooltip;

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
			let family = "Family";
			let model = "Model";
			let instance = "Instance";
			if(so.hasOwnProperty("Perso")) {
				family = so.Perso.NameFamily;
				model = so.Perso.NameModel;
				instance = so.Perso.NameInstance;
			}
			items.push("<div class='objects-item object-perso' title='" + so.Name + "'><div class='name-family'>" + family + "</div><div class='name-model'>" + model + "</div><div class='name-instance'>" + instance + "</div></div>");
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
			let family = alwaysData.SpawnablePersos[i].NameFamily;
			let model = alwaysData.SpawnablePersos[i].NameModel;
			let instance = alwaysData.SpawnablePersos[i].NameInstance;
			items.push("<div class='objects-item object-always object-perso' title='Spawnable'><div class='name-family'>" + family + "</div><div class='name-model'>" + model + "</div><div class='name-instance'>" + instance + "</div></div>");
			let persoSO = {
				Offset: alwaysData.SpawnablePersos[i].Offset,
				Type: "Perso",
				Perso: alwaysData.SpawnablePersos[i],
				Position: alwaysData.SpawnablePersos[i].Position,
				Rotation: alwaysData.SpawnablePersos[i].Rotation,
				Scale:    alwaysData.SpawnablePersos[i].Scale
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
function setAllJSON(jsonString) {
	//alert(jsonString);
	console.log(JSON.stringify(jsonString)); 
	let msg = $.parseJSON(jsonString);
	if(msg.hasOwnProperty("Hierarchy")) {
		fullData = msg.Hierarchy;
		if(fullData != null) {
			let totalWorld = [];
			if(fullData.hasOwnProperty("Always")) {
				let fakeAlwaysWorld = parseAlways(fullData.Always);
				totalWorld = totalWorld.concat(fakeAlwaysWorld);
			}
			if(fullData.hasOwnProperty("TransitDynamicWorld")) {
				let transitDynamicWorld = parseSuperObject(fullData.TransitDynamicWorld, 0);
				totalWorld = totalWorld.concat(transitDynamicWorld);
			}
			if(fullData.hasOwnProperty("ActualWorld")) {
				let actualWorld = parseSuperObject(fullData.ActualWorld, 0);
				totalWorld = totalWorld.concat(actualWorld);
			}
			if(fullData.hasOwnProperty("DynamicWorld")) {
				let dynamicWorld = parseSuperObject(fullData.DynamicWorld, 0);
				totalWorld = totalWorld.concat(dynamicWorld);
			}
			if(fullData.hasOwnProperty("DynamicSuperObjects")) {
				let dynamicWorld = parseDynamicSuperObjects(fullData.DynamicSuperObjects);
				totalWorld = totalWorld.concat(dynamicWorld);
			}
			if(fullData.hasOwnProperty("FatherSector")) {
				let fatherSector = parseSuperObject(fullData.FatherSector, 0);
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
function showObjectDescription(so) {
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
		stateSelector.empty();
		objectListSelector.empty();
		//objectListSelector.append("<option value='0'>None</option>");

		if(perso.hasOwnProperty("States")) {
			$.each(perso.States, function (idx, state) {
				stateSelector.append("<option value='" + idx + "'>" + state.Name + "</option>");
			});
			stateSelector.prop("selectedIndex", perso.State);
		}
		if(perso.hasOwnProperty("ObjectLists")) {
			$.each(perso.ObjectLists, function (idx, poList) {
				objectListSelector.append("<option value='" + idx + "'>" + poList + "</option>");
			});
			objectListSelector.prop("selectedIndex", perso.ObjectList);
		}
		
		selectButton($("#btn-enabled"), perso.IsEnabled);
		$("#objectName").html(
			"<div class='name-family'>" + perso.NameFamily +
			"</div><div class='name-model'>" + perso.NameModel +
			"</div><div class='name-instance'>" + perso.NameInstance + "</div>");
		
		// Animation stuff
		selectButton($("#btn-playAnimation"), perso.PlayAnimation);
		selectButton($("#btn-autoNextState"), perso.AutoNextState);
		$('#animationSpeed').val(perso.AnimationSpeed);
		
		// Scripts
		$("#content-brain").empty();
		if(perso.hasOwnProperty("Brain")) {
			let allBehaviors = [];
			let brain = perso.Brain;
			let reg = /^.*\.(.*?)\[(\d*?)\](?:\[\"(.*?)\"\])?$/;
			if(brain.hasOwnProperty("Intelligence") && brain.Intelligence.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='behaviors-intelligence-collapse'><div class='collapse-sign'>+</div>Intelligence behaviors</div><div id='behaviors-intelligence-collapse' style='display: none;'>");
				$.each(brain.Intelligence, function (idx, val) {
					let match = reg.exec(val.Name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.Name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			if(brain.hasOwnProperty("Reflex") && brain.Reflex.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='behaviors-reflex-collapse'><div class='collapse-sign'>+</div>Reflex behaviors</div><div id='behaviors-reflex-collapse' style='display: none;'>");
				$.each(brain.Reflex, function (idx, val) {
					let match = reg.exec(val.Name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.Name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			if(brain.hasOwnProperty("Macros") && brain.Macros.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='macros-collapse'><div class='collapse-sign'>+</div>Macros</div><div id='macros-collapse' style='display: none;'>");
				$.each(brain.Macros, function (idx, val) {
					let match = reg.exec(val.Name);
					if (match != null) {
						allBehaviors.push("<div class='behaviors-item behavior'><div class='behavior-number'>" + match[1] + " " + (parseInt(match[2])+1)  + "</div>" + (match[3] != null ? ("<div class='behavior-name'>" + match[3] + "</div>") : "") + "</div>");
					} else {
						allBehaviors.push("<div class='behaviors-item behavior'>" + val.Name + "</div>");
					}
				});
				allBehaviors.push("</div>");
			}
			if(brain.hasOwnProperty("DsgVars") && brain.DsgVars.length > 0) {
				allBehaviors.push("<div class='behaviors-item category' data-collapse='dsgvars-collapse'><div class='collapse-sign'>+</div>DSG Variables</div><div id='dsgvars-collapse' style='display: none;'>");
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
	let dsgString = "<div class='dsgvars-item dsgvar'><div class='dsgvar-type dsgvar-type-" + dsg.Type + "'>" + getDsgVarIcon(dsg.Type) + "</div>";
	dsgString += "<div class='dsgvar-name'>" + dsg.Name + "</div>";
	if(hasCurrent) dsgString += getDsgVarTypeString("current", dsg.ValueCurrent);
	if(hasInitial) dsgString += getDsgVarTypeString("initial", dsg.ValueInitial);
	if(hasModel) dsgString += getDsgVarTypeString("model", dsg.ValueModel);
	dsgString += "</div>";
	return dsgString;
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

function getDsgVarTypeString(valueType, val) {
	if(val === undefined || val === null) {
		let dsgString = "<div class='dsgvar-value dsgvar-value-null'></div>";
		return dsgString;
	}
	let dsgString = "<div class='dsgvar-value dsgvar-value-" + valueType + " dsgvar-value-" + val.Type;
	if(val.hasOwnProperty("AsArray")) {
		dsgString += " dsgvar-value-array'>"
	} else {
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
				dsgString += "'>" + val.AsText;
				break;
			case "Vector":
				dsgString += " vector'>(" + val.AsVector.x + ", " + val.AsVector.y + ", " + val.AsVector.z + ")";
				break;
			case "Perso":
				if(val.hasOwnProperty("AsPerso")) {
					dsgString += "perso' data-offset='" + val.AsPerso.Offset + "'>" + val.AsPerso.NameInstance;
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
		if(fullData != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(!objectsList[i].hasOwnProperty("Perso")) {
					continue;
				}
				if(objectsList[i].Perso.Offset === perso.Offset) {
					objectsList[i].Perso = perso;
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
			currentSO = objectsList[index_selection];
			showObjectDescription(currentSO);
		}
	} else {
		let so_selection, index_selection = -1;
		if(fullData != null) {
			for (let i = 0; i < objectsList.length; i++) {
				if(!objectsList[i].hasOwnProperty("Perso")) {
					continue;
				}
				if(objectsList[i].Perso.Offset === perso.Offset) {
					objectsList[i].Perso = perso;
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
			currentSO = objectsList[index_selection];
			showObjectDescription(currentSO);
		}
	}
}
function handleMessage_highlight(msg) {
	if(msg.hasOwnProperty("Perso")) {
		perso_tooltip.html("<div class='name-family'>" + msg.Perso.NameFamily + "</div><div class='name-model'>" + msg.Perso.NameModel + "</div><div class='name-instance'>" + msg.Perso.NameInstance + "</div>");
		perso_tooltip.removeClass("hidden-tooltip");
	} else {
		perso_tooltip.addClass("hidden-tooltip");
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
		$('#script-popup').addClass('hidden-popup');
		selectButton($('#btn-levelselect'), false);
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