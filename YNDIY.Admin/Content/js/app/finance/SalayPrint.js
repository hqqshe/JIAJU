require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ type: $("._opacity_select").val(), searchKey: $(".search_input").val(), pageIndex: _page_index }, "/ERPFinance/GetCustomerAcount")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
    });
});