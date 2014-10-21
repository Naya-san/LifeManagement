function dis() {
    if (document.getElementsByName("EndDate")[0].value == "" && document.getElementsByName("StartDate")[0].value == "") {
        document.getElementsByName("Alerts")[0].disabled = true;
    } else {
        document.getElementsByName("Alerts")[0].disabled = false;
    }
}

function showDate() {
    if (document.getElementsByName("RepeatPosition")[0].value == "-1") {
        document.getElementById("ForHide").style.display = "none";
    } else {
        document.getElementById("ForHide").style.display = "block";
    }
}



//document.getElementsByName("EndTime")[0].disabled = false;
//document.getElementsByName("EndTime")[0].required = true;
//} else {
//    document.getElementsByName("EndTime")[0].disabled = true;
//    document.getElementsByName("EndTime")[0].required = false;
//}
function dis2() {
    if (document.getElementsByName("location")[0].checked == true) {
        document.getElementsByName("Start")[0].disabled = true;
        document.getElementsByName("End")[0].disabled = true;
        document.getElementsByName("Days")[0].disabled = false;
        document.getElementsByName("Hours")[0].disabled = false;
        document.getElementsByName("Minutes")[0].disabled = false;
        document.getElementsByName("onStartDate")[0].disabled = false;
        document.getElementsByName("onDueDate")[0].disabled = false;
        document.getElementsByName("onDeadline")[0].disabled = false;
        disDueDate();
        disDeadline();
        disStartDate();
        document.getElementsByName("Days")[0].required = true;
        document.getElementsByName("Hours")[0].required = true;
        document.getElementsByName("Minutes")[0].required = true;

        document.getElementsByName("End")[0].required = false;
        document.getElementsByName("Start")[0].required = false;
    } else {
        document.getElementsByName("End")[0].required = true;
        document.getElementsByName("Start")[0].required = true;
        document.getElementsByName("Start")[0].disabled = false;
        document.getElementsByName("End")[0].disabled = false;
        document.getElementsByName("Days")[0].disabled = true;
        document.getElementsByName("Hours")[0].disabled = true;
        document.getElementsByName("Minutes")[0].disabled = true;
        document.getElementsByName("onStartDate")[0].disabled = true;
        document.getElementsByName("onDueDate")[0].disabled = true;
        document.getElementsByName("onDeadline")[0].disabled = true;
        document.getElementsByName("StartDate")[0].disabled = true;
        document.getElementsByName("DueDate")[0].disabled = true;
        document.getElementsByName("Deadline")[0].disabled = true;
        document.getElementsByName("Days")[0].required = false;
        document.getElementsByName("Hours")[0].required = false;
        document.getElementsByName("Minutes")[0].required = false;
    }
}

function disDeadline() {
    if (document.getElementsByName("onDeadline")[0].checked == true) {
        document.getElementsByName("Deadline")[0].disabled = false;
        document.getElementsByName("Deadline")[0].required = true;
    } else {
        document.getElementsByName("Deadline")[0].disabled = true;
        document.getElementsByName("Deadline")[0].required = false;
    }
}

function disDueDate() {
    if (document.getElementsByName("onDueDate")[0].checked == true) {
        document.getElementsByName("DueDate")[0].disabled = false;
        document.getElementsByName("DueDate")[0].required = true;
        limitStartDate();
    } else {
        document.getElementsByName("DueDate")[0].disabled = true;
        document.getElementsByName("DueDate")[0].required = false;
    }
}

function disStartDate() {
    if (document.getElementsByName("onStartDate")[0].checked == true) {
        document.getElementsByName("StartDate")[0].disabled = false;
        document.getElementsByName("StartDate")[0].required = true;
        limitDueDate();

    } else {
        document.getElementsByName("StartDate")[0].disabled = true;
        document.getElementsByName("StartDate")[0].required = false;
    }
}

function disStartDate() {
    if (document.getElementsByName("onStartDate")[0].checked == true) {
        document.getElementsByName("StartDate")[0].disabled = false;
        document.getElementsByName("StartDate")[0].required = true;

    } else {
        document.getElementsByName("StartDate")[0].disabled = true;
        document.getElementsByName("StartDate")[0].required = false;
    }
}

