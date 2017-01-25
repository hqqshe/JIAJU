require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'domReady'], function ($, base) {
        base.init();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ pageIndex: _page_index }, "/ERPBrand/GetProcedureList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //新建品牌
        $("._btns").on("click", "a", function () {
            $('.new_brand_con').fadeIn().find(".sub_num").val("").parent().find(".tip_txt").val("");
        });
        //提交新建品牌
        $(".new_brand_con").on("click", ".sub_mit", function () {
            var _val = $('.new_brand_con .sub_num').val();
            if (!_val) {
                $.Message({ type: "alert", content: "请输入品牌名称！" });
                return;
            }
            $.when(base.getCodeData({ name: _val, remarks: $(".new_brand_con .tip_txt").val() }, "/ERPBrand/AddGongDuan")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.new_brand_con').fadeOut()
                _load_items(1);
            });
        });
        //删除品牌
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            var txt = _that.parent().parent().children().eq(0).text();
            $.Message({
                type: "confirm", content: "确定删除 " + txt + " 工段吗？删除后数据不可恢复，请慎重！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/AddGongDuan")).done(function () {
                        $.Message({ type: "alert", content: "提交成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});