var toTime = function (val) {
    if (val < 0) {
        return "00:00";
    }
    if (val > 1439) {
        return "23:59";
    }
    return Math.floor(val / 60) + ":" + ((val % 60 > 9) ? val % 60 : "0" + val % 60);
}
var initElements = function (val) {
    $(".datepicker").datepicker({ dateFormat: "dd.mm.yy", }, $.datepicker.setDefaults($.datepicker.regional[LocalizeCode]));
    $(".timepicker").timepicker($.timepicker.setDefaults($.timepicker.regional[LocalizeCode]));
    $(".sliderSpend").slider(console.log($(".sliderSpend").attr("data-start-value")), {
        range: "min",
        min: 5,
        max: 1439,
        value: $(".sliderSpend").attr("data-start-value"),
        slide: function (event, ui) {
            $("#TimeToFill").val(toTime(ui.value));
            $(".sliderSpend").attr("data-start-value", ui.value);
        }
    });
    $("#sliderWorking").slider(console.log($("#sliderWorking").attr("data-start-value")), {
        range: "min",
        min: 5,
        max: 1439,
        value: $("#sliderWorking").attr("data-start-value"),
        slide: function (event, ui) {
            $("#WorkingTime").val(toTime(ui.value));
            $("#sliderWorking").attr("data-start-value", ui.value);
        }
    });
    $(".sliderPercentage").slider(console.log($(".sliderSpend").attr("data-start-value")), {
        range: "min",
        min: 1,
        max: 100,
        value: $(".sliderPercentage").attr("data-start-value"),
        slide: function (event, ui) {
            $("#ParallelismPercentage").val(ui.value);
            $(".sliderPercentage").attr("data-start-value", ui.value);
        }
    });

    $("#slider-rangeLow").slider({
        range: true,
        min: 4,
        max: 1439,
        values: [$("#slider-rangeLow").attr("data-start-value-from"), $("#slider-rangeLow").attr("data-start-value-to")],
        slide: function (event, ui) {
            $("#slider-rangeLow").attr("data-start-value-from", ui.values[0]);
            $("#ComplexityLowFrom").val(toTime(ui.values[0]));
            $("#ComplexityLowTo").val(toTime(ui.values[1]));
            if($("#slider-rangeLow").attr("data-start-value-to") !== ui.values[1]){
                $("#slider-rangeLow").attr("data-start-value-to", ui.values[1]);                            
                var difference = $("#slider-rangeMedium").attr("data-start-value-to") - $("#slider-rangeMedium").attr("data-start-value-from");
                $("#slider-rangeMedium").attr("data-start-value-from", $("#slider-rangeLow").attr("data-start-value-to"));
                $("#slider-rangeMedium").attr("data-start-value-to", parseInt($("#slider-rangeLow").attr("data-start-value-to")) + difference);
                $("#slider-rangeMedium").slider("values", [$("#slider-rangeMedium").attr("data-start-value-from"), $("#slider-rangeMedium").attr("data-start-value-to")]);
                var differenceH = $("#slider-rangeHight").attr("data-start-value-to") - $("#slider-rangeHight").attr("data-start-value-from");
                $("#slider-rangeHight").attr("data-start-value-from", $("#slider-rangeMedium").attr("data-start-value-to"));
                $("#slider-rangeHight").attr("data-start-value-to", parseInt($("#slider-rangeMedium").attr("data-start-value-to")) + differenceH);
                $("#slider-rangeHight").slider("values", [$("#slider-rangeHight").attr("data-start-value-from"), $("#slider-rangeHight").attr("data-start-value-to")]);

                             
                $("#ComplexityMediumTo").val(toTime($("#slider-rangeMedium").attr("data-start-value-to")));
                $("#Medium>#ComplexityLowTo").val(toTime($("#slider-rangeLow").attr("data-start-value-to")));
                $("#Hight>#ComplexityMediumTo").val(toTime($("#slider-rangeMedium").attr("data-start-value-to")));
                $("#ComplexityHightTo").val(toTime($("#slider-rangeHight").attr("data-start-value-to")));
            }  
        }
    });
    $("#slider-rangeMedium").slider({
        range: true,
        min: 4,
        max: 1439,
        values: [$("#slider-rangeMedium").attr("data-start-value-from"), $("#slider-rangeMedium").attr("data-start-value-to")],
        slide: function (event, ui) {
            $("#slider-rangeMedium").attr("data-start-value-from", ui.values[0]);
            $("#slider-rangeMedium").attr("data-start-value-to", ui.values[1]);
            $("#Medium>#ComplexityLowTo").val(toTime(ui.values[0]));
            $("#ComplexityMediumTo").val(toTime(ui.values[1]));
            var differenceL = $("#slider-rangeLow").attr("data-start-value-to") - $("#slider-rangeLow").attr("data-start-value-from");
            var differenceH = $("#slider-rangeHight").attr("data-start-value-to") - $("#slider-rangeHight").attr("data-start-value-from");

            $("#slider-rangeLow").attr("data-start-value-from", $("#slider-rangeMedium").attr("data-start-value-from") - differenceL);
            $("#slider-rangeLow").attr("data-start-value-to", $("#slider-rangeMedium").attr("data-start-value-from"));
            $("#slider-rangeLow").slider("values", [$("#slider-rangeLow").attr("data-start-value-from"), $("#slider-rangeLow").attr("data-start-value-to")]);

            $("#slider-rangeHight").attr("data-start-value-from", $("#slider-rangeMedium").attr("data-start-value-to"));
            $("#slider-rangeHight").attr("data-start-value-to", (parseInt($("#slider-rangeMedium").attr("data-start-value-to")) + differenceH));
            $("#slider-rangeHight").slider("values", [$("#slider-rangeHight").attr("data-start-value-from"), $("#slider-rangeHight").attr("data-start-value-to")]);

            $("#ComplexityLowFrom").val(toTime($("#slider-rangeLow").attr("data-start-value-from")));
            $("#ComplexityLowTo").val(toTime($("#slider-rangeLow").attr("data-start-value-to")));
            $("#Hight>#ComplexityMediumTo").val(toTime($("#slider-rangeMedium").attr("data-start-value-to")));
            $("#ComplexityHightTo").val(toTime($("#slider-rangeHight").attr("data-start-value-to")));
        }
    });
    $("#slider-rangeHight").slider({
        range: true,
        min: 4,
        max: 1439,
        values: [$("#slider-rangeHight").attr("data-start-value-from"), $("#slider-rangeHight").attr("data-start-value-to")],
        slide: function (event, ui) {
                           
            $("#slider-rangeHight").attr("data-start-value-to", ui.values[1]);
            $("#Hight>#ComplexityMediumTo").val(toTime(ui.values[0]));
            $("#ComplexityHightTo").val(toTime(ui.values[1]));
            if ($("#slider-rangeHight").attr("data-start-value-from") !== ui.values[0]) {
                $("#slider-rangeHight").attr("data-start-value-from", ui.values[0]);

                var difference = $("#slider-rangeMedium").attr("data-start-value-to") - $("#slider-rangeMedium").attr("data-start-value-from");
                $("#slider-rangeMedium").attr("data-start-value-from",$("#slider-rangeHight").attr("data-start-value-from") - difference);
                $("#slider-rangeMedium").attr("data-start-value-to", $("#slider-rangeHight").attr("data-start-value-from"));
                $("#slider-rangeMedium").slider("values", [$("#slider-rangeMedium").attr("data-start-value-from"), $("#slider-rangeMedium").attr("data-start-value-to")]);
                                
                var differenceL = $("#slider-rangeLow").attr("data-start-value-to") - $("#slider-rangeLow").attr("data-start-value-from");
                $("#slider-rangeLow").attr("data-start-value-from", $("#slider-rangeMedium").attr("data-start-value-from") - differenceL);
                $("#slider-rangeLow").attr("data-start-value-to", $("#slider-rangeMedium").attr("data-start-value-from"));
                $("#slider-rangeLow").slider("values", [$("#slider-rangeLow").attr("data-start-value-from"), $("#slider-rangeLow").attr("data-start-value-to")]);

                $("#ComplexityLowFrom").val(toTime($("#slider-rangeLow").attr("data-start-value-from")));
                $("#ComplexityLowTo").val(toTime($("#slider-rangeLow").attr("data-start-value-to")));
                $("#Medium>#ComplexityLowTo").val(toTime($("#slider-rangeLow").attr("data-start-value-to")));
                $("#ComplexityMediumTo").val(toTime($("#slider-rangeMedium").attr("data-start-value-to")));

            }
        }
    });
}

