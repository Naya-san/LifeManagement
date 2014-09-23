(function ($) {
    $(function () {
        jQuery.noConflict(true);
        function createCookie(name, value, days) {
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                var expires = "; expires=" + date.toGMTString();
            }
            else var expires = "";
            document.cookie = name + "=" + value + expires + "; path=/";
        }
        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }
        function eraseCookie(name) {
            createCookie(name, "", -1);
        }

        $('ul.tabs').each(function (i) {
            var cookie = readCookie('tabCookie' + i);
            if (cookie) $(this).find('li').eq(cookie).addClass('current').siblings().removeClass('current')
                .parents('div.section').find('div.box').hide().eq(cookie).show();
        })

        $('ul.tabs').on('click', 'li:not(.current)', function () {
            $(this).addClass('current').siblings().removeClass('current')
                .parents('div.section').find('div.box').eq($(this).index()).fadeIn(150).siblings('div.box').hide();
            var ulIndex = $('ul.tabs').index($(this).parents('ul.tabs'));
            eraseCookie('tabCookie' + ulIndex);
            createCookie('tabCookie' + ulIndex, $(this).index(), 365);
        })

    })
})(jQuery)