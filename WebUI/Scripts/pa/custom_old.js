// Global Variables used for tab navigation
var keypressCode = -1; 
var keypressShift = false;
var invalidCharacters = ["&bull;"];

$(function () {

	
	/***************************************
	 Color Box Border
	***************************************/
	$(".box-color .box-title").append("<div class='box-title-border'></div>");
	
	
	
	
	/***************************************
	 Bootstrap Dropdown Menu
	***************************************/
	
	
	
	
	
	/***************************************
	 Slimscroll Bar (Autohide)
	***************************************/
		
	var topPos = $(window).width() <= 768 ? '0px' : 'auto'

	
	$("aside#left_panel").slimscroll({
	
		width:'270px',
		height: 'auto',
		size: '3px',
		position: 'right',
		color: '#666'
	
	}).parent().css({
		'position': 'fixed',
		'top': topPos,
	});
	
	
	
	
		
	/***************************************
	 Slimscroll Bar (Show)
	***************************************/

	$(".overflow-scroll.visible-scroll").each(function(){

		$(this).slimscroll({

			size: '4px',
			height:'auto',
			alwaysVisible: true,
			color: '#666'
		
		})
	
	});
	
	$(".overflow-scroll.visible-scroll").each(function () {

	    $(this).slimscroll({

	        size: '4px',
	        height: 'auto',
	        alwaysVisible: true,
	        color: '#666'

	    })

	});
	
	
	/*$(".onlyScrolls").each(function () {

	    $(this).slimscroll({

	        size: '4px',
	        height: 'auto',
	        alwaysVisible: true,
	        color: '#666'

	    })

	}); */
	
	
	/***************************************
	 Toggle Top Menu and Aside
	***************************************/
	$(".toggle-topmenu").click(function(){
		$('nav#main_topnav ul').slideToggle();
	});
	
	//Click Devices
	$(".toggle-aside").on('click', function(){
		slideAside()
	});
	
	
	
	/***************************************
	 Some Global Variable
	***************************************/
	var winHeight = $(window).height();
	var headHeight = $('header').innerHeight();
	var navHeight = $('nav#main_topnav').innerHeight();
	
	
	
	
	/*Set Aside Height*/
	function asideHeight(){
		$('aside').height( winHeight - headHeight );
	}
	
	asideHeight()
	
	
	
	
	
	function slideAside(){
		$('section#main_content, nav#main_topnav, header').toggleClass('movefor-aside');
		$('aside').toggleClass('asideopen');
		setTimeout(" $('body, html').toggleClass('overf-hide')",150);
	}
	
	
	
	function swipeAside(){
		
		$("body, *, html, document").swipe({			
		swipeRight:function() { 
		$('section#main_content, nav#main_topnav, header').addClass('movefor-aside');
		$('aside').addClass('asideopen');
		},
		swipeLeft:function() { 
		$('section#main_content, nav#main_topnav, header').removeClass('movefor-aside');
		$('aside').removeClass('asideopen');
		},
		//Default is 75px, set to 0 for demo so any distance triggers swipe
  		threshold:75
		});
		
	}
	
	
	
	//Touch Devices
	$(window).resize(function() {
		
		asideHeight()
							  
		if ($(window).width() <= 768) {
			swipeAside();
			
		}
		else{$('nav#main_topnav ul').removeAttr('style')}
		
 	});
	
	if ($(window).width() <= 768) {
		swipeAside();
		
	}
	
	
	
	
	
	
	
	
	
	
		
	
	
	
	
	/***************************************
	 Fixed Position After Scroll
	***************************************/

	$("nav#main_topnav").css({"top":Math.max(0,70-$(this).scrollTop())});
	$("aside#left_panel").css("top",Math.max(50,120-$(this).scrollTop()));
	$(window).scroll(function(){
		$("nav#main_topnav").css({"top":Math.max(0,70-$(this).scrollTop())});
		$("aside#left_panel").css("top",Math.max(50,120-$(this).scrollTop()));
	});
	

	
	
	
	
	/***************************************
	 Aside Dropdown Menu
	***************************************/
	$("nav#aside_nav > ul > li > a").click(function () {
		$(this).parent().children('ul').slideToggle();
		$(this).parent().siblings('li.open').removeClass('open').children('ul').slideToggle();
		$(this).parent().has('ul').toggleClass('open');
	})
	
	$("nav#aside_nav > ul > li > ul > li > a").click(function () {
	    $(this).parent().children('ul').slideToggle();
	    $(this).parent().siblings('li.open').removeClass('open').children('ul').slideToggle();
	    $(this).parent().has('ul').toggleClass('open');
	})

	
	
	/***************************************
	 Metro Table
	***************************************/
	$('.metro-table tbody tr td').each(function(){
		$(this).wrapInner('<span></span>')
	});
	
	
	
	
	
	
	
	
	/***************************************
	 Full Calendar
	***************************************/

	var date = new Date();
	var d = date.getDate();
	var m = date.getMonth();
	var y = date.getFullYear();
	
	var calendar = $('#calendar').fullCalendar({
		header: {
			left: 'title',
			right: 'prev,next today,month,agendaWeek,agendaDay'
		},
		selectable: true,
		selectHelper: true,
		select: function(start, end, allDay) {
				var title = prompt('Reminder / Plan Name:');
				if (title) {
					calendar.fullCalendar('renderEvent',
						{
							title: title,
							start: start,
							end: end,
							allDay: allDay
						},
						true // make the event "stick"
					);
				}
				calendar.fullCalendar('unselect');
		},
		editable: true,
		droppable: true, // this allows things to be dropped onto the calendar !!!
		drop: function(date, allDay) { // this function is called when something is dropped
			
				// retrieve the dropped element's stored Event Object
				var originalEventObject = $(this).data('eventObject');
				
				// we need to copy it, so that multiple events don't have a reference to the same object
				var copiedEventObject = $.extend({}, originalEventObject);
				
				// assign it the date that was reported
				copiedEventObject.start = date;
				copiedEventObject.allDay = allDay;
				
				// render the event on the calendar
				// the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
				$('#calendar').fullCalendar('renderEvent', copiedEventObject, true);
				
				// is the "remove after drop" checkbox checked?
				if ($('#drop-remove').is(':checked')) {
					// if so, remove the element from the "Draggable Events" list
					$(this).remove();
				}
		},
		events: [
			{
				title: 'Public Holiday',
				start: new Date(y, m, 1),
				backgroundColor: 'red',
                color: 'white'
			},
            {
                title: 'Assigned: Code Review (Salary Processing)',
                start: new Date(y, m, 6),
                end: new Date(y, m, 9),
                backgroundColor: 'darkblue',
                color: 'white'
            },
            {
                title: 'Reminder: Scheduled Client Meeting',
                start: new Date(y, m, 15),
                end: new Date(y, m, 16),
            },
			{
				title: 'Long Event',
				start: new Date(y, m, d-5),
				end: new Date(y, m, d-2)
			},
			{
				id: 999,
				title: 'Repeating Event',
				start: new Date(y, m, d-3, 16, 0),
				allDay: false
			},
			{
				id: 999,
				title: 'Repeating Event',
				start: new Date(y, m, d+4, 16, 0),
				allDay: false
			},
			{
				title: 'Meeting',
				start: new Date(y, m, d, 10, 30),
				allDay: false
			},
			{
				title: 'Lunch',
				start: new Date(y, m, d, 12, 0),
				end: new Date(y, m, d, 14, 0),
				allDay: false
			},
			{
				title: 'Birthday Party',
				start: new Date(y, m, d+1, 19, 0),
				end: new Date(y, m, d+1, 22, 30),
				allDay: false
			},
			{
				title: 'Click for Google',
				start: new Date(y, m, 28),
				end: new Date(y, m, 29),
				url: 'http://google.com/'
			}
		]
	});
	

	/* initialize the external events
		-----------------------------------------------------------------*/
	
	var dragEvent = function(ed) {
	
		// create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
		// it doesn't need to have a start or end
		var eventObject = {
			title: $.trim(ed.text()) // use the element's text as the event title
		};
		
		// store the Event Object in the DOM element so we can get to it later
		ed.data('eventObject', eventObject);
		
		// make the event draggable using jQuery UI
		ed.draggable({
			zIndex: 999,
			revert: true,      // will cause the event to go back to its
			revertDuration: 0  //  original position after the drag
		});
		
	};
		
	
	$(document).on('click', '#add-event',function(){
		
		var eventValue = $('#event-value').val();
		
		if( $('#event-value').val() == 0 ){
			var eventValue = "Untitled Event"
			}
		
		var eventHTML = $('<li>'+eventValue+'</li>')
		$('ul.events-list').prepend(eventHTML)
		dragEvent(eventHTML)
	});
	
	
	$('ul.events-list li').each(function () {
               dragEvent($(this))
     });	
		
		
	$('td.fc-header-center').remove()
	 
	
	
	
	
	
	
	
	
	/***************************************
	 Popovers
	***************************************/
		
	$(".popovers").popover({
		trigger: $(this).attr('trigger'),
		title: $(this).attr('title'),
		position: $(this).attr('placement'),
		content: $(this).attr('content'),
	    html: true
	});
	
	
	
	
	
	
	
	/***************************************
	 Tooltips
	***************************************/
	
	$('.tooltip-demo').tooltip({
		position: $(this).attr('placement')
	});
	
	
	
	
	
	
	/***************************************
	 Visualize Charts
	***************************************/

	// Charts	
	function drawCharts(sourceTable) {
	
		
		var chartWidth = $('#areachart').parent('div').width() - 25;
		
			sourceTable.hide().visualize({
				type: 'area',
				width: chartWidth,
				height: 250,
				lineDots: 'double',				
				lineWeight: 3,
				multiHover: 5,
				appendTitle: false,
				appendKey: false
				
			}).appendTo('.stats_charts').trigger('visualizeRefresh');;

		$('.visualize,  .visualize-area').css('margin', '0px 0px 20px 22px');
	}
	
	
	// Init the charts
	$('table.stats').each(function() {
		drawCharts( $(this) );
	});
	
	
	// Redraw the carts
	$(window).resize(function() {
		$('.visualize').remove();
		
		$('table.stats').each(function() {
			drawCharts( $(this) );
		});
	});
	
	
	
	
	
	
	
	
	
	/***************************************
	 Box Toolbar buttons
	***************************************/
	
	/*refreash*/
	$('.reload-box').click(function(){
		$(this).parent().parent().parent().parent().find('.panel-body').prepend('<div class="box-loader"></div>');
		$(this).parent().parent().parent().parent().find('.panel-body .box-loader').delay(3000).fadeOut('slow', function(){ $(this).remove() });
		return false;
	});

	
	/*minimize mazimize*/
	$('.mini-max').click(function(){
		$(this).parent().parent().parent().parent().find('.panel-body').slideToggle('fast');
		return false;
	});
	
	/*close*/
	$('.close-box').click(function(){
		$(this).parent().parent().parent().parent().fadeTo(400, 0, function () {$(this).hide(400, function(){ $(this).remove()});});
		return false;
	});
	





	/***************************************
	 Static Notifications
	***************************************/
	
	$('.notification').click(function(){
		$(this).parent().fadeTo(400, 0, function () {$(this).hide(400, function(){ $(this).remove()});});
		return false;
	});
	
	
	
	
	
	
	/***************************************
	 Input File styling
	***************************************/
	
	$('input[type=file].styled').each(function(){
		
		$(this).css({'opacity':'0', 'height':'0px', 'width':'0px'})
		var getClass = $(this).attr('class')
		$(this).before('<div class="styled-input-file '+ getClass +'"><div class="input-file-box"><input type="text" class="form-control col-lg-12"></div><input type="button" value="Browse" class="btn gray-bg"></div>');
		
		 var btnWidth = $(this).siblings('.styled-input-file').find('.btn').innerWidth();
		 
		 $('.styled-input-file .input-file-box').css('marginRight', btnWidth + 'px')
		
	});
	
	
	$('.styled-input-file').click(function(){
		
		$(this).siblings('input[type=file]').click();
		$(this).siblings('input[type=file]').change(function(){
			var inputVal = $(this).val()
			$(this).siblings('.styled-input-file').find('.input-file-box input').val(inputVal)
		});
		
	});
	
	
	
	
	
	/***************************************
	 Custom TagsInput
	***************************************/

	$('#tags_1').tagsInput({});

	$('.tagsinput').each(function(){
		
		var prevClass =  $(this).siblings('input').attr('class') 
		$(this).addClass( prevClass )	
		
	});
	
	
	
	
	
	
	/***************************************
	 Date/Time Picker - Bootstrap
	***************************************/
	
	/* 
	These are the default options for initializing the widget:
	  
	maskInput: true,           // disables the text input mask
	pickDate: true,            // disables the date picker
	pickTime: true,            // disables de time picker
	pick12HourFormat: false,   // enables the 12-hour format time picker
	pickSeconds: true,         // disables seconds in the time picker
	startDate: -Infinity,      // set a minimum date
	endDate: Infinity          // set a maximum date
	*/
	
	$('.datepicker').datetimepicker({
                    pickTime: false
                });
	
	
	$('.datetimepicker').datetimepicker();
	
	
	$('.datetimepicker-12').datetimepicker();
	
	
	$('.timepicker').datetimepicker({
                    pickDate: false
                });
		
  
	
	
	
	
	
	
	
	/***************************************
	 Maskedinput
    ***************************************/

	$(".mask_date").inputmask("d/m/y", {autoUnmask: true});  //direct mask        
	$(".mask_date1").inputmask("d/m/y",{ "placeholder": "*"}); //change the placeholder
	$(".mask_date2").inputmask("d/m/y",{ "placeholder": "dd/mm/yyyy" }); //multi-char placeholder
	$(".mask_phone").inputmask("mask", {"mask": "(999) 999-9999"}); //specifying fn & options
	$(".mask_tin").inputmask({"mask": "99-9999999"}); //specifying options only
	$(".mask_number").inputmask({ "mask": "9", "repeat": 10, "greedy": false });  // ~ mask "9" or mask "99" or ... mask "9999999999"
	$(".mask_decimal").inputmask('decimal', { rightAlignNumerics: false }); //disables the right alignment of the decimal input
	$(".mask_currency").inputmask('€ 999.999.999,99', { numericInput: true });  //123456  =>  € ___.__1.234,56
	$(".mask_currency2").inputmask('€ 999,999,999.99', { numericInput: true, rightAlignNumerics: false  }); //123456  =>  € ___.__1.234,56
	$(".mask_ssn").inputmask("999-99-9999", {placeholder:" ", clearMaskOnLostFocus: true }); //default
	$(".mask_callback").inputmask({ "mask": "9", "repeat": 5,  "oncomplete": function(){ alert('Callback Function !'); } }); // callback function
	$(".mask_duration").inputmask("999:99", { placeholder: " ", clearMaskOnLostFocus: true }); //default
	$(".mask_percent").inputmask({ "mask": "9", "repeat" : 3, "greedy" : false }, { placeholder: " ", clearMaskOnLostFocus: true }); //default

	
	
	
	
	/***************************************
	 MS Dropdown (images)
    ***************************************/
	
	$("#countries").msDropdown();
	
	
	
	
	
	/***************************************
	 Colorpicker
    ***************************************/
    
	/*Default Picker*/        
	$('input.minicolors-default').minicolors({

		swatchPosition: 'right',
		defaultValue:'#ed518d',
		
		change: function(hex, opacity) {
			// Generate text to show in console
			mdt = hex ? hex : 'transparent';
			if( opacity ) mdt += ', ' + opacity;
			mdt += ' / ' + $(this).minicolors('rgbaString');
			
			$('#mdtvalue').text(mdt)
		}
		
	});
	
	/*Without Input Field*/
	$('input.minicolors-nofield').minicolors({

		swatchPosition: 'right',
		defaultValue:'#ac54f0',
		textfield:false,
		
		change: function(hex, opacity) {
			// Generate text to show in console
			mnf = hex ? hex : 'transparent';
			if( opacity ) mnf += ', ' + opacity;
			mnf += ' / ' + $(this).minicolors('rgbaString');
			
			$('#mnfvalue').text(mnf)
		}
		
	});
	
	/*Inline Picker*/
	$('input.minicolors-inline').minicolors({

		defaultValue:'#ed518d',
		inline:true
		
	});
	
	
	
	
	
	
	/***************************************
	 Styled Checkbox and Radio buttons
    ***************************************/

	
	$(".icheck").each(function(){
		var $el = $(this);
		var skin = ($el.attr('data-skin') !== undefined) ? "_"+$el.attr('data-skin') : "",
		color = ($el.attr('data-color') !== undefined) ? "-"+$el.attr('data-color') : "";

		var opt = {
			checkboxClass: 'icheckbox' + skin + color,
			radioClass: 'iradio' + skin + color,
			increaseArea: "10%"
		}

		$el.iCheck(opt);
	});
	  
	

	
	$('.skin-line input').each(function(){
		var self = $(this),
		label = self.next(),
		label_text = label.text();
		
		label.remove();
		self.iCheck({
		checkboxClass: 'icheckbox_line-blue',
		radioClass: 'iradio_line-blue',
		insert: '<div class="icheck_line-icon"></div>' + label_text
		});
	});
	

	
	
	/*for color selector*/
  	$('.colors li').click(function() {
    var self = $(this);

    if (!self.hasClass('active')) {
      self.siblings().removeClass('active');

      var skin = self.closest('.skin'),
        color = self.attr('class') ? '-' + self.attr('class') : '',
        checkbox = skin.data('icheckbox'),
        radio = skin.data('iradio'),
        checkbox_default = 'icheckbox_minimal',
        radio_default = 'iradio_minimal';

      if (skin.hasClass('skin-square')) {
        checkbox_default = 'icheckbox_square', radio_default = 'iradio_square';
        checkbox == undefined && (checkbox = 'icheckbox_square-green', radio = 'iradio_square-green');
      };

      if (skin.hasClass('skin-flat')) {
        checkbox_default = 'icheckbox_flat', radio_default = 'iradio_flat';
        checkbox == undefined && (checkbox = 'icheckbox_flat-red', radio = 'iradio_flat-red');
      };

      if (skin.hasClass('skin-line')) {
        checkbox_default = 'icheckbox_line', radio_default = 'iradio_line';
        checkbox == undefined && (checkbox = 'icheckbox_line-blue', radio = 'iradio_line-blue');
      };

      checkbox == undefined && (checkbox = checkbox_default, radio = radio_default);

      skin.find('input, .skin-states .state').each(function() {
        var element = $(this).hasClass('state') ? $(this) : $(this).parent(),
          element_class = element.attr('class').replace(checkbox, checkbox_default + color).replace(radio, radio_default + color);

        element.attr('class', element_class);
      });

      skin.data('icheckbox', checkbox_default + color);
      skin.data('iradio', radio_default + color);
      self.addClass('active');
    };
  });

  
	
	
	
	
	/***************************************
	 Bootstrap wysihtml5
    ***************************************/
	
	$('.wysihtml5').wysihtml5();
	
	

	
	
	
	
	/***************************************
	 Form Validation
    ***************************************/
	
	$('#basic-validation').validate({
	    rules: {			
	      firstname: "required",
		  
		  lastname: "required",
		  
		  username: {
			required: true,
			minlength: 2
		  },
			
	      email: {
	        required: true,
	        email: true
	      },
		  
	      subject: {
	      	minlength: 2,
	        required: true
	      },
		  
	      message: {
	        minlength: 2,
	        required: true
	      }
	    },
			highlight: function(element) {
				$(element).closest('.form-group').removeClass('has-success').addClass('has-error');
			},
			success: function(element) {
				element
				.text('OK!').addClass('valid')
				.closest('.form-group').removeClass('has-error').addClass('has-success');
			}
	  });
	  
	$('#masterProc-validation').validate({
	    rules: {

	        procName: "required",

	    },
	    highlight: function (element) {
	        $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
	    },
	    success: function (element) {
	        element
            .text('OK!').addClass('valid')
            .closest('.form-group').removeClass('has-error').addClass('has-success');
	    }
	});

	  
	  $('#advanced-validation').validate({
	    rules: {
			
	      url: { 
	         url: true ,
			 required: true
	      },
		   
		  date: { 
	         date: true ,
			 required: true
	      },
		  
		  dateISO: { 
	         dateISO: true ,
			 required: true
	      },
		  
		  number: { 
	         number: true ,
			 required: true
	      },
		  
		  digits: { 
	         digits: true ,
			 required: true
	      },
		  
		  creditcard: { 
	         creditcard: true ,
			 required: true
	      },
		  
		   field: {
			 required: true,
			 extension: "xls|csv"
		   },
		  
		   password: "required",
			 password_again: {
			 equalTo: "#password",
			 required: true
		   },
		   
		    maxlenth: {
			  required: true,
			  maxlength: 4
			},
			
			minlenth: {
			  required: true,
			  minlength: 6
			},
			
			 range: {
				required: true,
				range: [13, 23]
			},
			
			 rangelength: {
				required: true,
				rangelength: [2, 6]
			}
		  
	    },
			highlight: function(element) {
				$(element).closest('.form-group').removeClass('has-success').addClass('has-error');
			},
			success: function(element) {
				element
				.text('OK!').addClass('valid')
				.closest('.form-group').removeClass('has-error').addClass('has-success');
			}
	  });
	
	
	



	
	/***************************************
	 Wizard
    ***************************************/
	
	 $(".wizard").bwizard();
	 
	 
	 

	 
	 
	/***************************************
	 Buttons
    ***************************************/ 
	 
	$('.load-state')
      .click(function () {
        var btn = $(this)
        btn.button('loading')
        setTimeout(function () {
          btn.button('complete')
        }, 3000)
      });
	  
	  
	 $('.nav-tabs').button('toggle');
	 
	 






	/***************************************
	 Advanced Table 
    ***************************************/
	
	$('.advanced-table').dataTable({
									"sPaginationType": "bootstrap",
									"aLengthMenu": [
													[5, 10, 15, -1],
													[5, 10, 15, "All"] // change per page values here
												   ],
												// set the initial value
												"iDisplayLength": 5
								  });
								  
	
	
	
	
	
	

	/***************************************
	 Ajax Modal
    ***************************************/

	var $modal = $('#ajax-modal');

	$('#ajax').on('click', function(){
	  // create the backdrop and wait for next modal to be triggered
	  $('body').modalmanager('loading');
	
	  setTimeout(function(){
		 $modal.load('modal_ajax_test.html', '', function(){
		  $modal.modal();
		});
	  }, 1000);
	});
	
	$modal.on('click', '.update', function(){
	  $modal.modal('loading');
	  setTimeout(function(){
		$modal
		  .modal('loading')
		  .find('.modal-body')
			.prepend('<div class="alert alert-info fade in">' +
			  'Updated!<button type="button" class="close" data-dismiss="alert">&times;</button>' +
			'</div>');
	  }, 1000);
	});


	
	
	
	
	
	
	/***************************************
	 Sliders
    ***************************************/
	
	$('.slider').slider();
	
	var RGBChange = function() {
          $('#RGB').css('background', 'rgb('+r.getValue()+','+g.getValue()+','+b.getValue()+')')
        };

        var r = $('#R').slider()
                .on('slide', RGBChange)
                .data('slider');
        var g = $('#G').slider()
                .on('slide', RGBChange)
                .data('slider');
        var b = $('#B').slider()
                .on('slide', RGBChange)
                .data('slider');

        $('#eg input').slider();
		
		
		






	/***************************************
	 Tiles
    ***************************************/	
		
	$('#tiles').freetile({
			animate: true,
			elementDelay: 30
		});		
	
	
	
	function showDashBoard(){
        $('.tile').each(function(){
            $(this).addClass('fadeInForward').removeClass('fadeOutback');
        });
    }
	
	
	function fadeDashBoard(){
        $('.tile').addClass('fadeOutback').removeClass('fadeInForward');
    }
	
	
	$('.tile').each(function(){
		
		$(this).click(function(){
		  
		  var Bgcolor = $(this).css('background-color'); 
		  $('.tile-page').addClass('openpage').css('background', Bgcolor);
		  fadeDashBoard()
		  
		});
	
  	});
	
	

	  $('.close-button').click(function(){
		$(this).parent().addClass('slidePageLeft').one('webkitAnimationEnd oanimationend msAnimationEnd animationend', function(e) {
					$(this).removeClass('slidePageLeft').removeClass('openpage');
				  });
		showDashBoard();
	  });	
		
	
	
	
	
	
	/***************************************
	 Contact List
    ***************************************/

	$('#contact_list').sliderNav();
	
	
	
	
	/***************************************
	 Gallery
    ***************************************/
	
	$('ul.gallery li').each(function(){
	
		var imgTitle = $(this).find('img').attr('alt');
		var imgSrc =  $(this).find('img').attr('src');

		$(this).append('<div class="overlay-options"><a href="'+ imgSrc +'" title="'+imgTitle+'" data-gallery="gallery" class="btn green-bg"><i class="icon-">&#xf0b2;</i></a> &nbsp;<a href="#" id="delete-img" class="btn red-bg"><i class="icon-">&#xf014;</i></a></div>');
		
	});
	
	
	$(document).on('click','#delete-img', function(){
		
		var confirmDel=confirm("Are you sure")
		if (confirmDel==true)
		  {$(this).parent().parent().fadeTo(400, 0, function () {$(this).hide(400, function(){ $(this).remove()});});}
		
	});








	/***************************************
	 Toggle fullscreen button
    ***************************************/
	
     $('#toggle-fullscreen').button().click(function () {
        var button = $(this),
            root = document.documentElement;
        if (!button.hasClass('active')) {
            $('#modal-gallery').addClass('modal-fullscreen');
            if (root.webkitRequestFullScreen) {
                root.webkitRequestFullScreen(
                    window.Element.ALLOW_KEYBOARD_INPUT
                );
            } else if (root.mozRequestFullScreen) {
                root.mozRequestFullScreen();
            }
        } else {
            $('#modal-gallery').removeClass('modal-fullscreen');
            (document.webkitCancelFullScreen ||
                document.mozCancelFullScreen ||
                $.noop).apply(document);
        }
    });
	
	
	
	
	
	

	/***************************************
	 Image Cropping
    ***************************************/
	

	$('#demo-crop').imgAreaSelect({ 
		handles: true,
		fadeSpeed: 200,
		onSelectChange: preview });

 
	/*Preview Crop*/
	function preview(img, selection) {
    if (!selection.width || !selection.height)
        return;
    
    var scaleX = 100 / selection.width;
    var scaleY = 100 / selection.height;

    $('#preview img').css({
        width: Math.round(scaleX * 300),
        height: Math.round(scaleY * 300),
        marginLeft: -Math.round(scaleX * selection.x1),
        marginTop: -Math.round(scaleY * selection.y1)
    });

    $('#x1').val(selection.x1);
    $('#y1').val(selection.y1);
    $('#x2').val(selection.x2);
    $('#y2').val(selection.y2);
    $('#w').val(selection.width);
    $('#h').val(selection.height);    
	}

	
	
	
});

