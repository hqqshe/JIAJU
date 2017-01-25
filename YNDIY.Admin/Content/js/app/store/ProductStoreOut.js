require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'utils', 'laydate', 'domReady'], function ($, base) {
        base.init();
        require(['laydate'], function () {
            var start_date = {
                elem: '#start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    _load_items(1);
                }
            };
            laydate(start_date);
            $('#start_date').text(laydate.now);
            //加载数据
            window._load_items = function (_page_index) {
                $.when(base.loadHtmlData({ outDate: $("#start_date").text(), searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPStore/GetProductStoreOut")
                 ).done(function (res) { $("._ajax_container").empty().append(res); });
            }
            _load_items(1);
        });
        //提交出库
        $("._ajax_container").on("click", ".store_out", function () {
            //$('.plan_con').fadeIn();
            var _that = $(this);
            $.Message({
                type: "confirm", content: "确定出库此订单吗?", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPStore/SetProductStoreOut")).done(function () {
                        $.Message({ type: "alert", content: "提交成功！" });
                        _load_items(1);
                    });
                }
            });
        });

        //提交出库
        $(".plan_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.plan_con input').eq(0).val());
            if (!num) {
                $.Message({ type: "alert", content: "请输入本次出库数量！" });
                return;
            }
            $.when(base.getCodeData({ id: order_id, num: num, date: date, tip: tip }, "/ERPStore/SetProductStoreOut")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items(1);
            });
        });
    });
});