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
                    end.min = datas; //开始日选好后，重置结束日的最小日期
                    end.start = datas //将结束日的初始值设定为开始日
                }
            };
            var end = {
                elem: '#end_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    start.max = datas; //结束日选好后，重置开始日的最大日期
                    _load_items(1);
                }
            };
            var jiaohuo = {
                elem: '#jiaohuo_start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    jiaohuo_end.min = datas; //开始日选好后，重置结束日的最小日期
                    jiaohuo_end.start = datas //将结束日的初始值设定为开始日
                }
            };
            var jiaohuo_end = {
                elem: '#jiaohuo_end_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    jiaohuo.max = datas; //结束日选好后，重置开始日的最大日期
                    _load_items(1);
                }
            };
            var pop_start = {
                elem: '#pop_start',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                choose: function (datas) {
                    end.min = datas; //开始日选好后，重置结束日的最小日期
                    end.start = datas //将结束日的初始值设定为开始日
                }
            };
            var pop_end = {
                elem: '#pop_end',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    start.max = datas; //结束日选好后，重置开始日的最大日期
                }
            };
            laydate(pop_start);
            laydate(pop_end);
            laydate(start);
            laydate(end);
            laydate(jiaohuo);
            laydate(jiaohuo_end);
            $('#pop_start').val(laydate.now);
        });
        var order_id;
        //加载数据
        window._load_items = function (_page_index) {
            $.when(base.loadHtmlData({ lateState: $("#late_state").val(), orderState: $("#order_state").val(), orderStartDate: $("#start_date").val(), orderEndDate: $("#end_date").val(), jiaohuoStartDate: $("#jiaohuo_start_date").val(), jiaohuoEndDate: $("#jiaohuo_end_date").val(), searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPProduct/GetProductList")
            ).done(function (res) { $("._ajax_container").empty().append(res); base.repainWindow(); });
        }
        _load_items(1);
        //制定生产计划面板
        $("._ajax_container").on("click", ".edit_paln", function () {
            $('.plan_con').fadeIn();
            order_id = $(this).attr("_id");
        });
        //查询条件
        $("._opacity_select").on("change", function () {
            _load_items(1);
        });
        //提交生产计划
        $(".plan_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.plan_con input').eq(0).val());
            if (!num) {
                $.Message({ type: "alert", content: "请输入本次出库数量！" });
                return;
            }
            var date = $('.plan_con input').eq(1).val();
            if (!date) {
                $.Message({ type: "alert", content: "请选择出库日期！" });
                return;
            }
            var tip = $('.plan_con input').eq(2).val();
            _show_loading();
            $.ajax({
                type: "get",
                url: "/ERPFinance/Recharge",
                data: { order_id: order_id, num: num, date: date, tip: tip },
                success: function (response) {
                    _hide_loading()
                    if (response.code == 1) {
                        $.Message({ type: "alert", content: "提交成功！" });
                        $('.plan_con').fadeOut()
                        _load_items(1);
                    } else {
                        $.Message({ type: "alert", content: response.message });
                    }
                },
                error: function () {
                    _hide_loading();
                    $.Message({ type: "alert", content: "网络异常,请刷新重试！" });
                }
            });
        });
        //删除生产计划单
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            var txt = _that.parents("tr").first().children().eq(0).text();
            $.Message({
                type: "confirm", content: "确定删除 " + txt + "生产计划吗？删除后数据不可恢复，请慎重！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteProduct")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});