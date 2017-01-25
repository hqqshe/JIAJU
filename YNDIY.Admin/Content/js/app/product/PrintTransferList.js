require.config({
    baseUrl: "/Content/minjs",
    paths: {
        'jquery': 'jquery-1.7.2',
        'barcode': 'jquery-barcode'
    },
    shim: {
        'barcode': 'jquery'
    }
});
require(['jquery'], function ($) {
    require(['barcode'], function () {
        $("._bar_code_container").each(function (k, v) {
            var code = $(v).text();
            $(v).barcode(code, "code128", { barWidth: 1, barHeight: 40, showHRI: false });
        });
    });
    $("._start_time").each(function (i, o) {
        var _text = $(o).text();
        $(o).text(_text.replace(/\//g, "-").replace(/(\s|0:00:00)/g, ""));
    });
    $(".year_date").each(function (i, o) {
        $(o).text(new Date().getFullYear())
    });
})