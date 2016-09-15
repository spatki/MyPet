


/*!
 * Bootstrap Wizard plugin
 *
 * Licensed under the GPL license:
 * http://www.gnu.org/licenses/gpl.html
 *
 */

(function( $, undefined ) {
$.widget("bootstrap.bwizard", {
	//default option values
	options: {
			// Determines whether step tabs are clickable
		clickableSteps: true,
			// Determines whether panels are automatically displayed in order.
		autoPlay: false,
			// Determines the time span between panels in autoplay mode.
		delay: 3000,
			// Determines whether start from the first panel
			// when reaching the end in autoplay mode.
		loop: false,
			// This is an animation option for hiding the panel content.
			// e.g. { blind: true, fade: true, duration: 200}
		hideOption: { fade: true },
			// This is an animation option for showing the panel content.
			// e.g. { blind: true, fade: true, duration: 200}
		showOption: { fade: true, duration: 400 },
			// Additional Ajax options to consider when
			// loading panel content (see $.ajax).
		ajaxOptions: null,
			// Whether or not to cache remote bwizard content;
			// Cached content is being lazy loaded; e.g once and
			// only once for the panel is displayed.
			// Note that to prevent the actual Ajax requests from being cached
			// by the browser you need to provide an extra cache:
			// false flag to ajaxOptions.
		cache: false,
			// Store the latest active index in a cookie.
			// The cookie is then used to determine the initially active index
			// if the activeIndex option is not defined.
			// Requires cookie plugin. The object needs to have key/value pairs
			// of the form the cookie plugin expects as options.
			// e.g. { expires: 7, path: '/', domain: 'jquery.com', secure: true }
		cookie: null,
			// HTML template for step header when a new panel is added with the
			// add method or  when creating a panel for a remote panel on the fly.
		stepHeaderTemplate: '',
			// HTML template from which a new panel is created
			// by adding a panel with the add method or
			// when creating a panel for a remote panel on the fly.
		panelTemplate: '',
			// The HTML content of this string is shown in a panel
			// while remote content is loading.
			// Pass in empty string to deactivate that behavior.
		spinner: '',
			// A value that indicates the text of back button.
			// Code example:
			// $("#element").bwizard("option", "backBtnText", "Back Button");
		backBtnText: '&larr; Previous',
			// A value that indicates the text of next button.
			// Code example:
			// $("#element").bwizard("option", "nextBtnText", "next Button");
		nextBtnText: 'Next &rarr;',
			// The add event handler. A function called when a panel is added.
			// Default: null.
			// Type: Function.
			// Code example: $("#element").bwizard({ add: function (e, ui) { } });
		add: null,
			// The remove event handler. A function called when a panel is removed.
			// Default: null.
			// Type: Function.
			// Code example: $("#element").bwizard({ remove: function (e, ui) { } });
		remove: null,
			// The activeIndexChanged event handler.
			// A function called when the activeIndex changed.
			// Default: null.
			// Type: Function.
			// Code example:
			// $("#element").bwizard({ activeIndexChanged: function (e, ui) { } });
		activeIndexChanged: null,
			// The show event handler. A function called when a panel is shown.
			// Default: null.
			// Type: Function.
			// Code example: $("#element").bwizard({ show: function (e, ui) { } });
		show: null,
			// The load event handler.
			// A function called after the content of a remote panel has been loaded.
			// Default: null.
			// Type: Function.
			// Code example: $("#element").bwizard({ load: function (e, ui) { } });
		load: null,
			// The validating event handler.
			// A function called before moving to next panel. Cancellable.
			// Default: null.
			// Type: Function.
			// Code example: $("#element").bwizard({ validating: function (e, ui) { } });
		validating: null
	},

	_defaults: {
		stepHeaderTemplate: '<li>#{title}</li>',
		panelTemplate: '<div></div>',
		spinner: '<em>Loading&#8230;</em>'
	},

	_create: function () {
		var self = this;
		self._pageLize(true);
	},

	_init: function () {
		var o = this.options,
			dis = o.disabled;
		if (o.disabledState) {
			this.disable();
			o.disabled = dis;
		} else {
			if (o.autoPlay) {
				this.play();
			}
		}
	},

	_setOption: function (key, value) {
		$.Widget.prototype._setOption.apply(this, arguments);

		switch (key) {
		case 'activeIndex':
			this.show(value);
			break;

		case 'navButtons':
			this._createButtons();
			break;

		default:
			this._pageLize();
			break;
		}
	},

	play: function () {
		var o = this.options, self = this, id;
		if (!this.element.data('intId.bwizard')) {
			id = window.setInterval(function () {
				var index = o.activeIndex + 1;
				if (index >= self.panels.length) {
					if (o.loop) {
						index = 0;
					} else {
						self.stop();
						return;
					}
				}
				self.show(index);
			}, o.delay);

			this.element.data('intId.bwizard', id);
		}
	},

	stop: function () {
		var id = this.element.data('intId.bwizard');
		if (id) {
			window.clearInterval(id);
			this.element.removeData('intId.bwizard');
		}
	},

	_normalizeBlindOption: function (o) {
		if (o.blind === undefined) {
			o.blind = false;
		}
		if (o.fade === undefined) {
			o.fade = false;
		}
		if (o.duration === undefined) {
			o.duration = 200;
		}
		if (typeof o.duration === 'string') {
			try {
				o.duration = parseInt(o.duration, 10);
			}
			catch (e) {
				o.duration = 200;
			}
		}
	},

	_createButtons: function () {
		var self = this, o = this.options, bt,
			backBtnText = o.backBtnText,
			nextBtnText = o.nextBtnText;

		this._removeButtons();
		if (o.navButtons === 'none') {
			return;
		}

		if (!this.buttons) {
			bt = o.navButtons;

			var requiresPager = false;
			this.buttons = $('<ul class="pager"/>');
			this.buttons.addClass('bwizard-buttons');
			if(backBtnText != ''){
				this.backBtn =
					$("<li class='previous'><a href='#' class='btn btn-small gray-bg'>" +
						backBtnText + "</a></li>")
					.appendTo(this.buttons).bind({
						'click': function () {
							self.back();
							return false;
						}
					}).attr("role", "button");
				var requiresPager = true;
			}
			if(nextBtnText != ''){
				this.nextBtn =
					$("<li class='next'><a href='#' class='btn btn-small blue-bg'>" +
						nextBtnText + "</a>")
					.appendTo(this.buttons).bind({
						'click': function () {
							self.next();
							return false;
						}
					}).attr("role", "button");
				var requiresPager = true;
			}
			if(requiresPager) {
				this.buttons.appendTo(this.element);
			} else {
				this.buttons = null;
			}
		}
	},

	_removeButtons: function () {
		if (this.buttons) {
			this.buttons.remove();
			this.buttons = undefined;
		}
	},

	_pageLize: function (init) {
		var self = this, o = this.options,
			fragmentId = /^#.+/; // Safari 2 reports '#' for an empty hash;

		//Fix a bug that when no title and has ul li element in its content
		//this.list = this.element.find('ol,ul').eq(0);
		var isOL = false;
		this.list = this.element.children('ol,ul').eq(0);
		var l = this.list.length;
		if (this.list && l === 0) {
			this.list = null;
		}
		if (this.list) {
			if (this.list.get(0).tagName.toLowerCase() === "ol") {
				isOL = true;
			}
			this.lis = $('li', this.list);
			this.lis.each(function(i){
				if (o.clickableSteps){
					$(this).click(function (args) {
						args.preventDefault();
						self.show(i);
					});
					$(this).contents().wrap('<a href="#step' + (i+1) + '" class="hidden-phone"/>');
				} else {
					$(this).contents().wrap('<span class="hidden-phone"/>');
				}
				$(this).attr("role", "tab")
				$(this).css('z-index',self.lis.length-i);
				$(this).prepend('<span class="label">' + (i+1) + '</span>');
				if (!isOL) {
					$(this).find('.label').addClass('visible-phone');
				}
			});
		}

		if (init) {
			this.panels = $('> div', this.element);

			this.panels.each(function (i, p) {
				$(this).attr('id', 'step'+(i+1))
				var url = $(p).attr('src');
				// inline
				if (url && !fragmentId.test(url)) {
					// mutable data
					$.data(p, 'load.bwizard', url.replace(/#.*$/, ''));
				}
			});

			this.element.addClass('bwizard clearfix');
			if (this.list) {
				this.list
					.addClass('bwizard-steps clearfix')
					.attr("role", "tablist");
				if (o.clickableSteps){
					this.list.addClass('clickable')
				}
			}
			this.container = $('<div/>');
			this.container.addClass('well');
			this.container.append(this.panels);
			this.container.appendTo(this.element);
			this.panels.attr("role", "tabpanel");

			// Activate a panel
			// use "activeIndex" option or try to retrieve:
			// 1. from cookie
			// 2. from actived class attribute on panel
			if (o.activeIndex === undefined) {
				if (typeof o.activeIndex !== 'number' && o.cookie) {
					o.activeIndex = parseInt(self._cookie(), 10);
				}
				if (typeof o.activeIndex !== 'number' &&
						this.panels.filter('.bwizard-activated').length) {
					o.activeIndex = this.panels
						.index(this.panels.filter('.bwizard-activated'));
				}
				o.activeIndex = o.activeIndex || (this.panels.length ? 0 : -1);
			} else if (o.activeIndex === null) {
				// usage of null is deprecated, TODO remove in next release
				o.activeIndex = -1;
			}

			// sanity check - default to first page...
			o.activeIndex = ((o.activeIndex >= 0 && this.panels[o.activeIndex]) ||
				o.activeIndex < 0) ? o.activeIndex : 0;

			this.panels.addClass('hide').attr('aria-hidden', true);
			if (o.activeIndex >= 0 && this.panels.length) {
				// check for length avoids error when initializing empty pages
				this.panels.eq(o.activeIndex).removeClass('hide')
					.addClass('bwizard-activated').attr('aria-hidden', false);
				this.load(o.activeIndex);
			}

			this._createButtons();
		} else {
			this.panels = $('> div', this.container);
			o.activeIndex = this.panels
				.index(this.panels.filter('.bwizard-activated'));
		}

		this._refreshStep();

		// set or update cookie after init and add/remove respectively
		if (o.cookie) {
			this._cookie(o.activeIndex, o.cookie);
		}

		// reset cache if switching from cached to not cached
		if (o.cache === false) {
			this.panels.removeData('cache.bwizard');
		}

		if (o.showOption === undefined || o.showOption === null) {
			o.showOption = {};
		}
		this._normalizeBlindOption(o.showOption);

		if (o.hideOption === undefined || o.hideOption === null) {
			o.hideOption = {};
		}
		this._normalizeBlindOption(o.hideOption);

		// remove all handlers
		this.panels.unbind('.bwizard');
	},

	_refreshStep: function () {
		var o = this.options;

		if (this.lis) {
			this.lis.removeClass('active').attr('aria-selected', false).find('.label').removeClass('badge-inverse');
			if (o.activeIndex >= 0 && o.activeIndex <= this.lis.length - 1) {
				if (this.lis) {
					this.lis.eq(o.activeIndex).addClass('active').attr('aria-selected', true).find('.label').addClass('badge-inverse');
				}
			}
		}

		if (this.buttons && !o.loop) {
			this.backBtn[o.activeIndex <= 0 ? 'addClass' :
				'removeClass']('disabled')
				.attr('aria-disabled', o.activeIndex === 0);
			this.nextBtn[o.activeIndex >= this.panels.length - 1 ?
				'addClass' : 'removeClass']('disabled')
				.attr('aria-disabled', (o.activeIndex >= this.panels.length - 1));
		}
	},

	_sanitizeSelector: function (hash) {
		// we need this because an id may contain a ":"
		return hash.replace(/:/g, '\\:');
	},

	_cookie: function () {
		var cookie = this.cookie || (this.cookie = this.options.cookie.name);
		return $.cookie.apply(null, [cookie].concat($.makeArray(arguments)));
	},

	_ui: function (panel) {
		return {
			panel: panel,
			index: this.panels.index(panel)
		};
	},

	_removeSpinner: function () {
		// restore all former loading bwizard labels
		var spinner = this.element.data('spinner.bwizard');
		if (spinner) {
			this.element.removeData('spinner.bwizard');
			spinner.remove();
		}
	},

	// Reset certain styles left over from animation
	// and prevent IE's ClearType bug...
	_resetStyle: function ($el) {
		$el.css({ display: '' });
		if (!$.support.opacity) {
			$el[0].style.removeAttribute('filter');
		}
	},

	destroy: function () {
		var o = this.options;
		this.abort();
		this.stop();
		this._removeButtons();
		this.element.unbind('.bwizard')
			.removeClass([
				'bwizard',
				'clearfix'
			].join(' '))
			.removeData('bwizard');

		if (this.list) {
			this.list.removeClass('bwizard-steps clearfix')
				.removeAttr('role');
		}

		if (this.lis) {
			this.lis.removeClass('active').removeAttr('role');
			this.lis.each(function () {
				if ($.data(this, 'destroy.bwizard')) {
					$(this).remove();
				} else {
					$(this).removeAttr('aria-selected');
				}
			});
		}

		this.panels.each(function () {
			var $this = $(this).unbind('.bwizard');
			$.each(['load', 'cache'], function (i, prefix) {
				$this.removeData(prefix + '.bwizard');
			});

			if ($.data(this, 'destroy.bwizard')) {
				$(this).remove();
			} else {
				$(this).removeClass([
					'bwizard-activated',
					'hide'
				].join(' ')).css({ position: '', left: '', top: '' })
				.removeAttr('aria-hidden');
			}
		});

		this.container.replaceWith(this.container.contents());

		if (o.cookie) {
			this._cookie(null, o.cookie);
		}

		return this;
	},

	add: function (index, title) {
		if (index === undefined) {
			index = this.panels.length; // append by default
		}

		if (title === undefined) {
			title = "Step " + index;
		}

		var self = this, o = this.options,
			$panel = $(o.panelTemplate || self._defaults.panelTemplate)
				.data('destroy.bwizard', true),
			$li;
		$panel.addClass('hide')
			.attr('aria-hidden', true);

		if (index >= this.panels.length) {
			if (this.panels.length > 0) {
				$panel.insertAfter(this.panels[this.panels.length - 1]);
			} else {
				$panel.appendTo(this.container);
			}
		} else {
			$panel.insertBefore(this.panels[index]);
		}

		if (this.list && this.lis) {
			$li = $((o.stepHeaderTemplate || self._defaults.stepHeaderTemplate)
				.replace(/#\{title\}/g, title));
			$li.data('destroy.bwizard', true);

			if (index >= this.lis.length) {
				$li.appendTo(this.list);
			} else {
				$li.insertBefore(this.lis[index]);
			}
		}

		this._pageLize();

		if (this.panels.length === 1) { // after pagelize
			o.activeIndex = 0;
			$li.addClass('ui-priority-primary');
			$panel.removeClass('hide')
				.addClass('bwizard-activated')
				.attr('aria-hidden', false);
			this.element.queue("bwizard", function () {
				self._trigger('show', null, self._ui(self.panels[0]));
			});

			this._refreshStep();
			this.load(0);
		}

		// callback
		this._trigger('add', null, this._ui(this.panels[index]));
		return this;
	},

	remove: function (index) {
		var o = this.options,
			//$li = this.lis.eq(index).remove(),
			$panel = this.panels.eq(index).remove();

		this.lis.eq(index).remove();
		if (index < o.activeIndex) {
			o.activeIndex--;
		}

		this._pageLize();

		//Ajust the active panel index in some case
		if ($panel.hasClass('bwizard-activated') && this.panels.length >= 1) {
			this.show(index + (index < this.panels.length ? 0 : -1));
		}

		// callback
		this._trigger('remove', null, this._ui($panel[0]));
		return this;
	},

	_showPanel: function (p) {
		var self = this, o = this.options, $show = $(p), props;
		$show.addClass('bwizard-activated');
		if ((o.showOption.blind || o.showOption.fade) && o.showOption.duration > 0) {
			props = { duration: o.showOption.duration };
			if (o.showOption.blind) {
				props.height = 'toggle';
			}
			if (o.showOption.fade) {
				props.opacity = 'toggle';
			}
			$show.hide().removeClass('hide') // avoid flicker that way
				.animate(props, o.showOption.duration || 'normal', function () {
					self._resetStyle($show);
					self._trigger('show', null, self._ui($show[0]));
					self._removeSpinner();
					$show.attr('aria-hidden', false);
					self._trigger('activeIndexChanged', null, self._ui($show[0]));
				});
		} else {
			$show.removeClass('hide').attr('aria-hidden', false);
			self._trigger('show', null, self._ui($show[0]));
			self._removeSpinner();
			self._trigger('activeIndexChanged', null, self._ui($show[0]));
		}
	},

	_hidePanel: function (p) {
		var self = this, o = this.options, $hide = $(p), props;
		$hide.removeClass('bwizard-activated');
		if ((o.hideOption.blind || o.hideOption.fade) && o.hideOption.duration > 0) {
			props = { duration: o.hideOption.duration };
			if (o.hideOption.blind) {
				props.height = 'toggle';
			}
			if (o.hideOption.fade) {
				props.opacity = 'toggle';
			}
			$hide.animate(props, o.hideOption.duration || 'normal', function () {
				$hide.addClass('hide').attr('aria-hidden', true);
				self._resetStyle($hide);
				self.element.dequeue("bwizard");
			});
		} else {
			$hide.addClass('hide').attr('aria-hidden', true);
			this.element.dequeue("bwizard");
		}
	},

	show: function (index) {
		if (index < 0 || index >= this.panels.length) {
			return this;
		}

		// previous animation is still processing
		if (this.element.queue("bwizard").length > 0) {
			return this;
		}

		var self = this, o = this.options,
			args = $.extend({}, this._ui(this.panels[o.activeIndex])),
			$hide, $show;
		args.nextIndex = index;
		args.nextPanel = this.panels[index];
		if (this._trigger('validating', null, args) === false) {
			return this;
		}

		$hide = this.panels.filter(':not(.hide)');
		$show = this.panels.eq(index);
		o.activeIndex = index;

		this.abort();

		if (o.cookie) {
			this._cookie(o.activeIndex, o.cookie);
		}

		this._refreshStep();
		// show new panel
		if ($show.length) {
			if ($hide.length) {
				this.element.queue("bwizard", function () {
					self._hidePanel($hide);
				});
			}

			this.element.queue("bwizard", function () {
				self._showPanel($show);
			});

			this.load(index);
		}
		else {
			throw 'Bootstrap Wizard: Mismatching fragment identifier.';
		}

		return this;
	},

	next: function () {
		var o = this.options,
			index = o.activeIndex + 1;
		if (o.disabled) {
			return false;
		}
		if (o.loop) {
			index = index % this.panels.length;
		}

		if (index < this.panels.length) {
			this.show(index);
			return true;
		}
		return false;
	},

	back: function () {
		var o = this.options,
			index = o.activeIndex - 1;
		if (o.disabled) {
			return false;
		}
		if (o.loop) {
			index = index < 0 ? this.panels.length - 1 : index;
		}

		if (index >= 0) {
			this.show(index);
			return true;
		}
		return false;
	},

	load: function (index) {
		var self = this,
			o = this.options,
			p = this.panels.eq(index)[0],
			url = $.data(p, 'load.bwizard'),
			spinner;

		this.abort();

		// not remote or from cache
		if (!url || this.element.queue("bwizard").length !== 0 &&
				$.data(p, 'cache.bwizard')) {
			this.element.dequeue("bwizard");
			return;
		}

		// load remote from here on
		if (o.spinner) {
			spinner = this.element.data('spinner.bwizard');
			if (!spinner) {
				spinner = $('<div class="modal" id="spinner" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"/>');
				spinner.html(o.spinner || self._defaults.spinner);
				spinner.appendTo(document.body);
				this.element.data('spinner.bwizard', spinner);
				spinner.modal();
			}
		}

		this.xhr = $.ajax($.extend({}, o.ajaxOptions, {
			url: url,
			dataType: 'html',
			success: function (r, s) {
				$(p).html(r);

				if (o.cache) {
					// if loaded once do not load them again
					$.data(p, 'cache.bwizard', true);
				}

				// callbacks
				self._trigger('load', null, self._ui(self.panels[index]));
				try {
					if (o.ajaxOptions && o.ajaxOptions.success) {
						o.ajaxOptions.success(r, s);
					}
				}
				catch (e1) { }
			},
			error: function (xhr, s) {
				// callbacks
				self._trigger('load', null, self._ui(self.panels[index]));
				try {
					// Passing index avoid a race condition when this method is
					// called after the user has selected another panel.
					if (o.ajaxOptions && o.ajaxOptions.error) {
						o.ajaxOptions.error(xhr, s, index, p);
					}
				}
				catch (e2) { }
			}
		}));

		// last, so that load event is fired before show...
		self.element.dequeue("bwizard");

		return this;
	},

	abort: function () {
		// Terminate all running panel ajax requests and animations.
		this.element.queue([]);
		this.panels.stop(false, true);

		// "bwizard" queue must not contain more than two elements,
		// which are the callbacks for hide and show
		this.element.queue("bwizard",
			this.element.queue("bwizard").splice(-2, 2));

		// terminate pending requests from other bwizard
		if (this.xhr) {
			this.xhr.abort();
			delete this.xhr;
		}

		// take care of spinners
		this._removeSpinner();
		return this;
	},

	url: function (index, url) {
		this.panels.eq(index).removeData('cache.bwizard')
			.data('load.bwizard', url);
		return this;
	},

	count: function () {
		return this.panels.length;
	}

});
} (jQuery));