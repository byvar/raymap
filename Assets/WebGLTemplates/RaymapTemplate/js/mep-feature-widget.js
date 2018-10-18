/**
 * @file MediaElement Loop Feature (plugin).
 * @author Andrew Berezovsky <andrew.berezovsky@gmail.com>
 * Twitter handle: duozersk
 * @author Original author: Junaid Qadir Baloch <shekhanzai.baloch@gmail.com>
 * Twitter handle: jeykeu
 * Dual licensed under the MIT or GPL Version 2 licenses.
 */

(function($) {
  $.extend(mejs.MepDefaults, {
    loopText: 'Repeat On/Off'
  });

  $.extend(MediaElementPlayer.prototype, {
    // LOOP TOGGLE
    buildloop: function(player, controls, layers, media) {
      var t = this;

      var loop = $('<div class="mejs-button mejs-loop-button ' + (player.options.loop ? 'mejs-loop-all' : 'mejs-loop-off') + '">' +
        '<button type="button" aria-controls="' + player.id + '" title="' + player.options.loopText + '"></button>' +
        '</div>')
        // append it to the toolbar
        .appendTo(controls)
        // add a click toggle event
        .click(function(e) {
		  if(!player.options.loop) {
			  player.options.loop = true;
              loop.removeClass('mejs-loop-off').addClass('mejs-loop-all');
		  } else {
			  player.options.loop = false;
              loop.removeClass('mejs-loop-all').addClass('mejs-loop-off');
		  }
        });

      t.loopToggle = t.controls.find('.mejs-loop-button');
    },
    loopToggleClick: function() {
      var t = this;
      t.loopToggle.trigger('click');
    },
	// PLAYLIST FEATURE
	buildplaylist: function(player, controls, layers, media) {
	  // start playing playlist if no source is set
      media.addEventListener('play', function() {
		if(media.src == null || media.src == '' || media.src == silenceURL) {
		  playSong();
		}
	  }, false);
	}
  });

})(mejs.$);
