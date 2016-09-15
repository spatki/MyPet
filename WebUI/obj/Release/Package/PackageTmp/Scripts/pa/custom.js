// Global Variables used for tab navigation
var timeCount = 1;
var timeEvent;
var monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
var weekDay = ["Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sat"];
var keypressCode = -1;
var keypressShift = false;
var invalidCharacters = ["&bull;", "&ldquo;", "&rdquo;"];
var replaceCharacter = ["&lt;Invalid Character&gt;", "&quot;", "&quot;"];

var viewDate = new Date();  // This will the current date used for tracking plan
var dateFormat = "dd/MM/yyyy";
var currentScrollPosition = $(window).scrollTop();

function parseDate(str) {
    "use strict";
    var mdy = str.split('/');
    return new Date(mdy[2], mdy[1] - 1, mdy[0]);
}

function formatDate(dt) {
    // Format date to dd/mm/yyyy format
    return (dt.getDate() < 10 ? "0" : "") + dt.getDate() + "/" + (dt.getMonth() < 9 ? "0" : "") + (dt.getMonth() + 1) + "/" + dt.getFullYear();
}

function daydiff(first, second) {
    "use strict";
    return (second - first) / (1000 * 60 * 60 * 24);
}

$(function () {

	
	/***************************************
	 Color Box Border
	loadProj
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

	$("nav#main_topnav").css({"top":Math.max(0,75-$(this).scrollTop())});
	$("aside#left_panel").css("top", Math.max(50, 120 - $(this).scrollTop()));
	$("div#fixHeader").css("top", Math.max(50, 150 - $(this).scrollTop()));
	$(window).scroll(function(){
		$("nav#main_topnav").css({"top":Math.max(0,75-$(this).scrollTop())});
		$("aside#left_panel").css("top",Math.max(50,120-$(this).scrollTop()));
		$("div#fixHeader").css("top", Math.max(0, -290 + $(this).scrollTop()));
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


// Functions for in-line editing and navigating using tab or enter keys: START

$("body").delegate(".EditNumber", "click", function () {
    var OriginalContent = $(this).text();
    var idName = $(this).data("id");

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("EditNumber");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='3' id='mask_decimal' class='inputValue' data-class='EditNumber' data-id='" + idName + "'/>");
    //$("#mask_decimal").inputmask({ 'mask': ["9[9][.99]", "999", "9", "99"] }, { placeholder:" ", rightAlignNumerics: true, clearMaskOnLostFocus: true });
    $("#mask_decimal").inputmask('[9][99][999][.99]', { "placeholder": " ", rightAlignNumerics: true, clearMaskOnLostFocus: true });
    $(this).children().first().focus();
    $(this).children().first().select();
});

$("body").delegate(".EditDate", "click", function () {
    var OriginalContent = $(this).text();
    var idName = $(this).data("id");

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("EditDate");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='12' id='mask_date' class='inputValue' data-class='EditDate' data-navkey='" + $(this).data("navkey") + "' data-id='" + idName + "'/>");
    $("#mask_date").inputmask("d/m/y", { "placeholder": "dd/mm/yyyy" }); //multi-char placeholder
    $(this).children().first().focus();

});

$("body").delegate(".Edit", "click", function () {
    var OriginalContent = $(this).text();
    var idName = $(this).data("id");

    keypressCode = -1;
    keypressShift = false;
    $(this).addClass("cellEditing");
    $(this).removeClass("Edit");
    $(this).html("<input type='text' value='" + OriginalContent + "' size='30' data-class='Edit' data-navkey='" + $(this).data("navkey") + "' class='inputValue' data-id='" + idName + "'/>");
    $(this).children().first().focus();
    $(this).children().first().select();
});

$("body").delegate(".EditGSC", "click", function () {
    var OriginalContent = $(this).text();
    var idName = $(this).data("id");

    $(this).addClass("cellEditing");
    $(this).removeClass("EditGSC");
    $(this).html("<select class='inputValue' data-class='EditGSC' data-navkey='EditGSC' data-id='" + idName + "'><option value='0'>&nbsp;0&nbsp; </option><option value='1'>&nbsp;1&nbsp; </option><option value='2'>&nbsp;2&nbsp;&nbsp;</option><option value='3'>&nbsp;3&nbsp; </option><option value='4'>&nbsp;4&nbsp; </option><option value='5'>&nbsp;5&nbsp; </option></select>");
    $(this).children().first().val(OriginalContent);
    $(this).children().first().focus();
});

$("body").delegate(".inputValue", "keypress", function (e) {
    if (e.which == 13) {
        var newContent = $(this).val();
        var editClass = $(this).data("class");
        var parentEle = $(this).parent();
        var updateID = $("#" + $(this).data("id"));

        if (editClass == "EditNumber" || editClass == "EditTime") {
            if (newContent.trim() == "") newContent = "0";
            parentEle.text(parseFloat(newContent));
            if (updateID.length > 0) updateID.val(parseFloat(newContent));
            if (editClass == "EditTime") calculateTimeTotals(parentEle);
        }
        else {
            parentEle.text(newContent);
            if (updateID.length > 0) updateID.val(newContent);
        }
        parentEle.removeClass("cellEditing");
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
    var updateID = $("#" + $(this).data("id"));

    if (editClass == "EditNumber" || editClass == "EditTime") {
        if (newContent.trim() == "") newContent = "0";
        parentEle.text(parseFloat(newContent));
        if (updateID.length > 0) updateID.val(parseFloat(newContent));
        if (editClass == "EditTime") calculateTimeTotals(parentEle);
    }
    else {
        parentEle.text(newContent);
        if (updateID.length > 0) updateID.val(newContent);
    }
    parentEle.removeClass("cellEditing");
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
    var loadInWindow = $(this).data("pageload");
    if ($(this).data("back") == true) {
        if ($('#processTable').length > 0) $('#processTable').dataTable().fnDestroy();
        var backContents = $("#containerDetails").html();
        $("#containerDetails").empty().off("*");
        $("#backContents").empty().off("*");
        $("#backContents").html(backContents);
        if ($('#processTable').length > 0) $('#processTable').dataTable().fnDestroy();
    }
    else {
        $("#backContents").empty().off("*");
        $("#backContents").html("");
    }
    if (loadInWindow == "page") {
        location.replace($(this).data("source"));
    }
    else {
        $("#containerDetails").empty().off("*");
        $("#containerDetails").html(processing("Loading "));
        $("#containerDetails").load($(this).data("source"), function (response, status, xhr) {
            if (status == "error") {
                $("#containerDetails").empty().off("*");
                if (xhr.status == 403) {
                    $("#openDialogBox").html("");
                    $("#openDialogBox").html(xhr.responseText);
                    $("#openDialogBox").modal("show");
                }
                else {
                    var msg = "Sorry but there was an error: ";
                    //$( "#error" ).html( msg + xhr.status + " " + xhr.statusText );
                    alert(msg + xhr.status + " " + xhr.statusText);
                }
            }
        });
    }
});

$("body").delegate(".navigateBack","click", function() {
    if ($("#backContents").html() != "") $("#containerDetails").html($("#backContents").html());
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

$("body").delegate(".reLoadChange", "click", function () {
    var controlId = $(this).data("control");
    $("#" + controlId).change();
});

$("body").delegate(".reLoadOnChange", "change", function () {
    $(".reLoad").click();
});

$("body").delegate(".closeDialog", "click", function () {
    var dialog = $("#" + $(this).data("id"));    // Dialog box to be closed

    if ($(this).data("id") == null || $(this).data("id") == "") {
        dialog = $(this).parent().parent().parent().parent().parent().parent();
    }
    dialog.modal("hide");
});

$("body").delegate(".saveDefaultExtra", "click", function () {
    var form = $("#" + $(this).data("form"));
    var mode = $(this).data("mode");
    var updateId = $(this).data("id");
    var dialogBox = $("#" + $(this).data("dialog"));
    var pageUpdateID = $("#" + $(this).data("pageid"));

    // Called by nested (extra) dialog boxes
    if ($(this).data("delete") == "client" || mode == "deleteOnClient") {
        // Just remove this element on the client side as it is not been saved on server yet
        $("#" + updateId).remove();
        dialogBox.modal("hide");
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
                case "editExtra":
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
                    $("#" + updateId).empty().off("*");
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
                case "editExtra":
                    dialogBox.empty().off("*");
                    dialogBox.html(data.responseText);
                    return;
                    break;
                case "search":
                    $("#" + updateId).empty().off("*");
                    $("#" + updateId).html(data.responseText);
                    break;
                case "saveActivity":
                    pageUpdateID.replaceWith(data.responseText);
                    break;
                default:
                    $("#openDialogBox").empty().off("*");
                    $("#openDialogBox").html(data.responseText);
                    break;
                }
            dialogBox.modal("hide");
        }
    });
});

$("body").delegate(".saveWorkflow", "click", function () {
    processHTMLData(".showEditor");     // In case a WYSIWYG editor is being used
    var dialog = $("#openDialogBox");
    if ($(this).data("confirm") == "True") {
        $("#revCommentsDialog").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close closeDialog" data-id="revCommentsDialog" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="revCommentsDialog">No</button>' +
//                                        '<button type="button" class="btn blue-bg saveWorkflow" data-id="containerDetails" data-source="' + $(this).data("source") + '" data-method="' + $(this).data("method") + '" data-form="' + $(this).data("form") + '" data-status="' + $(this).data("status") + '" data-userid="' + $(this).data("userid") + '" data-usertype="' + $(this).data("usertype") + '" data-message="' + $(this).data("message") + '" data-confirm="false" data-workflow="' + $(this).data("workflow") + '">Yes</button>' +
                                        '<button type="button" class="btn blue-bg saveWorkflow" data-id="' + 'openDialogBox' +
                                        '" data-source="' + $(this).data("source") + '" data-method="' + $(this).data("method") +
                                        '" data-form="' + $(this).data("form") + '" data-status="' + $(this).data("status") + '" data-userid="' +
                                        $(this).data("userid") + '" data-usertype="' + $(this).data("usertype") + '" data-message="' + $(this).data("message") +
                                        '" data-confirm="false" data-key="' +
                                            $(this).data("key") + '" data-workflow="' + $(this).data("workflow") +
                                            '" data-statustype = ' + $(this).data("statustype") + '>Yes</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>')
        $("#revCommentsDialog").modal("show");
        return;
    }
    else {
        $("#revCommentsDialog").modal("hide");
    }
/*    var form = $(this).data("form");
    var updateId = $(this).data("id");
    var source = $(this).data("source");
    var method = $(this).data("method");
    var status = $(this).data("status");
    var userid = $(this).data("userid");
    var usertype = $(this).data("usertype");
    var message = $(this).data("message");
    var workflow = $(this).data("workflow");
    var statusType = $(this).data("statustype");

    $("#workflow_status").val(status);
    $("#followWF").val(true);
    $("#statusWF").val(status);
    $("#workflow").val(workflow);
*/

    var form = $(this).data("form");
    var updateId = $(this).data("id");
    var source = $(this).data("source");
    var method = $(this).data("method");
    var status = $(this).data("status");
    var statusType = $(this).data("statustype");
    var userid = $(this).data("userid");
    var usertype = $(this).data("usertype");
    var message = $(this).data("message");
    var workflow = $(this).data("workflow");
    var method = $(this).data("method");
    var statusType = $(this).data("statustype");
    var key = $(this).data("key");

    $("#workflow_status").val(status);
    $("#followWF").val(true);
    $("#statusWF").val(status);
    $("#workflow").val(workflow);
    $("#StatusType").val(statusType);

    if (usertype == "user") {
        // Prompt user to assign the next workflow
        $("#workflowUser").val(userid);
        processWorkflow(form, updateId, source, message, method);
    }
    else {
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: usertype,
//            data: { 'formName': form, 'updateID': updateId, 'source': source, 'message': message, 'workflow': workflow, 'method': method, 'status': status, 'displayID': updateId, 'statusType' : statusType },
            data: { 'formName': form, 'updateID': key, 'source': source, 'message': message, 'workflow': workflow, 'method': method, 'status': status, 'displayID': updateId, 'statusType': statusType },
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
    }
});

$("body").delegate(".saveExtraWorkflow", "click", function () {
    processHTMLData(".showEditor");     // In case a WYSIWYG editor is being used
    var parentDialog = $(this).parent().parent().parent().parent().parent().parent();
    var zindex = parentDialog.css("z-index");
    
    if (zindex == "auto") {
        zindex = 1050;
        parentDialog.css("z-index", zindex);
    }
    var dialog = $("#extraDialogBox");
    dialog.css("z-index", (zindex + 1)); // so that this dialog appears on the top of the parent dialog box

    if ($(this).data("confirm") == "True") {
        $("#revCommentsDialog").css("z-index", zindex + 2);
        $("#revCommentsDialog").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close closeDialog" data-id="revCommentsDialog" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="revCommentsDialog">No</button>' +
                                        '<button type="button" class="btn blue-bg saveExtraWorkflow" data-id="' + parentDialog.prop("id") +
                                        '" data-source="' + $(this).data("source") + '" data-method="' + $(this).data("method") +
                                        '" data-form="' + $(this).data("form") + '" data-status="' + $(this).data("status") + '" data-userid="' +
                                        $(this).data("userid") + '" data-usertype="' + $(this).data("usertype") + '" data-message="' + $(this).data("message") +
                                        '" data-confirm="false" data-key="' +
                                            $(this).data("key") + '" data-workflow="' + $(this).data("workflow") +
                                            '" data-statustype = ' + $(this).data("statustype") + '>Yes</button>' +
                                    '</div>' +
                                '</div>' +
                        '</div>' +
                    '</div>')
        $("#revCommentsDialog").modal("show");
        return;
    }
    else {
        $("#revCommentsDialog").modal("hide");
    }
    $(this).prop("disabled", "disabled");   // Disable this button
    var form = $(this).data("form");
    var updateId = $(this).data("id");
    var source = $(this).data("source");
    var method = $(this).data("method");
    var status = $(this).data("status");
    var statusType = $(this).data("statustype");
    var userid = $(this).data("userid");
    var usertype = $(this).data("usertype");
    var message = $(this).data("message");
    var workflow = $(this).data("workflow");
    var method = $(this).data("method");
    var statusType = $(this).data("statustype");
    var key = $(this).data("key");

    $("#workflow_status").val(status);
    $("#followWF").val(true);
    $("#statusWF").val(status);
    $("#workflow").val(workflow);
    $("#StatusType").val(statusType);

    if (usertype == "user") {
        $("#workflowUser").val(userid);
        processWorkflow(form, updateId, source, message, method);
    }
    else {
        // Prompt user to assign the next workflow
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: usertype,
            data: { 'formName': form, 'updateID': key, 'source': source, 'message': message, 'workflow': workflow, 'method': method, 'status': status, 'displayID': updateId, 'statusType': statusType },
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
                dialog.modal("show");
                //Check if there is any parent updates to be made
                var makeUpdate = dialog.find("#parentUpdate").length;
                if (makeUpdate > 0) {
                    var mode = dialog.find("#parentMode").text();
                    var pageid = dialog.find("#parentUpdateID").text();
                    var updateTable = dialog.find("#parentSearchTable").text(); // To enable jstable features used for paging and searching
                    var contents = dialog.find("#parentContents").html();
                    if (updateTable != "") {
                        $("#" + updateTable).dataTable().fnDestroy();
                    }
                    switch (mode) {
                        case "add":
                            $("#" + pageid).append(contents);
                            break;
                        case "edit":
                            $("#" + pageid).replaceWith(contents);
                            break;
                        default:
                            break;
                    }
                    if (updateTable != "") {
                        jsTable(updateTable);
                    }
                }
            },
            error: function (data) {
                dialog.empty().off("*");
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
                                '<button type="button" class="close closeDialog" data-id="revCommentsDialog" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="revCommentsDialog">No</button>' +
                                        '<button type="button" class="btn blue-bg openWorkflow" data-id="containerDetails" data-source="' +
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
    else {
        $("#revCommentsDialog").modal("hide");
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
    var method = $(this).data("method");
    var statustype = $(this).data("statustype");

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
            type: method,
            url: source,
            data: (method == "POST" ? $("#" + form).serialize() : { 'id': key, 'workflowUser': userid, 'status': status, 'message': message, 'workflow': workflow, 'statusType' : statustype }),
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                $.pnotify({
                    title: "Check for errors",
                    type: 'info'
                });
                dialog.empty().off("*");
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
            data: { 'formName': form, 'workflowUser': userid, 'updateID': updateId, 'source': source, 'message': message, 'workflow': workflow, 'method': method, 'status': status, 'displayID': 'openDialogBox', 'statusType': statustype },
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
    }
});