/* Custom added by Shweta : Start */

$(".toggle-showAside").on('click', function () {
    hideAside()
});

function hideAside() {
    $('section#main_content, nav#main_topnav, header').toggleClass('moveRightfor-aside');
    $('aside').toggleClass('asideopen');
    setTimeout(" $('body, html').toggleClass('overf-hide')", 150);
}

$(document).on("click", "#hideMenu", function () {
    var direction = $(this).data("direction");

    if (direction == "in") {
        // Hide this menu
        $("#left_panel").hide("slide", { direction: "left" }, 1000);
        $(this).data("direction", "out");
        // Change the image
        $(this).children("i").eq(0).html("&#xf054;");
    }
    else {
        // show this menu
        $("#left_panel").show("slide", { direction: "left" }, 1000);
        $(this).data("direction", "in");
        // Change the image
        $(this).children("i").eq(0).html("&#xf053;");
    }
    $("#main_content").toggleClass("adjustMain_content", 1000);
});

/***************************************
 Project Calendar
***************************************/

var date = new Date();
var d = date.getDate();
var m = date.getMonth();
var y = date.getFullYear();

var projCalendar = $('#showProjCalendar').fullCalendar({
    header: {
        left: 'title',
        right: 'prev,next today'
    },
    selectable: true,
    selectHelper: true,
    select: function (start, end, allDay) {
        var calEvent = {
            id: 0,
            title: "",
            start: start,
            end: end,
            allDay: allDay
        };
        setHolidayDetails(calEvent);
        $("#editHolidays").modal("show");
        $("#deleteHoliday").addClass("hidden");
    },
    eventClick: function (event, element) {
        setHolidayDetails(event);
        $("#editHolidays").modal("show");
        $("#deleteHoliday").removeClass("hidden");
    },
    
    editable: true,
    droppable: true, // this allows things to be dropped onto the calendar !!!
    drop: function (date, allDay) { // this function is called when something is dropped

        // retrieve the dropped element's stored Event Object
        var originalEventObject = $(this).data('eventObject');

        // we need to copy it, so that multiple events don't have a reference to the same object
        var copiedEventObject = $.extend({}, originalEventObject);

        // assign it the date that was reported
        copiedEventObject.start = date;
        copiedEventObject.allDay = allDay;

        // render the event on the calendar
        // the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
        $('#showProjCalendar').fullCalendar('renderEvent', copiedEventObject, true);

        // is the "remove after drop" checkbox checked?
        if ($('#drop-remove').is(':checked')) {
            // if so, remove the element from the "Draggable Events" list
            $(this).remove();
        }
    },
    events: [
        {
            id: 1,
            title: 'Republic Day',
            start: new Date(2014, 0, 26)
        },
        {
            id: 2,
            title: 'Independence Day',
            start: new Date(2014, 7, 15)
        },
        {
            id: 3,
            title: 'Diwali',
            start: new Date(2014, 9, 22)
        },
        {
            id: 4,
            title: 'Diwali',
            start: new Date(2014, 9, 23)
        },
        {
            id: 5,
            title: 'Christmas',
            start: new Date(2014, 11, 25)
        },
        {
            id: 6,
            title: 'New Year',
            start: new Date(2014, 0, 1)
        }
    ]
});


/* initialize the external events
    -----------------------------------------------------------------*/

var dragEvent = function (ed) {

    // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
    // it doesn't need to have a start or end
    var eventObject = {
        title: $.trim(ed.text()) // use the element's text as the event title
    };

    // store the Event Object in the DOM element so we can get to it later
    ed.data('eventObject', eventObject);

    // make the event draggable using jQuery UI
    ed.draggable({
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        revertDuration: 0  //  original position after the drag
    });

};


$(document).on('click', '#add-event', function () {

    var eventValue = $('#event-value').val();

    if ($('#event-value').val() == 0) {
        var eventValue = "Untitled Event"
    }

    var eventHTML = $('<li>' + eventValue + '</li>')
    $('ul.events-list').prepend(eventHTML)
    dragEvent(eventHTML)
});


$('ul.events-list li').each(function () {
    dragEvent($(this))
});

$('td.fc-header-center').remove()

/***************************************
 Timesheet Calendar
***************************************/

var date = new Date();
var d = date.getDate();
var m = date.getMonth();
var y = date.getFullYear();

