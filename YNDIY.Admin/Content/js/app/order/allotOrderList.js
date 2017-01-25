require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'laydate', 'domReady'], function ($, _, base, _, _) {
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
            
            laydate(start);
            laydate(end);

        });
        var part_item = '<tr><td></td><td></td><td></td></tr>';
        var order_id; //订单号
        //加载数据
        window._load_items = function (_page_index) {
            $.when(base.loadHtmlData({ status: $("#status").val(), pageIndex: _page_index, start_data: $('#start_date').val(), end_data: $('#end_date').val() }, "/ERPOrder/GetallotOrderList")).done(function (res) { $("._ajax_container").empty().append(res); base.repainWindow(); });
        }
        _load_items(1);
        
        //出库计划面板
        $("._ajax_container").on("click", ".join", function () {
            var par_tr = $(this).parents('tr').eq(0).children();
            $('.join_con .order_info tbody td').each(function (k, v) {
                if (k === 4) {
                    $(v).text(par_tr.eq(10).text() - par_tr.eq(11).text() - par_tr.eq(12).text());
                } else {
                    $(v).text(par_tr.eq(k + 10).text());
                }
            });
            order_id = $(this).attr("_id");
            fillHistory();
            $('.join_con').fadeIn();
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
            var limit= $('.join_con .order_info tbody td').eq(4).text();
            if (num > limit) {
                $.Message({ type: "alert", content: "本次出库数量不能超过待出库数量" });
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
            fillStoreCon($(this).attr("_id"), $(this).attr("pid"));
        });
        function fillStoreCon(id,pid){
            $.when(base.loadHtmlData({ productId: pid, id: id }, "/ERPOrder/GetAllocation")).done(function (res) { $(".get_store_con .pop_con").empty().append(res); $('.get_store_con').fadeIn(); });
        }
        //提交出库计划
        $(".get_store_con").on("click", ".sub_mit", function () {

        });
        //获取批量选择id
        function getOrderIds() {
            var ids = "";
            $('.get_store_con tbody tr.plus').each(function (k, v) {
                var order_id = $(v).attr("_id");
                ids += order_id;
                ids += ",";
            });
            return ids.replace(/,$/, "");
        }
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