$("body").delegate(".saveWorkflowExtra", "click", function () {
    var form = $("#" + $(this).data("form"));
    var actionNeeded = $(this).data("mode");
    var updateId = $(this).data("id");
    var message = $(this).data("message");
    var dialogBox = $(this).parent().parent().parent().parent().parent().parent();
    //var pageUpdateID = $("#" + $(this).data("pageid"));

    $.ajax({
        cache: false,
        async: true,
        type: "POST",
        url: form.attr('action'),
        data: form.serialize(),
        success: function (data) {
            dialogBox.empty().off("*");
            dialogBox.modal("hide");
            $.pnotify({
                title: message,
                type: 'info'
            });
            switch (actionNeeded) {
                case "add":
                    $("#" + updateId).append(data);
                    break;
                case "edit":
                    $("#" + updateId).replaceWith(data);
                    break;
                case "delete":
                    var listing = $("#processTable");
                    if (listing.length != 0) {
                        $('#processTable').dataTable().fnDestroy();
                    }
                    $("#" + updateId).remove();
                    $("#openDialogBox").modal("hide");
                    if (listing.length != 0) {
                        jsTable("processTable");
                    }
                    break;
                case "workflowEdit":
                    var updateIn = $("#" + updateId);
                    updateIn.empty().off("*");
                    updateIn.html(data);
                    //Check if there is any parent updates to be made
                    var makeUpdate = updateIn.find("#parentUpdate").length;
                    if (makeUpdate > 0) {
                        var mode = updateIn.find("#parentMode").text();
                        var pageid = updateIn.find("#parentUpdateID").text();
                        var updateTable = updateIn.find("#parentSearchTable").text(); // To enable jstable features used for paging and searching
                        var contents = updateIn.find("#parentContents").html();
                        if (updateTable != "") {
                            $("#" + updateTable).dataTable().fnDestroy();
                        }
                        switch (mode) {
                            case "add":
                                $("#" + pageid).append(contents);
                                break;
                            case "edit":
                                $("#" + pageid).replaceWith(contents);
                                break;
                            default:
                                break;
                        }
                        if (updateTable != "") {
                            jsTable(updateTable);
                        }
                    }
                    break;
                default:
                    break;
            }
        },
        error: function (data) {
            if (actionNeeded == "delete") {
                dialogBox.empty().off("*");
                dialogBox.modal("hide");
                // Show the error message
                $.pnotify({
                    title: 'Delete Failed',
                    text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                    type: 'error'
                });
            }
            else {
                $.pnotify({
                    title: "Check Error",
                    type: 'info'
                });
                dialogBox.empty().off("*");
                dialogBox.html(data.responseText);
            }
        }
    });
});


