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
function showMore() {
    var obj = document.getElementById('editionalInfo');
    var label = document.getElementById('moreLabel');
    if (obj.style.display == 'none') {
        obj.style.display = 'block';
        label.className = 'openBlock';
    }
    else {
        obj.style.display = 'none';
        label.className = 'closeBlock';
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