function disProject() {
    if (document.getElementsByName("position")[0].checked == true) {
        document.getElementsByName("ParentTaskId")[0].disabled = true;
        document.getElementsByName("ProjectId")[0].disabled = false;
    } else {
        document.getElementsByName("ParentTaskId")[0].disabled = false;
        document.getElementsByName("ProjectId")[0].disabled = true;
    }
}



function limitEnd() {

    var dateStart = new Date(document.getElementsByName("Start")[0].value);
    dateStart.setMinutes(dateStart.getMinutes() + 1);
    var startStr = dateStart.toISOString().substring(0, dateStart.toISOString().length - 5);
    document.getElementsByName("End")[0].min = startStr;
    
    var dateEnd = new Date(document.getElementsByName("End")[0].value);
    if (dateEnd.getTime() < dateStart.getTime()) {
        document.getElementsByName("End")[0].value = startStr;
    }
}

function limitStart() {
    var dateEnd = new Date(document.getElementsByName("End")[0].value);
    dateEnd.setMinutes(dateEnd.getMinutes() -1);
    var endStr = dateEnd.toISOString().substring(0, dateEnd.toISOString().length - 5);
    document.getElementsByName("Start")[0].max = endStr;

    var start = new Date(document.getElementsByName("Start")[0].value);
    if (start.getTime() > dateEnd.getTime()) {
        
        document.getElementsByName("Start")[0].value = endStr;
    }
}

function limitDueDate() {
    var dateStart = new Date(document.getElementsByName("StartDate")[0].value);
    dateStart.setMinutes(dateStart.getMinutes() + getEstimation());
    
    var startStr = dateStart.toISOString().substring(0, dateStart.toISOString().length - 5);
    document.getElementsByName("DueDate")[0].min = startStr;
    document.getElementsByName("Deadline")[0].min = startStr;
    var dateEnd = new Date(document.getElementsByName("DueDate")[0].value);
    if (dateEnd.getTime() < dateStart.getTime()) {
        document.getElementsByName("DueDate")[0].value = startStr;
    }
    dateEnd = new Date(document.getElementsByName("Deadline")[0].value);
    if (dateEnd.getTime() < dateStart.getTime()) {
        document.getElementsByName("Deadline")[0].value = startStr;
    }
}

function limitStartDate() {
    var dateEnd1 = new Date(document.getElementsByName("DueDate")[0].value);
    var dateEnd2 = new Date(document.getElementsByName("Deadline")[0].value);
    var dateEnd = dateEnd1;
    if (dateEnd2.getTime() < dateEnd1.getTime()) {
        dateEnd = dateEnd2;
    }
    var dateMax = new Date(dateEnd);
    dateMax.setMinutes(dateEnd.getMinutes() - getEstimation());
    var startMin = new Date(document.getElementsByName("Start")[0].min);
    var endStr = dateEnd.toISOString().substring(0, dateEnd.toISOString().length - 5);
    if (dateMax.getTime() > startMin.getTime()) {
        
        document.getElementsByName("StartDate")[0].max = endStr;
        var start = new Date(document.getElementsByName("Start")[0].value);
        if (start.getTime() > dateEnd.getTime()) {
            document.getElementsByName("StartDate")[0].value = endStr;
        }
    } else {

        document.getElementsByName("StartDate")[0].value = startMin.toISOString().substring(0, dateEnd.toISOString().length - 5);
       limitDueDate();
    }

}

function getEstimation() {
    var d = parseInt(document.getElementsByName("Days")[0].value);
    var h = parseInt(document.getElementsByName("Hours")[0].value);
    var m = parseInt(document.getElementsByName("Minutes")[0].value);
    return (d * 24 + h) * 60 + m;
}
function limitMinutes() {
    var d = parseInt(document.getElementsByName("Days")[0].value);
    var h = parseInt(document.getElementsByName("Hours")[0].value);
    if (d == 0 && h == 0) {
        document.getElementsByName("Minutes")[0].min = 5;
    } else {
        document.getElementsByName("Minutes")[0].min = 0;
    }
}

function checkdate() {
    limitMinutes();
    if (document.getElementsByName("onDueDate")[0].checked == true || document.getElementsByName("onDeadline")[0].checked == true) {
        limitStartDate();
    }
    if (document.getElementsByName("onStartDate")[0].checked == true) {
        limitDueDate();
    }
    
}