$("body").delegate(".selectUser", "click", function () {
    var selectedUser = $("#reviewUser").val();
    var dialog = $(this).parent().parent().parent().parent().parent().parent(); // This dialog box
    if (selectedUser == "") {
        alert("Pl. select a user");
    }
    else {
        $("#workflowUser").val(selectedUser);   // In case a form is submitted

        var key = $(this).data("id");
        var updateID = $(this).data("updateid");
        var userid = selectedUser;
        var status = $(this).data("status");
        var message = $(this).data("message");
        var workflow = $(this).data("workflow");
        var source = $(this).data("source");
        var method = $(this).data("method");
        var form = $("#" + $(this).data("form"));
        var displayIn = $(this).data("displayid");
        $.ajax({
            cache: false,
            async: true,
            type: method,
            url: source,
            data: (method == "POST" ? form.serialize() : { 'id': key, 'workflowUser': userid, 'status': status, 'message': message, 'workflow': workflow }),
            success: function (data) {
                if (method == "GET") {
                    dialog.empty().off("*");
                    var checkResult = $(data).find("#FinalResult");
                    if (checkResult != undefined && checkResult.eq(0).val() == "True") {
                        dialog.modal("hide");
                        $.pnotify({
                            title: message,
                            type: 'info'
                        });
                        var updateIn = $("#openDialogBox");
                        updateIn.empty().off("*");
                        updateIn.html(data);
                        //Check if there is any parent updates to be made
                        var makeUpdate = updateIn.find("#parentUpdate").length;
                        if (makeUpdate > 0) {
                            var mode = updateIn.find("#parentMode").text();
                            var pageid = updateIn.find("#parentUpdateID").text();
                            var updateTable = updateIn.find("#parentSearchTable").text(); // To enable jstable features used for paging and searching
                            var contents = updateIn.find("#parentContents").html();
                            if (updateTable != "") {
                                $("#" + updateTable).dataTable().fnDestroy();
                            }
                            switch (mode) {
                                case "add":
                                    $("#" + pageid).append(contents);
                                    break;
                                case "edit":
                                    $("#" + pageid).replaceWith(contents);
                                    break;
                                default:
                                    break;
                            }
                            if (updateTable != "") {
                                jsTable(updateTable);
                            }
                        }
                    }
                    else {
                        dialog.html(data);
                    }
                }
                else {
                    // Server has processed something and we have to show the results and make relevant updates
                    dialog.empty().off("*");
                    dialog.modal("hide");
                    $.pnotify({
                        title: message,
                        type: 'info'
                    });
                    var updateIn = $("#" + displayIn);
                    updateIn.empty().off("*");
                    updateIn.html(data);
                    
                    //Check if there is any parent updates to be made
                    var makeUpdate = updateIn.find("#parentUpdate").length;
                    if (makeUpdate > 0) {
                        var mode = updateIn.find("#parentMode").text();
                        var pageid = updateIn.find("#parentUpdateID").text();
                        var updateTable = updateIn.find("#parentSearchTable").text(); // To enable jstable features used for paging and searching
                        var contents = updateIn.find("#parentContents").html();
                        if (updateTable != "") {
                            $("#" + updateTable).dataTable().fnDestroy();
                        }
                        switch (mode) {
                            case "add":
                                $("#" + pageid).append(contents);
                                break;
                            case "edit":
                                $("#" + pageid).replaceWith(contents);
                                break;
                            default:
                                break;
                        }
                        if (updateTable != "") {
                            jsTable(updateTable);
                        }
                    }
                }
            },
            error: function (data) {
                $.pnotify({
                    title: "Check for errors",
                    type: 'info'
                });
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
    }
});

$("body").delegate("#orgClientID", "change", function () {
    var clientID = $("#orgClientID").val()
    $("#ClientType").val("");
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

    function processWorkflow (form, updateId, source, message, method)
    {
        var formObj = $("#" + form);
        $.ajax({
            cache: false,
            async: true,
            type: method,
            url: source,
            data: formObj.serialize(),
            success: function (data) {
                var dialog = $("#" + updateId);
                if (method == "GET") {
                    dialog.empty().off("*");
                    dialog.html(data);
                    dialog.modal("show");
                }
                else {
                    // Server has processed something and we have to show the results and make relevant updates
                    if (message == "") message = "Transaction Successful";
                    $.pnotify({
                        title: message,
                        type: 'info'
                    });
                    dialog.empty().off("*");
                    dialog.html(data);
                    //Check if there is any parent updates to be made
                    var makeUpdate = dialog.find("#parentUpdate").length;
                    if (makeUpdate > 0) {
                        var mode = dialog.find("#parentMode").text();
                        var pageid = dialog.find("#parentUpdateID").text();
                        var updateTable = dialog.find("#parentSearchTable").text(); // To enable jstable features used for paging and searching
                        var contents = dialog.find("#parentContents").html();
                        if (updateTable != "") {
                            $("#" + updateTable).dataTable().fnDestroy();
                        }
                        switch (mode) {
                            case "add":
                                $("#" + pageid).append(contents);
                                break;
                            case "edit":
                                $("#" + pageid).replaceWith(contents);
                                break;
                            default:
                                break;
                        }
                        if (updateTable != "") {
                            jsTable(updateTable);
                        }
                    }
                }
            },
            error: function (data) {
                $.pnotify({
                    title: "Check for errors",
                    type: 'info'
                });
                $("#" + updateId).empty().off("*");
                $("#" + updateId).html(data.responseText);
            }
        });

    }

    $("body").delegate("#newCycle", "click", function () {
        var form = $("#" + $(this).data("form"));
        var source = $(this).data("source");
        var updateId = $(this).data("id");

        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: source,
            data: form.serialize(),
            success: function (data) {
                $.pnotify({
                    title: 'Estimation Saved and New Cycle Created',
                    type: 'info'
                });
                $("#" + updateId).empty().off("*");
                $("#" + updateId).html(data);
            },
            error: function (data) {
                $.pnotify({
                    title: 'Estimation Not Saved. Check Errors !',
                    type: 'info'
                });
                $("#" + updateId).empty().off("*");
                $("#" + updateId).html(data.responseText);
            }
        });
    });

    $("body").delegate(".save", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var updateId = $(this).data("id");
        var dialog = $(this).parent().parent().parent().parent().parent().parent();

        if (dialog == "undefined" || dialog == "" || !dialog.hasClass("modal")) dialog = $("#openDialogBox");
        if (mode == "addGSC") {
            // Update contents for CKEditor
            for (var i in CKEDITOR.instances) {
                CKEDITOR.instances[i].updateElement();
            }
        }
        if (mode == "linkSkills") {
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
                        $("#" + updateId).append(data);
                        dialog.modal("hide");
                        break;
                    case "addGSC":
                        $.pnotify({
                            title: 'New Record Created',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        dialog.modal("hide");
                        break;
                    case "addEstmGroup":
                        $.pnotify({
                            title: 'Module Saved',
                            type: 'info'
                        });
                        $("#" + updateId).children().last().before(data);    // Replace old group with new group selected
                        // var key = parseInt($("#sizeEstmParams").data("keycount"));
                        // $("#sizeEstmParams").data("keycount", key + 1); // Update the key for next new group
                        dialog.modal("hide");
                        $('html, body').animate({
                            scrollTop: $("#" + updateId).children().last().offset().top
                        }, 2000);
                        break;
                    case "editEstmGroup":
                        $.pnotify({
                            title: 'Module Saved',
                            type: 'info'
                        });
                        var nodeParent = $("#" + updateId).parent();    // Save the parent node
                        nodeParent.children().eq(0).remove();
                        $("#" + updateId).replaceWith(data);    // Replace old group with new group selected
                        var newID = nodeParent.children("a").eq(1).data("id"); // Get the new group id (Second child element)

                        $(nodeParent).find(".group").each(function (index, element) {
                            $(element).val(newID); // Update all values with the new group id
                        });
                        dialog.modal("hide");
                        break;
                    case "addEstmParameter":
                        $.pnotify({
                            title: 'Parameter Added',
                            type: 'info'
                        });
                        var parameterTable = $("#" + updateId).next();       // Get the UL component
                        var newParameterNode = parameterTable.find("#NewParameter").eq(0);
                        newParameterNode.replaceWith(data);
                        var key = parseInt($("#sizeEstmParams").data("keycount"));
                        $("#sizeEstmParams").data("keycount", key + 1); // Update the key for next new group
                        dialog.modal("hide");
                        $('html, body').animate({
                            scrollTop: parameterTable.find("#NewParameter").eq(0).offset().top
                        }, 2000);
                        break;
                    case "addEffSchDefect":
                        $("#PhaseParams").find(".addNewOption").last().before($(data).find("#PhaseParams").html());
                        $("#ScheduleParams").append($(data).find("#ScheduleParams").html());
                        $("#DefectParams").append($(data).find("#DefectParams").html());
                        var key = parseInt($("#" + updateId).data("keycount"));
                        var message = $("#" + updateId).data("message");
                        $("#" + updateId).data("keycount", key + 1); // Update the key for next new group
                        dialog.modal("hide");
                        $("#CUT_Phase").text($("#CUT_EffortPercent").val());     // If the added phase is a CUT phase, then update the cut percent
                        $("#CUT_PhaseEff").text((parseFloat($("#CUT_Phase").text()) * parseFloat($("#Overall_Project_PDs").text()) / 100).toFixed(2));  // Phase Efforts
                        $.pnotify({
                            title: message,
                            type: 'info'
                        });
                        $('html, body').animate({
                            scrollTop: $("#" + updateId).children().last().offset().top
                        }, 2000);
                        break;
                    case "addLineItem":
                        var position = $("#" + updateId).find(".addNewOption").last();
                        position.before(data);       // Get the UL component
                        var key = parseInt($("#" + updateId).data("keycount"));
                        var message = $("#" + updateId).data("message");
                        $("#" + updateId).data("keycount", key + 1); // Update the key for next new group
                        dialog.modal("hide");
                        $.pnotify({
                            title: message,
                            type: 'info'
                        });
                        $('html, body').animate({
                            scrollTop: position.offset().top
                        }, 2000);
                        break;
                    case "linkSkills":
                        var params = $("this").data("params");
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);       // Get the UL component
                        var key = parseInt($("#" + params).data("keycount"));
                        var message = $("#" + params).data("message");
                        $("#" + params).data("keycount", key + 1); // Update the key for next new group
                        dialog.modal("hide");
                        $.pnotify({
                            title: message,
                            type: 'info'
                        });
                        $('html, body').animate({
                            scrollTop: $("#" + updateId).children().last().offset().top
                        }, 2000);
                        break;
                    case "editDropDown":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).replaceWith(data);
                        dialog.modal("hide");
                        break;
                    case "edit":
                        $.pnotify({
                            title: 'Changes Saved',
                            type: 'info'
                        });
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);
                        dialog.modal("hide");
                        break;
                    case "projectRepository":
                        $.pnotify({
                            title: 'Repository has been reset with latest mappings',
                            type: 'info'
                        });
                        printProjectMapping($("#" + updateId), data);
                        dialog.modal("hide");
                        break;
                    case "delete":
                        $.pnotify({
                            title: 'Record Deleted',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        dialog.modal("hide");
                        break;
                    case "search":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);
                        break;
                    case "update":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);
                        $.pnotify({
                            title: 'Saved Successfully',
                            type: 'info'
                        });
                        break;
                    case "projectLocationMapping":
                        $.pnotify({
                            title: 'Location to Org. Level Mapping is set',
                            type: 'info'
                        });
                        dialog.modal("hide");
                        break;
                    case "addProjectAllocation":
                        $('#existingEmployees').dataTable().fnDestroy();
                        $.pnotify({
                            title: 'New team member added',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        jsTable("existingEmployees");
                        // Refresh the list of available employees
                        dialog.modal("hide");
                        $("#GoButton").click();
                        break;
                    case "editProjectAllocation":
                        $('#existingEmployees').dataTable().fnDestroy();
                        $.pnotify({
                            title: 'Allocation changes saved',
                            type: 'info'
                        });
                        $("#" + updateId).replaceWith(data);
                        jsTable("existingEmployees");
                        dialog.modal("hide");
                        break;
                    case "deleteProjectAllocation":
                        // Remove line item from exising employees
                        $('#existingEmployees').dataTable().fnDestroy();
                        $.pnotify({
                            title: 'Team Member released',
                            type: 'info'
                        });
                        $("#" + updateId).remove();
                        jsTable("existingEmployees");
                        // Add details in released employee
                        $("#employeeHistory").dataTable().fnDestroy();
                        $("#empHistoryData").append(data);
                        jsTable("employeeHistory");
                        dialog.modal("hide");
                        break;
                    case "changePassword":
                        $.pnotify({
                            title: 'Password Changed',
                            type: 'info'
                        });
                        dialog.modal("hide");
                        break;
                    case "updateSuppDocs":
                        $.pnotify({
                            title: 'Supporting Documents Configured',
                            type: 'info'
                        });
                        dialog.modal("hide");
                        break;
                    case "uptSuppTailoredDocs":
                        $.pnotify({
                            title: 'Changes to Actvity/Task Saved',
                            type: 'info'
                        });
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data);
                        $("#" + updateId).find("span a").eq(0).text($("#openDialogBox").find("#TailorName").val());
                        break;
                    case "addTailorTask":
                        $.pnotify({
                            title: 'Actvity/Task Added',
                            type: 'info'
                        });
                        $("#" + updateId).append(data);
                        $("#openDialogBox").modal("hide");
                        break;
                    default:
                        break;
                }
            },
            error: function (data) {
                switch (mode) {
                    case "delete":
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
                        break;
                    case "changePassword":
                        $.pnotify({
                            title: 'Check Errors',
                            type: 'info'
                        });
                        $("#" + updateId).html(data.responseText);
                        break;
                    case "search":
                    case "update":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data.responseText);
                        break;
                    case "uptSuppTailoredDocs":
                    case "addTailorTask":
                        $.pnotify({
                            title: 'Check Errors',
                            type: 'info'
                        });
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        break;
                    default:
                        dialog.empty().off("*");
                        dialog.html(data.responseText);
                        break;
                }
            }
        });
    });

    $("body").delegate(".process", "click", function () {
        var processNode = $(this);
        var mode = processNode.data("mode");
        var updateId = processNode.data("id");
        var source = processNode.data("source");
        var params = processNode.data("param");
        var message = processNode.data("message");

        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            data: params,
            success: function (data) {
                if (message != "") {
                    $.pnotify({
                        title: message,
                        type: 'info'
                    });
                }
                switch (mode) {
                    case "add":
                        $("#" + updateId).append(data);
                        break;
                    case "edit":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);
                        break;
                    case "delete":
                        $("#" + updateId).remove();
                        break;
                    case "search":
                    case "update":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data);
                        break;
                    case "IncludeRepo":
                        // Make changes to process icon
                        processNode.data("mode", "ExcludeRepo");
                        processNode.data("source", "/PrjProcessTailor/ExcludeRepo");
                        processNode.attr("title", "Click to exclude");
                        processNode.children().eq(0).removeClass("icon-ok");
                        processNode.children().eq(0).addClass("icon-remove");
                        // Display effect on processed node
                        var node = $("#" + updateId);
                        node.children().eq(0).removeClass("label-strikeThrough");
                        node.children().eq(1).remove(); // Remove excluded indicator
                        $("#" + "Repository" + node.data("id")).removeClass("hideElement");
                        break;
                    case "ExcludeRepo":
                        // Make changes to process icon
                        processNode.data("mode", "IncludeRepo");
                        processNode.data("source", "/PrjProcessTailor/IncludeRepo");
                        processNode.attr("title", "Click to Include");
                        processNode.children().eq(0).removeClass("icon-remove");
                        processNode.children().eq(0).addClass("icon-ok");
                        // Display effect on processed node
                        var node = $("#" + updateId);
                        node.children().eq(0).addClass("label-strikeThrough");
                        node.children().eq(0).after('&nbsp;<span class="label label-danger">Excluded</span>');
                        $("#" + "Repository" + node.data("id")).addClass("hideElement");
                        break;
                    default:
                        break;
                }
            },
            error: function (data) {
                if (mode == "delete" || mode == 'ExcludeRepo' || data.responseText.indexOf("DELETE") > 0) {
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
                    $("#" + updateId).empty().off("*");
                    $("#" + updateId).html(data.responseText);
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
                        case "deleteRecord":
                            // delete the process recording
                            $("#" + updateId).replaceWith(data);
                            break;
                        case "search":
                        case "searchTasks":
                        case "searchEmployees":
                            $("#" + updateId).empty().off("*");
                            $("#" + updateId).html(data);
                            break;
                        case "update":
                            $.pnotify({
                                title: 'Saved Successfully',
                                type: 'info'
                            });
                            $("#" + updateId).empty().off("*");
                            $("#" + updateId).html(data);
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
                switch (mode)
                {
                    case "searchEmployees":
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        $("#openDialogBox").modal("show");
                        break;
                    case "delete":
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
                        break;
                    case "search":
                    case "update":
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data.responseText);
                        break;
                    case "searchTasks":
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        $("#openDialogBox").modal("show");
                    default:
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        break;
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
                        $("#" + pageUpdateID).empty().off("*");
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
                        $("#" + updateId).empty().off("*");
                        $("#" + updateId).html(data.responseText);
                    }
                    else {
                        $("#openDialogBox").empty().off("*");
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

    $("body").delegate(".uploadVersion", "click", function () {
        $("#selectedDoc").val($(this).data("id"));  // For updating with changes
        dialog = $("#openDialogBox");
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
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
    });

    $("body").delegate(".openExtraWorkflow", "click", function () {
        var parentDialog = $(this).parent().parent().parent().parent().parent().parent();
        var zindex = parentDialog.css("z-index");
        
        if (zindex == "auto") {
            zindex = 1050;
            parentDialog.css("z-index", zindex);
        }

        var dialog = $("#extraDialogBox");  
        dialog.css("z-index", (zindex + 1)); // so that this dialog appears on the top of the parent dialog box

        if ($(this).data("confirm") == "True") {
            $("#revCommentsDialog").css("z-index", zindex + 2);
            $("#revCommentsDialog").html('<div class="modal-dialog" style=width:30%;">' +
                            '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                    '<button type="button" class="close closeDialog"  data-id="revCommentsDialog" aria-hidden="true">&times;</button>' +
                                    '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                                '</div>' +
                                    '<div class="modal-body"> ' +
                                        '<div class="row col-md-12">' +
                                            '<p class="control-label col-md-12 text-center">Do you want to proceed with this action ?</p>' +
                                        '</div>' +
                                    '</div>' +
                                    '<div class="modal-footer">' +
                                        '<div class="form-group">' +
                                            '<button type="button" class="btn gray-bg closeDialog" data-id="revCommentsDialog">No</button>' +
                                            '<button type="button" class="btn blue-bg openExtraWorkflow" data-id="extraDialogBox" data-source="' +
                                            $(this).data("source") + '" data-method="' + $(this).data("method") + '" data-form="' + $(this).data("form") +
                                            '" data-status="' + $(this).data("status") + '" data-userid="' + $(this).data("userid") + '" data-usertype="' +
                                            $(this).data("usertype") + '" data-message="' + $(this).data("message") + '" data-confirm="false" data-key="' +
                                            $(this).data("key") + '" data-workflow="' + $(this).data("workflow") +
                                            '" data-statustype = ' + $(this).data("statustype") + '>Yes</button>' +
                                        '</div>' +
                                    '</div>' +
                            '</div>' +
                        '</div>')
            $("#revCommentsDialog").modal("show");
            return;
        }
        else {
            $("#revCommentsDialog").modal("hide");
        }
        var form = $(this).data("form");
        var updateId = $(this).data("id");
        var source = $(this).data("source");
        var method = $(this).data("method");
        var status = $(this).data("status");
        var statusType = $(this).data("statustype");
        var userid = $(this).data("userid");
        var usertype = $(this).data("usertype");
        var message = $(this).data("message");
        var key = $(this).data("key");
        var workflow = $(this).data("workflow");
        var method = $(this).data("method");

        $("#workflow_status").val(status);
        $("#followWF").val(true);
        $("#statusWF").val(status);
        $("#workflow").val(workflow);
        $("#StatusType").val(statusType);

        if (usertype == "user") {
            // The next workflow user is already identified, hence proceed with the workflow
            $("#workflowUser").val(userid);
            $.ajax({
                cache: false,
                async: true,
                type: method,
                url: source,
                data: (method == "POST" ? $("#" + form).serialize() : { 'id': key, 'workflowUser': userid, 'status': status, 'message': message, 'workflow': workflow }),
                success: function (data) {
                    dialog.empty().off("*");
                    dialog.html(data);
                    dialog.modal("show");
                },
                error: function (data) {
                    $.pnotify({
                        title: "Check for errors",
                        type: 'info'
                    });
                    dialog.empty().off("*");
                    dialog.html(data.responseText);
                    dialog.modal("show");
                }
            });
        }
        else {
            // The next user in the workflow is not identified, hence forward this request to get the user first. Workflow will proceed from there.
            $.ajax({
                cache: false,
                async: true,
                type: "GET",
                url: usertype,
                data: { 'formName': form, 'workflowUser': userid, 'updateID': key, 'source': source, 'message': message, 'workflow': workflow, 'method' : method, 'status' : status, 'displayID' : 'extraDialogBox' },
                success: function (data) {
                    dialog.empty().off("*");
                    dialog.html(data);
                    dialog.modal("show");
                },
                error: function (data) {
                    dialog.empty().off("*");
                    dialog.html(data.responseText);
                    dialog.modal("show");
                }
            });
        }
    });

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
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
        //dialog.modal("show");
    });

    $("body").delegate(".openDialogExtra", "click", function () {
        var dialog = $("#" + $(this).data("dialog"));
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
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
        //dialog.modal("show");
    });

    $("body").delegate(".completeTask", "click", function () {
        var dialog = $("#openDialogBox");
        var source = $(this).data("source");
        var updateID = $(this).data("id");
        var month = viewDate.getMonth() + 1;
        var day = viewDate.getDate();

        source = source + "&viewDate=" + (day < 10 ? '0' : '') + day + "/" + (month < 10 ? '0' : '') + month + '/' + viewDate.getFullYear();
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            success: function (data) {
                $.pnotify({
                    title: 'Task is marked Complete',
                    type: 'info'
                });
                $("#" + updateID).empty().off("*");
                $("#" + updateID).html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
                dialog.modal("show");
            }
        });
        //dialog.modal("show");
    });

    $("body").delegate(".reopenTask", "click", function () {
        var dialog = $("#openDialogBox");
        var source = $(this).data("source");
        var updateID = $(this).data("id");
        var month = viewDate.getMonth() + 1;
        var day = viewDate.getDate();

        source = source + "&viewDate=" + (day < 10 ? '0' : '') + day + "/" + (month < 10 ? '0' : '') + month + '/' + viewDate.getFullYear();
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            success: function (data) {
                $.pnotify({
                    title: 'Task is Re-opened',
                    type: 'info'
                });
                $("#" + updateID).empty().off("*");
                $("#" + updateID).html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
                dialog.modal("show");
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
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
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
                                '<button type="button" class="close closeDialog" data-id="openDialogBox" aria-hidden="true">&times;</button>' +
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
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="openDialogBox">No</button>' +
                                        '<button type="button" class="btn blue-bg saveDefault"  data-dismiss="modal" data-mode="delete" data-id = "' + $(this).data("id") + '" data-form="submitLevel">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".deleteEditedDocument", "click", function () {
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
                                        '<button type="button" class="btn blue-bg saveDefault"  data-dismiss="modal" data-mode="deleteRecord" data-id = "' + $(this).data("id") + '" data-form="submitLevel">Yes</button>' +
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
                                        '<button type="button" class="btn gray-bg closeDialog" data-id="extraDialogBox">No</button>' +
                                        '<button type="button" class="btn blue-bg saveDefaultExtra" data-mode="' + mode + '" data-id = "' + $(this).data("id") + '" data-form="submitLevel" data-dialog="extraDialogBox" data-pageid="' + pageUpdateID + '">Yes</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".relativeAddNew", "click", function () {
        var dialog = $("#" + $(this).data("dialog"));
        if (dialog.length == 0) dialog = $("#openDialogBox");
        var sourceID = "#" + $(this).data("sourceid");
        var source = $(this).data("source")
        dialog.html('<div class="modal-dialog" style="width:30%;">' +
                '<div class="modal-content">' +
                        '<div class="modal-body"> ' + processing("Processing. Please wait ") + '</div>' +
                 '</div>' +
             '</div>');

        if ($(sourceID).find("option:selected").eq(0).val() != 0) source = source +  "?value=" + $(sourceID).find("option:selected").eq(0).val() + "&text=" + $(sourceID).find("option:selected").eq(0).text();
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: source,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
        dialog.modal("show");
    });

    $("body").delegate(".relativeAddNew2", "click", function () {
        var dialog = $("#" + $(this).data("dialog"));
        if (dialog.length == 0) dialog = $("#openDialogBox");
        var sourceID = "#" + $(this).data("sourceid");
        var source = $(this).data("source")
        dialog.html('<div class="modal-dialog" style="width:30%;">' +
                '<div class="modal-content">' +
                        '<div class="modal-body"> ' + processing("Processing. Please wait ") + '</div>' +
                 '</div>' +
             '</div>');

        if ($(sourceID).find("option:selected").eq(0).val() != 0)
        {
            source = source + "?id=" + $(this).data("id") + "&value=" + $(sourceID).find("option:selected").eq(0).val() + "&text=" + $(sourceID).find("option:selected").eq(0).text();
            // Search for documents already attached
            var exclude = "";
            var key = 1;
            $(".mappedID").each(function (index, element) {
                var selectedID = $(element).val();
                var docType = $("#" + $(element).prop("id") + "DocType").val();
                if (selectedID >= key) key = parseInt(selectedID) + 1;
                exclude = exclude + (exclude == "" ? "" : ",") + docType + "-" + selectedID;
            });
            source = source + "&excludeIDs=" + exclude + "&key=" + key;
        }
        else
        {
            dialog.html('<div class="modal-dialog" style="width:30%;">' +
                    '<div class="modal-content">' +
                            '<div class="modal-body"> Insufficient input !! </div>' +
                     '</div>' +
		             '<div class="modal-footer">' +
    			        '<div class="form-group">' +
		                    '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                        '</div>' +
                    '</div>' +
                 '</div>');
            dialog.modal("show");
        }
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
        dialog.modal("show");
    });

    $("body").delegate(".editDropDown", "click", function () {
        var dialog = $("#" + $(this).data("dialog"));
        var updateID = $(this).data("id");
        var sourceID = $("#" + $(this).data("sourceid")).find("option:selected").eq(0).val();  // Generic drop down
        var source = $(this).data("source")     // URL to be requested
        // More codes can be accomodated as and when needed
        if (sourceID != 0) source = source + "?id=" + sourceID; else source = source + "?id=";
        if (updateID != "" && updateID != "Undefined") source = source + "&callerID=" + updateID;
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: source,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
        dialog.modal("show");
    });

    $("body").delegate(".editProjectPlans", "click", function () {
        var dialog = $("#" + $(this).data("dialog"));
        var sourceID = $("#" + $(this).data("sourceid")).find("option:selected").eq(0).val();  // Plan drop down
        var source = $(this).data("source")     // URL to be requested
        var projectID = $("#" + $(this).data("projectid")).find("option:selected").eq(0).val(); // Selected project id
        var calledBy = $(this).data("calledby");    // To identify the caller. Plans can be added from multiple locations. 1 - Called from Filter, 2 - Called while adding new task
                                                    // More codes can be accomodated as and when needed
        if (sourceID != 0) source = source + "?id=" + sourceID; else source = source + "?id=";
        if (projectID != 0) source = source + "&projectID=" + projectID;
        if (calledBy != 0) source = source + "&caller=" + calledBy;

        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: $(this).data("id"),
            url: source,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
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
        $(".chzn-container").each(function () {
            $(this).removeAttr("style");
            $(this).attr("style", "width:100%");
        });
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
            ddl.empty().off("*");
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
                                        '<a class="tree-toggle closed repositoryDetails loadRepo" data-toggle="branch" role="branch" data-id="' + element.ID + '" id="Node' + element.ID + '" href="javascript:;">' +
                                            '<span id ="LvlDesc' + element.ID + '" class="' + (element.IsActivity == 'Y' ? 'blue-bg' : '') + '">' + (element.Sequence == null || element.Sequence == 0 ? '' : element.Sequence + '. ') + element.levelName + ' : ' + element.nodeName + '&nbsp;&nbsp;</span>' +
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
                                                '<span class="' + (element.IsActivity == "Y" ? 'label label-info' : '') + '">' + element.levelName + ' - ' + element.nodeName + '</span>' +
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
                                                '&nbsp;&nbsp;<span class="label label-info">' + element.levelName + ' - ' + element.nodeName + '</span>' +
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

    $("body").delegate("#resetMapping", "click", function () {
        var projectID = $(this).data("project");
        var phaseID = $(this).data("phase");
        var source = $(this).data("source");
        var updateid = $(this).data("updateid");

        var dialog = $("#openDialogBox");

        if (projectID == null || projectID == "") {
            dialog.html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ....</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Select the project / phase to configure before invoking this action</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg"  data-dismiss="modal">Close</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
            dialog.modal("show");
            return;
        }
        // Ask for confirmation
        dialog.html('<div class="modal-dialog" style=width:30%;">' +
                '<div class="modal-content">' +
                    '<div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                        '<h4 class="modal-title" id="dialogTitle">Confirm Action</h4>' +
                    '</div>' +
                    '<form method="POST" action="' + source + '" id="refreshForm">' +
                        '<div class="modal-body"> ' +
                            '<div class="row col-md-12">' +
                                '<input type="hidden" id="id" name="id" value="' + projectID + '"/>' +
                                '<input type="hidden" id="phaseID" name="phaseID" value="' + phaseID + '"/>' +
                                '<p class="control-label col-md-12 text-center"><strong>Do you want to refresh the process mapping ?</strong></p>' +
                                '<p class="control-label col-md-12 text-center">Note: The existing process mappings will be overwritten. You will loose process tailoring if any.</p>' +
                            '</div>' +
                        '</div>' +
                        '<div class="modal-footer">' +
                            '<div class="form-group">' +
                                '<button type="button" class="btn gray-bg"  data-dismiss="modal">No</button>' +
                                '<button type="button" class="btn blue-bg save" data-mode="projectRepository" data-id = "' + updateid + '" data-form="refreshForm">Yes</button>' +
                            '</div>' +
                        '</div>' +
                    '</form>' +
                '</div>' +
            '</div>');
        dialog.modal("show");
    });

    $("body").delegate(".showProjectMapping", "click", function () {
        $("#selectedNode").html("Mapping <i class='icon-circle-arrow-right'> </i>" + $(this).data("name"));
        $("#resetMapping").data("project", $(this).data("project"));
        $("#resetMapping").data("phase", $(this).data("phase"));
        var dataParam = $(this).data("param");
        var treeRoot = $("#" + $(this).data("treeroot"));
        var className = $(this).data("classname");


        $("#processContainer").data("param", dataParam);

        $(".FocusedItem").removeClass("FocusedItem");
        $(this).siblings("a").eq(0).addClass("FocusedItem");

        $.getJSON('/PrjProcessTailor/getMapping?' + dataParam, function (result) {
            printProjectMapping(treeRoot, result);
        }).fail(function (result) {
            $.pnotify({
                title: 'Error in configuration setting',
                type: 'info'
            });
            $(".FocusedItem").removeClass("FocusedItem");
            $("#selectedNode").html('Click <i class="icon-wrench"></i> icon to configure processes.');
        });
    });

    function printProjectMapping(treeRoot,result) {
        var ddl = treeRoot;
        ddl.empty();
        // First add an option to add a new node
        activeNode = ddl;
        if (result.length == 0) {
            $.pnotify({
                title: 'No Process Mappings found',
                type: 'info'
            });
            activeNode.append("<li>Process Mappings not found. Processes are mapped when a project is initiated. Click Refresh to get latest mappings</li>");
        }
        else {
            $.each(result, function (index, element) {
                if (element.Type == "Rep") // Repository
                {
                    if (element.Level == 1) {
                        activeNode = ddl; // Add to the root
                    }
                    else {
                        activeNode = ddl.find("#C" + element.ParentID).eq(0); // search for the parent id and add this node under it
                        if (activeNode.length == 0) activeNode = ddl;
                    }
                    // Append the new node
                    activeNode.append('<li><a href="javascript:;" id="tailor' + element.accessID + '" class="process" data-id="Node' + element.accessID +
                                           '" data-source="/PrjProcessTailor/' + (element.Exclude == true ? 'IncludeRepo' : 'ExcludeRepo') + '" data-param="' + element.paramName +
                                           '" data-mode="' + (element.Exclude == true ? 'IncludeRepo' : 'ExcludeRepo') + '" data-message="" title = "Click to ' + (element.Exclude == true ? 'include' : 'exclude') +  '"><i class="icon-' + (element.Exclude == true ? 'ok' : 'remove') + '"/></a>' +
                                           '<a class="tree-toggle closed" data-toggle="branch" role="branch" data-id="' + element.accessID + '" id="Node' + element.accessID + '" href="javascript:">' +
                                                '<span class="label label-info ' + (element.Exclude == true ? 'label-strikeThrough' : '') + '">' + element.levelName + ' - ' + element.nodeName + '</span>' +
                                                (element.Exclude == true ? '&nbsp;<span class="label label-danger">Excluded</span>' : '') +
                                            '</a>' +
                                            '<ul class="branch" id="C' + element.accessID + '" >' +
                                                '<li id="Repository' + element.accessID + '" class="' + (element.Exclude == true ? "hideElement" : "") + '"><a href="#" class="tree-toggle closed" data-toggle="branch"><span class="label label-repository">Repository</span></a>' +
                                                    '<ul id="Rep' + element.accessID + '" class="branch">' +
                                                        '<li><a href="#" class="tree-toggle closed" role="branch" data-toggle="branch"><span class="label label-item">Tasks / Activities</span></a>' +
                                                            '<ul class="branch" id="Task' + element.accessID + '">' +
                                                                '<li><a href="javascript:;" class="openDialog" data-source="/PrjProcessTailor/addNewTask?' + element.paramName + '" data-id="">Add New Task / Activity</a></li>' +
                                                                 '<li id="NilTask' + element.accessID + '" >No Tasks / Activities found</li>' +
                                                            '</ul>' +
                                                        '</li>' +
                                                        '<li><a href="#" class="tree-toggle closed" role="branch" data-toggle="branch"><span class="label label-item">Process Artifacts</span></a>' +
                                                            '<ul class="branch" id="Proc' + element.accessID + '">' +
                                                                '<li><a href="javascript:;" class="openDialog" data-source="/PrjProcessTailor/manageProjProcedures?' + element.paramName + '" data-id="">Click to add/remove Process Artifacts</a></li>' +
                                                                 '<li id="NilProc' + element.accessID + '" >No Process Artifacts found</li>' +
                                                            '</ul>' +
                                                        '</li>' +
                                                        '<li><a href="#" class="tree-toggle closed" role="branch" data-toggle="branch"><span class="label label-item">Templates</span></a>' +
                                                            '<ul class="branch" id="Template' + element.accessID + '">' +
                                                                '<li><a href="javascript:;" class="openDialog" data-source="/PrjProcessTailor/manageProjTemplates?' + element.paramName + '" data-id="">Click to add/remove Templates</a></li>' +
                                                                 '<li id="NilTemplate' + element.accessID + '" >No Templates found</li>' +
                                                            '</ul>' +
                                                        '</li>' +
                                                        '<li><a href="#" class="tree-toggle closed" role="branch" data-toggle="branch"><span class="label label-item">Checklists</span></a>' +
                                                            '<ul class="branch" id="Checklist' + element.accessID + '">' +
                                                                '<li><a href="javascript:;" class="openDialog" data-source="/PrjProcessTailor/manageProjChecklists?' + element.paramName + '" data-id="">Click to add/remove Checklists</a></li>' +
                                                                 '<li id="NilChecklist' + element.accessID + '" >No Checklists found</li>' +
                                                            '</ul>' +
                                                        '</li>' +
                                                        '<li><a href="#" class="tree-toggle closed" role="branch" data-toggle="branch"><span class="label label-item">Documents</span></a>' +
                                                            '<ul class="branch" id="Document' + element.accessID + '">' +
                                                                '<li><a href="javascript:;" class="openDialog" data-source="/PrjProcessTailor/manageDocuments?' + element.paramName + '" data-id="">Click to add/remove Documents</a></li>' +
                                                                 '<li id="NilDoc' + element.accessID + '" >No Documents found</li>' +
                                                            '</ul>' +
                                                        '</li>' +
                                                    '</ul>' +
                                                '</li>' +
                                            '</ul>' +
                                       '</li>');
                }
                else // Activity, Procedure, Template, Checklist etc
                {
                    activeNode = ddl.find("#" + element.Type + element.ParentID).eq(0); // search for the parent id and add this node under it
                    activeNode.children("#Nil" + element.Type + element.ParentID).eq(0).remove(); // Remove status "No items found
                    // Append the new node
                    activeNode.append('<li id = "L' + element.Type + element.ID + '">' +
                                            (element.paramName == "" ? '' : '<a href="javascript:;" class="process" data-mode="edit" data-id="L' + element.Type + element.ID + '" data-source="' + element.paramName +
                                            '" data-param="' + element.ID + '" data-message="' + element.levelName + '" title="Click to ' + (element.levelID == 1 ? 'include' : 'exclude') +
                                            ' this task"><i class="' + (element.levelID == 1 ? 'icon-ok' : 'icon-remove') + '"></i></a>') +
                                            '&nbsp;&nbsp;<span class="label label-' + (element.levelID == 1 ? 'strikeThrough' : 'configured') + '">' +
                                            (element.previewURL == "" ? '' : '<a href="javascript:" class="label-configured ' + element.previewClass + '" data-id="" data-source="' + element.previewURL + '">') +
                                            element.nodeName +
                                            (element.previewURL == "" ? '' : '</a>') +
                                            '</span>' + (element.levelID == 1 ? '&nbsp;<span class="label label-danger">Excluded</span>' : '') +
                                       '</li>');
                }
            });
        }

    }
    function addSection(activeNode, index, sequence, title, detail, procID, grpID, type) {
        var functionName = (type == "proc" ? "Procedure" : "Template");
        var node;
        if (activeNode == "#DefaultDocNode") node = $(activeNode).children().last().prev();
        else node = $(activeNode).children().last();
        node.before('<li class="sortable-item MainSection" id="Section' + index + '">' +
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
        // Go to the new section
        $('html, body').animate({
            scrollTop: $("#" + "Section" + index).offset().top
        }, 2000);
        $("#sectionTitle" + index).focus();
    };

    function configureCkEditor(controlID) {
        CKEDITOR.config.pasteFromWordRemoveFontStyles = false;
        CKEDITOR.config.pasteFromWordRemoveStyles = false;
        CKEDITOR.config.extraAllowedContent = 'td{background}';
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            var editor = CKEDITOR.replace(this.id);
            editor.setData($(this).text());
        });
    }

    function configureCkSmallEditor(controlID) {
        CKEDITOR.config.pasteFromWordRemoveFontStyles = false;
        CKEDITOR.config.pasteFromWordRemoveStyles = false;
        CKEDITOR.config.extraAllowedContent = 'td{background}';
        CKEDITOR.config.toolbar = [
                    ['Bold', 'Italic', 'Underline', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                    ['Outdent', 'Indent']];
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            var editor = CKEDITOR.replace(this.id, {
                uiColor: '#14B8C4', toolbar: [
                    ['Bold', 'Italic', 'Underline', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                    ['Outdent', 'Indent']]
            });
            editor.setData($(this).text());
        });
    }

    function configureInlineCkEditor(controlID) {
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            CKEDITOR.inline(textDetail.attr("id"), {
                uiColor: '#14B8C4', toolbar: [
                    ['Bold', 'Italic','Underline', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                    ['Outdent', 'Indent']]
            });
        });
    }

    function configureInlineCkEditorFullMenu(controlID) {
        $(controlID).each(function (index, Element) {
            var textDetail = $(this);
            CKEDITOR.inline(textDetail.attr("id"));
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
        for (var i in CKEDITOR.instances) {
            CKEDITOR.instances[i].updateElement();
        }
        var textValue = "";
        $(className).each(function () {      // Set sequences
            $(this).val($(this).val().replace(new RegExp("&nbsp;", 'g'), "&#160;"));
            textValue = $(this).val();
            $.each(invalidCharacters,function (index, element) {
                if (textValue.indexOf(element) >= 0) textValue = textValue.replace(new RegExp(element, 'g'), replaceCharacter[index]);
            });
            $(this).val(textValue);
            textValue = "";
        }); 
    };

    var reportError = function () {
        var textValue = "";
        var titleID = "";

        for (var i in CKEDITOR.instances) {
            textValue = CKEDITOR.instances[i].getData();
            titleID = "#" + i + "Title";

            var arrSpecialChars = textValue.match(new RegExp("&\.+?;", 'g'));
            if ($(titleID).hasClass("red-bg")) {
                $(titleID).removeClass("red-bg");
                $(titleID).addClass("blue-bg");
            }
            if (arrSpecialChars != null) {
                $.each(arrSpecialChars, function (index, element) {
                    if (element != "&nbsp;" && replaceCharacter.indexOf(element) < 0) {
                        if (!$(titleID).hasClass("red-bg")) $(titleID).addClass("red-bg");
                        if ($(titleID).hasClass("blue-bg")) $(titleID).removeClass("blue-bg");
                        textValue = textValue.replace(new RegExp(element, 'g'), '<span style="color:#FF0000;background-color:#FFFF00">' + element + '</span>');
                    }
                });
                CKEDITOR.instances[i].setData(textValue);   // Update the updated string to indicate relevant error
                CKEDITOR.instances[i].updateElement();
            }
            textValue = "";
            titleID = "";
        }
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
                        $("#" + updateId).empty().off("*");
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
                else {
                    $("#openDialogBox").empty().off("*");
                    $("#openDialogBox").html(data.responseText);
                }
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
                else {
                    $("#openDialogBox").empty().off("*");
                    $("#openDialogBox").html(data.responseText);
                }
            }
        });
    });


    $("body").delegate(".confirmAction", "click", function () {
        // Generic confirmation dialog
        currentScrollPosition = $(window).scrollTop();
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
                                    '<div class="row">' +
                                        '<div class="form-group col-md-12">' +
                                            '<label class="control-label col-md-12">' + msg + '</label>' +
                                        '</div>' +
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
                $("#" + updateId).empty().off("*");
                $("#" + updateId).html(data);
            },
            error: function (data) {
                if (data.responseText.indexOf("undeclared entity") > 0) {
                    $.pnotify({
                        title: '<strong class="label label-danger">Operation Terminated</strong> <hr/> Illegal characters found in the input. This may happen due to copy/paste of information. <br/>',
                        type: 'error'
                    });
                    reportError();
                }
                else
                {
                    $.pnotify({
                        title: 'Check Errors',
                        type: 'error'
                    });
                    $("#" + updateId).html(data.responseText);
                }
                //$("#openDialogBox").html(data.responseText);
            }
        });
    });

    $("body").delegate(".saveDefaultWithoutReview", "click", function () {
        $("#formReviewComments").val("");           // Review comments not needed
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
                $("#" + updateId).empty().off("*");
                $("#" + updateId).html(data);
            },
            error: function (data) {
                if (data.responseText.indexOf("undeclared entity") > 0) {
                    $.pnotify({
                        title: '<strong class="label label-danger">Operation Terminated</strong> <hr/> Illegal characters found in the input. This may happen due to copy/paste of information. <br/>',
                        type: 'error'
                    });
                    reportError();
                }
                else {
                    $.pnotify({
                        title: 'Check Errors',
                        type: 'error'
                    });
                    $("#" + updateId).html(data.responseText);
                }
                //$("#openDialogBox").html(data.responseText);
            }
        });
    });

    $("body").delegate(".saveRecord", "click", function () {
        $("#Contents").val($("#filledContents").html());

        processHTMLData("#Contents");

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
                $("#" + updateId).replaceWith(data);
            },
            error: function (data) {
                if (data.responseText.indexOf("undeclared entity") > 0) {
                    $.pnotify({
                        title: '<strong class="label label-danger">Operation Terminated</strong> <hr/> Illegal characters found in the input. This may happen due to copy/paste of information. <br/>',
                        type: 'error'
                    });
                }
                else {
                    $.pnotify({
                        title: 'Check Errors',
                        type: 'error'
                    });
                    $("#" + updateId).empty().off("*");
                    $("#" + updateId).html(data.responseText);
                }
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
        idToDelete = $(this).data("id");
        $("#Section" + idToDelete).remove();  // Hide this section
        $('html, body').animate({
            scrollTop: currentScrollPosition
        });
    });

    $("body").on("click", ".removeItem", function () {
        idToDelete = $(this).data("id");
        $("#Item" + idToDelete).remove();  // Hide this section
        $('html, body').animate({
            scrollTop: currentScrollPosition
        });
    });

    $("body").on("click", ".minimise", function () {
        var scroll = $(window).scrollTop();
        $(this).parent().parent().parent().next().slideToggle('fast')
        //$(this).parent().parent().parent().next().toggle().slow();
        // scroll to relevent section
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
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
        if ($("#editGrpName").val() == "" || $("#editGrpName").val().length > 50) {
            var message = "";
            if ($("#editGrpName").val() == "") message = "Pl. enter the group name.";
            else message = "Group name cannot be more than 50 characters.";
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">' + message + '</p>' +
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
        if ($("#newGrpName").val() == "" || $("#newGrpName").val().length > 50) {
            var message = "";
            if ($("#newGrpName").val() == "") message = "Pl. enter the group name.";
            else message = "Group name cannot be more than 50 characters.";
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">' + message + '</p>' +
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
            $("#items").data("nextgroupid", parseInt(newGrpId) + 1);
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
            newGroup = newGroup + '<li><a href="Javascript:;" class="newSection btn btn-med blue-bg" data-parent="' + newGrpId + '" data-proc="' + procID + '" data-type="' + type + '">Add Section</a><hr/></li>' +
                            '</ul>' +
                        '</li>';
        }
        if (mode == "root")   // Add as a new group under the root document
        {
            if (type == "chklst") {
                $("#L" + node).before(newGroup);        // ************ Check Impact, review and rewrite. Was changed from .prev().before to .before
            }
            else {
                $("#L" + node).prev().before(newGroup);        // ************ Check Impact, review and rewrite. Was changed from .prev().before to .before
            }
        }
        else  // Add as a sub node
        {
            $("#C" + node).children("li").last().before(newGroup);
        }
        $("#sections").data("nextgroupid", parseInt(newGrpId) + 1)        // Update the new group id
    
        // For allowing drag & drop feature
        //$('#procDef .sortable-list').unbind();
        $(".sortable-list").sortable({ cancel: ".EditEditor", connectWith: '.sortable-list' });
        $('html, body').animate({
            scrollTop: $("#L" + newGrpId).offset().top
        }, 2000);
    });

    $("body").delegate(".addSubGroup", "click", function () {      // Function called when the user provides group name and clicks save
        // Check whether group name is provided
        if ($("#editGrpName").val() == "" || $("#editGrpName").val().length > 50) {
            var message = "";
            if ($("#editGrpName").val() == "") message = "Pl. enter the group name.";
            else message = "Group name cannot be more than 50 characters.";
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">' + message + '</p>' +
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
        var level = 1;
        var procID = $("#NodeProc").val();
        var node = $("#editNodeID").val();
        var type = $("#type").val();
        if (type == "chklst") {
            newGrpId = $("#items").data("nextgroupid");
            $("#items").data("nextgroupid", parseInt(newGrpId) + 1);
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
            newGroup = newGroup + '<li><a href="Javascript:;" class="newSection btn btn-med blue-bg" data-parent="' + newGrpId + '" data-proc="' + procID + '" data-type="' + type + '">Add Section</a><hr/></li>' +
                            '</ul>' +
                        '</li>';
        }

        $("#C" + node).children("li").last().before(newGroup);   // Add this node as a child node
        $("#sections").data("nextgroupid", newGrpId + 1)        // Update the new group id

        // For allowing drag & drop feature
        //$('#procDef .sortable-list').unbind();
        //$('#procDef .sortable-list').sortable({
          //  connectWith: '#procDef .sortable-list'
        //});
        $(".sortable-list").sortable({ cancel: ".EditEditor", connectWith: '.sortable-list' });
        $('html, body').animate({
            scrollTop: $("#L" + newGrpId).offset().top
        }, 2000);
        
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
                $("#openDialogBox").empty().off("*");
                $("#openDialogBox").html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty().off("*");
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
        $('html, body').animate({
            scrollTop: $("#Item" + id).offset().top
        });
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
                                        '<div class="row EditRow">' +
                                            '<div class="col-lg-1 NoEdit CheckItem" data-width="2px">&nbsp;&nbsp;&nbsp;&nbsp;<a href="#" class="icon-button confirmAction" data-messagetitle="Confirm Deletion" data-message="Do you want to delete this check item ?" data-class="removeItem" data-classparams="data-id=' + index + '" data-id="Check' + index + '"><i class="icon-">&#xf05c</i></a></div>' +
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
                $("#configureProcess").empty().off("*");
                $("#configureProcess").html(data);
            },
            error: function (data) {
                $("#configureProcess").empty().off("*");
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

    $("body").delegate(".loadProjList", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();

        if (selectedID == "") {
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
            '<div class="modal-content">' +
                '<div class="modal-header">' +
                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                    '<h4 class="modal-title" id="dialogTitle">Error ...</h4>' +
                '</div>' +
                    '<div class="modal-body"> ' +
                        '<div class="row col-md-12">' +
                            '<p class="control-label col-md-12 text-center">Pl. select a project.</p>' +
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
        var loadUplDocs = $("#uplDocs").prop("checked");
        var loadPrsDocs = $("#prsDocs").prop("checked");
        var loadRecDocs = $("#trnDocs").prop("checked");
        var processTable = false;

        if (!loadUplDocs && !loadPrsDocs && !loadRecDocs) {
            $('#processTable').dataTable().fnDestroy();
            $("#listing").html("");
            jsTable("processTable");
            return;
        }
        var source = $("#projectParams").data("source");
        if (loadUplDocs) {
            $.get(source + "?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                $('#processTable').dataTable().fnDestroy();
                $("#listing").html(result);
                if (loadPrsDocs) {
                    $.get("/Project/GetProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                        $("#listing").append(result);
                        if (loadRecDocs) {
                            $.get("/TimesheetRecord/ViewProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                                $("#listing").append(result);
                                jsTable("processTable");
                            });
                        }
                        else {
                            jsTable("processTable");
                        }
                    });
                }
                else {
                    if (loadRecDocs) {
                        $.get("/TimesheetRecord/ViewProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                            $("#listing").append(result);
                            jsTable("processTable");
                        });
                    }
                    else {
                        jsTable("processTable");
                    }
                }
            });
        }
        else {
            if (loadPrsDocs) {
                $.get("/Project/GetProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                    $('#processTable').dataTable().fnDestroy();
                    $("#listing").html(result);
                    if (loadRecDocs) {
                        $.get("/TimesheetRecord/ViewProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                            $("#listing").append(result);
                            jsTable("processTable");
                        });
                    }
                    else {
                        jsTable("processTable");
                    }
                });
            }
            else {
                if (loadRecDocs) {
                    $.get("/TimesheetRecord/ViewProcessDocuments?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                        $('#processTable').dataTable().fnDestroy();
                        $("#listing").html(result);
                        jsTable("processTable");
                    });
                }
            }
        }
    });