var calendar = $('#showTSCalendar').fullCalendar({
    header: {
        left: 'title',
        right: 'prev,next today,month,agendaWeek,agendaDay'
    },
    selectable: true,
    selectHelper: true,
    defaultAllDayEventDuration: 8,
    select: function (start, end, allDay) {
        var days = parseInt((end-start)/(1000*60*60*24));
        var startHours = start.getHours();
        var startMinutes = start.getMinutes();
        var endHours = end.getHours();
        var endMinutes = end.getMinutes();
        var taskDuration = (((endHours * 60) + endMinutes) - ((startHours * 60) + startMinutes)) / 60;

        var calEvent = {
            id: 0,
            title: "",
            start: start,
            end: end,
            allDay: allDay,
            project: "1",
            group: "Design Team",
            task: "0",
            Duration: (taskDuration) ? taskDuration : "08.0",
            CP: 50,
            Remarks: "",
            TC: "0"
          };

        $("#TSDays").data("days", days);
        if (days > 0) $("#dateRange").text(start.getDate() + "/" + (start.getMonth() + 1) + "/" + start.getFullYear() + " - " + end.getDay() + "/" + (end.getMonth()+1) + "/" + end.getFullYear());
        else $("#dateRange").text(start.getDate() + "/" + (start.getMonth() + 1) + "/" + start.getFullYear());
         
        $("#fromTime").val(startHours + ":" + startMinutes + ((startHours > 12) ? " PM" : " AM"));
        $("#toTime").val(endHours + ":" + endMinutes + ((endHours > 12) ? " PM" : " AM"));
        if (taskDuration) $("#taskDuration").attr("disabled", "disabled"); else $("#taskDuration").removeAttr("disabled");
        
        if (days > 0) {
            $("#dist").show();
            $("#durDistribute").val(1);
        }
        else {
            $("#dist").hide();
        }
        setTimeDetails(calEvent);
        $("#editTS").modal("show");
        $("#deleteTime").addClass("hidden");
    },
    eventClick: function (event, element) {
        var startHours = event.start.getHours();
        var startMinutes = event.start.getMinutes();
        if (event.end == null) {
            var days = 0;
            endHours = startHours;
            endMinutes = startMinutes;
        }
        else
        {
            var days = parseInt((event.end - event.start) / (1000 * 60 * 60 * 24));
            var endHours = event.end.getHours();
            var endMinutes = event.end.getMinutes();
        }

        var taskDuration = (((endHours * 60) + endMinutes) - ((startHours * 60) + startMinutes)) / 60;
        $("#TSDays").data("days", days);
        if (days > 0) $("#dateRange").text(event.start.getDate() + "/" + (event.start.getMonth() + 1) + "/" + event.start.getFullYear() + " - " + event.end.getDay() + "/" + (event.end.getMonth() + 1) + "/" + event.end.getFullYear());
        else $("#dateRange").text(event.start.getDate() + "/" + (event.start.getMonth() + 1) + "/" + event.start.getFullYear());

        $("#fromTime").val(startHours + ":" + startMinutes + ((startHours > 12) ? " PM" : " AM"));
        $("#toTime").val(endHours + ":" + endMinutes + ((endHours > 12) ? " PM" : " AM"));
        if (taskDuration) $("#taskDuration").attr("disabled", "disabled"); else $("#taskDuration").removeAttr("disabled");

        if (days > 0) {
            $("#dist").show();
            $("#durDistribute").val(1);
        }
        else {
            $("#dist").hide();
        }

        setTimeDetails(event);
        $("#editTS").modal("show");
        $("#deleteTime").removeClass("hidden");
    },
    editable: true,
    droppable: true, // this allows things to be dropped onto the calendar !!!
    drop: function (date, allDay) { // this function is called when something is dropped

        // retrieve the dropped element's stored Event Object
        var originalEventObject = $(this).data('eventObject');

        // we need to copy it, so that multiple events don't have a reference to the same object
        var copiedEventObject = $.extend({}, originalEventObject);

        // assign it the date that was reported
        copiedEventObject.start = date;
        copiedEventObject.allDay = allDay;

        // render the event on the calendar
        // the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
        $('#showTSCalendar`').fullCalendar('renderEvent', copiedEventObject, true);

        // is the "remove after drop" checkbox checked?
        if ($('#drop-remove').is(':checked')) {
            // if so, remove the element from the "Draggable Events" list
            $(this).remove();
        }
    }
});


/* initialize the external events
    -----------------------------------------------------------------*/

var dragEvent = function (ed) {

    // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
    // it doesn't need to have a start or end
    var eventObject = {
        title: $.trim(ed.text()) // use the element's text as the event title
    };

    // store the Event Object in the DOM element so we can get to it later
    ed.data('eventObject', eventObject);

    // make the event draggable using jQuery UI
    ed.draggable({
        zIndex: 999,
        revert: true,      // will cause the event to go back to its
        revertDuration: 0  //  original position after the drag
    });

};


$(document).on('click', '#add-event', function () {

    var eventValue = $('#event-value').val();

    if ($('#event-value').val() == 0) {
        var eventValue = "Untitled Event"
    }

    var eventHTML = $('<li>' + eventValue + '</li>')
    $('ul.events-list').prepend(eventHTML)
    dragEvent(eventHTML)
});


$('ul.events-list li').each(function () {
    dragEvent($(this))
});


$('td.fc-header-center').remove()

var dateAdd = function (valDate, days) {
    var currDays = valDate.getDate();
    var monthDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    var newDays
    var newMonth
    var newYear

    newDays = currDays + days;

    if (newDays < 1) {
        // decrement days
        newMonth = ((valDate.getMonth() - 1) < 0) ? 11 : (valDate.getMonth() - 1);
        newYear = ((valDate.getMonth() - 1) < 0) ? (valDate.getFullYear() - 1) : valDate.getFullYear();
        newDays = monthDays[newMonth] + newDays;
    }
    else {
        newMonth = valDate.getMonth();
        newYear = valDate.getFullYear();
        //              alert(newDays);
        if (newDays > monthDays[newMonth]) {
            if (valDate.getMonth() == 11) { newMonth = 0; newYear = valDate.getFullYear() + 1; }
            else newMonth = valDate.getMonth() + 1;
            newDays = newDays - monthDays[valDate.getMonth()];
        }
        //                alert("M: " + newMonth + " D: " + newDays + " Y: " + newYear);
    }
    return new Date(newYear, newMonth, newDays);
};

// Functions for in-line editing and navigating using tab or enter keys: START

$("body").delegate(".EditNumber", "click", function () {
    var OriginalContent = $(this).text();

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("EditNumber");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='3' id='mask_decimal' class='inputValue' data-class='EditNumber'/>");
    //$("#mask_decimal").inputmask({ 'mask': ["9[9][.99]", "999", "9", "99"] }, { placeholder:" ", rightAlignNumerics: true, clearMaskOnLostFocus: true });
    $("#mask_decimal").inputmask('[9][99][999][.99]', { "placeholder": " ", rightAlignNumerics: true, clearMaskOnLostFocus: true });
    $(this).children().first().focus();
    $(this).children().first().select();
});

$("body").delegate(".EditDate", "click", function () {
    var OriginalContent = $(this).text();

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("EditDate");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='12' id='mask_date2' class='inputValue' data-class='EditDate' data-navkey='" + $(this).data("navkey") + "'/>");
    $("#mask_decimal").inputmask('decimal', { rightAlignNumerics: true })
    $(this).children().first().focus();

});

$("body").delegate(".Edit", "click", function () {
    var OriginalContent = $(this).text();

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("Edit");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='30' data-class='Edit' data-navkey='" + $(this).data("navkey") + "' class='inputValue'/>");
    $(this).children().first().focus();
    $(this).children().first().select();
});

$("body").delegate(".EditGSC", "click", function () {
    var OriginalContent = $(this).text();

    $(this).addClass("cellEditing");
    $(this).removeClass("EditGSC");
    $(this).html("<select class='inputValue' data-class='EditGSC' data-navkey='EditGSC'><option value='0'>&nbsp;0&nbsp; </option><option value='1'>&nbsp;1&nbsp; </option><option value='2'>&nbsp;2&nbsp;&nbsp;</option><option value='3'>&nbsp;3&nbsp; </option><option value='4'>&nbsp;4&nbsp; </option><option value='5'>&nbsp;5&nbsp; </option></select>");
    $(this).children().first().val(OriginalContent);
    $(this).children().first().focus();
});

$("body").delegate(".inputValue", "keypress", function (e) {
    if (e.which == 13) {
        var newContent = $(this).val();
        var editClass = $(this).data("class");
        var parentEle = $(this).parent();

        if (editClass == "EditGSC") {
            // Load help information
            parentEle.next().children("div").first().children("div.info").first().children().hide();
            parentEle.next().children("div").first().children("div.info").first().children(".GSC" + newContent).first().show();
        }
        if (editClass == "EditNumber") parentEle.text(parseFloat(newContent)); else parentEle.text(newContent);
        parentEle.removeClass("cellEditing");
        // calculate values
        if (parentEle.data("type") == "size") calculateSizeLineItem(parentEle);
        calculateValues();
        parentEle.addClass(editClass);
        keypressCode = 9;
        keypressShift = false;
        navigate(parentEle);
    }
});

$("body").delegate(".inputValue", "blur", function () {
    var newContent = $(this).val();
    var editClass = $(this).data("class");
    var parentEle = $(this).parent();

    if (editClass == "EditGSC") {
        // Load help information
        parentEle.next().children("div").first().children("div.info").first().children().hide();
        parentEle.next().children("div").first().children("div.info").first().children(".GSC" + newContent).first().show();
    }
    if (editClass == "EditNumber") parentEle.text(parseFloat(newContent)); else parentEle.text(newContent);
    parentEle.removeClass("cellEditing");
    // calculate values
    if (parentEle.data("type") == "size") calculateSizeLineItem(parentEle);
    calculateValues();
    parentEle.addClass(editClass);
    navigate(parentEle);
});

$("body").delegate(".inputValue", "keydown", function (e) {
    keypressCode = e.which;
    keypressShift = e.shiftKey;
});

$("body").delegate(".nav", "keydown", function (e) {
    keypressCode = e.which;
    keypressShift = e.shiftKey;
});

$("body").delegate(".nav", "blur", function () {
    if ($(this).is("td")) navigate($(this));
    else navigate($(this).parent());
});

var navigate = function (parentEle) {
    var navigateTo = parentEle.data("navkey");

    if (keypressCode == 9 && keypressShift) {
        // Tab + Shift
        if (parentEle.prev("." + navigateTo).length > 0) parentEle.prev("." + navigateTo).click();
        else if (parentEle.prevAll("." + navigateTo).length > 0) parentEle.prevAll("." + navigateTo).first().click();
        else if (parentEle.parent().prev(".EditRow").length > 0) parentEle.parent().prev(".EditRow").find("." + navigateTo).last().click();
        else parentEle.parent().prevAll(".EditRow").first().find("." + navigateTo).last().click();
    }
    else if (keypressCode == 9) {
        if (parentEle.next("." + navigateTo).length > 0) parentEle.next("." + navigateTo).click();
        else if (parentEle.nextAll("." + navigateTo).length > 0) parentEle.nextAll("." + navigateTo).first().click();
        else if (parentEle.parent().next(".EditRow").length > 0) parentEle.parent().next(".EditRow").find("." + navigateTo).first().click();
        else parentEle.parent().nextAll(".EditRow").first().find("." + navigateTo).first().click();
    }
    keypressCode = -1;
    keypressShift = false;
};

// Functions for in-line editing and navigating using tab or enter keys: END
// Load relevant page on the click of a sub-menu
$("body").delegate(".subMenu", "click", function () {
    $("#containerDetails").html(processing("Loading "));
    $("#containerDetails").load($(this).data("source"), function (response, status, xhr) {
        if (status == "error") {
            var msg = "Sorry but there was an error: ";
            //$( "#error" ).html( msg + xhr.status + " " + xhr.statusText );
            alert(msg + xhr.status + " " + xhr.statusText);
            $("#containerDetails").html("");
        }
    });
});

$("body").delegate(".reLoad", "click", function () {
    var filterFrom = $(this).data("filter");
    var containerID = "#" + $(this).data("container");
    var url = $(this).data("url");

    if (filterFrom != "") {
        url = url + "?filter=" + $("#" + filterFrom).find("option:selected").eq(0).val();
        $("#masterTitle").text($("#" + filterFrom).find("option:selected").eq(0).text() + " List");
    }
    $(containerID).html(processing(""));
    $(containerID).load(url, function (response, status, xhr) {
        if (status == "error") {
            var msg = "Sorry but there was an error: ";
            //$( "#error" ).html( msg + xhr.status + " " + xhr.statusText );
            alert(msg + xhr.status + " " + xhr.statusText);
            $(containerID).html("Reload Failed. Contact the system administrator");
        }
        else {
            $(containerID).html(response);
            jsTable("processTable");
        }
    });
});

$("body").delegate(".reLoadOnChange", "change", function () {
    $(".reLoad").click();
});

$("body").delegate(".closeDialog", "click", function () {
    var dialog = $("#" + $(this).data("id"));    // Dialog box to be closed
    dialog.modal("hide");
});

$("body").delegate(".saveDefaultExtra", "click", function () {
    var form = $("#" + $(this).data("form"));
    var mode = $(this).data("mode");
    var updateId = $(this).data("id");
    var dialogBox = $("#" + $(this).data("dialog"));
    var pageUpdateID = $("#" + $(this).data("pageid"));

    // Called by nested (extra) dialog boxes
    if ($(this).data("delete") == "client") {
        // Just remove this element on the client side as it is not been saved on server yet
        $("#" + updateId).remove();
        dialog.modal("hide");
        return;
    }
    dialogBox.html('<div class="modal-dialog" style=width:30%;">' +
            '<div class="modal-content">' +
                    '<div class="modal-body"> ' + processing("Processing. Please wait ") +
             '</div>' +
         '</div>');
    dialogBox.modal("show");
    $.ajax({
        cache: false,
        async: true,
        type: "POST",
        url: form.attr('action'),
        data: form.serialize(),
        success: function (data) {
            switch (mode) {
                case "add":
                    $("#" + updateId).append(data);
                    break;
                case "edit":
                    $("#" + updateId).replaceWith(data);
                    break;
                case "saveActivity":
                    $("#" + updateId).replaceWith(data);
                    $("#configureProcess").empty(); // Special handling for activities which uses the same section for display
                    $("#repositoryTree").show();
                    break;
                case "delete":
                    $("#" + updateId).remove();
                    pageUpdateID.replaceWith(data);
                    break;
                case "search":
                    $("#" + updateId).html(data);
                    break;
                default:
                    break;
            }
            dialogBox.modal("hide");
        },
        error: function (data) {
            switch (mode) {
                case "delete":
                    // Show the error message
                    $.pnotify({
                        title: 'Delete Failed',
                        text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                        type: 'error'
                    });
                    break;
                case "search":
                    $("#" + updateId).html(data.responseText);
                    break;
                case "saveActivity":
                    pageUpdateID.replaceWith(data.responseText);
                    break;
                default:
                    $("#openDialogBox").html(data.responseText);
                    break;
                }
            dialogBox.modal("hide");
        }
    });
});

$("body").delegate(".saveWorkflow", "click", function () {
    var dialog = $("#openDialogBox");
    if ($(this).data("confirm") == "True") {
        $("#revCommentsDialog").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">No</button>' +
                                        '<button type="button" class="btn blue-bg saveWorkflow"  data-dismiss="modal" data-id="containerDetails" data-source="' + $(this).data("source") + '" data-method="' + $(this).data("method") + '" data-form="' + $(this).data("form") + '" data-status="' + $(this).data("status") + '" data-userid="' + $(this).data("userid") + '" data-usertype="' + $(this).data("usertype") + '" data-message="' + $(this).data("message") + '" data-confirm="false" data-workflow="' + $(this).data("workflow")  + '">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>')
        $("#revCommentsDialog").modal("show");
        return;
    }
    var form = $(this).data("form");
    var updateId = $(this).data("id");
    var source = $(this).data("source");
    var method = $(this).data("method");
    var status = $(this).data("status");
    var userid = $(this).data("userid");
    var usertype = $(this).data("usertype");
    var message = $(this).data("message");
    var workflow = $(this).data("workflow");

    $("#workflow_status").val(status);
    $("#followWF").val(true);
    $("#statusWF").val(status);
    $("#workflow").val(workflow);

    if (usertype == "user") {
        // Prompt user to assign the next workflow
        $("#workflowUser").val(userid);
        processWorkflow(form, updateId, source, message);
    }
    else {
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: usertype,
            data: { 'formName': form, 'updateID': updateId, 'source': source, 'message': message, 'workflow': workflow },
            success: function (data) {
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
    }
});