$(function () {

    $.ajaxSetup({
        cache: false
    });

    // Wire up the click event of any current or future dialog links
    $(".dialogLink").live("click", function () {
        var element = $(this);

        // Retrieve values from the HTML5 data attributes of the link        
        var dialogTitle = element.attr("data-dialog-title");
        var updateTargetId = "#" + element.attr("data-update-target-id");
        var updateUrl = element.attr("data-update-url");
        // Generate a unique id for the dialog div
        var dialogId = "uniqueName-" + Math.floor(Math.random() * 1000);
        var dialogDiv = "<div id='" + dialogId + "'></div>";

        // Load the form into the dialog div
        $(dialogDiv).load(this.href, function () {
            //$(".datepicker").datepicker({ dateFormat: 'dd.mm.yy' });
            //$(".timepicker").timepicker();
            $(this).dialog({
                width: element.attr("data-dialog-width"),
                height: "auto",
                close: function() { $(this).remove(); },
                title: dialogTitle,
                create: initElements,
                modal: true,
                draggable: true,
                resizable: false,
                position: ["center", "center"]
            });

            // Enable client side validation
      //      $.validator.unobtrusive.parse(this);

            // Setup the ajax submit logic
          
            wireUpForm(this, updateTargetId, updateUrl);
        });
        return false;
    });
});
function wireUpForm(dialog, updateTargetId, updateUrl) {
    $("form", dialog).submit(function () {

        // Do not submit if the form
        // does not pass client side validation
        if (!$(this).valid())
            return false;

        // Client side validation passed, submit the form
        // using the jQuery.ajax form
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                // Check whether the post was successful
                if (result.success) {                    
                    // Close the dialog 
                    $(dialog).dialog("close");

                    // Reload the updated data in the target div
                    if (updateUrl == "") {
                        location.reload();
                    } else {
                        $(updateTargetId).load(updateUrl);
                    }
                } else {
                    // Reload the dialog to show model errors                    
                    $(dialog).html(result);
                    initElements();
                    //console.log($(".datepicker").length);
                    //$(".datepicker").datepicker({ dateFormat: "dd.mm.yy" }, $.datepicker.regional[LocalizeCode]);
                    //$(".timepicker").timepicker($.timepicker.setDefaults($.timepicker.regional[LocalizeCode]));
                    //$(".sliderSpend").slider({
                    //    min: 5,
                    //    max: 1440,
                    //    slide: function (event, ui) {
                    //        $("#TimeToFill").val(Math.floor(ui.value / 60) + ":" + ui.value % 60);
                    //    }
                    //});
                    // Enable client side validation
                    //$.validator.unobtrusive.parse(dialog);

                    // Setup the ajax submit logic
                    wireUpForm(dialog, updateTargetId, updateUrl);
                }
            }
        });
        return false;
    });
}