require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'laydate', 'pop_window', 'domReady'], function ($, base) {
        base.init();
        require(['laydate'], function () {
            var out_date = {
                elem: '#_from_date',
                format: 'YYYY-MM-DD',
                max: '2099-06-16 23:59:59',
                istime: false,
                istoday: false,
                choose: function (datas) {
                    _load_items(1);
                }
            };
            laydate(out_date);
            $('#_from_date').text(laydate.now());
            //加载数据
            window._load_items = function (_page_index) {
               $.when(base.loadHtmlData({ outDate: $("#_from_date").text(), isExamine: $('.finace_check').val(), examineState: $('.check_state').val(), saleNum: $('.search_input').val(), pageIndex: _page_index }, "/ERPOrder/GetDeliverPlan")
                ).done(function (res) {$("._ajax_container").empty().append(res);});
            }
            _load_items(1);
        });
        var cid = window.location.search.split("=")[1];
        var order_id;
        //修改出库数量面板
        $("._ajax_container").on("click", ".modify", function () {
            $('.modify_con').fadeIn();
            order_id = $(this).attr("_id");
        });
        //提交修改出库数量
        $(".modify_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.modify_con input').eq(0).val());
            if (!num) {
                $.Message({ type: "alert", content: "请输入本次出库数量！" });
                return;
            }
            var _that = $(this);
            $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteBrand")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.modify_con').fadeOut()
                _load_items(1);
            });
        });
        //批量打印
        $('._btns').on("click", "a", function () {
            var index = $(this).index();
            var ids = [];
            $('._ajax_container .plus').each(function (k,v) {
                ids.push($(v).attr('_id'));
            });
            if (ids.length === 0) {
                $.Message({ type: "alert", content: "请选择销售单！" });
                return;
            }
            if (index === 0) {
                window.open('/ERPOrder/PickingList?ids=' + ids.join(','));
            } else {
                window.open('/ERPOrder/SalesList?ids=' + ids.join(','));
            }
        });
        //删除销售单
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "你确定删除此产品吗?删除后该产品下面的所有内容将会清除！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteBrand")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});