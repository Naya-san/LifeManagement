/// <reference path="../Scripts/jquery-1.10.2.js" />
/// <reference path="../Scripts/jquery.signalR-2.0.3.js" />

if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {
    var ticker = $.connection.tickerHub,
    $timetable = $('div.timetable'),
    rowTemplate = '<div class="task {Type} {Priority}" style="height: {Height}px; position: relative"><h6><span style="float: left;">{Name}</span></h6><span style="float: right">{Readiness}%</span><br /> <div class="time">{Time}</div></div>',
    readinessTemplate = '<div data-readiness="{RoutineId}" class="readiness">{Readiness}%</div>';
    function formatStock(routine) {
        return $.extend(routine, {
            Type: routine.Type,
            Priority: routine.Type === "Free" ? "" : routine.Priority,
            Height: Math.round(routine.Duration*3),
            Name: routine.Name,
            Readiness: routine.Type === "Free" ? "" : routine.Readiness.toFixed(2),
            Time: routine.TimeString,
            RoutineId: routine.RoutineId
        });
    }
    $.extend(ticker.client, {
        updateSpentTime: function () {
            //   $timetable.empty();
           
            ticker.server.getRoutinesInfo();
        },
        setUsersRoutinesInfo: function (routines) {
            $.each(routines, function () {
                var routine = formatStock(this);
                $timetable.find('div [data-readiness=' + routine.RoutineId + ']')
                    .replaceWith(readinessTemplate.supplant(routine));
                if (routine.Readiness >= 100) {
                    $timetable.find('div [data-link=' + routine.RoutineId + ']').empty();
                };
            });
        }
    });

    // Start the connection
    $.connection.hub.start();
});