// This function is used to populate any project related details on change of project selection.
// It loads details in a div tag called listing
    $("body").delegate(".loadProjDetails", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        var source = $("#" + $(this).prop("id") + "Params").data("source");
        if (selectedID == "") {
            $("#listing").html("Select a project");
        }
        else {
            $("#listing").html(processing("Loading "));
            $.get(source + "?id=" + (selectedID == "" ? "0" : selectedID), function (result) {
                $("#listing").html(result);
            }).fail(function (result) {
                $("#listing").html("Operation Failed. Pl. retry or contact the system administrator");
            });
        }
    });

    $("body").delegate(".populateText", "change", function () {
        var selectedText = $(this).find("option:selected").eq(0).text();
        var options = $("#" + $(this).prop("id") + "Params");
        var updateID = options.data("updateid");
        $("#" + updateID).val(selectedText);
    });

// Created for project drop down which need some dependent control to be populated based on change in project.
// Pre-requisite: The project dropdown has an id by name Project and the dependent drop down information is 
// taken from another control which is accessed by name ProjectParams.
    $("body").delegate(".propogateChange", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        var options = $("#" + $(this).prop("id") + "Params");
        var updateID = options.data("updateid");
        var fillDependentData = options.data("fill");

        $(updateID).val(selectedID);
        if (fillDependentData != "undefined" && fillDependentData != "") {
            var url = options.data("fillurl");
            var populateCmb = $("#" + fillDependentData);

            $.getJSON(url + "?id=" + selectedID, function (result) {
                populateCmb.empty();
                populateCmb.append('<option value="" selected="selected">Select...</option>');
                $.each(result, function (index, element) {
                    populateCmb.append('<option value="' + element.index + '" >' + element.name + '</option>');
                });
            });
        }
    });

    $("body").delegate(".loadProjectAllocationDetails", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        // Empty the employee search results
        $("#listing").empty();
        // Load employees allocated to this project
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjAllocation/GetExistingTeam?id=" + selectedID,
            success: function (data) {
                $('#existingEmployees').dataTable().fnDestroy();
                $("#allocatedEmployees").empty().off("*");
                $("#allocatedEmployees").html(data);
                jsTable("existingEmployees");
            },
            error: function (data) {
                $("#existingEmployees").empty().off("*");
                $("#existingEmployees").html(data.responseText);
            }
        });
        // Load list of employees that have been released from the project
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjAllocation/EmployeeHistory?id=" + selectedID,
            success: function (data) {
                $('#employeeHistory').dataTable().fnDestroy();
                $("#empHistoryData").empty().off("*");
                $("#empHistoryData").html(data);
                jsTable("employeeHistory");
            },
            error: function (data) {
                $("#empHistoryData").empty().off("*");
                $("#empHistoryData").html(data.responseText);
            }
        });
    });

    $("body").delegate(".loadResourcePlanDetails", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        var source = $("#ProjectParams").data("source");
        var type = $("#ProjectParams").data("type");
        // Empty the employee search results
        $("#listing").empty();
        // Load existing plan
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source + "?id=" + selectedID,
            success: function (data) {
                $('#processTable').dataTable().fnDestroy();
                $("#listing").empty().off("*");
                $("#listing").html(data);
                jsTable("processTable");
            },
            error: function (data) {
                $("#listing").empty().off("*");
                $("#listing").html(data.responseText);
            }
        });
        if (type == "human") {

            // Load employees allocated to this project
            $.ajax({
                cache: false,
                async: true,
                type: "GET",
                url: "/ProjAllocation/GetExistingTeam?id=" + selectedID,
                success: function (data) {
                    $('#existingEmployees').dataTable().fnDestroy();
                    $("#allocatedEmployees").empty().off("*");
                    $("#allocatedEmployees").html(data);
                    jsTable("existingEmployees");
                },
                error: function (data) {
                    $("#existingEmployees").empty().off("*");
                    $("#existingEmployees").html(data.responseText);
                }
            });
            // Load list of employees that have been released from the project
            $.ajax({
                cache: false,
                async: true,
                type: "GET",
                url: "/ProjAllocation/EmployeeHistory?id=" + selectedID,
                success: function (data) {
                    $('#employeeHistory').dataTable().fnDestroy();
                    $("#empHistoryData").empty().off("*");
                    $("#empHistoryData").html(data);
                    jsTable("employeeHistory");
                },
                error: function (data) {
                    $("#empHistoryData").empty().off("*");
                    $("#empHistoryData").html(data.responseText);
                }
            });
        }
    });

    $("body").delegate(".loadDetails", "click", function () {
        var divElement = $("#" + $(this).data("element"));
        var source = $(this).data("source");
        var sourceid = $(this).data("sourceid");
        var mode = $(this).data("mode");

        if (sourceid != null && sourceid != "undefined" && sourceid != "") {
            if (mode == "documents")
                source = source + "&id=" + $("#" + sourceid).find("option:selected").eq(0).val();
            else
                source = source + "?id=" + $("#" + sourceid).find("option:selected").eq(0).val();
        }
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            success: function (data) {
                switch (mode) {
                    case "newResourceWithAddOption":
                        divElement.children("tr").last().replaceWith(data);
                        break;
                    case "showAndCloseDialog":
                        divElement.empty().off("*");
                        divElement.html(data);
                        $("#openDialogBox").modal("hide");
                        break;
                    case "edit":
                    case "documents":
                        divElement.replaceWith(data);
                        break;
                    case "timesheet":
                        divElement.html(data);
                        $.pnotify({
                            title: "Timesheet processed successfully",
                            type: 'info'
                        });
                        break;
                    default:
                        divElement.empty().off("*");
                        divElement.html(data);
                        break;
                }
            },
            error: function (data) {
                switch (mode) {
                    case "showAndCloseDialog":
                        $.pnotify({
                            title: "Check Error",
                            type: 'info'
                        });
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        break;
                    case "timesheet":
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        $("#openDialogBox").modal("show");
                        break;
                    default:
                        $.pnotify({
                            title: data.responseText,
                            type: 'info'
                        });
                        break;
                }
            }
        });
    });

    function getAllResourceDetails() {
        $(".getResourceDetails").each(function (index, element) {
            var selectedOption = $(this).find("option:selected");
            var actualStart = selectedOption.parent().parent().children().eq(1);
            var actualEnd = selectedOption.parent().parent().children().eq(2);
            var allocID = selectedOption.parent().parent().children().eq(3);

            actualStart.val(selectedOption.data("start"));
            actualEnd.val(selectedOption.data("end"));
            allocID.val(selectedOption.data("id"));

            if (selectedOption.val() == "") {
                $(this).parent().prev().children("span").eq(0).html("");
            }
            else {
                $(this).parent().prev().children("span").eq(0).html(' | <a href="javascript:;" title="Allocated from ' + selectedOption.data("start") + ' to ' + selectedOption.data("end") + '"><i class="icon-info-sign"></i></a>');
            }
        });
    };

    $("body").delegate(".getResourceDetails", "change", function () {
        var selectedOption = $(this).find("option:selected");
        var actualStart = selectedOption.parent().parent().children().eq(1);
        var actualEnd = selectedOption.parent().parent().children().eq(2);
        var allocID = selectedOption.parent().parent().children().eq(3);

        actualStart.val(selectedOption.data("start"));
        actualEnd.val(selectedOption.data("end"));
        allocID.val(selectedOption.data("id"))

        actualEnd.val(selectedOption.data("end"));
        if (selectedOption.val() == "") {
            $(this).parent().prev().children("span").eq(0).html("");
        }
        else {
            $(this).parent().prev().children("span").eq(0).html(' | <a href="javascript:;" title="Allocated from ' + selectedOption.data("start") + ' to ' + selectedOption.data("end") + '"><i class="icon-info-sign"></i></a>');
        }

    });


    $("body").delegate(".loadProjectData", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        var source = $("#ProjectParams").data("source");
        var updateID = $("#ProjectParams").data("id");
        // Load depending drop down boxes in the search filter
        if (selectedID == "" || selectedID == "0") {
            $("#openDialogBox").empty().off("*");
            $("#openDialogBox").html('<div class="modal-dialog" style=width:30%;">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                '<h4 class="modal-title" id="dialogTitle">Error</h4>' +
                            '</div>' +
                                '<div class="modal-body"> ' +
                                    '<div class="row col-md-12">' +
                                        '<p class="control-label col-md-12 text-center">Pl. select a project and re-try.</p>' +
                                    '</div>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                    '<div class="form-group">' +
                                        '<button type="button" class="btn gray-bg" data-dismiss="modal">Close</button>' +
                                    '</div>' +
                                '</div>' +
                            '</form>' +
                        '</div>' +
                    '</div>');
            $("#openDialogBox").modal("show");
            return;
        }
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source + "?id=" + selectedID,
            success: function (data) {
                $("#" + updateID).empty().off("*");
                $("#" + updateID).html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty().off("*");
                $("#openDialogBox").html(data.responseText);
                $("#openDialogBox").modal("show");
            }
        });
    });

    $("body").delegate(".loadProjectPlanDetails", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        // Load depending drop down boxes in the search filter
        $("#SearchProjectID").val(selectedID);    // Set the project id
        // Get Project Plans if any
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjPlanname/getListItemsJSONFor?projectID=" + selectedID,
            success: function (data) {
                $("#SearchPlanID").empty();
                $("#SearchPlanID").append('<option value="" selected="selected">Select...</option>');
                $.each(data, function (index, element) {
                    $("#SearchPlanID").append('<option value="' + element.index + '" id="Plan"' + element.index + '>' + element.name + '</option>');
                });
            },
            error: function (data) {
                $("#SearchPlanID").empty();
                $("#SearchPlanID").append('<option value="" selected="selected">Select...</option>');
            }
        });
        // Populate Groups
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjGroup/getListItemsJSONFor?projectID=" + selectedID,
            success: function (data) {
                $("#SearchGroupID").empty();
                $("#SearchGroupID").append('<option value="" selected="selected">Select...</option>');
                $.each(data, function (index, element) {
                    $("#SearchGroupID").append('<option value="' + element.index + '" >' + element.name + '</option>');
                });
            },
            error: function (data) {
                $("#SearchGroupID").empty();
                $("#SearchGroupID").append('<option value="" selected="selected">Select...</option>');
            }
        });
        // Populate Plan for this project
        PopulateProjectPlan(selectedID);
        // Populate plan tracking detalis
        PopulateTaskTracking(selectedID);
    });
    
    $("body").delegate(".selectOption", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0);
        $(this).find("option").each(function (index, element) {
            $(element).removeAttr("selected");
        });
        selectedID.attr("selected", "selected");
        selectedID.prop("selected", true);
    });

    $("body").delegate("getAuditPlan", "change", function () {
        // Check whether this is an observation sheet or an Audit sheet
        var AuditType = $("#AuditType").val();
        var selectedID = $(this).find("option:selected").eq(0).val();
        var updateID = $(this).data("id");

        var source = "/ProjPlanAudit/GetPlan?projectID=" & selectedID & "&type=" + AuditType;

        // Get planned tasks for this project
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: source,
            success: function (data) {
                $("#" + updateID).empty().off("*");
                $("#" + updateID).html(data);
            },
            error: function (data) {
                $("#" + updateID).empty().off("*");
                $("#" + updateID).html(data.responseText);
            }
        });
    });

    $("body").delegate(".loadTaskRecordings", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        // Load depending drop down boxes in the search filter
        $("#SearchProjectID").val(selectedID);    // Set the project id

        // Get Task recordings for the selected project
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/TimesheetRecord/ProjectRecording?Project=" + selectedID,
            success: function (data) {
                $("#listing").empty().off("*");
                $("#listing").html(data);
            },
            error: function (data) {
                $("#listing").empty().off("*");
                $("#listing").html(data.responseText);
            }
        });
        // Populate Groups
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjGroup/getListItemsJSONFor?projectID=" + selectedID,
            success: function (data) {
                $("#SearchGroupID").empty();
                $("#SearchGroupID").append('<option value="" selected="selected">Select...</option>');
                $.each(data, function (index, element) {
                    $("#SearchGroupID").append('<option value="' + element.index + '" >' + element.name + '</option>');
                });
            },
            error: function (data) {
                $("#SearchGroupID").empty();
                $("#SearchGroupID").append('<option value="" selected="selected">Select...</option>');
            }
        });
        // Populate Tasks
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/MapRepository/getProjectTasksJSON?projectID=" + selectedID,
            success: function (data) {
                $("#SearchTaskID").empty();
                $("#SearchTaskID").append('<option value="" selected="selected">Select...</option>');
                var groupName = "";
                var counter = 0;
                while (counter < data.count) {
                    if (data[counter].GroupName == "") {
                        $("#SearchTaskID").append('<option value="' + data[counter].ID + '" >' + data[counter].DisplayText + '</option>');
                    }
                    else {
                        groupName = data[counter].GroupName;
                        $("#SearchTaskID").append('<optgroup label="' + groupName + '">');
                        while (counter < data.count && groupName == data[counter].GroupName) {
                            $("#SearchTaskID").append('<option value="' + data[counter].ID + '" >' + data[counter].DisplayText + '</option>');
                            counter++;
                        }
                        $("#SearchTaskID").append('</optgroup>');
                    }
                }
            },
            error: function (data) {
                $("#SearchTaskID").empty();
                $("#SearchTaskID").append('<option value="" selected="selected">Select...</option>');
            }
        });
    });

    function PopulateProjectPlan(projectID) {
        if (projectID == "") return;
        
        $("#listing").empty().off("*");
        $("#listing").html(processing("Loading "));
        // Populate tracking information - By Task
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjPlan/GetPlannedTasks?Project=" + projectID,
            success: function (data) {
                $("#listing").empty().off("*");
                $("#listing").html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty().off("*");
                $("#openDialogBox").html(data.responseText);
                $("#openDialogBox").modal("show");
            }
        });
    }

    function PopulateTaskTracking(projectID) {
        if (projectID == "") return;
        var month = viewDate.getMonth() + 1;
        var day = viewDate.getDate();

        var today = viewDate.getFullYear() + "/" + (month<10 ? '0' : '') + month + '/' + (day<10 ? '0' : '') + day;

        $("#trackByTask").empty();
        $("#trackByTask").html(processing("Loading "));
        // Populate tracking information - By Task
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjPlan/getTaskWiseTracking?projectID=" + projectID + "&reportingDate=" + today,
            success: function (data) {
                $("#trackByTask").empty().off("*");
                $("#trackByTask").html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty().off("*");
                $("#openDialogBox").html(data.responseText);
                $("#openDialogBox").modal("show");
            }
        });
        
        $("#trackByResource").empty();
        $("#trackByResource").html(processing("Loading "));
        // Populate tracking information - By Resource
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/ProjPlan/getResourceWiseTracking?projectID=" + projectID + "&reportingDate=" + today,
            success: function (data) {
                $("#trackByResource").empty().off("*");
                $("#trackByResource").html(data);
            },
            error: function (data) {
                $("#openDialogBox").empty().off("*");
                $("#openDialogBox").html(data.responseText);
                $("#openDialogBox").modal("show");
            }
        });  
    }

    $("body").delegate("#showTaskView", "click", function () {
        $("#ResourceView").hide();
        if (!$(this).parent().hasClass("active")) $(this).parent().addClass("active");
        if ($("#showResView").parent().hasClass("active")) $("#showResView").parent().removeClass("active");
        $("#TaskView").show();
    });

    $("body").delegate("#showResView", "click", function () {
        $("#TaskView").hide();
        if (!$(this).parent().hasClass("active")) $(this).parent().addClass("active");
        if ($("#showTaskView").parent().hasClass("active")) $("#showTaskView").parent().removeClass("active");
        $("#ResourceView").show();
    });

    $("body").delegate(".addProjDetail", "click", function () {
        var selectedID = $("#Project").find("option:selected").eq(0).val();
        $("#selectedDoc").val(selectedID);
        dialog = $("#openDialogBox");
        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            data: { id: (selectedID == "" ? 0 : selectedID) },
            url: $(this).data("source"),
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
                dialog.modal("show");
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
                dialog.modal("show");
            }
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
                $("#clientusr").hide();
                break;
            case "2": // Client
                $("#empcode").hide();
                $("#splusr").hide();
                $("#clientusr").show();
                break;
            case "3":   // Special User
                $("#empcode").hide();
                $("#clientusr").hide();
                $("#splusr").show();
                break;
            default:
                $("#empcode").hide();
                $("#splusr").hide();
                $("#clientusr").hide();
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
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
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

    
    $("body").delegate(".loadSupportingDocs", "change", function () {
        var group = $(this).find("option:selected").eq(0).closest('optgroup').length;
        $(".mappedDocs").remove();      // Remove any previously loaded documents
        if (group > 0) {
            var selectedID = $(this).find("option:selected").eq(0).val();
            var phaseID = $("#ProjectPhase").find("option:selected").eq(0).val();

            $("#TaskName").val($(this).find("option:selected").eq(0).text());
            // get the documents associated with this task
            $.get("/PrjProcessTailor/getSupportingDocuments?processMapID=" + selectedID + "&planID=" + (phaseID == "" ? "0" : phaseID) + "&excludeIDs=&key=1", "", function (data, textStatus, jqXHS) {
                $("#supportingDocs").children().last().before(data);
            }).fail(function (result) {
                alert("No Supporting documents found");
            });
        }
    });

    $("body").delegate(".invokeEvent", "click", function () {
        var eventSource = $(this).data("source");
        var eventName = $(this).data("event");
        switch (eventName) {
            case "click":
                $("#" + eventSource).click();
                break;
            case "change":
                $("#" + eventSource).change();
                break;
            default:
                break;
        }
    });

    $("body").delegate(".reloadProjectTasks", "click", function () {
        var mode = $(this).data("mode");
        var projectControl = $("#tbl_Mapped_Proj_ProcessID");
        var projectID = $("#tbl_Org_ProjectID").val();
        var phase = $("#ProjectPhase").find("option:selected").eq(0).val();

        if (mode == "1") {
            // Show all tasks
            $(this).data("mode", "2");
            $(this).children("i").eq(0).removeClass("icon-eye-close");
            $(this).children("i").eq(0).addClass("icon-eye-open");
            $(this).removeClass("orange-bg")
            $(this).addClass("blue-bg");
            $(this).prop("title", "Show Only activities");
            mode = 2;
        }
        else
        {
            $(this).data("mode", "1");
            $(this).children("i").eq(0).removeClass("icon-eye-open");
            $(this).children("i").eq(0).addClass("icon-eye-close");
            $(this).removeClass("blue-bg")
            $(this).addClass("orange-bg");
            $(this).prop("title", "Show all tasks");
            mode = 1;
        }
        projectControl.removeClass("loadSupportingDocs");   // To avoid triggering the change event when the drop down is being reloaded.
        projectControl.empty();
        loadTasks(projectID, phase, mode, projectControl);
        $(".mappedDocs").remove();      // Remove any previously loaded documents
        projectControl.addClass("loadSupportingDocs");   // Relink the change event. No need to trigger the event as the default option is "Select"

    });


    $("body").delegate(".loadTasksForPhase", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        var projectControl = $("#tbl_Mapped_Proj_ProcessID");
        var projectID = $("#tbl_Org_ProjectID").val();
        var mode = $("#TaskFilter").data("mode");

        projectControl.removeClass("loadSupportingDocs");   // To avoid triggering the change event when the drop down is being reloaded.
        projectControl.empty();
        loadTasks(projectID, selectedID, mode, projectControl);
        $(".mappedDocs").remove();      // Remove any previously loaded documents
        projectControl.addClass("loadSupportingDocs");   // Relink the change event. No need to trigger the event as the default option is "Select"
    });

    function loadTasks(projectID, phaseID, mode, projectControl) {
        $.getJSON("/MapRepository/getProjectTasksJSON?projectID=" + projectID + "&mode=" + mode + (phaseID == "" ? "" : "&phase=" + phaseID), function (result) {
            projectControl.append('<option value="" selected="selected">Select...</option>');
            var groupName = "";
            var groupPresent = false;
            var newTasks = "";
            $.each(result, function (index, element) {
                if (element.GroupName != groupName) {
                    if (groupPresent) {
                        newTasks += '</optgroup>';
                    }
                    newTasks += '<optgroup label="' + element.GroupName + '">';
                    groupName = element.GroupName;
                    groupPresent = true;
                }
                newTasks += '<option value="' + element.ID + '" >' + (groupPresent ? '&nbsp;&nbsp;&nbsp;' : '') + element.DisplayText + '</option>';
            });
            if (groupPresent) {
                newTasks += '</optgroup>';
            }
            projectControl.append(newTasks);
        });
    }

    $(document).on("click", "#showToday", function () {
        viewDate = new Date();
        populateWeek(viewDate);
    });

    $(document).on("click", "#prevWeek", function () {
        viewDate = dateAdd(viewDate, -7);
        populateWeek(viewDate);
    });

    $(document).on("click", "#nextWeek", function () {
        viewDate = dateAdd(viewDate, 7);
        populateWeek(viewDate);
    });

    var populateWeek = function (currDate) {
        var monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var scroll = $(window).scrollTop(); // Save the window position before this operation was invoked

        if (currDate.getDay() > 1) currDate = dateAdd(currDate, (-1) * (currDate.getDay() - 1));

        $("#weekStart").text(currDate.getDate() + "-" + monthName[currDate.getMonth()] + "-" + currDate.getFullYear());

        var selectedID = $("#Project").find("option:selected").eq(0).val();
        PopulateTaskTracking(selectedID);

        $('html, body').animate({
            scrollTop: scroll
        }, 2000);

    };


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


    $("body").delegate(".group", "click", function () {
        $("." + $(this).data("childid")).toggle();
        $(this).toggleClass("closed");
    });

    $(document).on("click", "#showTodayTS", function () {
        viewDate = new Date();
        if ($("#viewTitle").text() == "Weekly View") {
            populateView(viewDate, "week");
            $("#weeklyView").click();
        }
        else {
            populateView(viewDate, "day");
            $("#dailyView").click();
        }
    });

    $(document).on("click", "#prevWeekTS", function () {
        if ($("#viewTitle").text() == "Weekly View") {
            viewDate = dateAdd(viewDate, -7);
            populateView(viewDate, "week");
            $("#weeklyView").click();
        }
        else {
            viewDate = dateAdd(viewDate, -1);
            populateView(viewDate, "day");
            $("#dailyView").click();
        }
    });

    $(document).on("click", "#nextWeekTS", function () {
        if ($("#viewTitle").text() == "Weekly View") {
            viewDate = dateAdd(viewDate, 7);
            populateView(viewDate, "week");
            $("#weeklyView").click();
        }
        else {
            viewDate = dateAdd(viewDate, 1);
            populateView(viewDate, "day");
            $("#dailyView").click();
        }
    });


    var populateView = function (currDate, viewName) {
        var scroll = $(window).scrollTop(); // Save the window position before this operation was invoked

        switch (viewName) {
            case "week":
                if (currDate.getDay() > 1) {
                    currDate = dateAdd(currDate, (-1) * (currDate.getDay() - 1));
                }
                else {
                    if (currDate.getDay() == 0) currDate = dateAdd(currDate, -6);
                }
                viewDate = currDate;

                $("#weekStart").text(currDate.getDate() + "-" + monthName[currDate.getMonth()] + "-" + currDate.getFullYear());
                for (var i = 1; i <= 7; i++) {
                    $("#weekDt" + i).html("<small>" + currDate.getDate() + "-" + monthName[currDate.getMonth()] + "</small>");
                    currDate = dateAdd(currDate, 1);
                }
                break;
            case "day":
                viewDate = currDate;
                $("#weekStart").text(currDate.getDate() + "-" + monthName[currDate.getMonth()] + "-" + currDate.getFullYear());
                break;
            default:
                break;
        };

        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    }

    $(document).on("click", "#dailyView", function () {
        $("#WeeklyTS").hide();
        $("#WeeklyTSDetails").empty();
        $("#MonthlyTS").hide();
        $("#MonthlyTS").empty();
        $("#viewTitle").text("Daily View");
        $("#vTitlePrefix").text("");
        populateView(viewDate, "day");
        var month = viewDate.getMonth() + 1;
        var day = viewDate.getDate();
        $("#DailyTSDetails").load("/Timesheet/DailyView?startDate=" + viewDate.getFullYear() + "/" + (month < 10 ? '0' : '') + month + '/' + (day < 10 ? '0' : '') + day,
            function (response, status, xhr) {
            if (status == "error") {
                $("#DailyTSDetails").html("");
                if (xhr.status == 403) {
                    $("#openDialogBox").html(xhr.responseText);
                    $("#openDialogBox").modal("show");
                }
                else {
                    var msg = "Sorry but there was an error: ";
                    //$( "#error" ).html( msg + xhr.status + " " + xhr.statusText );
                    alert(msg + xhr.status + " " + xhr.statusText);
                }
            }
        });
        $("#DailyTS").show();
        $("#weeklyView").parent().removeClass("active");
        $("#monthlyView").parent().removeClass("active");
        $(this).parent().addClass("active");
        $(".nav_options").show();
    });


    $(document).on("click", "#weeklyView", function () {
        $("#DailyTS").hide();
        $("#DailyTSDetails").empty();
        $("#MonthlyTS").hide();
        $("#MonthlyTS").empty();
        $("#viewTitle").text("Weekly View");
        $("#vTitlePrefix").text("Starting ");
        populateView(viewDate, "week");
        var month = viewDate.getMonth() + 1;
        var day = viewDate.getDate();
        $("#WeeklyTSDetails").html(processing("Loading "));

        $.ajax({
            cache: false,
            async: true,
            type: "GET",
            url: "/Timesheet/WeeklyView?startDate=" + viewDate.getFullYear() + "/" + (month < 10 ? '0' : '') + month + '/' + (day < 10 ? '0' : '') + day,
            success: function (data) {
                $("#WeeklyTSDetails").empty().off("*");
                $("#actions").remove();
                $("#WeeklyTSDetails").html(data);
            },
            error: function (data) {
                $("#WeeklyTSDetails").empty().off("*");
                $("#WeeklyTSDetails").html(data.responseText);
            }
        });
        $("#WeeklyTS").show();
        $("#dailyView").parent().removeClass("active");
        $("#monthlyView").parent().removeClass("active");
        $(this).parent().addClass("active");
        $(".nav_options").show();
    });


    $(document).on("click", "#monthlyView", function () {
        $("#DailyTS").hide();
        $("#DailyTS").empty();
        $("#WeeklyTS").hide();
        $("#WeeklyTS").empty();
        $("#viewTitle").text("Monthly View");
        $("#vTitlePrefix").text("");
        $("#MonthlyTS").load("/Timesheet/MonthlyView?month=" + viewDate.getMonth(),
            function (response, status, xhr) {
                if (status == "error") {
                    $("#MonthlyTS").html("");
                    if (xhr.status == 403) {
                        $("#openDialogBox").html(xhr.responseText);
                        $("#openDialogBox").modal("show");
                    }
                    else {
                        var msg = "Sorry but there was an error: ";
                        //$( "#error" ).html( msg + xhr.status + " " + xhr.statusText );
                        alert(msg + xhr.status + " " + xhr.statusText);
                    }
                }
            });
        $("#MonthlyTS").show();
        populateView(viewDate, "week");
        $("#dailyView").parent().removeClass("active");
        $("#weeklyView").parent().removeClass("active");
        $(this).parent().addClass("active");
        $(".nav_options").hide();
    });


    $("body").delegate(".EditTime", "click", function () {
        if ($(this).children().length > 0) {
            $(this).children().eq(1).focus();
        }
        else {
            var OriginalContent = $(this).text();
            var updateID = $(this).data("id");

            keypressCode = -1;
            keypressShift = false;
            $(this).addClass("cellEditing");
            $(this).removeClass("EditTime");
            $(this).html("<input type='text' value='" + OriginalContent + "' size='3' id='mask_time' class='inputValue' data-class='EditTime' data-id='" + updateID + "'/>");
            $("#mask_time").inputmask("[9]9[.99]", { placeholder: " ", clearMaskOnLostFocus: true, greedy: true, clearIncomplete: true });
            $(this).children().first().focus();
        }
    });

    $("body").delegate(".EditTimeStamp", "click", function () {
        if ($(this).children().length > 0) {
            $(this).children().eq(1).focus();
        }
        else {
            var OriginalContent = $(this).text();
            var updateID = $(this).data("id");

            keypressCode = -1;
            keypressShift = false;
            $(this).addClass("cellEditing");
            $(this).removeClass("EditTime");
            $(this).html("<input type='text' value='" + OriginalContent + "' size='3' id='mask_time' class='inputValue' data-class='EditTimeStamp' data-id='" +

updateID + "'/>");
            $("#mask_time").inputmask("[9]9[:99]", { placeholder: " ", clearMaskOnLostFocus: true, greedy: true, clearIncomplete: true });
            $(this).children().first().focus();
        }
    });

    function calculateTimeTotals(element) {
        var elementType = element.data("type");
        var id = element.data("typeid");
        var processRow = element.parent();
        var totalHours = parseFloat($(element).text()); // Include this elements hours
        $("#" + $(element).data("id")).val(totalHours);

        $(processRow).children(".EditTime").each(function (index, element) {
            totalHours = totalHours + parseFloat($(element).text());   // Get the total time enetered in this row
        });

        var baseDuration = parseFloat($("#" + elementType + "I" + id + "BaseActualDuration").val());
        totalHours = totalHours + baseDuration;
        $("#" + elementType + "I" + id + "Total").text(totalHours);
        $("#Actual" + $(element).data("id")).val(totalHours);
    }

    /******************************************************************/
    /*** Project Estimation functions: START
    /******************************************************************/

    $("body").delegate(".RecalculateEstm", "change", function () {
        calculateValues();
    });

    function calculateValues() {
        calculateFP(); 
        calculateGSC();
        calculateTeamEffort();
        calculateEffSchDefects();
        // Load input from display fields to hidden fields to be sent to the server
        $(".updateInput").each(function (index, element) {
            $("#" + $(element).prop("id") + "Input").val($(element).text());
        });

        $(".updateDate").each(function (index, element) {
            $("#" + $(element).prop("id") + "Input").val($(element).text());
        });
    }

    // Calculate Simple, Medium & Complex function points
    function calculateFP() {
        var sizeWP = 0;
        var totalSimpleFP = 0;
        var totalMediumFP = 0;
        var totalComplexFP = 0;
        var TotalFP = 0;
        // Calculate Individual Line Items
        $(".FP").each(function (index, element) {
            var node = $(element).prop("id");
            sizeWP = (parseFloat($(element).text()) * parseFloat($("#" + node + "W").text())).toFixed(1);
            $("#" + node + "WP").text(sizeWP);
            switch ($(element).data("type")) {
                case "simple":
                    totalSimpleFP = parseFloat(totalSimpleFP) + parseFloat(sizeWP);
                    break;
                case "medium":
                    totalMediumFP = parseFloat(totalMediumFP) + parseFloat(sizeWP);
                    break;
                case "complex":
                    totalComplexFP = parseFloat(totalComplexFP) + parseFloat(sizeWP);
                    break;
                default:
                    break;
            }
            TotalFP = parseFloat(TotalFP) + parseFloat(sizeWP);
            // Update the totals
            $("#SimpleEfforts").text(totalSimpleFP.toFixed(2));
            $("#MediumEfforts").text(totalMediumFP.toFixed(2));
            $("#ComplexEfforts").text(totalComplexFP.toFixed(2));
            $("#TotalEfforts").text(TotalFP.toFixed(2));
        });
    }


