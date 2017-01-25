require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'laydate', 'domReady'], function ($, _, base, utils, _) {
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
                    end_date.min = datas; //开始日选好后，重置结束日的最小日期
                    end_date.start = datas; //将结束日的初始值设定为开始日
                }
            };
            var end_date = {
                elem: '#end_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                isclear: false,
                issure: false,
                choose: function (datas) {
                    start_date.max = datas; //结束日选好后，重置开始日的最大日期
                    _load_items(1);
                }
            };
            laydate(start_date);
            laydate(end_date);
            var start_date1 = {
                elem: '#start_date1',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                isclear: false,
                issure: false,
                choose: function (datas) {
                    end_date.min = datas; //开始日选好后，重置结束日的最小日期
                    end_date.start = datas; //将结束日的初始值设定为开始日
                }
            };
            var end_date1 = {
                elem: '#end_date1',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59', //最大日期
                istime: false,
                istoday: false,
                isclear: false,
                issure: false,
                choose: function (datas) {
                    start_date.max = datas; //结束日选好后，重置开始日的最大日期
                    _load_items(1);
                }
            };
            laydate(start_date1);
            laydate(end_date1);
        });
        var shopId = window.location.href.split('=')[1];
        var record_type = 0;//记录类型 0 订单信息 1收款记录 
        var saleId;//订单号
        //加载数据
        window._load_items = function (_page_index) {
            var _url = "";
            var _data = {};
            if (record_type === 0) {
                _url = "/ERPFinance/InfoOrderList";
                _data = { searchType: $('#data_type').val(), startTime: $('#start_date').val(), endTime: $('#end_date').val(), payStatus: $("#order_status").val(), searchKey: $('.search_input').eq(0).val() };
            } else if (record_type === 1) {
                _url = "/ERPFinance/InfoPendingList";
                    _data = { startTime: $('#start_date1').val(), endTime: $('#end_date1').val(),searchKey: $('.search_input').eq(1).val() };
                } else {
                _url = "/ERPFinance/InfoReceipt";
            }
            _data.pageIndex = _page_index; _data.id = shopId;
            $.when(base.loadHtmlData(_data, _url)).done(function (res) { $("._ajax_container").empty().append(res); });
            $('#total_money').text('');
        }
        _load_items(1);
        //记录列表切换
        $(".detail_btn").on("click", "a", function () {
            record_type = $(this).index();
            $(this).addClass("blue cur").siblings().removeClass("blue cur");
            record_type < 2 ? $('.order_seach').eq(record_type).show().siblings('.order_seach').hide() : $('.order_seach').hide();
            _load_items(1);
        });
        //表格复选框单选
        $("._ajax_container").on("click", "tbody input[type=checkbox]", function () {
            if (record_type !== 1) return;
            var sum = 0;
            $("._ajax_container .plus td:nth-child(11)").each(function (k,v) {
                sum+=+$(v).text();
            });
            sum ? $('#total_money').text('欠款为：' + sum) : $('#total_money').text('');
        });
        //审核面板
        $("._ajax_container").on("click", ".check", function () {
            order_id = $(this).attr("_id");
            var par_tr = $(this).parents('tr').eq(0).children();
            $('.plan_con .order_info tbody td').each(function (k, v) {
                if (k === 4) {
                    $(v).text(par_tr.eq(9).text());
                }
            });
            $('.plan_con').fadeIn();
        });
        //提交审核
        $(".plan_con").on("click", ".sub_mit", function () {
            //var num = parseInt($('.plan_con input').eq(0).val());
            $.when(base.getCodeData({ planId: order_id, examineState: $('.plan_con input[name=check]:checked').val(), remark: "" }, "/ERPFinance/ExaminePlanOut")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items(1);
            });
        });

        //定金/冲抵面板
        $("._ajax_container").on("click", ".deposit,.pay", function () {
            if ($(this).hasClass("pay")) {
                $('.pay_con').fadeIn().find(".panl_name").text("冲抵");
            } else {
                $('.pay_con').fadeIn().find(".panl_name").text("定金");
            }
            saleId = $(this).attr("_id")
        });
        //批量冲抵面板
        $(".all_pay").on("click", function () {
            var count = $('._ajax_container tbody tr.plus').length;
            if (count < 1) {
                $.Message({ type: "alert", content: "请选择要冲抵的订单！" });
                return;
            }
            $('.all_pay_con').fadeIn().find(".tip").eq(0).next().text(count).parent().next().children().eq(1).text(getOrdersMoney());
        });
        //获取批量选择金额
        function getOrdersMoney() {
            var money = 0;
            $('._ajax_container tbody tr.plus').each(function (k, v) {
                var num = parseFloat($(v).children().eq(14).text());
                money = num ? money += num : null;
            });
            return money;
        }
        //打开付款面板
        $("._btns").on("click", "a", function () {
            $('.invent_con').fadeIn().find(".sub_num").val("").parent().find(".tip_txt").val("");
        });
        //提交付款
        $(".invent_con").on("click", ".sub_money", function () {
            var money = parseFloat($('.invent_con .sub_num').val())
            if (!money) {
                $.Message({ type: "alert", content: "请输入正确金额！" });
                return;
            }
            $.when(base.getCodeData({ shopId: shopId, money: money, chargeType: $(".invent_con input[name='chargeType']:checked").val(), remarks: $(".invent_con .tip_txt").val() }, "/ERPFinance/Recharge")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.invent_con').fadeOut()
                window.location.reload();
            });
        });
        //获取批量选择id
        function getOrderIds() {
            var ids = "";
            $('._ajax_container tbody tr.plus').each(function (k, v) {
                var order_id = $(v).attr("_id");
                ids += order_id;
                ids += ",";
            });
            return ids.replace(/,$/, "");
        }
        //冲抵/定金 提交
        $(".pay_con").on("click", ".submit", function () {
            var money = parseFloat($('.pay_con .sub_num').val())
            if (!money) {
                $.Message({ type: "alert", content: "请输入正确金额！" });
                return;
            }
            $.when(base.getResponsData({ saleId: saleId, shopId: shopId, type: $("#data_type").val(), money: $(".pay_con input").val(), startTime: $("#start_date").val(), endTime: $("#end_date").val() }, "/ERPFinance/PayTheOrderMoney")).done(function (res) {
                $.Message({ type: "alert", content: res.message });
                $('.pay_con').fadeOut().find(".sub_num").val("");
                _load_items(1);
                //window.location.reload();
            });
        });
        //批量冲抵 提交
        $(".all_pay_con").on("click", ".submit", function () {
            var ids = getOrderIds();
            if (ids == "") {
                $.Message({ type: "alert", content: "请选择要冲抵的订单！" });
                return;
            }
            var money = parseFloat($('.all_pay_con .sub_num').val())
            if (!money) {
                $.Message({ type: "alert", content: "请输入正确金额！" });
                return;
            }
            $.when(base.getResponsData({ shopId: shopId, orderList: ids, type: $("#data_type").val(), startTime: $("#start_date").val(), endTime: $("#end_date").val(), money: money }, "/ERPFinance/PayOrderListMoney")).done(function (res) {
                $.Message({ type: "alert", content: res.message });
                $('.all_pay_con').fadeOut();
                _load_items(1);
                //window.location.reload();
            });
        });
    });
});