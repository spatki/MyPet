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
    select: function (start, end, allDay) {
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
            start: new Date(y, m, d - 5),
            end: new Date(y, m, d - 2)
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: new Date(y, m, d - 3, 16, 0),
            allDay: false
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: new Date(y, m, d + 4, 16, 0),
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
            start: new Date(y, m, d + 1, 19, 0),
            end: new Date(y, m, d + 1, 22, 30),
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
        var days = parseInt((end - start) / (1000 * 60 * 60 * 24));
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
        if (days > 0) $("#dateRange").text(start.getDate() + "/" + (start.getMonth() + 1) + "/" + start.getFullYear() + " - " + end.getDay() + "/" + (end.getMonth() + 1) + "/" + end.getFullYear());
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
        else {
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
