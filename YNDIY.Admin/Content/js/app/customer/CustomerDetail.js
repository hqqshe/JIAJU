require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'laydate', 'pop_window', 'uploadForm', 'domReady'], function ($, base) {
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
                }
            };
            laydate(start);
            laydate(end);
        });
        //导入客户
        $("._upload").uploadForm({
            type: "customer_excel",
            success: function (_result) {
                eval("var response =" + _result);
                $.Message({ type: "alert", content: response.message });
                if (response.code) {
                    window.location.reload();
                }
            }
        });
        var cid = window.location.search.split("=")[1];
        var record_type=0//客户订单/品牌
        //加载数据
        window._load_items = function (_page_index) {
            var _url = "";
            var _data = {};
            if (record_type === 0) {
                _url = "/ERPCustomer/CustOrderList";
                _data = { id: cid, pageIndex: _page_index, startTime: $('#start_date').val(), endTime: $('#end_date').val(), payStatus: $("#order_status").val(), searchKey: $('order_seach .search_input').val() };
            } else {
                _url = "/ERPCustomer/CustBrandList";
                _data = { id: cid, pageIndex: _page_index, searchKey: $('.brand_seach .search_input').val(), brandId: $('#brand_id').val() };
            }
           $.when(base.loadHtmlData(_data, _url)).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        var _search = window.location.href;
        //查询条件
        $("._opacity_select").on("change", function () {
            _load_items(1);
        });
        //记录列表切换
        $(".detail_btn").on("click", "a", function () {
            record_type = $(this).index();
            $(this).addClass("blue cur").siblings().removeClass("blue cur")
            if (record_type === 0) {
                $('.order_seach').show().next().hide();
            } else {
                $('.order_seach').hide().next().show();
            }
            _load_items(1);
        });
    });
});