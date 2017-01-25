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
            laydate(jiaohuo);
            laydate(jiaohuo_end);
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
           $.when(base.loadHtmlData({orderStartDate: $("#start_date").val(), orderEndDate: $("#end_date").val(), jiaohuoStartDate: $("#jiaohuo_start_date").val(), jiaohuoEndDate: $("#jiaohuo_end_date").val(), searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPProduct/GetPendingList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //查询条件
        $("._opacity_select").on("change", function () {
            _load_items(1);
        });
        //选择计划生产日期
        $("._choose_date").live("click", function () {
            var _me = $(this);
            $("#laydate").attr("id", "");
            _me.attr("id", "laydate");
            laydate({
                elem: '#laydate',
                format: 'YYYY-MM-DD',
                min: laydate.now(),
                isclear: false,
                istime: false,
                issure: false,
                istoday: false,
                choose: function (dates) { //选择好日期的回调
                    $.Message({
                        type: "confirm", content: "您计划开始日期是：" + dates, okFun: function () {
                            SubmitPlan(_me, dates);
                        },
                        cancelFun: function () {
                            _me.val('请选择计划日期');
                        }
                    });
                }
            });
        });
        //提交计划生产日期
        function SubmitPlan(_obj, dates) {
            $.when(base.getCodeData({ id: _obj.attr("_id"), starTime: dates }, "/ERPPlan/setStartTime")).done(function () {
                _load_items(1);
            });
        }
    });
});