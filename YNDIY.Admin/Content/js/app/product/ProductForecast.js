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
            $('#pop_start').val(laydate.now);
        });
        var order_id;
        var _customer_list = null;
        var _request_brand_id = null;
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ orderStartDate: $("#start_date").val(), orderEndDate: $("#end_date").val(), safeState: $("#order_status").val(), productName: $('.search_input').val(), pageIndex: _page_index }, "/ERPProduct/GetProductForecast")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //查询条件
        $("._opacity_select").on("change", function () {
            _load_items(1);
        });
        //制定生产计划面板
        $("._ajax_container").on("click", ".make_plan", function () {
            var _brand_id = $(this).parent().parent().find("td:eq(1)").attr("_id");
            if (typeof _brand_id == "undefined") {
                $.Message({ type: "alert", content: "请求品牌参数错误" });
                return;
            }
            if (_request_brand_id != _brand_id || _customer_list == null) {
                $.when(base.getData({ brandId: _brand_id }, "/ERPProduct/GetCustomerListByBrandId")).done(function (res) {
                    RenderCustomerList(res);
                    _customer_list = res;
                    _request_brand_id = _brand_id;
                });
            } else {
                RenderCustomerList(_customer_list);
            }
            var par_tr = $(this).parents('tr').children();
            $('.plan_con .order_info tbody td').each(function (k, v) {
                $(v).text(par_tr.eq(k).text());
                if (k == 0) {
                    $(v).attr("_id", par_tr.eq(0).attr("_id"));
                }
                if (k == 1) {
                    $(v).attr("_id", par_tr.eq(1).attr("_id"));
                }
            });
            $('.plan_con').fadeIn();
            order_id = $(this).attr("_id");
            $("._customer_input").attr("_id", "").val("");
            $("#pop_end,._number,._plan_remark").val("");
        });
        //获取客户信息
        function GetCustomerList(brand_id) {
            _request_brand_id = brand_id;
            $.when(base.getData({ brandId: brand_id }, "/ERPProduct/GetCustomerListByBrandId")).done(function (res) {
                _customer_list = res;
            });
        }
        //渲染可选择客户
        function RenderCustomerList(list) {
            var _container = $("._check_customer");
            _container.empty();
            for (var i = 0; i < list.length; i++) {
                var item = $('<div class="_select_customer_item" _id="' + list[i].id + '">' + list[i].shop_name + '</div>');
                item.bind("click", function () {
                    var _this = $(this);
                    _this.parent().prev().attr("_id", _this.attr("_id")).val(_this.text().replace(/\s/g, ""));
                    _this.parent().hide();
                });
                _container.append(item);
            }
        }
        $("._customer_input").bind("click", function () {
            $("._check_customer").show();
            return false;
        });
        $("body").bind("click", function () {
            $("._check_customer").hide();
        });
        //获取提交数据
        function GetSubmitData() {
            var data = {};
            data.number = $("._number").val();
            data.customerId = $("._customer_input").attr("_id");
            data.customer = $("._customer_input").val();
            data.createTime = $("#pop_start").val();
            data.deliveryTime = $("#pop_end").val();
            data.remarks = $("._plan_remark").val();
            $("._produt_info td").each(function (i, o) {
                if (i == 0) {
                    data.productName = $(o).text().replace(/\s/g, "");
                    data.productId = $(o).attr("_id");
                }
                else if (i == 1) {
                    data.brandName = $(o).text().replace(/\s/g, "");
                    data.brandId = $(o).attr("_id");
                }
                else if (i == 2) {
                    data.productModel = $(o).text().replace(/\s/g, "");
                }
                else if (i == 3) {
                    data.productFormat = $(o).text().replace(/\s/g, "");
                }
                else if (i == 4) {
                    data.productColor = $(o).text().replace(/\s/g, "");
                }
            });
            return data;
        }
        //提交生产计划
        $(".plan_con").on("click", ".sub_mit", function () {
            var _data = GetSubmitData();
            if (!/^[1-9]\d*$/.test(_data.number)) {
                $.Message({ type: "alert", content: "请输入正确出库数量！" });
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
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items(1);
            });
        });
    });
});