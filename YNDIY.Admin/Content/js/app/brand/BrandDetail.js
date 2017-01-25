require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'domReady'], function ($,base) {
        base.init();
        //记录列表切换
        $(".detail_btn").on("click", "a", function () {
            var index = $(this).index();
            $(this).addClass("blue cur").siblings().removeClass("blue cur")
            $('.detail_tab').children().eq(index).fadeIn().siblings().hide();
        });
    });
});
