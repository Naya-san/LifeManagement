var toTime = function(val) {
    return Math.floor(val / 60) + ":" + ((val % 60 > 9) ? val % 60 : "0" + val % 60);
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
                create: function() {
                //    var pers = $(".sliderPercentage").slider("value");
                 //   console.log(">" + per + "<");
                    $(".datepicker").datepicker({ dateFormat: "dd.mm.yy", }, $.datepicker.setDefaults($.datepicker.regional[LocalizeCode]));
                    $(".timepicker").timepicker($.timepicker.setDefaults($.timepicker.regional[LocalizeCode]));
                    $(".sliderSpend").slider(console.log($(".sliderSpend").attr("data-start-value")), {
                        min: 5,
                        max: 1440,
                        value: $(".sliderSpend").attr("data-start-value"),
                        slide: function (event, ui) {
                            $("#TimeToFill").val(toTime(ui.value));
                        }
                    });
                    $("#sliderWorking").slider(console.log($("#sliderWorking").attr("data-start-value")), {
                        min: 5,
                        max: 1440,
                        value: $("#sliderWorking").attr("data-start-value"),
                        slide: function (event, ui) {
                            $("#WorkingTime").val(toTime(ui.value));
                        }
                    });
                    $(".sliderPercentage").slider(console.log($(".sliderSpend").attr("data-start-value")), {
                        min: 1,
                        max: 100,
                        value: $(".sliderPercentage").attr("data-start-value"),
                        slide: function (event, ui) {
                            $("#ParallelismPercentage").val(ui.value);
                        }
                    });

                    $("#slider-rangeLow").slider({
                        range: true,
                        min: 4,
                        max: 1440,
                        values: [$("#slider-rangeLow").attr("data-start-value-from"), $("#slider-rangeLow").attr("data-start-value-to")],
                        slide: function (event, ui) {
                       //     var value = Math.floor(ui.values[0] / 60) + ":" + ((ui.values[0] % 60 > 9) ? ui.values[0] % 60 : "0" + ui.values[0] % 60);
                            $("#ComplexityLowFrom").val(toTime(ui.values[0]));
                            $("#ComplexityLowTo").val(toTime(ui.values[1]));
                        }
                    });
                },
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
                    //console.log($(".datepicker").length);
                    $(".datepicker").datepicker({ dateFormat: "dd.mm.yy" }, $.datepicker.regional[LocalizeCode]);
                    $(".timepicker").timepicker($.timepicker.setDefaults($.timepicker.regional[LocalizeCode]));
                    $(".sliderSpend").slider({
                        min: 5,
                        max: 1440,
                        slide: function (event, ui) {
                            $("#TimeToFill").val(Math.floor(ui.value / 60) + ":" + ui.value % 60);
                        }
                    });
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