require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'laydate', 'domReady'], function ($, base) {
        base.init();
        require(['laydate'], function () {
            var start_date = {
                elem: '#start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                isclear: false,
                issure: false,
                choose: function (datas) {
                    _load_items(1);
                }
            };
            laydate(start_date);
            $("#start_date").text(laydate.now());
            //加载数据
            window._load_items = function (_page_index) {
                $.when(base.loadHtmlData({ outDate: $("#start_date").text(), searchKey: $(".search_input").val(), pageIndex: _page_index }, "/ERPFinance/GetStoreOutCheck")
                ).done(function (res) {$("._ajax_container").empty().append(res);});
            }
            _load_items(1);
        });
        var order_id = 0;
        //审核面板
        $("._ajax_container").on("click", ".check", function () {
            order_id = $(this).attr("_id");
            var par_tr = $(this).parents('tr').eq(0).children();
            $.when(base.getData({ id: par_tr.eq(2).attr('_id') }, "/ERPFinance/GetOutExamineCustomerInfo")).done(function (res) {
                $('.plan_con .order_info tbody td').each(function (k, v) {
                    if (k === 0) {
                        $(v).text(par_tr.eq(2).text());
                    } else if (k === 1) {
                        $(v).text(res.jie_suan);
                    } else if (k === 2) {
                        $(v).text(res.balance_money);
                    } else if (k === 3) {
                        $(v).text(res.credit);
                    } else if (k === 4) {
                        $(v).text(par_tr.eq(9).text());
                    }
                });
            });
            $('.plan_con').fadeIn();
        });
        //提交审核
        $(".plan_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.plan_con input').eq(0).val());
            $.when(base.getCodeData({ planId: order_id, examineState: $('.plan_con input[name=check]:checked').val(), remark: "" }, "/ERPFinance/ExaminePlanOut")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items(1);
            });
        });
    });
});