// Calculate the TDI for General Systems Characteristics
    function calculateGSC() {
        var total = 0;

        $(".GSCRating").each(function () {
            total = total + parseFloat($(this).text());
        });
        $("#TDI").text(total);
        $("#VAF").text((0.65 + (total * 0.01)).toFixed(2));  
    }

    // Calculate the team effort for CUT team based on per role productivity & assignment percentage
    function calculateTeamEffort() {
        var prodSum = 0;
        var totalSum = 0;
        var teamSize = 0;
        var cut_effort_fp = 0;
        var cut_effort_pd = 0;

        // Default values for some parameters
        if ($("#PMSBR").val() == "") $("#PMSBR").val(150000);
        if ($("#DefectDensity").val() == "") $("#DefectDensity").val(2);
        $(".teamProd").each(function (index,element) {
            prodSum = (parseFloat($(element).text()) * parseFloat($("#" + $(element).prop("id") + "FP").text()) * (parseFloat($("#" + $(element).prop("id") + "PC").text()) / 100));
            totalSum = totalSum + prodSum;
            teamSize += parseFloat($(element).text());
        });
        // Update the grand totals
        $("#CUT_TeamSize").text(teamSize);
        $("#Team_Productivity").text(isNaN(totalSum / parseInt($("#CUT_TeamSize").text())) ? 0 : (totalSum / parseInt($("#CUT_TeamSize").text())).toFixed(2));
        cut_effort_fp = parseFloat($("#TotalEfforts").text()) * parseFloat($("#VAF").text());
        $("#CUT_Effort_FPs").text(isNaN(cut_effort_fp) ? 0 : cut_effort_fp.toFixed(2));
        if ($("#CUT_EffortPercent").val() == "" || $("#CUT_EffortPercent").val() == null) $("#CUT_EffortPercent").val(65);  // If not CUT percentage is provided, default it to 65%
        $("#CUT_Phase").text($("#CUT_EffortPercent").val());     // If the added phase is a CUT phase, then update the cut percent
        // In case a CUT phase is added. Load this percentage in it.
        cut_effort_pd = ($("#Team_Productivity").text() == "0" || $("#Team_Productivity").text() == "") ? 0 : parseFloat($("#CUT_Effort_FPs").text()) / parseFloat($("#Team_Productivity").text());
        $("#CUT_Effort_PDs").text(isNaN(cut_effort_pd) ? 0 : cut_effort_pd.toFixed(2));
        $("#Size").text($("#CUT_Effort_PDs").text());
        $("#Overall_Project_PDs").text(((parseFloat($("#CUT_Effort_PDs").text()) / parseInt($("#CUT_EffortPercent").val())) * 100).toFixed(2));
    }

    function calculateEffSchDefects () {
        var startDate = new Date();
        var endDate = new Date();
        var totalPercent = 0;
        var totalPDs = 0;
        var totalProjectCost = 0;
        var totalPMCost = 0;
        var totalOtherCost = 0;
        var schTotalDays = 0;
        var processData = 0;

        startDate = null;
        $(".effSchEst").each(function (index, element) {
            // Effort and Cost Estimation
            var node = $(element).prop("id");

            if (isNaN($(element).text())) {
                $(element).text(0);
            }
            else {
                $(element).text(parseFloat($(element).text()).toFixed(0));  // Round off the percentage
            }
            $("#" + node + "Eff").text(((parseFloat($("#Overall_Project_PDs").text()) * parseFloat($(element).text())) / 100).toFixed(2)); // Person Days
            $("#" + node + "EffForSch").text($("#" + node + "Eff").text()); // Update efforts in schedule tab
            $("#" + node + "EffForDef").text($("#" + node + "Eff").text()); // Update efforts in Defects tab
            $("#" + node + "PMEffort").text(((parseFloat($("#" + node + "Eff").text()) * parseFloat($("#" + node + "PMLoad").text())) / 100).toFixed(2)); // PM Effort
            $("#" + node + "PMCost").text(((parseFloat($("#" + node + "PMEffort").text()) * parseFloat($("#PMSBR").val())) / 22).toFixed(2)); // PM Cost = PM Effort * PM SBR
            if (parseFloat($("#" + node + "PMLoad").text()) < 100)
            {
                $("#" + node + "OtherCost").text(((parseFloat($("#" + node + "Eff").text()) * parseFloat($("#" + node + "SBR").text())) / 22).toFixed(2)); // PM Cost = PM Effort * PM SBR
            }
            else
            {
                $("#" + node + "OtherCost").text(0); // No other costs considered. All efforts are that of PM skills
            }
            // Grand Totals
            totalPercent = parseInt(totalPercent) + parseInt($(element).text());
            totalPDs = parseInt(totalPDs) + parseInt($("#" + node + "Eff").text());
            totalProjectCost = (parseFloat(totalProjectCost) + parseFloat($("#" + node + "PMCost").text()) + parseFloat($("#" + node + "OtherCost").text())).toFixed(2);
            totalPMCost = (parseFloat(totalPMCost) + parseFloat($("#" + node + "PMCost").text())).toFixed(2);
            totalOtherCost = (parseFloat(totalOtherCost) + parseFloat($("#" + node + "OtherCost").text())).toFixed(2);

            // Schedule Estimation
            $("#" + node + "SchPD").text((parseFloat($("#" + node + "SchPpl").text()) > 0 ? (parseFloat($("#" + node + "Eff").text()) / parseFloat($("#" + node + "SchPpl").text())).toFixed(2) : 0));
            if (startDate == null) {
                // This is the first row in the table. So the start date should be editable for this row
                if (!$("#" + node + "SchStDate").hasClass("EditDate")) {
                    $("#" + node + "SchStDate").addClass("EditDate");
                    $("#" + node + "SchStDate").data("navkey", "EditNumber");
                }
                if ($("#" + node + "SchStDate").hasClass("TableDisabledLabel")) $("#" + node + "SchStDate").removeClass("TableDisabledLabel");
                if ($("#" + node + "SchStDate").text().trim() == "") {
                    // Schedule start date not found
                    if (startDate == null) startDate = new Date();  // First Phase entry, so default to todays date
                    $("#" + node + "SchStDate").text(formatDate(startDate));
                }
                else {
                    var strDate = $("#" + node + "SchStDate").text();
                    startDate = parseDate(strDate);
                }
            }
            else {
                // Disable this row for edits and overwrite the start date
                if ($("#" + node + "SchStDate").hasClass("EditDate")) {
                    $("#" + node + "SchStDate").removeClass("EditDate");
                    $("#" + node + "SchStDate").data("navkey", "");
                }
                if (!$("#" + node + "SchStDate").hasClass("TableDisabledLabel")) $("#" + node + "SchStDate").addClass("TableDisabledLabel");
                $("#" + node + "SchStDate").text(formatDate(startDate));
            }
            if (isNaN($("#" + node + "SchPD").text())) $("#" + node + "SchPD").text(0);
            if (isNaN($("#" + node + "SchHoliday").text())) $("#" + node + "SchHoliday").text(0);
            endDate = startDate;
            endDate.setDate(startDate.getDate() + parseInt($("#" + node + "SchPD").text()) + parseInt(parseInt($("#" + node + "SchPD").text()) / 5) + parseInt($("#" + node + "SchHoliday").text()));
            $("#" + node + "SchEndDt").text((endDate.getDate() < 10 ? "0" : "") + endDate.getDate() + "/" + (endDate.getMonth() < 9 ? "0" : "") + (endDate.getMonth() + 1) + "/" + endDate.getFullYear());
            schTotalDays = parseInt(schTotalDays) + parseInt(daydiff(startDate, endDate));
            startDate = endDate;
            startDate.setDate(endDate.getDate() + 1);
        });

        var TotalDefects = (parseFloat($("#CUT_Effort_FPs").text()) * parseFloat($("#DefectDensity").val())).toFixed(2);
        $("#TotalDefects").text(isNaN(TotalDefects) ? 0 : TotalDefects);

        $(".effSchEst").each(function (index, element) {
            var node = $(element).prop("id");
            // Defect Estimation
            if (parseFloat($("#" + node + "Eff").text()) == 0) {
                $("#" + node + "Defects").text(0);
            }
            else {
                $("#" + node + "Defects").text((parseFloat(totalPDs) <= 0 ? 0 : (TotalDefects * parseFloat($("#" + node + "Eff").text()) / totalPDs).toFixed(2)));
            }
        });

        // Update the totals
        // Schedule
        $("#schDays").text(schTotalDays);
        $("#schMonths").text(isNaN(schTotalDays) ? 0 : (parseInt(schTotalDays) / 30).toFixed(2));
        $("#ScheduleMonths").text($("#schMonths").text());
        // Cost
        $("#Cost").text(isNaN(totalProjectCost) ? 0 : parseFloat(totalProjectCost).toFixed(2));
        // Effort
        $("#PCTotal").text(isNaN(totalPercent) ? 0 : parseFloat(totalPercent).toFixed(2));
        $("#EffTotal").text(isNaN(totalPDs) ? 0 : parseFloat(totalPDs).toFixed(2));
        $("#PMCostTotal").text(isNaN(totalPMCost) ? 0 : parseFloat(totalPMCost).toFixed(2));
        $("#OtherCostTotal").text(isNaN(totalOtherCost) ? 0 : parseFloat(totalOtherCost).toFixed(2));
    }


    function calculateNValues() {
        var total
        var totalPDs
        total = 0;
        grandTotal = 0;

        $(".simpleW").each(function () {
            total = total + parseFloat($(this).text());
        });
        $("#STotal").text(total);
        grandTotal = grandTotal + total;
        total = 0;
        $(".mediumW").each(function () {
            total = total + parseFloat($(this).text());
        });
        $("#MTotal").text(total);
        grandTotal = grandTotal + total;
        total = 0;
        $(".complexW").each(function () {
            total = total + parseFloat($(this).text());
        });
        $("#CTotal").text(total);
        grandTotal = grandTotal + total;
        $("#Total").text(grandTotal);
    }


    $("body").delegate(".addLineItem", "click", function () {
        var source = $(this).data("source");
        var Params = $("#" + $(this).data("params"));
        // Get the last count
        var key = Params.data("keycount");
        var Duplicate = $(this).data("duplicate");
        var excludeIDs = "";
        if ($(this).data("type") == "parameter") {
            var groupID = $(this).parent().parent().parent().parent().parent().parent().parent().children("a").eq(1).data("id");
            var parentNode = $(this).parent().parent().parent().parent().parent().parent().parent();
            source = source + '&GroupID=' + groupID;
            // Search for parameter ids only within this group.
            if (Duplicate != "undefined" && Duplicate != "") {
                parentNode.find("." + Duplicate).each(function (index, element) {
                    excludeIDs = excludeIDs + (excludeIDs != "" ? ($(element).data("id") != "" ? "," : "") : "") + $(element).data("id");
                });
            }
        }
        else {
            if (Duplicate != "undefined" && Duplicate != "") {
                $("." + Duplicate).each(function (index, element) {
                    excludeIDs = excludeIDs + (excludeIDs != "" ? ($(element).data("id") != "" ? "," : "") : "") + $(element).data("id");
                });
            }
        }
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
            url: source + '&Key=' + key + '&ExcludeIDs=' + excludeIDs,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
    });

    $("body").delegate(".editLineItem", "click", function () {
        var source = $(this).data("source");
        var updateID = $(this).data("id");
        var Duplicate = $(this).data("duplicate");
        var currentID = $(this).data("id");
        var Params = $("#" + $(this).data("params"));
        // Get the last count
        var key = Params.data("keycount");
        var excludeIDs = "";

        if ($(this).data("type") == "parameter") {
            var groupID = $(this).parent().parent().parent().parent().parent().parent().parent().children("a").eq(1).data("id");
            var parentNode = $(this).parent().parent().parent().parent().parent().parent().parent();
            source = source + '&GroupID=' + groupID;
            // Search for parameter ids only within this group.
            if (Duplicate != "undefined" && Duplicate != "") {
                parentNode.find("." + Duplicate).each(function (index, element) {
                    excludeIDs = excludeIDs + (excludeIDs != "" ? "," : "") + $(element).data("id");
                });
            }
        }
        else {
            if (Duplicate != "undefined" && Duplicate != "") {
                $("." + Duplicate).each(function (index, element) {
                    if ($(element).data("id") != currentID) {
                        excludeIDs = excludeIDs + (excludeIDs != "" ? "," : "") + $(element).data("id");
                    }
                });
            }
        }

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
            url: source + '&Key=' + key + '&ExcludeIDs=' + excludeIDs,
            success: function (data) {
                dialog.empty().off("*");
                dialog.html(data);
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
    });

    $("body").delegate(".removeLineItem", "click", function () {
        var scroll = $(window).scrollTop(); // Save the window position before this operation was invoked

        $("#" + $(this).data("id")).remove();
        calculateValues();
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    });

    $("body").delegate(".removeEstmPhase", "click", function () {
        var scroll = $(window).scrollTop(); // Save the window position before this operation was invoked

        $("#" + $(this).data("id")).remove();
        $("#" + $(this).data("id") + "Sch").remove();
        $("#" + $(this).data("id") + "Defect").remove();
        calculateValues();
        $('html, body').animate({
            scrollTop: scroll
        }, 2000);
    });

    $("body").delegate("#CUT_EffortPercent", "keypress", function () {
        if (e.which == 13) {
            if (parseFloat($(this).val()) <= 0) {
                alert("CUT effort should be greater than 0");
            }
            else {
                calculateValues();
            }
        }
    });

    /******************************************************************/
    /*** Project Estimation functions: END
    /******************************************************************/


    $("body").delegate(".changeDashboard", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();
        switch (selectedID) {
            case "1":
                location.replace("/Home/Index");
                break;
            case "2":
                location.replace("/Home/Index2");
                break;
            case "3":
                location.replace("/Home/Index3");
                break;
            case "4":
                location.replace("/Home/Index4");
                break;
            default:
                location.replace("/Home/Index");
                break;
        }
    });
    
    $("body").delegate(".showFrequency", "change", function () {
        var selectedID = $(this).find("option:selected").eq(0).val();

        if (selectedID == 2) {
            $("#FrequencyDetails").show("slow");
            $("#StartLabel").html("Start Audits From");
            $("#FinishLabel").html("Recurr Till");
        }
        else {
            $("#FrequencyDetails").hide("slow");
            $("#Duration").val("1");
            $("#DurationUnit").val("");
            $("#Period").val("");
            $("#StartLabel").html("Planned Start");
            $("#FinishLabel").html("Planned Finish");
        }
    });

    $("body").delegate(".saveAuditPlan", "click", function () {
        var form = $("#" + $(this).data("form"));
        var mode = $(this).data("mode");
        var id = $(this).data("id");

        if (mode == "saveFinding")
        {
            $(".takeHtmlInput").each(function (index, element) {
                if ($(element).text() != "") {
                    $("#" + $(element).prop("id") + "Input").val($(element).html());
                }
                else {
                    $("#" + $(element).prop("id") + "Input").val("");
                }
            });
            $(".takeInput").each(function (index, element) {
                $("#" + $(element).prop("id") + "Input").val($(element).text());
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
                        $('#AuditPlanTable').dataTable().fnDestroy();
                        $('#AuditScheduleTable').dataTable().fnDestroy();
                        var planDetails = $(data).find("#plan");
                        var scheduleDetails = $(data).find("#schedule");
                        $("#listing").append(planDetails.html());
                        $("#ScheduleListing").append(scheduleDetails.html());

                        jsTable("AuditPlanTable");
                        jsTable("AuditScheduleTable");
                        $("#AuditPlanTable").removeAttr("style");
                        $("#AuditScheduleTable").removeAttr("style");
                        $.pnotify({
                            title: 'Save Successfully',
                            text: "Audit Plan Saved",
                            type: 'info'
                        });
                        $("#openDialogBox").modal("hide");
                        break;
                    case "edit":
                        var updatePlanTable = true;
                        if ($('#AuditPlanTable').length == 0) updatePlanTable = false;
                        if (updatePlanTable) {
                            $('#AuditPlanTable').dataTable().fnDestroy();
                            // Delete existing records first
                            $("#pl" + id).remove();
                            var planDetails = $(data).find("#plan");
                            $("#listing").append(planDetails.html());
                            jsTable("AuditPlanTable");
                            $("#AuditPlanTable").removeAttr("style");
                        }
                        $('#AuditScheduleTable').dataTable().fnDestroy();
                        // Delete existing records first
                        $(".sch" + id).remove();
                        // Add Updated entries
                        var scheduleDetails = $(data).find("#schedule");
                        $("#ScheduleListing").append(scheduleDetails.html());

                        jsTable("AuditScheduleTable");
                        $("#AuditScheduleTable").removeAttr("style");
                        $.pnotify({
                            title: 'Save Successfully',
                            text: "Audit Plan details saved",
                            type: 'info'
                        });
                        $("#openDialogBox").modal("hide");
                        break;
                    case "editSchedule":
                        $("#" + id).replaceWith(data);
                        $.pnotify({
                            title: 'Audit Scheduled',
                            type: 'info'
                        });
                        $("#openDialogBox").modal("hide");
                        break;
                    case "saveFinding":
                        $("#" + id).empty().off("*");
                        $("#" + id).html(data);
                        $.pnotify({
                            title: 'Saved Successfully',
                            text: 'Audit Findings saved successfully',
                            type: 'info'
                        });
                        $("#openDialogBox").modal("hide");
                        break;
                    case "closeAudit":
                        $("#" + id).empty().off("*");
                        $("#" + id).html(data);
                        $.pnotify({
                            title: 'Audit Findings Closed',
                            text: 'Audit Findings closed successfully',
                            type: 'info'
                        });
                        $("#openDialogBox").modal("hide");
                        break;
                    default:
                        break;
                }
            },
            error: function (data) {
                switch (mode) {
                    case "saveFinding":
                        $("#" + id).empty().off("*");
                        $("#" + id).html(data.responseText);
                        $.pnotify({
                            title: 'Check Errors',
                            text: "Check for highlighted inputs which are mandatory",
                            type: 'error'
                        });
                        break;
                    default:
                        $("#openDialogBox").empty().off("*");
                        $("#openDialogBox").html(data.responseText);
                        $.pnotify({
                            title: 'Operation Failed',
                            text: 'Check Errors',
                            type: 'error'
                        });
                        break
                }
            }
        });
    });

    $("body").delegate(".deleteAuditPlan", "click", function () {
        var id = $(this).data("id");

        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: "/Audit/Delete",
            data: { id: id },
            success: function (data) {
                $.pnotify({
                    title: "Plan Deleted",
                    type: 'info'
                });
                $('#AuditPlanTable').dataTable().fnDestroy();
                $('#AuditScheduleTable').dataTable().fnDestroy();
                $("#pl" + id).remove();
                $(".sch" + id).remove();
                jsTable("AuditPlanTable");
                jsTable("AuditScheduleTable");
                $("#AuditPlanTable").removeAttr("style");
                $("#AuditScheduleTable").removeAttr("style");
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

    $("body").delegate(".addOption", "click", function () {
        var source = $(this).data("source");
        var updateID = $(this).data("id");

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
            url: source,
            success: function (data) {
                $("#" + updateID).replaceWith(data);
                dialog.modal("hide");
            },
            error: function (data) {
                dialog.empty().off("*");
                dialog.html(data.responseText);
            }
        });
    });

    var calculateAuditPCI = function () {
        var pci_Count = 0;
        var totalPC = 0;
        var totalPC_Count = 0;
        var phasePC = 0;
        var pci = 0;
        var processID = "";

        // Update individual scores
        $(".PCIResult").each(function () {
            var value = $(this).find("option:selected").eq(0).val();
            processID = $(this).prop("id");
            $("#" + processID + "_W").removeClass("Audit_NC");  
            switch (value) {
                case "FI":
                    $("#" + processID + "_W").text("1.00");
                    $("#" + processID + "_WInput").val("1.00");
                    $("#" + processID + "_PC").children().first().text("100");
                    break;
                case "LI":
                    $("#" + processID + "_W").text("0.75");
                    $("#" + processID + "_WInput").val("0.75");
                    if ($("#" + processID + "_Applicable").find("option:selected").eq(0).val() == "0") {
                        $("#" + processID + "_PC").children().first().text("100");
                    }
                    else $("#" + processID + "_PC").children().first().text("75");
                    break;
                case "PI":
                    $("#" + $(this).prop("id") + "_W").text("0.50");
                    $("#" + $(this).prop("id") + "_WInput").val("0.50");
                    if ($("#" + processID + "_Applicable").find("option:selected").eq(0).val() == "0") $("#" + processID + "_PC").children().first().text("100");
                    else $("#" + processID + "_PC").children().first().text("50");
                    break;
                case "NI":
                    $("#" + processID + "_W").text("NC");
                    $("#" + processID + "_W").addClass("Audit_NC");
                    $("#" + processID + "_WInput").val("0");
                    if ($("#" + processID + "_Applicable").find("option:selected").eq(0).val() == "0") $("#" + processID + "_PC").children().first().text("100");
                    else $("#" + processID + "_PC").children().first().text("0");
                    break;
                case "NY":
                case "NA":
                    $("#" + processID + "_W").text("");
                    $("#" + processID + "_WInput").val("");
                    $("#" + processID + "_PC").children().first().text("100");
                    break;
            }
        });

        totalPC = 0;
        totalPC_Count = 0;
        $(".phaseScore").each(function () {
            phasePC = 0;
            pci_Count = 0
            pci = 0
            $("." + $(this).prop("id")).each(function () {
                pci_Count = pci_Count + 1;
                pci = pci + parseInt($(this).children().first().text());
            });
            phasePC = parseFloat(pci / pci_Count).toFixed(0);
            totalPC = parseInt(totalPC) + parseInt(phasePC);
            totalPC_Count = totalPC_Count + 1
            $(this).text(phasePC + " %");
        });

        $("#projScore").text(parseFloat(totalPC / totalPC_Count).toFixed(0) + " %");
    };

    $("body").delegate(".deleteRow", "click", function () {
        var dialogName = $(this).data("dialogname");
        var DeleteOnClient = $(this).data("clientdelete");
        var ConfirmDelete = $(this).data("confirm");
        var dataSource = $(this).data("source");
        var updateID = $(this).data("id");

        var dialog = $("#" + dialogName);
        if (ConfirmDelete != "Y") {
            dialog.html('<div class="modal-dialog" style=width:30%;">' +
                            '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                                    '<h4 class="modal-title" id="dialogTitle">Confirm Delete</h4>' +
                                '</div>' +
                                    '<div class="modal-body"> ' +
                                        '<div class="row col-md-12">' +
                                            '<p class="control-label col-md-12 text-center">Do you want to delete this entry ?</p>' +
                                        '</div>' +
                                    '</div>' +
                                    '<div class="modal-footer">' +
                                        '<div class="form-group">' +
                                            '<button type="button" class="btn gray-bg closeDialog" data-id="' + dialogName + '">No</button>' +
                                            '<button type="button" class="btn blue-bg deleteRow" data-confirm="Y" data-id = "' + updateID + '" data-source="' + dataSource + '" data-dialogname="' + dialogName + '" data-clientdelete = "' + DeleteOnClient + '">Yes</button>' +
                                        '</div>' +
                                    '</div>' +
                            '</div>' +
                        '</div>');
            dialog.modal("show");
        }
        else {
            dialog.modal("hide");
            if (DeleteOnClient == "Y") {
                $("#" + updateID).remove();
            }
            else {
                var specialCase = false;
                if (dataSource.search("PrjProcessTailor") >= 0) specialCase = true;
                $.ajax({
                    cache: false,
                    async: true,
                    type: "POST",
                    url: dataSource,
                    success: function (data) {
                        $.pnotify({
                            title: 'Record Deleted',
                            type: 'info'
                        });
                        if (specialCase == true) {
                            $("#" + updateID).replaceWith(data);
                        }
                        else $("#" + updateID).remove();
                    },
                    error: function (data) {
                        $.pnotify({
                            title: 'Delete Failed',
                            text: "This record cannot be deleted as it contains dependent entries. Clear the dependencies before deleting this entry.",
                            type: 'error'
                        });
                    }
                });
            }
        }
    });

    $("body").delegate(".ShowDiv", "click", function () {
        var divElement = $(this).data("id");
        if ($(this).val() == "false" || $(this).val() == "0") {
            $("#" + divElement).show();
        }
        else {
            $("#" + divElement).hide();
        }
    });

    $("body").delegate(".updateDocType", "change", function () {
        var selectedOption = $(this).find("option:selected").eq(0);
        var DocType = selectedOption.closest('optgroup').prop('label');
        var DocTypeElement = $("#" + $(this).prop("id") + "DocType");
        var DocumentName = $("#" + $(this).prop("id") + "DocumentName");
        $("#" + $(this).prop("id") + "RefID").val(selectedOption.data("value"));    // Update Reference ID
        if (DocumentName.length > 0) DocumentName.val(selectedOption.text().trim());
        switch (DocType) {      // Update Document Type
            case "Documents":
                DocTypeElement.val("1");
                break;
            case "Procedures":
                DocTypeElement.val("2");
                break;
            case "Templates":
                DocTypeElement.val("3");
                break;
            case "Checklists":
                DocTypeElement.val("4");
                break;
            default:
                DocTypeElement.val("0");
                break;
        }
    });

    $("body").delegate(".takeID", "click", function () {
        var id = $(this).data("detail");
        $("#selectedID").val(id);
        $(".label-repository").removeClass("label-repository");
        // Highlight the selected row
        $(this).addClass("label-repository");
    });

    $("body").delegate(".selectItem", "click", function () {
        var dataID = $(this).data("id");
        var pageID = $(this).data("pageid");
        var dialog = $(this).data("dialog");
        $("#" + pageID).val($("#" + dataID).val());
        $("#" + dialog).modal("hide");
    });

