require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'laydate', 'pop_window', 'utils', 'domReady'], function ($, base) {
        base.init();
        require(['laydate'], function () {
            var start = {
                elem: '#start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                choose: function (datas) {
                    _load_items(1);
                }
            };
            laydate(start);
            $('#start_date').text(laydate.now);
        });
        var order_id;
        var _customer_list = null;
        var _request_brand_id = null;
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ date: $("#start_date").text().replace(/\s/g, ""), pageIndex: _page_index }, "/ERPProduct/GetArrangedList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
            $("._ajax_container ._choose_date").each(function (i, o) {
                var _text = $(o).text();
                $(o).text(_text.replace(/\//g, "-").replace(/(\s|0:00:00)/g, ""));
            });
        }
        _load_items(1);     
        //取消生产计划
        $("._ajax_container").on("click", ".cancle_pro", function () {
            var _that = $(this);
            var txt = _that.parents("tr").first().children().eq(0).text();
            $.Message({
                type: "confirm", content: "确定取消 " + txt + " 生产计划吗？", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteProduct")).done(function () {
                        $.Message({ type: "alert", content: "取消成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});