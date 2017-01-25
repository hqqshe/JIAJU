require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'laydate', 'domReady'], function ($, _, base, _, _) {
        base.init();
        require(['laydate'], function () {
            var start = {
                elem: '#start_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: true,
                choose: function (datas) {
                    end.min = datas; //开始日选好后，重置结束日的最小日期
                    end.start = datas; //将结束日的初始值设定为开始日
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
            var out_date = {
                elem: '#out_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                }
            };
            laydate(start);
            laydate(end);
            laydate(out_date);
        });
        var part_item = '<tr><td></td><td></td><td></td></tr>';
        var order_id; //订单号
        //加载数据
        window._load_items = function (_page_index) {
            $.when(base.loadHtmlData({ order_status: $("#order_status").val(), yanqi_status: $("#yanqi_status").val(), kucun_status: $("#kucun_status").val(), searchKey: $('.search_input').val(), pageIndex: _page_index, start_data: $('#start_date').val(), end_data: $('#end_date').val() }, "/ERPOrder/GetOrderList")).done(function (res) { $("._ajax_container").empty().append(res); base.repainWindow(); });
        }
        _load_items(1);
        //出库计划面板
        $("._ajax_container").on("click", ".join", function () {
            var par_tr = $(this).parents('tr').eq(0).children();
            $('.join_con .order_info tbody td').each(function (k, v) {
                if (k === 1) {
                    $(v).text(par_tr.eq(10).text() - par_tr.eq(11).text() - par_tr.eq(12).text());
                } else if (k === 3) {
                    $(v).text(par_tr.eq(12).text());
                } else if (k === 2) {
                    $(v).text(par_tr.eq(11).text());
                }else if (k === 4) {
                    var num = +par_tr.eq(10).text() - +par_tr.eq(11).text() - +par_tr.eq(12).text();
                    var num1 = null;
                    var index = 0;
                    par_tr.eq(13).find('.link_item ').each(function (k,v) {
                        var temp = $(v).text();
                        temp = parseInt(temp.substring(temp.lastIndexOf(')') + 1));
                        if (num1 && num1 < temp) {
                            num1 = temp;
                        } else {
                            num1 = temp;
                        }
                        index = k;
                        //num1 = num1 && num1 < temp ? num1 : temp;
                    });
                    num1 = num1 + parseInt(par_tr.eq(14).children().eq(index).text());
                    num >= num1 && $(v).text(num1) || $(v).text(num);
                }
                else if (k === 0)
                {
                    $(v).text(par_tr.eq(10).text());
                }
            });
            order_id = $(this).attr("_id");
            fillHistory();
            $('.join_con').fadeIn().find('input').val('');
        });
        //填充出库历史
        function fillHistory() {
            $.when(base.getData({ orderId: order_id }, "/ERPOrder/GetSaleOrderOutHistory")).done(function (res) {
                var con = $('.join_con .out_history tbody');
                con.empty();
                for (var i = 0; i < res.length; i++) {
                    var item = $(part_item);
                    item.children().eq(0).text(res[i].plan_out_date);
                    item.children().eq(1).text(res[i].out_count);
                    item.children().eq(2).text(res[i].state);
                    con.append(item);
                }
            });
        }
        //提交出库计划
        $(".join_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.join_con input').eq(0).val());
            if (!num) {
                $.Message({ type: "alert", content: "请输入本次出库数量！" });
                return;
            }
            var limit = $('.join_con .order_info tbody td').eq(4).text();
            if (num > limit) {
                $.Message({ type: "alert", content: "本次出库数量不能超过可出库数量" });
                return;
            }
            var date = $('.join_con input').eq(1).val();
            if (!date) {
                $.Message({ type: "alert", content: "请选择出库日期！" });
                return;
            }
            var tip = $('.join_con input').eq(2).val();
            $.when(base.getCodeData({ orderId: order_id, outCount: num, outDate: date, remarks: tip }, "/ERPOrder/AddStorageOutPlan")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.join_con').fadeOut();
                _load_items(1);
            });
        });
        //调拨库存面板
        $("._ajax_container").on("click", ".get_store", function () {
            order_id = $(this).attr("_id");
            pid = $(this).attr("pid");
            $.when(base.loadHtmlData({ productId: pid, id: order_id }, "/ERPOrder/GetAllocation")).done(function (res) { $(".get_store_con .pop_con").empty().append(res); $('.get_store_con').fadeIn(); });
        });

        //提交调拨库存
        $(".get_store_con").on("click", ".sub_mit", function () {

            var number = $('.sub_num').val(), ids = '', counts = '';
            if (!number && $('.get_store_con tbody tr.plus').length === 0) {
                $.Message({ type: "alert", content: "请输入库存调拨数量或者选择销售单调拨库存" });
                return;
            }
            if (number > $('.sub_num').parent().prev().text()) {
                $.Message({ type: "alert", content: "库存调拨数量不能大于库存可用数量" });
                return;
            }
            var isok = false;
            $('.get_store_con tbody tr.plus').each(function (k, v) {
                ids += $(v).attr("_id") + ',';
                var count = $(v).find("input[type='text']").val();
                if (!count) {
                    $.Message({ type: "alert", content: "请输入订单" + $(v).children().eq(1).text() + " 调拨数量！" });
                    isok = true;
                    return false;
                }
                counts += count + ',';
            });
            if (isok) return;
            ids = ids.replace(/,$/, "");
            counts = counts.replace(/,$/, "");
            $.when(base.getCodeData({ id: order_id, stockCount: number, saleOrderIds: ids, saleOrderCount: counts }, "/ERPOrder/SetAllocation")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.get_store_con').fadeOut();
                _load_items(1);
            });
        });
        //删除销售单
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            var txt = _that.parents("tr").first().children().eq(0).text();
            $.Message({
                type: "confirm", content: "确定删除 " + txt + " 销售单？删除后数据不可恢复，请慎重！", okFun: function () {
                    var _id = _that.attr("_id");
                    $.when(base.getCodeData({ order_id: order_id, outCount: num, outDate: date, remarks: tip }, "/ERPOrder/AddStorageOutPlan")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});