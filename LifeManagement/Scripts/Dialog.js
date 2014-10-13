$.ajaxSetup({ cache: false });

$(document).ready(function () {

    //событие по клику объекта класса openDialog
    $(".openDialog").live("click", function (e) {

        //отмена действия по умолчанию 
        e.preventDefault();

        var loadRoute = this.href;

        var successfunction = $(this).attr("data-dialog-successfunction");

        var self = this;

        //создаем диалог
        $("<div></div>")
            .addClass("dialog")
            .attr("id", $(this).attr("data-dialog-id"))
            .appendTo("body")
            .dialog({
                title: $(this).attr("data-dialog-title"),
                close: function () { $(this).remove(); },
                width: $(this).attr("data-dialog-width"),
                height: $(this).attr("data-dialog-height"),
                modal: true,
                resizable: false,
                draggable: false,
                position: ['center', 'center']
            })
            .load(loadRoute, function (response, status, xhr) {
                if (status == 'error') {
                    $(".dialog").dialog("close");
                } else {
                    $(this).css('height', '');
                    $(this).dialog("option", "position", ['center', 'center']);
                }
            });
    });
});