$("body").delegate(".openWorkflow", "click", function () {
    var dialog = $("#openDialogBox");
    if ($(this).data("confirm") == "True") {
        $("#revCommentsDialog").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">No</button>' +
                                        '<button type="button" class="btn blue-bg openWorkflow"  data-dismiss="modal" data-id="containerDetails" data-source="' +
                                        $(this).data("source") + '" data-method="' + $(this).data("method") + '" data-form="' + $(this).data("form") +
                                        '" data-status="' + $(this).data("status") + '" data-userid="' + $(this).data("userid") + '" data-usertype="' +
                                        $(this).data("usertype") + '" data-message="' + $(this).data("message") + '" data-confirm="false" data-key="' + $(this).data("key") + '" data-workflow="' + $(this).data("workflow") + '">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>')
        $("#revCommentsDialog").modal("show");
        return;
    }
    var form = $(this).data("form");
    var updateId = $(this).data("id");
    var source = $(this).data("source");
    var method = $(this).data("method");
    var status = $(this).data("status");
    var userid = $(this).data("userid");
    var usertype = $(this).data("usertype");
    var message = $(this).data("message");
    var key = $(this).data("key");
    var workflow = $(this).data("workflow");

    $("#workflow_status").val(status);
    $("#followWF").val(true);
    $("#statusWF").val(status);
    $("#workflow").val(workflow);

    if (usertype == "user") {
        // Prompt user to assign the next workflow
        $("#workflowUser").val(userid);
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            data: { 'id': key, 'workflowUser': userid, 'status': status, 'message': message, 'workflow': workflow },
            success: function (data) {
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                $.pnotify({
                    title: "Check for errors",
                    type: 'info'
                });
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
    }
    else {
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: usertype,
            data: { 'formName': form, 'workflowUser': userid, 'updateID': updateId, 'source': source, 'message': message, 'workflow': workflow },
            success: function (data) {
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
    }
});

$("body").delegate(".saveWorkflowExtra", "click", function () {
    var form = $("#" + $(this).data("form"));
    var mode = $(this).data("mode");
    var updateId = $(this).data("id");
    var message = $(this).data("message");
    var dialogBox = $("#openDialogBox");
    //var pageUpdateID = $("#" + $(this).data("pageid"));

    $.ajax({
        cache: false,
        async: true,
        type: "POST",
        url: form.attr('action'),
        data: form.serialize(),
        success: function (data) {
            $.pnotify({
                title: message,
                type: 'info'
            });
            switch (mode) {
                case "add":
                    $("#" + updateId).append(data);
                    break;
                case "edit":
                    $("#" + updateId).replaceWith(data);
                    break;
                default:
                    break;
            }
            dialogBox.modal("hide");
        },
        error: function (data) {
            $.pnotify({
                title: "Check Error",
                type: 'info'
            });
            $("#openDialogBox").html(data.responseText);
        }
    });
});


$("body").delegate(".selectUser", "click", function () {
    var selectedUser = $("#reviewUser").val();
    if (selectedUser == "") {
        alert("Pl. select a user");
    }
    else {
        $("#workflowUser").val(selectedUser);
        $("#openDialogBox").modal("hide");
        processWorkflow($(this).data("form"), $(this).data("id"), $(this).data("source"), $(this).data("message"));
    }
});

$("body").delegate("#orgClientID", "change", function () {
    var clientID = $("#orgClientID").val()
    $("#ClientType")    .val("");
    $("#ClientName").val("");
    if (clientID != null && clientID != 0) {
        $.getJSON("/OrgClient/getClientDetailJSON/" + clientID, function (result) {
            $.each(result, function (index, element) {
                $("#ClientType").val(element.Type);
                $("#ClientName").val(element.Name);
                $("#ClientContactDetail").val(element.Description);
                $("#ClientContactPerson").val(element.PContactMailID);
            });
        }).fail(function () {
            //alert("System Error. Pl. contact the administrator");
        });
    }
});

    function processWorkflow (form, updateId, source, message)
    {
        var formObj = $("#" + form);
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: source,
            data: formObj.serialize(),
            success: function (data) {
                if (message == "") message = "Transaction Successful";
                $.pnotify({
                    title: message,
                    type: 'info'
                });
                $("#" + updateId).html(data);
            },
            error: function (data) {
                $.pnotify({
                    title: "Check for errors",
                    type: 'info'
                });
                $("#" + updateId).html(data.responseText);
            }
        });

    }


    $("body").delegate(".save", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");

        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                switch (mode) {
                    case "add":
                        $.pnotify({
                            title: 'New Record Created',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        break;
                    case "edit":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).html(data);
                        $("#openDialogBox").modal("hide");
                        break;
                    case "delete":
                        $.pnotify({
                            title: 'Record Deleted',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        break;
                    case "search":
                        $("#" + updateId).html(data);
                        break;
                    case "update":
                        $("#" + updateId).html(data);
                        $.pnotify({
                            title: 'Saved Successfully',
                            type: 'info'
                        });
                        break;
                    default:
                        break;
                }
            },
            error: function (data) {
                if (mode == "delete") {
                    if (data.status == 403) {
                        $.pnotify({
                            title: 'Delete Failed',
                            text: "You are not authorized to delete this entry.",
                            type: 'error'
                        });
                    }
                    else {
                        // Show the error message
                        $.pnotify({
                            title: 'Delete Failed',
                            text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                            type: 'error'
                        });
                    }
                }
                else {
                    if (mode == "search" || mode == "update") {
                        $("#" + updateId).html(data.responseText);
                    }
                    else {
                        $("#openDialogBox").html(data.responseText);
                    }
                }
            }
        });
    });



    $("body").delegate(".saveDefault", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");
        var reSequence = $("#reSequence").val();

        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                if (reSequence == "true") {
                    $(".reLoad").eq(0).click();
                }
                else {
                    $('#processTable').dataTable().fnDestroy();
                    switch (mode) {
                        case "add":
                            $.pnotify({
                                title: 'New Record Created',
                                type: 'info'
                            });
                            $("#" + updateId).append(data);
                            break;
                        case "edit":
                            $.pnotify({
                                title: 'Changes Saved',
                                type: 'info'
                            });
                            $("#" + updateId).replaceWith(data);
                            break;
                        case "delete":
                            $.pnotify({
                                title: 'Record Deleted',
                                type: 'info'
                            });
                            $("#" + updateId).remove();
                            break;
                        case "search":
                            $("#" + updateId).html(data);
                            break;
                        case "update":
                            $("#" + updateId).html(data);
                            $.pnotify({
                                title: 'Saved Successfully',
                                type: 'info'
                            });
                            break;
                        case "reset":
                            $.pnotify({
                                title: 'Password has been reset',
                                type: 'info'
                            });
                            break;
                        default:
                            break;
                    }
                    jsTable("processTable");
                }
                $("#openDialogBox").modal("hide");
                //$('#processTable').dataTable().fnStandingRedraw();
            },
            error: function (data) {
                if (mode == "delete") {
                    if (data.status == 403) {
                        $.pnotify({
                            title: 'Delete Failed',
                            text: "You are not authorized to delete this entry.",
                            type: 'error'
                        });
                    }
                    else {
                        // Show the error message
                        $.pnotify({
                            title: 'Delete Failed',
                            text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                            type: 'error'
                        });
                    }
                }
                else {
                    if (mode == "search" || mode == "update") {
                        $("#" + updateId).html(data.responseText);
                    }
                    else {
                        $("#openDialogBox").html(data.responseText);
                    }
                }
            }
        });
    });

    $("body").delegate(".saveDefaultPage", "click", function () {   // Function used to save pages that dont contain a sorted list of items
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");
        var pageUpdateID = $(this).data("pageid");

        // Special handling for configuring procedures in a process repository
        if (mode == "editProcedures" || mode == "editTemplates" || mode == "editChecklists" || mode == "editDocuments" || mode == "editActivity") {
            // Select all options so that it is posted to the server
            $("#selectedOptions").find('option').each(function (index, Element) {
                $(Element).prop("selected", "selected");
            });
        }
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                switch (mode) {
                    case "add":
                        $.pnotify({
                            title: 'New Record Created',
                            type: 'info'
                        });
                        $("#" + pageUpdateID).append(data);
                        break;
                    case "edit":
                    case "editProcedures":
                    case "editTemplates":
                    case "editChecklists":
                    case "editDocuments":
                    case "editActivity" :
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });

                        $("#" + pageUpdateID).replaceWith(data);
                        break;
                    case "deleteGeneralTask":
                        $.pnotify({
                            title: 'Record Deleted',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        break;
                    case "delete":
                        $.pnotify({
                            title: 'Record Deleted',
                            type: 'info'
                        });
                        $("#" + pageUpdateID).remove();
                        break;
                    case "search":
                        $("#" + pageUpdateID).html(data);
                        break;
                    default:
                        break;
                }
                $("#openDialogBox").modal("hide");
                //$('#processTable').dataTable().fnStandingRedraw();
            },
            error: function (data) {
                if (mode == "delete") {
                    // Show the error message
                    $.pnotify({
                        title: 'Delete Failed',
                        text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                        type: 'error'
                    });
                }
                else {
                    if (mode == "search") {
                        $("#" + updateId).html(data.responseText);
                    }
                    else {
                        $("#openDialogBox").html(data.responseText);
                    }
                }
            }
        });
    });

    function jsTable(tableID) {
        $('#' + tableID).dataTable({
            "sPaginationType": "bootstrap",
            "aLengthMenu": [
                            [5, 10, 15, -1],
                            [5, 10, 15, "All"] // change per page values here
            ],
            // set the initial value
            "iDisplayLength": 5
        });
    }

    $("body").delegate(".openGeneralDialog", "click", function () {
        var dialog = $("#" + $(this).data("dialogbox"));
        dialog.html('<div class="modal-dialog" style="width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + processing("Processing. Please wait ") + '</div>' +
                         '</div>' +
                     '</div>');
        dialog.modal("show");
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: $(this).data("source"),
            success: function (data) {
                dialog.html(data);
            },
            error: function (data) {
                dialog.html(data.responseText);
            }
        });
        //dialog.modal("show");
    });

    $("body").delegate(".openDialog", "click", function () {
        var dialog = $("#openDialogBox");
        dialog.html('<div class="modal-dialog" style="width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + processing("Processing. Please wait ") + '</div>' +
                         '</div>' +
                     '</div>');
        dialog.modal("show");
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: $(this).data("source"),
            success: function (data) {
                dialog.html(data);
            },
            error: function (data) {
                dialog.html(data.responseText);
            }
        });
        //dialog.modal("show");
    });

    $("body").delegate(".openDeleteDialog", "click", function () {
        var dialog = $("#openDialogBox");
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Delete</h4>' +
                            '</div>' +
                            '<form method="post" action="' + $(this).data("source") + '" id="submitLevel">' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<input type="hidden" id="id" value="' + $(this).data("id") + '"/>' +
                                        '<p class="control-label col-md-12 text-center">Do you want to delete this entry ?</p>' +
                                    '</div>' + 
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">No</button>' +
                                        '<button type="button" class="btn blue-bg saveDefault"  data-dismiss="modal" data-mode="delete" data-id = "' + $(this).data("id") + '" data-form="submitLevel">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".openDeleteDialog2", "click", function () {
        var mode = ($(this).data("delete") == "server" ? "delete" : "deleteOnClient");
        var dialog = $("#extraDialogBox");
        var pageUpdateID = $(this).data("pageid");

        // This is a saved entry, so will need to delete this on server (database)
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle" data-dismiss="modal">Confirm Delete</h4>' +
                            '</div>' +
                            '<form method="post" action="' + $(this).data("source") + '" id="submitLevel">' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<input type="hidden" id="id" value="' + $(this).data("id") + '"/>' +
                                        '<p class="control-label col-md-12 text-center">Do you want to delete this entry ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="extraDialogBox"  data-dismiss="modal">No</button>' +
                                        '<button type="button" class="btn blue-bg saveDefaultExtra" data-mode="' + mode + '" data-id = "' + $(this).data("id") + '" data-form="submitLevel" data-dialog="extraDialogBox" data-pageid="' + pageUpdateID + '">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".relativeAddNew", "click", function () {
        var dialog = $("#openDialogBox");
        var sourceID = "#" + $(this).data("sourceid");
        var source = $(this).data("source")

        if ($(sourceID).find("option:selected").eq(0).val() != 0) source = source +  "?value=" + $(sourceID).find("option:selected").eq(0).val() + "&text=" + $(sourceID).find("option:selected").eq(0).text();
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: source,
            success: function (data) {
                dialog.html(data);
            },
            error: function (data) {
                dialog.html(data.responseText);
            }
        });
        dialog.modal("show");
    });

    // parsing unobstructive attributes when we get content via ajax
    $(function () {
        $(document).ajaxComplete(function (e, xhr, settings) {
            $.validator.unobtrusive.parse(document);
        });
    });

    $.fn.dataTableExt.oApi.fnStandingRedraw = function (oSettings) {
        //redraw to account for filtering and sorting
        // concept here is that (for client side) there is a row got inserted at the end (for an add)
        // or when a record was modified it could be in the middle of the table
        // that is probably not supposed to be there - due to filtering / sorting
        // so we need to re process filtering and sorting
        // BUT - if it is server side - then this should be handled by the server - so skip this step
        if (oSettings.oFeatures.bServerSide === false) {
            var before = oSettings._iDisplayStart;
            oSettings.oApi._fnReDraw(oSettings);
            //iDisplayStart has been reset to zero - so lets change it back
            oSettings._iDisplayStart = before;
            oSettings.oApi._fnCalculateEnd(oSettings);
        }

        //draw the 'current' page
        oSettings.oApi._fnDraw(oSettings);
    };

    function specialDropDown(id) {
        $(id).chosen({ disable_search_threshold: 10 });
    }

    function multiComboConfig(sel) {
        $("#openDialogBox").find(sel).each(function (index, Element) {
            if ($(Element).css('display') != "none") {
                $(this).chosen({ disable_search_threshold: 10 });
            }
        });
    }


    /* Custom added by Shweta : End*/

    function printStructure(url,treeRoot,label,createSource,editSource) {
        $.getJSON(url, function (result) {
            var ddl = $("#" + treeRoot);
            ddl.empty();
            // First add an option to add a new node
            ddl.append('<li id="LnewNode">' + 
                           '<a class="openDialog" id="AnewNode" data-toggle="modal" href="javascript:;" data-id="" data-source="' + createSource + '?level=1&parent=0">' + label + '</a>' +
                        '</li>');
            activeNode = ddl;
            $.each(result,function (index,element) {
                if (element.Level == 1) {
                    activeNode = ddl; // Add to the root
                }
                else {
                    activeNode = ddl.find("#C" + element.ParentNodeID).eq(0); // search for the parent id and add this node under it
                }
                // Append the new node
                activeNode.append('<li id="L' + element.ID + '"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="L' + element.ID + '" data-source="' + editSource + '?id=' + element.ID + '"><i class="icon-edit"></i></a>' +
                                '<a class="tree-toggle" data-toggle="branch" role="branch" data-id="' + element.ID + '" id="' + element.ID + '" href="javascript:">' + element.nodeName + '</a>' +
                                '&nbsp;<ul id="C' + element.ID + '" class="branch in">' +
                                        '<li id="L' + element.ID + 'newNode"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="" data-source="' + createSource + '?level=' + (this.Level + 1) + '&parent=' + element.ID + '">' + label + '</a></li>' +
                                        '</ul>' +
                            '</li>');
            }); 
        }).fail(function() {
            //alert("System Error. Pl. contact the administrator");
        });
    };

    function printConfiguration(url, treeRoot, label, createSource, editSource) {
        $.getJSON(url, function (result) {
            var ddl = $("#" + treeRoot);
            ddl.empty();
            // First add an option to add a new node
            ddl.append('<li id="LnewNode">' +
                           '<a class="openDialog" id="AnewNode" data-toggle="modal" href="javascript:;" data-id="" data-source="' + createSource + '?level=1&parent=0">' + label + '</a>' +
                        '</li>');
            activeNode = ddl;
            $.each(result, function (index, element) {
                if (element.Level == 1) {
                    activeNode = ddl; // Add to the root
                }
                else {
                    activeNode = ddl.find("#C" + element.ParentID).eq(0); // search for the parent id and add this node under it
                }
                // Append the new node
                activeNode.append('<li id="L' + element.ID + '" class="sort-item"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="L' + element.ID + '" data-source="' + editSource + '?id=' + element.ID + '"><i class="icon-edit"></i></a>' +
                                        '<a class="tree-toggle closed repositoryDetails loadRepo" data-toggle="branch" role="branch" data-id="' + element.ID + '" id="Node' + element.ID + '" href="javascript:">' +
                                            '<span id="Lvl' + element.ID + '" class="label label-info">' + element.levelName +'</span>' +
                                            '<span id ="' + element.ID + '">' + element.nodeName + '</span>' +
                                        '</a>' +
                                        '<ul id="C' + element.ID + '" class="branch">' +
                                            '<li id="edit' + element.ID + '">' +
                                            '&nbsp;&nbsp;<a href="javascript:" class="tree-toggle closed repositoryDetails" role="branch" data-toggle="branch" id="editRep' + element.ID + '" data-basenode="' + element.ID + '"  data-id="' + element.ID + '">' +
                                            '<span class="label label-success">Repository</span></a>' +
                                            '<ul class="branch" id="repo' + element.ID + '"></ul>' +
                                            '</li>' +
                                            '<li id="L' + element.ID + 'newNode"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="" data-source="/PConfiguration/Create?level=' + (element.Level + 1) + '&parent=' + element.ID + '">Add Repository</a></li>' +
                                        '</ul>' +
                                   '</li>');
                /*            activeNode.append('<li id="L' + element.ID + '"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="L' + element.ID + '" data-source="' + editSource + '?id=' + element.ID + '"><i class="icon-edit"></i></a>' +
                                            '<a class="tree-toggle" data-toggle="branch" role="branch" data-id="' + element.ID + '" id="' + element.ID + '" href="javascript:">' + element.levelName + '</a>' +
                                            '&nbsp;<ul id="C' + element.ID + '" class="branch in">' +
                                                    '<li id="L' + element.ID + 'newNode"><a href="javascript:" data-toggle="modal" class="openDialog" data-id="" data-source="' + createSource + '?level=' + (this.Level + 1) + '&parent=' + element.ID + '">' + label + '</a></li>' +
                                                    '</ul>' +
                                        '</li>');  */

            });
        }).fail(function () {
            //alert("System Error. Pl. contact the administrator");
        });
    };

    function printConfigForMapping(url, treeRoot, className) {
        $.getJSON(url, function (result) {
            var ddl = $("#" + treeRoot);
            ddl.empty();
            // First add an option to add a new node
            activeNode = ddl;
            $.each(result, function (index, element) {
                if (element.Level == 1) {
                    activeNode = ddl; // Add to the root
                }
                else {
                    activeNode = ddl.find("#C" + element.ParentID).eq(0); // search for the parent id and add this node under it
                }
                if (element.Type == "R") // Repository
                {
                    // Append the new node
                    activeNode.append('<li id="L' + element.accessID + '"><input type="checkbox" class="' + className + '" id="' + element.accessID + '" data-param="' + element.paramName + '" data-value="' + element.paramValue + '" data-id="' + element.ID + '" />' +
                                            '<a class="tree-toggle closed" data-toggle="branch" role="branch" data-id="' + element.ID + '" id="Node' + element.ID + '" href="javascript:">' +
                                                '<span class="label label-info">' + element.levelName + ' - ' + element.nodeName + '</span>' +
                                            '</a>' +
                                            '<ul id="C' + element.ID + '" class="branch">' +
                                                '<li id="Nil' + element.ID + '">No Activities configured</li>' +
                                            '</ul>' +
                                       '</li>');
                }
                else // Activity
                {
                    activeNode.children("#Nil" + element.ParentID).eq(0).remove(); // Remove status "No Activities configured
                    // Append the new node
                    activeNode.append('<li id="L' + element.accessID + '"><input type="checkbox" class="' + className + '" id="' + element.accessID + '" data-param="' + element.paramName + '" data-value="' + element.paramValue + '" data-id="' + element.ParentID + '" />' +
                                                '&nbsp;&nbsp;<span class="label label-success">' + element.levelName + ' - ' + element.nodeName + '</span>' +
                                       '</li>');
                }
            });
        }).fail(function () {
            alert("Process Repository: System Error. Pl. contact the administrator");
        });
    };

    function printStructureForMapping(url, treeRoot, className) {
        $.getJSON(url, function (result) {
            var ddl = $("#" + treeRoot);
            ddl.empty();
            // First add an option to add a new node
            activeNode = ddl;
            $.each(result, function (index, element) {
                if (element.Level == 1) {
                    activeNode = ddl; // Add to the root
                }
                else {
                    activeNode = ddl.find("#C" + element.ParentNodeID).eq(0); // search for the parent id and add this node under it
                }
                // Append the new node
                activeNode.append('<li id="L' + element.ID + '"><a href="javascript:" class="' + className + '" data-param="orgLevelID=' + element.ID + '" data-name="' + element.nodeName + '"><i class="icon-wrench"></i></a>' +
                                        '&nbsp;<a class="tree-toggle closed" data-toggle="branch" role="branch" href="javascript:" >' + element.nodeName + '</a>' +
                                        '&nbsp;<ul id="C' + element.ID + '" class="branch">' +
                                        '</ul>' +
                                    '</li>');
            });
        }).fail(function () {
            alert("Organisation Level Configuration: System Error. Pl. contact the administrator");
        });
    };

    $("body").delegate(".mapProcess", "click", function () {
        var changedNode = $(this);
        var clickValue = $(this).prop("checked");
        var repoID = $(this).data("id");
        var linkID = $(this).data("param");
        var linkValue = $(this).data("value");
        var mappingFor = $("#processContainer").data("param");

        if (mappingFor == "") {
            $.pnotify({
                title: 'Select a organisation node to configure',
                type: 'info'
            });
            changedNode.removeProp("checked");
            return;
        }

        if (clickValue) {
            // Call add action
            $.getJSON('/MapRepository/Add?repoID=' + repoID + (linkID == "repoID" ? "" : '&' + linkID + '=' + linkValue) + '&' + mappingFor, function (result) {
                $.pnotify({
                    title: 'Mapping Successful',
                    type: 'info'
                });
            }).fail(function (result) {
                $.pnotify({
                    title: 'This mapping cannot be done',
                    type: 'info'
                });
                changedNode.removeProp("checked");
            });
        }
        else {
            // Call remove action
            $.getJSON('/MapRepository/Remove?repoID=' + repoID + (linkID == "repoID" ? "" : '&' + linkID + '=' + linkValue) + '&' + mappingFor, function (result) {
                $.pnotify({
                    title: 'Mapping Removed',
                    type: 'info'
                });
            }).fail(function (result) {
                $.pnotify({
                    title: 'This mapping cannot be removed',
                    type: 'info'
                });
                changedNode.Prop("checked", "checked");
            });
        }
    });

    $("body").delegate(".showMapping", "click", function () {
        $("#selectedNode").html("Mapping <i class='icon-circle-arrow-right'> </i>" + $(this).data("name"));
        var dataParam = $(this).data("param");
        $("#processContainer").data("param",dataParam);

        $(".FocusedItem").removeClass("FocusedItem");
        $(this).siblings("a").eq(0).addClass("FocusedItem");

        $.getJSON('/MapRepository/getMapping?' + dataParam, function (result) {
            // Select all the relevant options
            $("#PConfiguration").find("input:checked").each(function (index, element) {
                $(element).prop("checked",false);  // Reset all checkboxes before proceeding
            });
            $.each(result, function (index, element) {
                $("#" + element.accessID).prop("checked", "checked");
            });
        }).fail(function () {
            $.pnotify({
                title: 'Error in configuration setting',
                type: 'info'
            });
            $(".FocusedItem").removeClass("FocusedItem");
            $("#selectedNode").html('Click <i class="icon-wrench"></i> icon to configure processes.');
        });
    });

    function addSection(activeNode, index, sequence, title, detail, procID, grpID, type) {
        var functionName = (type == "proc" ? "Procedure" : "Template");
        $(activeNode).children().last().before('<li class="sortable-item MainSection" id="Section' + index + '">' +
                                '<div class="row">' +
                                    '<div class="col-lg-12 panel colored">' +
                                        '<div class="panel-heading  blue-bg">' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section.Index" value="' + index + '" />' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section[' + index + '].ID" value="' + index + '" />' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section[' + index + '].SequenceNo" class="sequence" value="' + sequence + '" />' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section[' + index + '].tbl_Process_' + functionName + 'ID" value="' + procID + '" />' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section[' + index + '].tbl_Process_' + (type == "proc" ? "Proc" : "Tmpl") + '_GroupID" class="Parent" data-lnode="Section' + index + '" value="' + ((grpID == 0) ? "" : grpID) + '" />' +
                                            '<input type="hidden" name="tbl_process_' + type + '_section[' + index + '].toDelete" value="false" />' +
                                                '<h1 class="panel-title"><span id="sectionTitle' + index + 'Div">Provide Title</span></h1>' +
                                                '<ul class="pull-right toolbar">' +
                                                    '<li><a href="#" class="icon-button minimise"><i class="icon-">&#xf0aa;</i></a></li>' +
                                                    '<li><a href="#" class="icon-button confirmAction" data-messagetitle="Confirm Deletion" data-message="Do you want to delete this section ?" data-class="deleteSection" data-classparams="data-id=' + index + '"><i class="icon-">&#xf057;</i></a></li>' +
                                                '</ul>' +
                                         '</div>' +
                                         '<div class="panel-body">' +
                                            '<div class="row">' +
                                                '<div class="form-group col-lg-12">' +
                                                    '<label>Title</label>' +
                                                    '<div class="controls">' +
                                                        '<input type="text" class="form-control sectionTitle" id="sectionTitle' + index + '" name="tbl_process_' + type + '_section[' + index + '].Title" value="' + title + '"/>' +
                                                        '<span class="field-validation-valid" data-valmsg-replace="true" data-val-msg-for="tbl_process_' + type + '_section[' + index + '].Title"></span>' +
                                                    '</div>' +
                                                '</div>' +
                                            '</div>' +
                                            '<div class="row" id="DetailRow' + index + '">' +
                                                 '<textarea class="col-lg-12 sectionDetail" rows="5" id="Detail' + index + '" name="tbl_process_' + type + '_section[' + index + '].Detail">' + detail + '</textarea>' +
                                                '<span class="field-validation-valid" data-valmsg-replace="true" data-val-msg-for="tbl_process_' + type + '_section[' + index + '].Detail"></span>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                             '</li>');
        // Invoke WYSIWYG editor
        var editor = CKEDITOR.replace('Detail' + index);
        editor.on("blur", function () {
            $('#Detail' + index).text(editor.getData());
        });
        // Go to the new section
        $('html, body').animate({
            scrollTop: $("#" + "Section" + index).offset().top
        }, 2000);
        $("#sectionTitle" + index).focus();
    };

    function configureCkEditor(controlID) {
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            var editor = CKEDITOR.replace(this.id);
            editor.setData($(this).text());
            editor.on("blur", function () {
                textDetail.text(editor.getData());
            });
        });
    }

    function configureInlineCkEditor(controlID) {
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            CKEDITOR.inline(textDetail.attr("id"), {
                uiColor: '#14B8C4', toolbar: [
                    ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                    ['FontSize', 'TextColor', 'BGColor']]
            });
        });
    }

    $("body").delegate(".newSection", "click", function () {
        var id = $("#sections").data("nextsectionid");
        var type = $(this).data("type");
        var grp = $(this).data("parent");
        var node = "#DefaultDocNode";

        if (grp != "0") {
            node = "#C" + grp;
        }
        // Add a new section on client )
        addSection(node, id, "", "", "", $(this).data("proc"), (grp == 0) ? null : grp, type); 
        // update the next id and next sequence
        $("#sections").data("nextsectionid", id + 1);
    });

    var processHTMLData = function (className) {
        var textValue = "";
        $(className).each(function () {      // Set sequences
            $(this).val($(this).val().replace(new RegExp("&nbsp;", 'g'), "&#160;"));
            textValue = $(this).val();
            $.each(invalidCharacters,function (index, element) {
                if (textValue.indexOf(element) >= 0) textValue = textValue.replace(new RegExp(element, 'g'), "&lt;Invalid Character&gt;");
            });
            $(this).val(textValue);
            textValue = "";
        });
    };

    var rearrangeSequences = function (nodeName) {
        var sequence = 1;
        $(nodeName).each(function () {      // Set sequences
            $(this).val(sequence);
            sequence = sequence + 1;
        });
    }

    var rearrangeGroups = function (nodeName) {
        $(nodeName).each(function () {      // Set groups
            $(this).val($("#" + $(this).data("lnode")).parent("ul").eq(0).data("groupid"));   // Take the li node and search for container ul. Pick data-groupid
        });
    }

    var rearrangeGroupLevels = function (nodeName) {
        $(nodeName).each(function () {      // Set groups
            $(this).val($("#" + $(this).data("lnode")).parents("ul").length);   // Take the li node and search for container ul. Pick data-groupid
        });
    }

    $("body").delegate(".saveStructure", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");
        var dialog = $(this).data("dialog");

        if (mode == "delete") {
            $("#" + dialog).html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + processing("Processing. Please wait ") +
                         '</div>' +
                     '</div>');
        }
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                switch (mode) {
                    case "add":
                        $.pnotify({
                            title: 'New Structure Created',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        break;
                    case "edit":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).replaceWith(data);
                        break;
                    case "editNode":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).html(data);
                        break;
                    case "delete":
                        $.pnotify({
                            title: 'Structure Deleted',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        $("#" + dialog).modal("hide");
                        break;
                    default:
                        break;
                }
                $("#openDialogBox").modal("hide");
            },
            error: function (data) {
                if (mode == "delete") {
                    // Show the error message
                    $.pnotify({
                        title: 'Delete Failed',
                        text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                        type: 'error'
                    });
                    $("#" + dialog).modal("hide");
                }
                else $("#openDialogBox").html(data.responseText);
            }
        });
    });

    $("body").delegate(".saveConfiguration", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");
        $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                '<div class="modal-content">' +
                        '<div class="modal-body"> ' + processing("Processing. Please wait ") +
                 '</div>' +
             '</div>');
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                switch (mode) {
                    case "add":
                        $.pnotify({
                            title: 'New Repository Group Created',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        break;
                    case "edit":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).replaceWith(data);
                        break;
                    case "delete":
                        $.pnotify({
                            title: 'Repository Folder and its contents deleted',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        break;
                    default:
                        break;
                }
                $("#openDialogBox").modal("hide");
            },
            error: function (data) {
                if (mode == "delete") {
                    // Show the error message
                    $.pnotify({
                        title: 'Delete Failed',
                        text: "This record folder cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                        type: 'error'
                    });
                }
                else $("#openDialogBox").html(data.responseText);
            }
        });
    });


    $("body").delegate(".confirmAction", "click", function () {
        // Generic confirmation dialog
        msgTitle = $(this).data("messagetitle");
        msg = $(this).data("message");
        className = $(this).data("class");
        classParams = $(this).data("classparams");
        actionSource = $(this).data("source");

        var dialog = $("#openDialogBox");
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">' + msgTitle + '</h4>' +
                            '</div>' +
                            '<form method="post" action="' + actionSource + '" id="deleteRecord">' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">' + msg + '</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">No</button>' +
                                        '<button type="button" class="btn blue-bg ' + className + '" data-dismiss="modal"' + ' ' + classParams + ' >Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".deleteDialog", "click", function () {
        var dialogName = $(this).data("dialogname");
        var dataid = $(this).data("id");
        var actionSource = $(this).data("source");

        var dialog = $("#" + dialogName);
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Delete</h4>' +
                            '</div>' +
                            '<form method="post" action="' + actionSource + '" id="deleteRecord">' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<input type="hidden" id="id" value="' + dataid + '"/>' +
                                        '<p class="control-label col-md-12 text-center">Do you want to delete this entry ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="' + dialogName + '">No</button>' +
                                        '<button type="button" class="btn blue-bg saveStructure" data-mode="delete" data-id = "' + dataid + '" data-form="deleteRecord" data-dialog="' + dialogName + '">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $(document).on("hidden.bs.modal", function (e) {
        if (e.target.id == "openDialogBox") {
            $(e.target).removeData("bs.modal").find(".modal-body").empty();
            // Remove all configured events
        }
    });

    $(document).on("shown.bs.modal", function (e) {     // Place focus on the first input element
        $(e.target).find("input:visible:first").focus();
    });


    $(document).on("focusout", ".sectionTitle", function () {
        if ($(this).val().trim() == "") $("#" + $(this).prop("id") + "Div").text("Provide Title");
        else 
            $("#" + $(this).prop("id") + "Div").text($(this).val());
    });

    $(document).on("click", "#AddNewSection", function () {
        addNewSectionDetails($(this), "append", this);
    });


    var addNewSectionDetails = function (refNode, addMode, addIn) {   // *********** Review and remove this function
        var sectionID = parseInt(refNode.data("last"));
        var oldSectionID = "#Section" + sectionID;

        if ($(oldSectionID).length) {
            newSection = $(oldSectionID).clone();
            newSection.prop("id", "Section" + (sectionID + 1));
            //	            newSection.find("#sectionSequence" + sectionID).eq(0).text("" + (sectionID + 1));
            newSection.find("#sectionSequence" + sectionID).eq(0).prop("id", "sectionSequence" + (sectionID + 1));
            newSection.find("#sectionSequence" + sectionID + "No").eq(0).text(sectionID + 1);
            newSection.find("#sectionSequence" + sectionID + "No").eq(0).prop("id", "sectionSequence" + (sectionID + 1) + "No");
            newSection.find("#sectionTitle" + sectionID + "Div").eq(0).prop("id", "sectionTitle" + (sectionID + 1) + "Div");
            newSection.find("#sectionTitle" + sectionID).eq(0).prop("id", "sectionTitle" + (sectionID + 1));
            newSection.find("#Details" + sectionID).eq(0).prop("id", "Details" + (sectionID + 1));
            newSection.find("#fileName" + sectionID).eq(0).prop("id", "fileName" + (sectionID + 1));
            newSection.find("#uploadFile" + sectionID).eq(0).prop("id", "uploadFile" + (sectionID + 1));
            newSection.find("#DetailRow" + sectionID).eq(0).replaceWith('<div class="row" id="DetailRow' + (sectionID + 1) + '"></div>');
            refNode.data("last", (sectionID + 1));
            // Replace all occurances of sectionID in textarea
            /*     $(newSection).find('[id*="Details' + sectionID + '"]').each(function () {
                     // Update the 'rules[0]' part of the name attribute to contain the latest count 
                     alert($(this).prop("id"));
                     $(this).prop('id', $(this).prop('id').replace('Details\[$sectionID\]', 'Details\[$(sectionID + 1)\]'));
                     //$(this).prop('id', $(this).prop('id').replace('Details1', 'Details2'));
                     alert("After change: " + $(this).prop("id"));
                 });
                 */
        }
        else {
            refNode.data("last", (sectionID + 1));
        }

        if (addMode == "append") {
            if ($(oldSectionID).length) {
                $(oldSectionID).after(newSection);
            }
            else {
                // Append in the default document node
                $("#DefaultDocNode").append(newSection);
            }
        }
        else {
            // Create a new section with reference to a particular node
            addIn.append(newSection);
        }

        // Clear previous events attached to min-max and close buttons
        $('.mini-max').unbind();
        $('.close-box').unbind();

        // Bind new events 
        /*minimize mazimize*/
        $('.mini-max').click(function () {
            $(this).parent().parent().parent().parent().find('.panel-body').slideToggle('fast');
            return false;
        });

        /*close*/
        $('.close-box').click(function () {
            $(this).parent().parent().parent().parent().parent().parent().fadeTo(400, 0, function () { $(this).hide(400, function () { $(this).remove() }); });
            return false;
        });
        // Go to the new section
        $('html, body').animate({
            scrollTop: $("#" + "Section" + (sectionID + 1)).offset().top
        }, 2000);

        // Clear previous events attached to ckeditor

        //$('.ckeditor').unbind();
        CKEDITOR.appendTo("DetailRow" + (sectionID + 1),
            null,
            ""
        );

        // Activate ckeditor editor
        //$('#Details' + (sectionID + 1)).ckeditor();
        //$.getScript("assets/js/plugins/ckeditor/ckeditor.js");
        //$('#Details' + (sectionID + 1)).ckeditor();

        // Keep a copy of the section. In case all sections are exhausted.
        newSection = newSection.clone();
        sectionID = parseInt(refNode.data("last"));
        newSection.prop("id", "Section" + (sectionID + 1));
        newSection.find("#sectionSequence" + sectionID).eq(0).prop("id", "sectionSequence" + (sectionID + 1));
        newSection.find("#sectionSequence" + sectionID + "No").eq(0).text(sectionID + 1);
        newSection.find("#sectionSequence" + sectionID + "No").eq(0).prop("id", "sectionSequence" + (sectionID + 1) + "No");
        newSection.find("#sectionTitle" + sectionID + "Div").eq(0).prop("id", "sectionTitle" + (sectionID + 1) + "Div");
        newSection.find("#sectionTitle" + sectionID).eq(0).prop("id", "sectionTitle" + (sectionID + 1));
        newSection.find("#Details" + sectionID).eq(0).prop("id", "Details" + (sectionID + 1));
        newSection.find("#fileName" + sectionID).eq(0).prop("id", "fileName" + (sectionID + 1));
        newSection.find("#uploadFile" + sectionID).eq(0).prop("id", "uploadFile" + (sectionID + 1));
        newSection.find("#DetailRow" + sectionID).eq(0).replaceWith('<div class="row" id="DetailRow' + (sectionID + 1) + '"></div>');
    };


    $(document).on("click", ".addTreeNode", function () {  // ******** Review and remove
        var myNodeId = $(this).data('id');
        $("#NewNodeID").val(myNodeId);
        $("#newGrpName").val("");
        // $("#newGrpName").focus();
        //$('#responsive').modal('show');
    });

    $('#responsive').on('shown.bs.modal', function () {
        $("#newGrpName").focus();
    })

    $('#editOptions').on('shown.bs.modal', function () {
        $("#editGrpName").focus();
    })

    $(document).on("click", ".editTreeNode", function () {    // ************ Review and remove 
        var myNodeId = $(this).data('id');
        $("#editNodeID").val(myNodeId);
        $("#editGrpName").val($("#" + myNodeId).text());
    });


    $(document).on("click", "#addNewGroup", function () {   // ************ Review and remove 
        var myNodeName = $("#NewNodeID").val();
        var myNodeId = "#" + myNodeName;
        var myNxtNode = myNodeName.substr(0, myNodeName.length - 2) + "R" + (parseInt(myNodeName.substr(myNodeName.length - 1, 1)) + 1);
        $(myNodeId).removeClass("addTreeNode");
        $(myNodeId).attr("data-childid", myNodeName + "R1");
        //$(myNodeId).addClass("editTreeNode");
        //$(myNodeId).prop("href", "#editOptions");
        $(myNodeId).attr("data-toggle", "branch");
        $(myNodeId).attr("role", "branch");
        $(myNodeId).prop("href", "#");
        $(myNodeId).text($("#newGrpName").val());
        $(myNodeId).before('<a href="#editOptions" data-toggle="modal" class="editTreeNode" data-id="' + myNodeName + '"><i class="icon-edit"></i></a>');
        $(myNodeId).after('<ul id="C' + myNodeName + '" class="sortable-list branch in ui-sortable"><li>Drag Sections in this group</li></ul>');

        //$('#procDef .sortable-list').unbind();
        $('#procDef .sortable-list').sortable({
            connectWith: '#procDef .sortable-list'
        });

        // Option to add new root
        $("#L" + myNodeName).after('<li id="L' + myNxtNode + '"><a class="tree-toggle addTreeNode" data-toggle="modal" data-id="' + myNxtNode + '" id="' + myNxtNode + '" href="#responsive">Add Group</a></li>');

        // Add a section in the new group by default
        addNewSectionDetails($("#AddNewSection"), "newSection", $("#C" + myNodeName));
    });

    $(document).on("click", "#deleteRole", function () {
        $('#' + $("#deleteNodeID").val()).remove();
    });

    $(document).on("click", "#saveGroup", function () {
        var myNodeName = $("#editNodeID").val();
        $("#" + myNodeName).text($("#editGrpName").val());
    });


    $(document).on("click", "#subGroup", function () {      // ***** Review and remove
        var myNodeName = $("#editNodeID").val();
        var myNodeId = "#" + myNodeName;
        var childNodeName = $(myNodeId).data("childid");
        var myNxtChildNode = childNodeName.substr(0, childNodeName.length - 2) + "R" + (parseInt(childNodeName.substr(childNodeName.length - 1, 1)) + 1);

        // if this is the first child node, then create a ul tag
        $(myNodeId).data("childid", myNxtChildNode);
        $("#C" + myNodeName).append('<li id="L' + childNodeName + '"><a href="#editOptions" data-toggle="modal" class="editTreeNode" data-id="' + childNodeName + '"><i class="icon-edit"></i></a><a class="tree-toggle" data-toggle="branch" role="branch" data-id = "' + childNodeName + '" id="' + childNodeName + '" data-childID = "' + childNodeName + 'R1" href="#">' + $("#editGrpName").val() + '</a></li>');

        //$(myNodeId).attr("data-toggle", "branch");
        //$(myNodeId).attr("role", "branch");

        $("#" + childNodeName).after('<ul id="C' + childNodeName + '" class="sortable-list branch in ui-sortable"><li>Drag Sections in this group</li></ul>');

        //$('#procDef .sortable-list').unbind();
        $('#procDef .sortable-list').sortable({
            connectWith: '#procDef .sortable-list'
        });

        // Add a section in the new group by default
        addNewSectionDetails($("#AddNewSection"), "newSection", $("#C" + childNodeName));
        // Add a section by default
    });

    $("body").delegate(".saveDefaultWithReview", "click", function () {
        // Check if review comments are provided
        if ($("#reviewComments").val() == "") {
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Pl. enter some review comments.</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>');
            $("#openDialogBox").modal("show");
            return;
        }
        else {
            $("#formReviewComments").val($("#reviewComments").val());
        }
        $("#revCommentsDialog").modal("hide");
        // The parents and group levels may change due to manual drag drop actions of the user
        rearrangeSequences(".sequence");    // Re-arrange sequences
        rearrangeGroups(".Parent");         // Re-arrange Parent of groups
        rearrangeGroupLevels(".level");     // Re-arrange group Levels
        processHTMLData(".sectionDetail");
        updateCustomDetails($(this).data("form"));  // Any other custom updates based on the calling program

        var form = $("#" + $(this).data("form"));
        var updateId = $(this).data("id");
        var successMessage = $(this).data("message");
        $.ajax({
            async: false,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                $.pnotify({
                    title: successMessage,
                    type: 'info'
                });
                //var result = $('<div/>').html(data.responseText).contents();$.parseHTML(data);
                $("#" + updateId).html(data);
            },
            error: function (data) {
                $.pnotify({
                    title: '<strong class="label label-danger">Operation Terminated</strong> <hr/> Illegal characters found in the input. This may happen due to copy/paste of information. <br/>',
                    type: 'error'
                });
                //$("#openDialogBox").html(data.responseText);
            }
        });
    });

    function updateCustomDetails(formname) {
        // Custom updates based on calling program
        switch (formname) {
            case "newChecklist":
                $(".EditEditor").each(function () {
                    // Transfer data from div to hidden inputs
                    $("#" + $(this).attr("id") + "input").val($(this).html());
                });
                break;
            default:
                // Do nothing
                break;
        }
    }

    $("body").delegate(".deleteOnServer", "click", function () {
        var updateID = $("#" + $(this).data("updateid"));
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: $(this).data("source"),
            success: function (data) {
                $.pnotify({
                    title: 'Record Deleted',
                    type: 'info'
                });
                updateID.remove();
            },
            error: function (data) {
                $.pnotify({
                    title: 'Delete Failed',
                    text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                    type: 'error'
                });
            }
        });
    });

    $("body").on("click", ".deleteSection", function () {
        var scroll = $(window).scrollTop();  // Save the window position before this operation was invoked
        idToDelete = $(this).data("id");
        $("#Section" + idToDelete).remove();  // Hide this section
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    });

    $("body").on("click", ".removeItem", function () {
        var scroll = $(window).scrollTop();  // Save the window position before this operation was invoked
        idToDelete = $(this).data("id");
        $("#Item" + idToDelete).remove();  // Hide this section
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    });

    $("body").on("click", ".minimise", function () {
        $(this).parent().parent().parent().next().toggle().slow();
        // scroll to relevent section
        $('html, body').animate({
            scrollTop: $($(this).parent().parent().parent()).offset().top
        }, 2000);
        $("#sectionTitle" + index).focus();

    });

    function updateSectionCounts(sectionid, groupid) {ew
        $("#sections").data("nextsectionid").val(sectionid);  // The next ID that can be assigned to a new section. The ID in this case is not auto-generated
        $("#sections").data("nextgroupid").val(groupid);  // The next ID that can be assigned to a new section. The ID in this case is not auto-generated
    }

    $("body").delegate(".addGroup", "click", function () {      // Function called when the user selects to add new group
        var parentGrpID = $(this).data("parent");
        $("#NewNodeID").val($(this).attr("id")); // update the hidden variable in the add group dialog box
        if (parentGrpID == 0) { $("#ParentNodeID").val(null); } else { $("#ParentNodeID").val(parentGrpID); }  // Parent group id
        $("#Nodelevel").val(1); // Node level
        $("#NodeProc").val($(this).data("proc"));  // Procedure id
        $("#newGrpName").val("")    // Reset the value, erase old input if any
        $("#addGroupDialog").modal("show");
    });

    $("body").delegate(".editGroup", "click", function () {      // Function called when the user selects to add new group
        var parentGrpID = $(this).data("parent");
        $("#editNodeID").val($(this).data("id")); // update the hidden variable in the add group dialog box
        if (parentGrpID == 0) { $("#ParentNodeID").val(null); } else { $("#ParentNodeID").val(parentGrpID); }  // Parent group id
        $("#editlevel").val(1); // Node level
        $("#editNodeProc").val($(this).data("proc"));  // Procedure id
        $("#editGrpName").val($("#" + $(this).data("id")).text())    // Reset the value, erase old input if any
        $("#editGroupDialog").modal("show");
    });

    $("body").delegate(".saveGroup", "click", function () {      // Function called when the user edits the group name and clicks save
        var node = $("#editNodeID").val();
        $("#" + node).text($("#editGrpName").val());
        $("#name" + node).val($("#editGrpName").val());
    });

    $("body").delegate(".deleteGroup", "click", function () {      // Function called when the user edits the group and clicks delete. The user is prompted for confirmation 
        var node = $("#editNodeID").val();
        $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                    '<div class="modal-content">' +
                        '<div class="modal-header">' +
                            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                            '<h4 class="modal-title" id="dialogTitle">Confirm Delete ...</h4>' +
                        '</div>' +
                            '<div class="modal-body"> ' +
                                '<div class="row col-md-12">' +
                                    '<p class="control-label col-md-12 text-center">Do you want to delete this group ? All sections within this group will also be deleted.</p>' +
                                '</div>' +
                            '</div>' +
                            '<div class="modal-footer">' +
                                '<div class="form-group">' +
                                    '<button type="button" class="btn gray-bg" data-dismiss="modal">No</button>' +
                                    '<button type="button" class="btn blue-bg confirmedDeleteGroup" data-dismiss="modal" data-id="' + node + '">Yes</button>' +
                                '</div>' +
                            '</div>' +
                    '</div>' +
                '</div>');
        $("#openDialogBox").modal("show");
    });

    $("body").delegate(".confirmedDeleteGroup", "click", function () {      // Executes delete after the user confirms the action
        var scroll = $(window).scrollTop();  // Save the window position before this operation was invoked
        $("#L" + $(this).data("id")).remove();
        $("#editGroupDialog").modal("hide");
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    });

    $("body").delegate(".addNewGroup", "click", function () {      // Function called when the user provides group name and clicks save
        // Check whether group name is provided
        if ($("#newGrpName").val() == "") {
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Pl. enter the group name.</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>');
            $("#openDialogBox").modal("show");
            return;
        }
        $("#addGroupDialog").modal("hide");
        // Create this group
        var mode = $(this).data("mode");
        var grpName = $("#newGrpName").val();
        var level = $("#Nodelevel").val();
        var procID = $("#NodeProc").val();
        var node = $("#NewNodeID").val();
        var type = $("#type").val();
        var newGrpId;

        if (type == "chklst") {
            newGrpId = $("#items").data("nextgroupid");
        }
        else {
            newGrpId = $("#sections").data("nextgroupid");
        }
   
        var newGroup = '<li class="branch-in" id="L' + newGrpId + '">' +
                            '<input type="hidden" name="tbl_process_' + type + '_group.Index" value="' + newGrpId + '" />' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].ID" value="' + newGrpId + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].Name" id= "name' + newGrpId + '" value="' + grpName + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].SequenceNo"  class="sequence" value=""/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].ParentGroup" class="Parent" data-lnode="L' + newGrpId + '" value="' + $("#ParentNodeID").val() + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].IsParent" value="false"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].Level" class="level" data-lnode="L' + newGrpId + '" value="1"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].tbl_Process_' + (type == "proc" ? "Procedure" : (type == "chklst" ? "Checklist" : "Template")) + 'ID" value="' + procID + '"/>' +
                            '<a class="editGroup" data-id="' + newGrpId + '" data-toggle="modal" href="Javascript:;"><i class="icon-edit"></i></a> ' +
                            '<a href="Javascript:;" role="branch in" class="tree-toggle" data-toggle="branch" role="branch" data-id="' + newGrpId + '" id="' + newGrpId + '">' + grpName + '</a>' +
                            '<ul class="' + (type == "chklst" ? '' : 'sortable-list ') +  'branch in" id="C' + newGrpId + '" data-groupid="' + newGrpId + '">';
        if (type == "chklst") {
            // Make provision to add item
            newGroup = newGroup + '<li>' +
                                        '<div class="row col-lg-12">' +
                                            '<div class="col-lg-1 NoEdit CheckItem" data-width="2px"><a href="javascript:;" class="newItem" data-parent="' + newGrpId + '" data-proc="' + procID + '"  data-type="chklst"><i class="icon-">&#xf055</i></a></div>' +
                                        '</div>' +
                                    '</li>' +
                                '</ul>' +
                            '</li>';
        }
        else {
            // Add section
            newGroup = newGroup + '<li><a href="Javascript:;" class="newSection" data-parent="' + newGrpId + '" data-proc="' + procID + '" data-type="' + type + '">Add Section</a></li>' +
                            '</ul>' +
                        '</li>';
        }
        if (mode == "root")   // Add as a new group under the root document
        {
            $("#L" + node).before(newGroup);
        }
        else  // Add as a sub node
        {
            $("#C" + node).children("li").last().before(newGroup);
        }
        $("#sections").data("nextgroupid", newGrpId + 1)        // Update the new group id
    
        // For allowing drag & drop feature
        //$('#procDef .sortable-list').unbind();
        $('#procDef .sortable-list').sortable({
            connectWith: '#procDef .sortable-list'
        });
    });

    $("body").delegate(".addSubGroup", "click", function () {      // Function called when the user provides group name and clicks save
        // Check whether group name is provided
        if ($("#editGrpName").val() == "") {
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Pl. enter the group name.</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>');
            $("#openDialogBox").modal("show");
            return;
        }
        $("#editGroupDialog").modal("hide");
        // Create this group
        var grpName = $("#editGrpName").val();
        var level = "1";
        var procID = "0";
        var node = $("#editNodeID").val();
        var type = $("#type").val();
        var newGrpId = $("#sections").data("nextgroupid");
        var newGroup = '<li class="branch-in" id="L' + newGrpId + '">' +
                            '<input type="hidden" name="tbl_process_' + type + '_group.Index" value="' + newGrpId + '" />' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].ID" value="' + newGrpId + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].Name" id= "name' + newGrpId + '" value="' + grpName + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].SequenceNo"  class="sequence" value=""/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].ParentGroup" class="Parent" data-lnode="L' + newGrpId + '" value="' + $("#ParentNodeID").val() + '"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].IsParent" value="false"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].Level" class="level" data-lnode="L' + newGrpId + '"  value="1"/>' +
                            '<input type="hidden" name="tbl_process_' + type + '_group[' + newGrpId + '].tbl_Process_' + (type == "proc" ? "Procedure" : "Template") + 'ID" value="' + procID + '"/>' +
                            '<a class="editGroup" data-id="' + newGrpId + '" data-toggle="modal" href="Javascript:;"><i class="icon-edit"></i></a> ' +
                            '<a href="Javascript:;" role="branch in" class="tree-toggle" data-toggle="branch" role="branch" data-id="' + newGrpId + '" id="' + newGrpId + '">' + grpName + '</a>' +
                            '<ul class="sortable-list branch in" id="C' + newGrpId + '" data-groupid="' + newGrpId + '">';
        if (type == "chklst") {
            // Make provision to add item
            newGroup = newGroup + '<li>' +
                                        '<div class="row col-lg-12">' +
                                            '<div class="col-lg-1 NoEdit CheckItem" data-width="2px"><a href="javascript:;" class="newItem" data-parent="' + newGrpId + '" data-proc="' + procID + '"  data-type="chklst"><i class="icon-">&#xf055</i></a></div>' +
                                        '</div>' +
                                    '</li>' +
                                '</ul>' +
                            '</li>';
        }
        else {
            // Add section
            newGroup = newGroup + '<li><a href="Javascript:;" class="newSection" data-parent="' + newGrpId + '" data-proc="' + procID + '" data-type="' + type + '">Add Section</a></li>' +
                            '</ul>' +
                        '</li>';
        }

        $("#C" + node).children("li").last().before(newGroup);   // Add this node as a child node
        $("#sections").data("nextgroupid", newGrpId + 1)        // Update the new group id

        // For allowing drag & drop feature
        //$('#procDef .sortable-list').unbind();
        $('#procDef .sortable-list').sortable({
            connectWith: '#procDef .sortable-list'
        });
    });

    $("body").delegate(".preview", "click", function () {
        var dialog = $("#openDialogBox");
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                       '<div class="modal-content">' +
                               '<div class="modal-body"> ' + processing("Processing. Please wait ") +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
        rearrangeSequences(".sequence");    // Re-arrange sequences
        rearrangeGroups(".Parent");         // Re-arrange Parent of groups
        rearrangeGroupLevels(".level");     // Re-arrange group Levels
        processHTMLData(".sectionDetail");
        var form = $("#" + $(this).data("form"));
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: $(this).data("url"),
            data: form.serialize(),
            success: function (data) {
                $("#openDialogBox").html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty();
                $("#openDialogBox").html(data.responseText);
            }
        });
    });

    $("body").delegate(".newItem", "click", function () {
        var id = $("#items").data("nextitemid");
        var type = $(this).data("type");
        var grp = $(this).data("parent");
        var node = "#DefaultDocNode";

        if (grp != "0") {
            node = "#C" + grp;
        }
        // Add a new section on client side
        addItem(node, id, "", $(this).data("proc"), (grp == 0) ? null : grp);
        // update the next id and next sequence
        $("#items").data("nextitemid", id + 1);
    });

    var addItem = function (activenode, index, sequence, itemID, grpID) {  // ************** Parked
        var checkItem = '<li class="sortable-item itemGroup" id="Item' + index + '">' +
                                        '<input type="hidden" name="tbl_process_chklst_item.Index" value="' + index + '" />' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].ID" value="' + index + '" />' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].SequenceNo" class="sequence" value="' + sequence + '" />' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].tbl_Process_ChecklistID" value="' + itemID + '" />' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].tbl_Process_Chklst_GroupID" class="Parent" data-lnode="Item' + index + '" value="' + grpID + '" />' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].ItemDescription" value="Enter Checkpoint Details" id="Check' + index + '_CPinput"/>' +
                                        '<input type="hidden" name="tbl_process_chklst_item[' + index + '].Remarks" value="Enter Remarks" id="Check' + index + '_Remarksinput"/>' +
                                        '<div class="row col-lg-12" class="EditRow">' +
                                            '<div class="col-lg-1 NoEdit CheckItem" data-width="2px"><a href="#" class="icon-button confirmAction" data-messagetitle="Confirm Deletion" data-message="Do you want to delete this check item ?" data-class="removeItem" data-classparams="data-id=' + index + '" data-id="Check' + index + '"><i class="icon-">&#xf05c</i></a></div>' +
                                            '<div class="col-lg-5 EditEditor CheckItem" data-width="20px" id="Check' + index + '_CP" contenteditable="true">Enter Checkpoint Details</div>' +
                                            '<div class="col-lg-3 NoEdit CheckItem" data-width="20px"><select data-placeholder="Choose Results" class="col-lg-12 chzn-nopadd resultOptions inputValue" data-navkey="EditEditor" multiple id="Check' + index + '_Result"  name="tbl_process_chklst_item[' + index + '].ChklstOptionList"><option value="1" selected="selected">Yes</option><option value="2" selected="selected">No</option><option value="3" selected="selected">NA</option></select></div>' +
                                            '<div class="col-lg-3 EditEditor CheckItem" data-width="20px" id="Check' + index + '_Remarks" data-navkey="NoEdit"  contenteditable="true">Enter Remarks</div>' +
                                        '</div>' +
                                '</li>';
        $(activenode).children().last().before(checkItem);
        // Invoke WYSIWYG editor
        CKEDITOR.inline('Check' + index + '_CP',{uiColor: '#14B8C4',toolbar: [
                ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                ['FontSize', 'TextColor', 'BGColor']    ]});
        CKEDITOR.inline('Check' + index + '_Remarks',{uiColor: '#14B8C4',toolbar: [
                ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                ['FontSize', 'TextColor', 'BGColor']    ]});
        $('#Check' + index + '_Result').chosen({ allow_single_deselect: false });
        /*
        CKEDITOR.inline("Check" + index + "_CP");
        CKEDITOR.inline("Check" + index + "_Remarks");
        
        // Go to the new check
        $('html, body').animate({
            scrollTop: $("#" + "Item" + index).offset().top
        }, 2000);
    
        $("#Check' + index + '_CP").focus();  */
    };

    $("body").delegate("#searchText", "keypress", function (e) {
        if (e.which == 13) {
            searchProcessRep();
        }
    });

    $("body").delegate("#searchbtn", "click", function () {
        searchProcessRep();
    });

    var searchProcessRep = function () {
        var nodesFound = false;

        if ($("#searchText").val().trim() == "") {
            // Show all nodes
            $("ul.tree").find("li").each(function () {
                if ($(this).hasClass("hide")) $(this).removeClass("hide");
            });
            $("#filterMessage").text("");
        }
        else {
            // To begin with close all tree nodes
            $("ul.tree").find("li").each(function () {
                if (!($(this).children(".tree-toggle").eq(0).hasClass("addTreeNode"))) {
                    if (!($(this).hasClass("hide"))) $(this).addClass("hide");
                    if (!($(this).children(".tree-toggle").eq(0).hasClass("closed"))) $(this).children(".tree-toggle").eq(0).addClass("closed");
                }
            });

            // Now search for the nodes that have the search text and show only those
            $("li:MyCaseInsensitiveContains('" + $("#searchText").val() + "')").each(function () {
                nodesFound = true;
                if ($(this).hasClass("hide")) $(this).removeClass("hide"); // Show this line item
                if (!($(this).parent().hasClass("in"))) $(this).parent().addClass("in");  // Open this branch, so that this item is visible
                if ($(this).parent().parent().children(".tree-toggle").eq(0).hasClass("closed")) $(this).parent().parent().children(".tree-toggle").eq(0).removeClass("closed") // Open the parent anchor
            });

            // Make all such nodes where the search ended, all items accessible
            $("li:MyCaseInsensitiveContains('" + $("#searchText").val() + "')").each(function () {
                $(this).find("a.closed").each(function () {
                    $(this).parent().find("li").each(function () {
                        if ($(this).hasClass("hide")) $(this).removeClass("hide"); // Show this line item
                    });
                });
            });
            if (!nodesFound) {
                alert("No Matching information found. Pl. revise your search criteria");
                $("#filterMessage").html("Filter applied: <b>No Match found.</b> Clear filter to view repository.");
            }
            else $("#filterMessage").text("Filter applied: Clear filter to view complete repository.");

        }
    };


    $.extend($.expr[":"], {
        "MyCaseInsensitiveContains": function (elem, i, match, array) {
            return (elem.textContent || elem.innerText || "").toLowerCase().indexOf((match[3] || "").toLowerCase()) >= 0;
        }
    });

    $(document).on("dblclick", ".repositoryDetails", function () {
        showRepository(this);
    });

    var showRepository = function (refNode) {
        myNode = $(refNode);
        baseNode = myNode.data("basenode");

        // Set the title
        $("#configTitle").text($("#Lvl" + baseNode).text() + " - " + $("#LvlDesc" + baseNode).text());
        $("#configGTlink").data("key", baseNode);
        // Show the details ** General Tasks
        viewRepDetails(baseNode, "GT");
        // Show the details ** Procedures
        viewRepDetails(baseNode, "Proc");
        // Show the details ** Checklists
        viewRepDetails(baseNode, "ChkLst");
        // Show the details ** Processes
        viewRepDetails(baseNode, "Process");
    };

    $("body").delegate(".loadRepo", "click", function () {
        var id = $(this).data("id");
        var node = $("#repo" + id); // Node to be loaded
        var clickedNode = $(this);

        if (id == null || id == "") { return; } // Insufficient input
        $.getJSON("/PConfiguration/getRepository/" + id, function (result) {
            var groupName = "";
            var recGroupName = "";
            var htmlString = "";

            $.each(result, function (index, element) {
                recGroupName = element.repoName;
                if (groupName != recGroupName) {
                    if (htmlString != "") {
                        // This is not the first item, hence add closing Li tag
                        htmlString = htmlString + '</ul></li>';
                    }
                    // Print header node
                    htmlString = htmlString + '<li id="' + element.itemName + '"><a href="javascript:" class="tree-toggle closed repositoryDetails" data-basenode="' + element.ID + '" role="branch" data-toggle="branch">' +
                                                '<span class="label label-warning">' + recGroupName + '</span>&nbsp;<span id="' + element.itemKey + 'Count" class="badge badge-warning">' + element.Level + '</span>' +
                                              '</a>' +
                                                    '<ul class="branch">' +
                                                        '<li><a href="javascript:" ' + (element.repoName == "Activities" ? '' : ' data-toggle="modal"') + ' class="' + element.className + '" data-id="' + element.ParentID + '" data-source="' + element.itemKey + '">Click to configure ' + recGroupName + '</a></li>';
                    groupName = recGroupName; // Continue processeing till the group is same.
                }
                else {
                    if (element.itemKey == "") {
                        htmlString = htmlString + '<li>' + element.itemName + '</li>';
                    }
                    else {
                        htmlString = htmlString + '<li><a href="javascript:" class="' + element.className + '" data-id="" data-source="' + element.itemKey + '">' + element.itemName + '</li>';
                    }
                }
                // Add any records if found
            });
            if (htmlString != "") {
                // This is not the first item, hence add closing Li tag
                htmlString = htmlString + '</ul></li>';
            }
            node.append(htmlString);
        });
        $(this).removeClass("loadRepo");
    });

    $(document).on("click", ".GTConfigure", function () {
        repoKey = $(this).data("key");

        $("#configDetails").data("key", $(this).data("key"));
        $("#configureDialog").data("remote", "PGeneralTasks.html");
        $("#configureDialog").modal();

        //            alert("Calling function " + $(this).data("key"));
        $("#GenTasks").data("key", $(this).data("key"));
        initiliazeGTDialogTasks();
    });

    $("body").delegate(".addCheck", "click", function () {
        var updateID = $("#" + $(this).data("id"));
        var repoID = updateID.data("repo");  // Repository id
        var sequence = updateID.data("checks"); // Sequence no
        var gt_id = updateID.data("key"); // ID for this general task

        $.get('/PConfiguration/addNewCheck?id=' + gt_id + '&repoID=' + repoID + "&sequence=" + sequence, function (result) {
            updateID.children().last().before(result);
            updateID.data("key", gt_id + 1);
            updateID.data("checks", sequence + 1);
        });
    });

    $("body").delegate(".shiftLeft", "click", function () {
        leftControl = $("#" + $(this).data("left"));
        rightControl = $("#" + $(this).data("right"));

        rightControl.find("option:selected").each(function (index, element) {
            leftControl.append($(element));
        });
    });

    $("body").delegate(".shiftRight", "click", function () {
        rightControl = $("#" + $(this).data("right"));
        leftControl = $("#" + $(this).data("left")); 

        leftControl.find("option:selected").each(function (index, element) {
            rightControl.append($(element));
        });
    });

    $("body").delegate(".shiftAllLeft", "click", function () {
        leftControl = $("#" + $(this).data("left"));
        rightControl = $("#" + $(this).data("right"));

        rightControl.find("option").each(function (index, element) {
            leftControl.append($(element));
        });
    });

    $("body").delegate(".shiftAllRight", "click", function () {
        rightControl = $("#" + $(this).data("right"));
        leftControl = $("#" + $(this).data("left"));

        leftControl.find("option").each(function (index, element) {
            rightControl.append($(element));
        });
    });


    $("body").delegate(".openDocument", "click", function () {
        var fileToView = $(this).data("source");
        window.open(fileToView, null, "height=400,width=600,status=yes,toolbar=no,menubar=no,location=no,top=200,left=300");
    });

    var addNewActivity = function () {
        var id = parseInt($("#cProcesses").data("id"));
        var sequence = parseInt($("#cProcesses").data("sequence"));
        var repo = parseInt($("#cProcesses").data("repo"));
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/PConfiguration/addNewActivity?id=" + id + "&repoID=" + repo + "&sequence=" + sequence ,
            success: function (data) {
                $("#cProcesses").append(data);
                // Increment and update the counters
                id = id + 1;
                sequence = sequence + 1;
                $("#cProcesses").data("id", id);
                $("#cProcesses").data("sequence", sequence);

                // Clear previous events attached to min-max and close buttons
                $('.close-box').unbind();

                // Bind new events 

                /*close*/
                $('.close-box').click(function () {
                    $(this).parent().parent().parent().parent().parent().fadeTo(400, 0, function () { $(this).hide(400, function () { $(this).remove() }); });
                    return false;
                });
            
                $('html, body').animate({
                    scrollTop: $("#inputPanel" + repo + "A" + id).offset().top
                }, 2000);
                $("#pNm" + repo + "A" + id).focus();
            },
            error: function (data) {
                $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + data.responseText +
                         '</div>' +
                     '</div>');
                $("#openDialogBox").modal("show");
            }
        });
    };

    $("body").delegate(".newActivity", "click", function () {
        addNewActivity();
    });

    $(document).on("click", ".openActivities", function (e) {
        $("#repositoryTree").hide();
        $("#configureProcess").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + processing("Processing. Please wait ") + 
                         '</div>' +
                     '</div>');
        $("#configureProcess").show();
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: $(this).data("source"),
            success: function (data) {
                $("#configureProcess").html(data);
            },
            error: function (data) {
                $("#configureProcess").html(data.responseText);
            }
        });
    });

    $("body").delegate(".backToRepository", "click", function () {
        $("#configureProcess").empty();
        $("#repositoryTree").show();
    });

    $("body").delegate(".procName", "blur", function () {
        var nm = $(this).val();
        var nm_id = $(this).prop("id");
        var seq = $("#" + nm_id + "Seq").val();

        if (nm.trim() == "") $("#" + nm_id + "Title").text(seq + ". " + "Provide Activity Name");
        else $("#" + nm_id + "Title").text(seq + ". " + nm);
    });

    $("body").delegate(".reSequenceOption", "click", function () {
        if ($(this).prop("checked") == true) {
            $(".reSequenceOption").removeProp("checked");
            $(this).prop("checked", "checked");
        }
    });
    
    $("body").delegate(".removeActivity", "click", function () {
        var mode = $(this).data("mode");
        var updateID = $("#" + $(this).data("updateid"));
        if (mode == "client")
        {
            updateID.remove();
            $("#openDialogBox").modal("hide");
            return;
        }
        $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                                '<div class="modal-body"> ' + processing("Processing. Please wait ") +
                         '</div>' +
                     '</div>');
        var id = $(this).data("id");
        var repo = $(this).data("repoid");
        var pageID = $("#" + $(this).data("pageid"));
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/PConfiguration/deleteActivity?id=" + id + "&repoID=" + repo,
            success: function (data) {
                updateID.remove();
                pageID.replaceWith(data);
                $("#openDialogBox").modal("hide");
            },
            error: function (data) {
                $.pnotify({
                    title: 'Delete Failed',
                    text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                    type: 'error'
                });
                $("#openDialogBox").modal("hide");

            }
        });
    });

    $("body").delegate(".populateMaster", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        $.getJSON("/OrgLevelMaster/getListItemsJSONFor?levelID=" + selectedID + '&excludeIDs=' + $("#excludeIDs").val(), function (result) {
            var populateCmb = $("#mstr_Org_Level_MasterID");
            populateCmb.empty();
            populateCmb.append('<option value="" selected="selected">Select...</option>');
            $.each(result, function (index, element) {
                populateCmb.append('<option value="' + element.index + '" >' + element.name + '</option>');
            });
        });
    });

    $("body").delegate(".expandAll", "click", function () {
        var node = "#" + $(this).data("id");

        $(node).find(".closed").each(function (index, element) {
            $(element).removeClass("closed");
        });
        $(node).find(".branch").each(function (index, element) {
            if (!$(element).hasClass("in")) $(element).addClass("in");
        });
    });

    $("body").delegate(".collapseAll", "click", function () {
        var node = "#" + $(this).data("id");

        $(node).find(".tree-toggle").each(function (index, element) {
            if (!$(element).hasClass("closed")) $(element).addClass("closed");
        });
        $(node).find(".in").each(function (index, element) {
            $(element).removeClass("in");
        });
    });

    $("body").delegate(".captureRevisionComments", "click", function () {
        $("#revCommentsDialog").html($("#saveRevisionComments").html());
        $("#revCommentsDialog").modal("show");
    });

    $(document).on("click", "#showAdvSearch", function () {
        $("#advancedSearchParams").show("slow");
    });

    $("body").delegate(".close-adv", "click", function () {
        $("#advancedSearchParams").hide("slow");
    });

    $("body").delegate(".sJoinOperator", "change", function () {
        var count = parseInt($("#advSearchOptions").data("pcount"));
        var options = "";

        if ($(this).val() != 1) {
            if ($(this).data("action") == "0") {
                count = count + 1;
                options = '<div class="row" id="pField' + count + '" data-rowcount="' + count + '">' +
                                '<select class="col-lg-2 sField">' +
                                    '<option value="1">Emp Code</option>' +
                                    '<option value="2">Emp Name</option>' +
                                    '<option value="3">Designation</option>' +
                                    '<option value="4">Department</option>' +
                                    '<option value="5">Location</option>' +
                                    '<option value="6">Date of Joining</option>' +
                                '</select>' +
                                '<select class="col-lg-2" id="pOp' + count + '">' +
                                    '<option value="1">equal to</option>' +
                                    '<option value="2">contains</option>' +
                                '</select>' +
                                '<div class="col-lg-6">' +
                                    '<input type="text" class="col-lg-12 tagsinput" value="" id="pValue' + count + '"/>' +
                                '</div>' +
                                '<select class="col-lg-2 sJoinOperator" id="pJoin' + count + '" data-item="' + count + '" data-action="0">' +
                                    '<option value="1" selected="selected">Search Ends</option>' +
                                    '<option value="2">And</option>' +
                                    '<option value="3">Or</option>' +
                                '</select>' +
                            '</div>';
                $("#advSearchOptions").append(options);
                $("#advSearchOptions").data("pcount", count);
                $(this).data("action", "1");
            }
        }
        else {
            // Remove all items coming after this
            var itemCount = parseInt(thisItem = $(this).data("item"));
            $("#advSearchOptions").find(".row").each(function () {
                if (parseInt($(this).data("rowcount")) > itemCount) $(this).remove();
            });
            $(this).data("action", "0");
        }
    });

    $(document).on("click", "#showfilter", function () {
        $("#filterParams").toggle("slow");
    });

    $("body").delegate(".userTypeValidation", "change", function () {
        var selValue = $("#UserType").val();

        switch (selValue) {
            case "1": // Employee
                $("#empcode").show();
                $("#splusr").hide();
                break;
            case "2": // Client
                $("#empcode").hide();
                $("#splusr").hide();
                break;
            case "3":   // Special User
                $("#empcode").hide();
                $("#splusr").show();
                break;
            default:
                $("#empcode").hide();
                $("#splusr").hide();
                break;
        }

    });


    $("body").delegate(".mapAccess", "click", function () {
        var roleid = $("#role").val();
        var menuID = $(this).data("id");
        var roleType = ($("#orgRole").prop("checked") ? "Org" : "Sys");
        var changedNode = $(this);
        var clickValue = $(this).prop("checked");

        if (roleid == "" || roleid == 0) {
            $.pnotify({
                title: 'Select the role first',
                type: 'info'
            });
            $(this).prop("checked", false);
            return;
        }

        if (clickValue) {
            // Update the database
            $.getJSON('/Menu/Add?roleID=' + roleid + '&MenuID=' + menuID + '&roleType=' + roleType, function (result) {
                $.pnotify({
                    title: 'Access Provided',
                    type: 'info'
                });
                $("#A" + menuID).text(result.Content);
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access Cannot be provided',
                    type: 'info'
                });
                changedNode.prop("checked",false);
            });

            var parentElement = $(this).parent().parent();
        
            while (!parentElement.hasClass("tree")) {
                if (!parentElement.parent().children().first().prop("checked")) {
                    parentElement.parent().children().first().click();  // Using click so that this status is also saved in the database
                }
                //    parentElement.parent().children().first().prop("checked", true);
                parentElement = parentElement.parent().parent();
            }
        }
        else {
            // Update the database
            $.getJSON('/Menu/Remove?roleID=' + roleid + '&MenuID=' + menuID + '&roleType=' + roleType, function (result) {
                $.pnotify({
                    title: 'Access Removed',
                    type: 'info'
                });
                $("#A" + menuID).text("");
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access cannot be removed',
                    type: 'info'
                });
                changedNode.prop("checked",true);
            });
            $("#C" + $(this).data("id")).children("li").each(function () {
                if ($(this).children("input").prop("checked")) $(this).children("input").click();
            });
        }
    });

    $("body").delegate(".restrictAccess", "click", function () {
        var roleid = $("#role").val();
        var menuID = $(this).data("id");
        var roleType = ($("#orgRole").prop("checked") ? "Org" : "Sys");
        var changedNode = $(this);

        var dialog = $("#openDialogBox");
        dialog.html('<div class="modal-dialog" style="width:30%;">' +
                    '<div class="modal-content">' +
                            '<div class="modal-body"> ' + processing("Processing. Please wait ") + '</div>' +
                     '</div>' +
                 '</div>');
        dialog.modal("show");
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/Menu/RestrictAccess?id=" + menuID + "&roleID=" + roleid + "&roleType=" + roleType,
            success: function (data) {
                dialog.html(data);
            },
            error: function (data) {
                dialog.html(data.responseText);
            }
        });
    });

    $("body").delegate(".mapData", "click", function () {
        var roleid = $("#role").val();
        var levelID = $(this).data("levelid");
        var levelMasterID = $(this).data("masterid");
        var changedNode = $(this);
        var clickValue = $(this).prop("checked");

        if (roleid == "" || roleid == 0) {
            $.pnotify({
                title: 'Select the role first',
                type: 'info'
            });
            $(this).prop("checked", false);
            return;
        }

        if (clickValue) {
            // Update the database
            $.getJSON('/Menu/AddDataAccess?roleID=' + roleid + '&levelID=' + levelID + '&levelMasterID=' + levelMasterID, function (result) {
                $.pnotify({
                    title: 'Access Provided',
                    type: 'info'
                });
                $("#A" + menuID).text(result.Content);
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access Cannot be provided',
                    type: 'info'
                });
                changedNode.prop("checked", false);
            });

        }
        else {
            // Update the database
            $.getJSON('/Menu/removeDataAccess?roleID=' + roleid + '&levelID=' + levelID + '&levelMasterID=' + levelMasterID, function (result) {
                $.pnotify({
                    title: 'Access Removed',
                    type: 'info'
                });
                $("#A" + menuID).text("");
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access cannot be removed',
                    type: 'info'
                });
                changedNode.prop("checked", true);
            });
        }
    });


    $("body").delegate(".roleType", "click", function () {
        if ($(this).val() == 1 && $(this).prop("checked")) {
            // Show relevant access options
            $("#MenuAccess").prop("class", "col-md-6");
            $("#DataAccess").show();
            // Org role
            $.getJSON("/OrgRole/getListItemsJSON", function (result) {
                var populateCmb = $("#role");
                populateCmb.empty();
                populateCmb.append('<option value="" selected="selected">Select...</option>');
                $.each(result, function (index, element) {
                    populateCmb.append('<option value="' + element.index + '" >' + element.name + '</option>');
                });
                $(".mapAccess").prop("checked", false);     // Set all checkboxes to false first
                $(".restrictAccess").text("");  // Clear all restrict access        
                $("#MReportingOptionCB").prop("checked", false);
                $(".mapData").prop("checked", false);
            });
        }
        if ($(this).val() == 2 && $(this).prop("checked")) {
            // Show relevant access options
            $("#MenuAccess").prop("class", "col-md-12");
            $("#DataAccess").hide();
            // Sys Role
            $.getJSON("/SysRole/getListItemsJSON", function (result) {
                var populateCmb = $("#role");
                populateCmb.empty();
                populateCmb.append('<option value="" selected="selected">Select...</option>');
                $.each(result, function (index, element) {
                    populateCmb.append('<option value="' + element.index + '" >' + element.name + '</option>');
                });
                $(".mapAccess").prop("checked", false);     // Set all checkboxes to false first
                $(".restrictAccess").text("");  // Clear all restrict access identifiers
                $("#MReportingOptionCB").prop("checked", false);
                $(".mapData").prop("checked", false);
            });
        }
    });

    $("body").delegate(".getRoleAccess", "change", function () {
        var roleID = $(this).find("option:selected").eq(0).val();
        var roleType = ($(sysRole).prop("checked") ? "Sys" : "Org");
        $.getJSON("/Menu/getAccessFor?roleID=" + roleID + "&roleType=" + roleType, function (result) {
            $(".mapAccess").prop("checked", false);     // Set all checkboxes to false first
            $(".restrictAccess").text("");  // Clear all restrict access identifiers
            $(".mapData").prop("checked", false);
            $("#MReportingOptionCB").prop("checked", false);
            $.each(result, function (index, element) {
                $("#M" + element.MenuID).prop("checked", true);
                $("#A" + element.MenuID).text(element.RestrictType);
            });
        });
    });

    $("body").delegate("#MReportingOptionCB", "click", function () {
        var roleid = $("#role").val();
        var roleType = ($(sysRole).prop("checked") ? "Sys" : "Org");
        var changedNode = $(this);
        var clickValue = $(this).prop("checked");

        if ($(this).prop("checked") == true) {
            $.getJSON('/Menu/addReportingAccess?roleID=' + roleid + '&roleType=' + roleType, function (result) {
                $.pnotify({
                    title: 'Access Provided',
                    type: 'info'
                });
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access Cannot be provided',
                    type: 'info'
                });
                changedNode.prop("checked", false);
            });

        }
        else {
            $.getJSON('/Menu/removeReportingAccess?roleID=' + roleid + '&roleType=' + roleType, function (result) {
                $.pnotify({
                    title: 'Access Removed',
                    type: 'info'
                });
            }).fail(function (result) {
                $.pnotify({
                    title: 'Access Cannot be removed',
                    type: 'info'
                });
                changedNode.prop("checked", false);
            });

        }
    });

    $("body").delegate(".showMessage", "click", function () {
        if ($(this).data("message") != "") {
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                        '<h4 class="modal-title" id="dialogTitle">Error Trace (For Administrator) ...</h4>' +
                    '</div>' +
                        '<div class="modal-body"> ' +
                            '<div class="row col-md-12">' +
                                '<p class="control-label col-md-12 text-center">' + $(this).data("message") +
                            '</div>' +
                        '</div>' +
                        '<div class="modal-footer">' +
                            '<div class="form-group">' +
                                '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                            '</div>' +
                        '</div>' +
                '</div>' +
            '</div>');
        }
        $("#openDialogBox").modal("show");
    });