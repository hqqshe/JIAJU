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
            $('#pop_start').val(laydate.now).attr('disabled','disabled');
        });
        var order_id;
        var _customer_list = null;
        var _request_brand_id = null;
        var cid = window.location.search.split("=")[1];
        //加载数据
        window._load_items = function (_page_index) {
            $.when(base.loadHtmlData({ productId: cid, startTime: $("#start_date").val(), endTime: $("#end_date").val(), isRelation: $("#isRelation").val(), shopName: $('.search_input').val(), pageIndex: _page_index }, "/ERPProduct/GetForecastDetail")
            ).done(function (res) { $("._ajax_container").empty().append(res); });
        }
        _load_items(1);
        //制定生产计划面板
        $(".make_plan").on("click", function () {
            var plus = $('._ajax_container tbody tr.plus');
            if (plus.length >0) {
                var number = 0;
                plus.each(function (k, v) {
                    number += +$(v).attr('_wait_process');
                });
            }
            $('.plan_con').fadeIn().find('._number').val(number);
            $("#pop_end,._plan_remark").val("");
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
        //获取提交数据
        function GetSubmitData() {
            var data = {};
            data.number = $("._number").val();
            data.customerId = $("#customer").val();
            data.customer = $("#customer option:selected").text();
            data.createTime = $("#pop_start").val();
            data.deliveryTime = $("#pop_end").val();
            data.remarks = $("._plan_remark").val();
            var tds = $('.plan_con .order_info ._produt_info td');
            data.brandId = tds.eq(0).attr('_id');
            data.brandName = tds.eq(0).text();
            data.productId = tds.eq(1).attr('_id');
            data.productName = tds.eq(1).text();
            data.productModel = tds.eq(2).text();
            data.productFormat = tds.eq(3).text();
            data.productColor = tds.eq(4).text();
            data.saleOrderIds = getOrderIds();
            return data;
        }
        //提交生产计划
        $(".plan_con").on("click", ".sub_mit", function () {
            var _data = GetSubmitData();
            if (!/^[1-9]\d*$/.test(_data.number)) {
                $.Message({ type: "alert", content: "请输入正确数量！" });
                return;
            }
            if (typeof _data.customerId == "undefined" || _data.customer == "") {
                $.Message({ type: "alert", content: "请选择客户" });
                return;
            }
            if (_data.createTime == "") {
                $.Message({ type: "alert", content: "请选择下单日期" });
                return;
            }
            if (_data.deliveryTime == "") {
                $.Message({ type: "alert", content: "请选择交货日期" });
                return;
            }
            var tip = $('.plan_con input').eq(2).val();
            $.when(base.getCodeData(_data, "/ERPProduct/submitAddProductionOrder")).done(function () {
                $.Message({ type: "alert", content: "保存成功！" });
                $('.plan_con').fadeOut();
                _load_items(1);
            });
        });
    });
});