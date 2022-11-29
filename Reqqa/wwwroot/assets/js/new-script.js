 
        $(window).on('load', function () {

            $(".layer-preloader").fadeOut(700, function () {

                $(".lds-dual-ring").delay(1000).fadeOut(700);

                $("body").css("overflow-y", "auto");

            });
        });
     