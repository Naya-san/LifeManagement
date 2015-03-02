$(function ($) {
    var refrashTask = function (e) {
        //   console.log("!!!" + e.data('id'));
        var carentKnob = e.find("input.knob").get(0);
        var newValue = carentKnob.value;
        var ID =  e.data('id'); // store the ID of the student (change to suit your property name)
        var url = taskUpdateCompleteURL; //'Tasks/UpdateCompleteLevel';
        $.post(url, { id: ID, level: newValue }, function (data) { 
            var taskDiv = e.parents('div:first');
            switch (data.success) {
                case 2: taskDiv.removeClass('task').addClass('Сompleted');
                   // carentKnob.data('fgcolor') = '#4e888a';
                    taskDiv.find('span.time').text(data.time);
                    break;
                case 3: taskDiv.removeClass('Сompleted').addClass('task');
                // carentKnob.data('fgcolor') = '#0e0e0e';
                    taskDiv.find('span.time').text(data.time);
                    break;
            }
        });
    };
    $("div.taskComleateLevel")
        .bind("mouseup", function () {
            refrashTask($(this));
        })
        .bind("touchend.k", function () {
            refrashTask($(this));
        })
        .bind('mousewheel', function () {
            var elem = $(this);
            clearTimeout($.data(this, 'timer'));

            $.data(this, 'timer', setTimeout(function () {
                refrashTask(elem);
            }, 500));         
        });
    $(".knob").knob({
        change: function (value) {
          //  console.log("change : " + value);
        },        
        /*format : function (value) {
            return value + '%';
        },*/
        draw: function () {

            // "tron" case
            if (this.$.data('skin') == 'tron') {

                this.cursorExt = 0.3;

                var a = this.arc(this.cv)  // Arc
                    , pa                   // Previous arc
                    , r = 1;

                this.g.lineWidth = this.lineWidth;

                if (this.o.displayPrevious) {
                    pa = this.arc(this.v);
                    this.g.beginPath();
                    this.g.strokeStyle = this.pColor;
                    this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, pa.s, pa.e, pa.d);
                    this.g.stroke();
                }

                this.g.beginPath();
                this.g.strokeStyle = r ? this.o.fgColor : this.fgColor;
                this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, a.s, a.e, a.d);
                this.g.stroke();

                this.g.lineWidth = 2;
                this.g.beginPath();
                this.g.strokeStyle = this.o.fgColor;
                this.g.arc(this.xy, this.xy, this.radius - this.lineWidth + 1 + this.lineWidth * 2 / 3, 0, 2 * Math.PI, false);
                this.g.stroke();

                return false;
            }
        }
    });

    // Example of infinite knob, iPod click wheel
    var v, up = 0, down = 0, i = 0
        , $idir = $("div.idir")
        , $ival = $("div.ival")
        , incr = function () { i++; $idir.show().html("+").fadeOut(); $ival.html(i); }
        , decr = function () { i--; $idir.show().html("-").fadeOut(); $ival.html(i); };
    $("input.infinite").knob(
                        {
                            min: 0
                        , max: 20
                        , stopper: false
                        , change: function () {
                            if (v > this.cv) {
                                if (up) {
                                    decr();
                                    up = 0;
                                } else { up = 1; down = 0; }
                            } else {
                                if (v < this.cv) {
                                    if (down) {
                                        incr();
                                        down = 0;
                                    } else { down = 1; up = 0; }
                                }
                            }
                            v = this.cv;
                        }
                        });
});